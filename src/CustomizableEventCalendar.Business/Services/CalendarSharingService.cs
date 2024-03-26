using CustomizableEventCalendar.src.CustomizableEventCalendar.Data.Repositories;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services
{
    internal class CalendarSharingService
    {
        private readonly SharedCalendarRepository _sharedCalendarRepository = new();

        public void AddSharedCalendar(SharedCalendar sharedEvent)
        {
            _sharedCalendarRepository.Insert(sharedEvent);
        }

        public List<SharedCalendar> GetSharedCalendars()
        {
            return [.._sharedCalendarRepository.GetAll(data => new SharedCalendar(data))
                                                                        .Where(sharedEvent =>
                                                                         sharedEvent.ReceiverUserId ==
                                                                         GlobalData.GetUser().Id)];
        }

        public SharedCalendar? GetSharedCalendarById(int sharedCalendarId)
        {
            return _sharedCalendarRepository.GetById(data => new SharedCalendar(data), sharedCalendarId);
        }

        public List<EventCollaborator> GetSharedEventsFromSharedCalendarId(int sharedCalendarId)
        {
            SharedCalendar? sharedCalendar = GetSharedCalendarById(sharedCalendarId);

            if (sharedCalendar == null) return [];

            EventRepository eventRepository = new();

            List<Event> events = eventRepository.GetAll(data => new Event(data));

            HashSet<int> sharedEventIds = GetSharedEventIdsFromSharedCalendar(events, sharedCalendar);

            List<EventCollaborator> sharedEvents = GetSharedEventsFromSharedCalendar(sharedCalendar, sharedEventIds);

            return sharedEvents;
        }

        private static HashSet<int> GetSharedEventIdsFromSharedCalendar(List<Event> events, SharedCalendar sharedCalendar)
        {
            HashSet<int> sharedEventIds = events.Where(eventObj => eventObj.UserId == sharedCalendar.SenderUserId)
                                                .Select(eventObj => eventObj.Id)
                                                .ToHashSet();

            return sharedEventIds;
        }

        private static List<EventCollaborator> GetSharedEventsFromSharedCalendar(SharedCalendar sharedCalendar, HashSet<int> sharedEventIds)
        {
            List<EventCollaborator> sharedEvents = GetAllSharedEventsBetweenGivenDate(sharedCalendar.FromDate, sharedCalendar.ToDate, sharedEventIds);

            return sharedEvents;
        }

        private static List<EventCollaborator> GetAllSharedEventsBetweenGivenDate(DateOnly fromDate, DateOnly toDate, HashSet<int> sharedEventIds)
        {

            EventCollaboratorRepository eventCollaboratorsRepository = new();

            List<EventCollaborator> sharedEvents = [.. eventCollaboratorsRepository.GetAll(data => new EventCollaborator(data))
                                                                    .Where(sharedEvent =>
                                                                      IsDateBetweenRange(fromDate,toDate,sharedEvent.EventDate) &&
                                                                      IsSharedEvent(sharedEventIds, sharedEvent.EventId))
                                                                    ];

            return sharedEvents;
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