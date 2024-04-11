using System.Transactions;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Data.Repositories;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Enums;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Models;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services
{
    internal class EventService
    {
        private readonly EventRepository _eventRepository = new();

        private readonly RecurrenceEngine _recurrenceEngine = new();

        private readonly EventCollaboratorService _eventCollaboratorsService = new();

        public int InsertEvent(EventModel eventModel)
        {
            int eventId = _eventRepository.Insert(eventModel);

            eventModel.Id = eventId;

            _recurrenceEngine.ScheduleEvents(eventModel);

            return eventId;
        }

        public List<EventModel> GetAllEvents()
        {
            List<EventModel> listOfEvents = [.. _eventRepository.GetAll()];

            return listOfEvents;
        }

        public List<EventModel> GetAllEventsOfLoggedInUser()
        {
            List<EventModel> events = GetAllEvents();

            return [.. events.Where(eventModel=> eventModel.Participants
                                                           .Any(participant=>participant.User.Id == GlobalData.GetUser().Id))
                             .Select(eventModel => {  eventModel.Participants = [..eventModel.Participants
                                                           .Where(participant=>participant.User.Id == GlobalData.GetUser().Id)];
                                                      return eventModel;
                                                    }
                    )];
        }

        public List<EventModel>? GetEventById(int eventId)
        {
            List<EventModel>? listOfEvents = _eventRepository.GetById(eventId);

            return listOfEvents;
        }

        public void DeleteEvent(int eventId)
        {
            using var scope = new TransactionScope();

            _eventCollaboratorsService.DeleteParticipantByEventId(eventId);

            _eventRepository.Delete(eventId);

            scope.Complete();
        }

        public void UpdateEvent(EventModel eventModel, int eventId)
        {
            using var scope = new TransactionScope();

            _eventRepository.Update(eventModel);

            _eventCollaboratorsService.DeleteParticipantByEventId(eventId);

            eventModel.Id = eventId;

            _recurrenceEngine.ScheduleEvents(eventModel);

            scope.Complete();
        }

        public void UpdateProposedEvent(EventModel eventModel)
        {
            _eventRepository.Update(eventModel);
        }

        public List<EventModel> GetProposedEvents()
        {
            return [.. GetAllEvents().Select(eventModel => {  eventModel.Participants = eventModel.Participants
                                                      .Where(participant=>participant.ParticipantRole != ParticipantRole.Organizer &&
                                                             participant.ParticipantRole != ParticipantRole.Collaborator)
                                                      .ToList();
                                                      return eventModel;
                                                    })];
        }

        public void ConvertProposedEventToScheduleEvent(int eventId)
        {
            _eventRepository.ConvertProposedEventToScheduleEvent(eventId);
        }

        public int GetTotalEventCount()
        {
            return GetAllEventsOfLoggedInUser().Count;
        }

        public int GetEventIdFromSerialNumber(int srNo)
        {
            return GetAllEventsOfLoggedInUser()[srNo - 1].Id;
        }
    }
}