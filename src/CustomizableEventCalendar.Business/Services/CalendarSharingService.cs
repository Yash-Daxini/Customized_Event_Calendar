using CustomizableEventCalendar.src.CustomizableEventCalendar.Data.Repositories;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Enums;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Models;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services
{
    internal class CalendarSharingService
    {
        private readonly SharedCalendarRepository _sharedCalendarRepository = new();

        public void AddSharedCalendar(SharedCalendarModel sharedEventModel)
        {
            _sharedCalendarRepository.Insert(sharedEventModel);
        }

        public List<SharedCalendarModel> GetSharedCalendars()
        {
            return [.._sharedCalendarRepository.GetAll().Where(sharedCalendar => sharedCalendar.ReceiverUser.Id == GlobalData.GetUser().Id)
                                                        .OrderBy(sharedCalendar => sharedCalendar.ToDate)];
        }

        public SharedCalendarModel? GetSharedCalendarById(int sharedCalendarId)
        {
            return _sharedCalendarRepository.GetById(sharedCalendarId);
        }

        public List<EventCollaborator> GetSharedEventsFromSharedCalendarId(int sharedCalendarId)
        {
            SharedCalendarModel? sharedCalendarModel = GetSharedCalendarById(sharedCalendarId);

            if (sharedCalendarModel == null) return [];

            EventRepository eventRepository = new();

            List<EventModel> eventModels = eventRepository.GetAll();

            HashSet<int> sharedEventIds = GetSharedEventIdsFromSharedCalendar(eventModels, sharedCalendarModel);

            List<Domain.Entities.EventCollaborator> sharedEvents = GetSharedEventsFromSharedCalendar(sharedCalendarModel, sharedEventIds);

            return sharedEvents;
        }

        private static HashSet<int> GetSharedEventIdsFromSharedCalendar(List<EventModel> eventModels, SharedCalendarModel sharedCalendarModel)
        {
            HashSet<int> sharedEventIds = [];

            foreach (var eventModel in eventModels)
            {
                UserModel eventOrganizer = eventModel.Participants.Where(participant => participant.ParticipantRole == ParticipantRole.Organizer).First().User;

                if (eventOrganizer.Id == sharedCalendarModel.SenderUser.Id) sharedEventIds.Add(eventModel.Id);
            }

            return sharedEventIds;
        }

        private static List<Domain.Entities.EventCollaborator> GetSharedEventsFromSharedCalendar(SharedCalendarModel sharedCalendarModel, HashSet<int> sharedEventIds)
        {
            List<Domain.Entities.EventCollaborator> sharedEvents = GetAllSharedEventsBetweenGivenDate(sharedCalendarModel.FromDate, sharedCalendarModel.ToDate, sharedEventIds);

            return sharedEvents;
        }

        private static List<ParticipantModel> GetAllSharedEventsBetweenGivenDate(DateOnly fromDate, DateOnly toDate, HashSet<int> sharedEventIds)
        {
            return [.. new EventCollaboratorService().GetAllParticipants().Where(participant => participant.User.Id != GlobalData.GetUser().Id && IsDateBetweenRange(fromDate,toDate,participant.EventDate)
                                                               &&IsSharedEvent(sharedEventIds, participant.EventId))
                                                        .OrderBy(participant => participant.EventDate)];
        }

        private static bool IsDateBetweenRange(DateOnly startDate, DateOnly endDate, DateOnly checkingDate)
        {
            return checkingDate >= startDate && checkingDate <= endDate;
        }

        private static bool IsSharedEvent(HashSet<int> sharedEventIds, int eventId)
        {
            return sharedEventIds.Contains(eventId);
        }
    }
}