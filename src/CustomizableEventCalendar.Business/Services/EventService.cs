using System.Transactions;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Data.Repositories;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Enums;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Model;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services
{
    internal class EventService
    {
        private readonly EventRepository _eventRepository = new();

        private readonly RecurrenceEngine _recurrenceEngine = new();

        private readonly EventCollaboratorService _eventCollaboratorsService = new();

        public int InsertEvent(Event eventObj)
        {
            int eventId = _eventRepository.Insert(eventObj);

            eventObj.Id = eventId;

            _recurrenceEngine.ScheduleEvents(eventObj);

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

            return events;

            //return [.. events.Select(eventObj => eventObj.Participants)
            //                         .Where(participant => participant.User.Id == GlobalData.GetUser().Id
            //                                                && participant.ParticipantRole == ParticipantRole.Organizer)];
        }

        public Event? GetEventById(int eventId)
        {
            Event? listOfEvents = _eventRepository.GetById(data => new Event(data), eventId);

            return listOfEvents;
        }

        public void DeleteEvent(int eventId)
        {
            using var scope = new TransactionScope();

            _eventCollaboratorsService.DeleteEventCollaboratorsByEventId(eventId);

            _eventRepository.Delete(eventId);

            scope.Complete();
        }

        public void UpdateEvent(Event eventObj, int eventId)
        {
            using var scope = new TransactionScope();

            _eventRepository.Update(eventObj, eventId);

            _eventCollaboratorsService.DeleteEventCollaboratorsByEventId(eventId);

            eventObj.Id = eventId;

            _recurrenceEngine.ScheduleEvents(eventObj);

            scope.Complete();
        }

        public void UpdateProposedEvent(Event eventObj, int eventId)
        {
            _eventRepository.Update(eventObj, eventId);
        }

        public List<EventModel> GetProposedEvents()
        {
            //return [.. GetAllEvents().Where(eventObj => eventObj.Participants)];
            return GetAllEvents();
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