using System.Data.SqlClient;
using System.Globalization;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Transactions;
using CustomizableEventCalendar.src.CustomizableEventCalendar.ConsoleApp;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Data.Repositories;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services
{
    internal class EventService
    {
        private readonly EventRepository _eventRepository = new();
        private readonly OverlappingEventService _overlappingEventService = new();
        private readonly RecurrenceEngine _recurrenceEngine = new();
        private readonly EventCollaboratorService _eventCollaboratorsService = new();

        public int InsertEvent(Event eventObj)
        {

            if (_overlappingEventService.IsOverlappingEvent(eventObj)) return -1;

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

        public Event GetEventsById(int eventId)
        {
            Event listOfEvents = _eventRepository.GetById(data => new Event(data), eventId);

            return listOfEvents;
        }

        public void DeleteEvent(int srNo)
        {

            using var scope = new TransactionScope();

            try
            {
                int eventId = GetEventIdFromSerialNumber(srNo);

                _eventCollaboratorsService.DeleteEventCollaboratorsByEventId(eventId);

                _eventRepository.Delete(eventId);

                scope.Complete();
            }
            catch
            {
                PrintHandler.PrintErrorMessage("Some error occurred ! Delete operation failed.");
            }
        }

        public bool UpdateEvent(Event eventObj, int srNo)
        {

            if (_overlappingEventService.IsOverlappingEvent(eventObj)) return false;

            using var scope = new TransactionScope();

            try
            {
                int eventId = GetEventIdFromSerialNumber(srNo);

                _eventRepository.Update(eventObj, eventId);

                _eventCollaboratorsService.DeleteEventCollaboratorsByEventId(eventId);

                eventObj.Id = eventId;

                _recurrenceEngine.ScheduleEvents(eventObj);

                scope.Complete();

                return true;
            }
            catch
            {
                PrintHandler.PrintErrorMessage("Some error occurred ! Update operation failed.");
            }

            return false;
        }

        public List<Event> GetProposedEvents()
        {
            return [.. GetAllEvents().Where(eventObj => eventObj.IsProposed)];
        }

        public string GenerateEventTable()
        {
            List<Event> events = GetAllEvents().Where(eventObj => eventObj.UserId == GlobalData.user.Id)
                                               .ToList();

            List<List<string>> outputRows = AddEventDetailsIn2DList(events);

            string eventTable = PrintHandler.GiveTable(outputRows);

            if (events.Count > 0)
            {
                return eventTable;
            }

            return "";
        }

        private static List<List<string>> AddEventDetailsIn2DList(List<Event> events)
        {
            List<List<string>> outputRows = [["Event NO.", "Title", "Description", "Location",
                                              "Event Repetition"]];

            foreach (var (eventObj, index) in events.Select((value, index) => (value, index)))
            {
                outputRows.Add([(index+1).ToString(), eventObj.Title, eventObj.Description, eventObj.Location,
                                RecurrencePatternMessageGenerator.GenerateRecurrenceMessage(eventObj)]);
            }

            return outputRows;
        }

        public void ConvertProposedEventToScheduleEvent(int eventId)
        {
            _eventRepository.ConvertProposedEventToScheduleEvent(eventId);
        }

        public int GetTotalEventCount()
        {
            return GetAllEvents().Count;
        }

        public int GetEventIdFromSerialNumber(int srNo)
        {
            return GetAllEvents()[srNo - 1].Id;
        }
    }
}