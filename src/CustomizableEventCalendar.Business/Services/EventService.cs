using System.Transactions;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Data.Repositories;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;

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

        public List<Event> GetAllEvents()
        {
            List<Event> listOfEvents = [.. _eventRepository.GetAll(data => new Event(data))];

            return listOfEvents;
        }

        public List<Event> GetAllEventsOfLoggedInUser()
        {
            return [.. GetAllEvents().Where(eventObj => eventObj.UserId == GlobalData.GetUser().Id)];
        }

        public Event GetEventById(int eventId)
        {
            Event listOfEvents = _eventRepository.GetById(data => new Event(data), eventId);

            return listOfEvents;
        }

        public void DeleteEvent(int eventId)
        {

            using var scope = new TransactionScope();

            try
            {
                _eventCollaboratorsService.DeleteEventCollaboratorsByEventId(eventId);

                _eventRepository.Delete(eventId);

                scope.Complete();
            }
            catch
            {
                //PrintHandler.PrintErrorMessage("Some error occurred ! Delete operation failed.");
            }
        }

        public bool UpdateEvent(Event eventObj, int eventId)
        {
            using var scope = new TransactionScope();

            try
            {
                _eventRepository.Update(eventObj, eventId);

                _eventCollaboratorsService.DeleteEventCollaboratorsByEventId(eventId);

                eventObj.Id = eventId;

                _recurrenceEngine.ScheduleEvents(eventObj);

                scope.Complete();

                return true;
            }
            catch
            {
                //PrintHandler.PrintErrorMessage("Some error occurred ! Update operation failed.");
            }

            return false;
        }

        public void UpdateProposedEvent(Event eventObj,int eventId)
        {
            _eventRepository.Update(eventObj, eventId);
        }

        public List<Event> GetProposedEvents()
        {
            return [.. GetAllEvents().Where(eventObj => eventObj.IsProposed)];
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