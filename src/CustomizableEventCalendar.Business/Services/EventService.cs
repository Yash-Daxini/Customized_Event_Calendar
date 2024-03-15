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
            List<Event> listOfEvents = [.. _eventRepository.GetAll<Event>(data => new Event(data))];

            return listOfEvents;
        }

        public Event GetEventsById(int eventId)
        {
            Event listOfEvents = _eventRepository.GetById<Event>(data => new Event(data), eventId);

            return listOfEvents;
        }

        public void DeleteEvent(int srNo)
        {

            using var scope = new TransactionScope();

            try
            {
                int eventId = GetEventIdFromSerialNumber(srNo);

                _eventCollaboratorsService.DeleteEventCollaboratorsByEventId(eventId);

                _eventRepository.Delete<Event>(eventId);

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

            List<List<string>> outputRows = [["Event NO.", "Title", "Description", "Location", "StartHour", "EndHour", "StartDate",
                                              "EndDate", "Frequency","Interval","Days","Week No.","Month Days","Month","Year"]];

            AddEventDetailsIn2DList(events, ref outputRows);

            string eventTable = PrintHandler.GiveTable(outputRows);

            if (events.Count > 0)
            {
                return eventTable;
            }

            return "";
        }

        public static void AddEventDetailsIn2DList(List<Event> events, ref List<List<string>> outputRows)
        {
            foreach (var (eventObj, index) in events.Select((value, index) => (value, index)))
            {
                outputRows.Add([(index+1).ToString(), eventObj.Title, eventObj.Description, eventObj.Location,
                                DateTimeManager.ConvertTo12HourFormat(eventObj.EventStartHour),
                                DateTimeManager.ConvertTo12HourFormat(eventObj.EventEndHour),
                                eventObj.EventStartDate.ToString(),eventObj.EventEndDate.ToString(),
                                eventObj.Frequency ?? "-" ,
                                eventObj.Interval == null ? "-" : eventObj.Interval.ToString(),
                                eventObj.ByWeekDay == null ? "-" : GetWeekDaysFromNumbers(eventObj.ByWeekDay),
                                eventObj.WeekOrder== null ? "-" : eventObj.WeekOrder.ToString(),
                                eventObj.ByMonthDay == null ? "-" : eventObj.ByMonthDay.ToString(),
                                eventObj.ByMonth == null ? "-" : DateTimeManager.GetMonthFromMonthNumber((int)eventObj.ByMonth),
                                eventObj.ByYear == null ? "-" : eventObj.ByYear.ToString()]);
            }
        }

        public void ConvertProposedEventToScheduleEvent(int eventId)
        {
            _eventRepository.ConvertProposedEventToScheduleEvent(eventId);
        }

        public static string GetWeekDaysFromNumbers(string days)
        {
            List<string> listOfDays = [.. days.Split(",").Select(day => day.Trim())];

            StringBuilder daysOfWeek = new();

            foreach (string day in listOfDays)
            {
                if (day.Length == 0) continue;
                daysOfWeek.Append(DateTimeManager.GetWeekDayFromWeekNumber(Convert.ToInt32(day)) + ",");
            }

            if (daysOfWeek.Length == 0) return "-";
            return daysOfWeek.ToString().Substring(0, daysOfWeek.Length - 1);
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