using System.Data.SqlClient;
using System.Globalization;
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

        public int InsertEvent(Event eventObj)
        {
            int eventId = _eventRepository.Insert(eventObj);

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

        public void DeleteEvent(int eventId)
        {
            _eventRepository.Delete<Event>(eventId);
        }

        public void UpdateEvent(Event eventObj, int eventId)
        {
            _eventRepository.Update(eventObj, eventId);
        }

        public string GenerateEventTable()
        {
            List<Event> events = GetAllEvents().Where(eventObj => eventObj.UserId == GlobalData.user.Id)
                                               .ToList();

            List<List<string>> outputRows = [["Event NO.", "Title", "Description", "Location", "StartHour", "EndHour", "StartDate",
                                              "EndDate", "Frequency","Interval","Days","Month Days","Month","Year"]];

            foreach (var eventObj in events)
            {
                outputRows.Add([eventObj.Id.ToString(), eventObj.Title, eventObj.Description, eventObj.Location,
                                eventObj.EventStartHour.ToString(), eventObj.EventEndHour.ToString(), eventObj.EventStartDate.ToString(),
                                eventObj.EventEndDate.ToString(),
                                eventObj.Frequency == null ? "-" : eventObj.Frequency ,
                                eventObj.Interval == null ? "-" : eventObj.Interval.ToString(),
                                eventObj.ByWeekDay == null ? "-" : GetWeekDaysFromNumbers(eventObj.ByWeekDay),
                                eventObj.ByMonthDay == null ? "-" : eventObj.ByMonthDay.ToString(),
                                eventObj.ByMonth == null ? "-" : GetMonthFromMonthNumber((int)eventObj.ByMonth),
                                eventObj.ByYear == null ? "-" : eventObj.ByYear.ToString()]);
            }

            string eventTable = PrintHandler.GiveTable(outputRows);

            if (events.Count > 0)
            {
                return eventTable;
            }

            return "";
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
                daysOfWeek.Append(GetWeekDayFromWeekNumber(Convert.ToInt32(day)) + ",");
            }

            if (daysOfWeek.Length == 0) return "-";
            return daysOfWeek.ToString().Substring(0, daysOfWeek.Length - 1);
        }

        public static string GetWeekDayFromWeekNumber(int dayNumber)
        {
            if (dayNumber < 1 || dayNumber > 7)
            {
                return "-";
            }

            DayOfWeek dayOfWeek = (DayOfWeek)(dayNumber - 1);

            return dayOfWeek.ToString();
        }

        public static string GetMonthFromMonthNumber(int month)
        {
            if (month <= 0 || month > 12) return "-";

            return CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);
        }

    }
}