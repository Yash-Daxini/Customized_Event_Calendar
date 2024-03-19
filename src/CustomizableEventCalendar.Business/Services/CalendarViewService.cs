using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using CustomizableEventCalendar.src.CustomizableEventCalendar.ConsoleApp;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;
using NodaTime;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services
{
    internal class CalendarViewService
    {
        private readonly EventService _eventService = new();
        private readonly EventCollaboratorService _eventCollaboratorsService = new();

        public string GenerateDailyView()
        {
            Dictionary<int, Event> hourEventMapping = MapEachHourWithEvent();

            StringBuilder dailyView = new();

            dailyView.AppendLine(PrintHandler.PrintHorizontalLine());

            dailyView.AppendLine("Schedule of date :- " + DateTime.Today.Date + "\n");

            List<List<string>> dailyViewTableContent = InsertTodayEventsWithDateIn2DList(hourEventMapping);

            dailyView.AppendLine(PrintService.GenerateTable(dailyViewTableContent));

            return dailyView.ToString();
        }

        private Dictionary<int, Event> MapEachHourWithEvent()
        {
            List<EventCollaborator> eventCollaborators = GetTodayEvents();

            Dictionary<int, Event> hourEventMapping = [];

            foreach (var eventCollaborator in eventCollaborators)
            {
                Event eventObj = _eventService.GetEventsById(eventCollaborator.EventId);

                AssignEventToSpecificHour(ref hourEventMapping, eventObj);
            }

            return hourEventMapping;
        }

        private bool IsProposedEvent(int eventId)
        {
            return _eventService.GetProposedEvents().Exists(eventObj => eventObj.Id == eventId);
        }

        private static List<List<string>> InsertTodayEventsWithDateIn2DList(Dictionary<int, Event> hourEventMapping)
        {
            List<List<string>> dailyViewTableContent = [["Date", "Event Title"]];

            DateTime today = DateTime.Today;

            while (today.Date <= DateTime.Today.Date)
            {
                int curHour = today.Hour;

                hourEventMapping.TryGetValue(curHour, out Event? eventObj);

                dailyViewTableContent.Add([DateTimeManager.GetDateWithAbbreviationFromDateTime(today), eventObj == null ? "-" :
                                                                                               eventObj.Title]);

                today = today.AddHours(1);
            }

            return dailyViewTableContent;
        }

        private List<EventCollaborator> GetTodayEvents()
        {
            return [.. _eventCollaboratorsService.GetAllEventCollaborators()
                   .Where(eventCollaborator => eventCollaborator.EventDate.Date == DateTime.Today
                                               && eventCollaborator.UserId == GlobalData.GetUser().Id
                                               && !IsProposedEvent(eventCollaborator.EventId))];
        }

        private static void AssignEventToSpecificHour(ref Dictionary<int, Event> eventRecordByHour, Event eventObj)
        {
            int startHour = eventObj.EventStartHour;
            int endHour = eventObj.EventEndHour;

            for (int i = startHour; i <= endHour; i++)
            {
                eventRecordByHour[i] = eventObj;
            }
        }

        private Dictionary<DateTime, List<int>> GetGivenWeekEventsWithDate(DateTime startDateOfWeek, DateTime endDateOfWeek)
        {
            List<EventCollaborator> eventCollaborators = _eventCollaboratorsService.GetAllEventCollaborators();

            Dictionary<DateTime, List<int>> currentWeekEvents = GenerateDictionaryOfDateTimeAndEventIdFromList(GetGivenWeekEvents(eventCollaborators, startDateOfWeek, endDateOfWeek));

            return currentWeekEvents;
        }

        private List<EventCollaborator> GetGivenWeekEvents(List<EventCollaborator> eventCollaborators, DateTime startDateOfWeek, DateTime endDateOfWeek)
        {
            return [..eventCollaborators.Where(eventCollaborator => IsEventOccurInGivenWeek(startDateOfWeek,
                                                                       endDateOfWeek, eventCollaborator))];
        }

        private bool IsEventOccurInGivenWeek(DateTime startDateOfWeek, DateTime endDateOfWeek,
                                             EventCollaborator eventCollaborator)
        {
            return eventCollaborator.EventDate.Date >= startDateOfWeek.Date
                   && eventCollaborator.EventDate.Date <= endDateOfWeek.Date
                   && eventCollaborator.UserId == GlobalData.GetUser().Id
                   && !IsProposedEvent(eventCollaborator.EventId);
        }

        public string GenerateWeeklyView()
        {
            StringBuilder weeklyView = new();

            DateTime startDateOfWeek = DateTimeManager.GetStartDateOfWeek(DateTime.Today);
            DateTime endDateOfWeek = DateTimeManager.GetEndDateOfWeek(DateTime.Today);

            Dictionary<DateTime, List<int>> currentWeekEvents = GetGivenWeekEventsWithDate(startDateOfWeek, endDateOfWeek);

            weeklyView.AppendLine(PrintHandler.PrintHorizontalLine());

            weeklyView.AppendLine("Schedule from date :- " + DateTimeManager.GetDateFromDateTime(startDateOfWeek)
                                   + " to date :- " + DateTimeManager.GetDateFromDateTime(endDateOfWeek) + "\n");

            List<List<string>> weeklyViewTableContent = Generate2DListForWeeklyEvents(startDateOfWeek, endDateOfWeek,
                                                                                              currentWeekEvents);

            weeklyView.AppendLine(PrintService.GenerateTable(weeklyViewTableContent));

            return weeklyView.ToString();
        }

        private List<List<string>> Generate2DListForWeeklyEvents(DateTime startDateOfWeek, DateTime endDateOfWeek, Dictionary<DateTime, List<int>> currentWeekEvents)
        {
            List<List<string>> weeklyViewTableContent = [["Date", "Day", "Event Title", "Start Time", "End Time"]];

            while (startDateOfWeek < endDateOfWeek)
            {
                InsertGivenWeekEventsWithDateIn2DList(startDateOfWeek, currentWeekEvents, weeklyViewTableContent);

                startDateOfWeek = startDateOfWeek.AddDays(1);
            }

            return weeklyViewTableContent;
        }

        private void InsertGivenWeekEventsWithDateIn2DList(DateTime startDateOfWeek, Dictionary<DateTime, List<int>> currentWeekEvents, List<List<string>> weeklyViewTableContent)
        {
            if (currentWeekEvents.TryGetValue(startDateOfWeek.Date, out List<int>? eventIds))
            {
                foreach (var eventId in eventIds)
                {

                    Event eventObj = _eventService.GetEventsById(eventId);

                    weeklyViewTableContent.Add([DateTimeManager.GetDateFromDateTime(startDateOfWeek),
                                                DateTimeManager.GetDayFromDateTime(startDateOfWeek),
                                                eventObj.Title,
                                                DateTimeManager.ConvertTo12HourFormat(eventObj.EventStartHour),
                                                DateTimeManager.ConvertTo12HourFormat(eventObj.EventEndHour)]);
                }
            }
            else
            {
                weeklyViewTableContent.Add([DateTimeManager.GetDateFromDateTime(startDateOfWeek),
                                            DateTimeManager.GetDayFromDateTime(startDateOfWeek),
                                            "-","-","-"]);
            }
        }

        public Dictionary<DateTime, List<int>> GetGivenMonthEventsWithDate(DateTime startDateOfMonth, DateTime endDateOfMonth)
        {
            List<EventCollaborator> eventCollaborators = _eventCollaboratorsService.GetAllEventCollaborators();

            Dictionary<DateTime, List<int>> currentMonthEvents = GenerateDictionaryOfDateTimeAndEventIdFromList
                                    (GetGivenMonthEvents(eventCollaborators, startDateOfMonth, endDateOfMonth));

            return currentMonthEvents;
        }

        private bool IsEventOccurInGivenMonth(DateTime startDateOfMonth, DateTime endDateOfMonth,
                                             EventCollaborator eventCollaborator)
        {
            return eventCollaborator.EventDate.Date >= startDateOfMonth.Date
                   && eventCollaborator.EventDate.Date <= endDateOfMonth.Date
                   && eventCollaborator.UserId == GlobalData.GetUser().Id
                   && !IsProposedEvent(eventCollaborator.EventId);
        }

        private List<EventCollaborator> GetGivenMonthEvents(List<EventCollaborator> eventCollaborators, DateTime startDateOfMonth, DateTime endDateOfMonth)
        {
            return [..eventCollaborators.Where(eventCollaborator => IsEventOccurInGivenMonth
                                                                    (startDateOfMonth, endDateOfMonth, eventCollaborator))];
        }

        private static Dictionary<DateTime, List<int>> GenerateDictionaryOfDateTimeAndEventIdFromList(List<EventCollaborator> eventCollaborators)
        {
            return eventCollaborators.GroupBy(eventCollaborator => eventCollaborator.EventDate.Date)
                                           .Select(eventCollaborator => new
                                           {
                                               ScheduleDate = eventCollaborator.Key,
                                               EventCollaboratorIds = eventCollaborator.Select(eventCollaborator =>
                                                                                               eventCollaborator.EventId)
                                                                                       .ToList()
                                           })
                                           .ToDictionary(key => key.ScheduleDate, val => val.EventCollaboratorIds);
        }

        public string GenerateMonthView()
        {
            StringBuilder monthlyView = new();

            DateTime startDateOfMonth = DateTimeManager.GetStartDateOfMonth(DateTime.Now);
            DateTime endDateOfMonth = DateTimeManager.GetEndDateOfMonth(DateTime.Now);

            Dictionary<DateTime, List<int>> currentMonthEvents = GetGivenMonthEventsWithDate(startDateOfMonth, endDateOfMonth);

            monthlyView.AppendLine(PrintHandler.PrintHorizontalLine());

            monthlyView.AppendLine("Schedule from date :- " + DateTimeManager.GetDateFromDateTime(startDateOfMonth)
                                    + " to date :- " + DateTimeManager.GetDateFromDateTime(endDateOfMonth) + "\n");

            List<List<string>> monthlyViewTableContent = Generate2DListForMonthlyEvents(ref startDateOfMonth, endDateOfMonth, currentMonthEvents);

            monthlyView.Append(PrintService.GenerateTable(monthlyViewTableContent));

            return monthlyView.ToString();
        }

        private List<List<string>> Generate2DListForMonthlyEvents(ref DateTime startDateOfMonth, DateTime endDateOfMonth, Dictionary<DateTime, List<int>> currentMonthEvents)
        {
            List<List<string>> monthlyViewTableContent = [["Date", "Day", "Event Title", "Start Time", "End Time"]];

            while (startDateOfMonth.Date <= endDateOfMonth.Date)
            {
                InsertGivenMonthEventsWithDateIn2DList(startDateOfMonth, currentMonthEvents, monthlyViewTableContent);

                startDateOfMonth = startDateOfMonth.AddDays(1);
            }

            return monthlyViewTableContent;
        }

        private void InsertGivenMonthEventsWithDateIn2DList(DateTime startDateOfMonth, Dictionary<DateTime, List<int>> currentMonthEvents, List<List<string>> monthlyViewTableContent)
        {
            if (currentMonthEvents.TryGetValue(startDateOfMonth.Date, out List<int>? eventIds))
            {
                foreach (var eventId in eventIds)
                {

                    Event eventObj = _eventService.GetEventsById(eventId);
                    monthlyViewTableContent.Add([DateTimeManager.GetDateFromDateTime(startDateOfMonth),
                                                 startDateOfMonth.DayOfWeek.ToString(),
                                                 eventObj.Title,
                                                 DateTimeManager.ConvertTo12HourFormat(eventObj.EventStartHour),
                                                 DateTimeManager.ConvertTo12HourFormat(eventObj.EventEndHour)]);
                }
            }
            else
            {
                monthlyViewTableContent.Add([DateTimeManager.GetDateFromDateTime(startDateOfMonth),
                                             startDateOfMonth.DayOfWeek.ToString(),
                                             "-","-","-"]);
            }
        }
    }
}