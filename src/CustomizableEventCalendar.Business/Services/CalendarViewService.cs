using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly EventCollaboratorsService _eventCollaboratorsService = new();

        public string GenerateDailyView()
        {
            DateTime nextDay = DateTime.Today;

            List<EventCollaborators> eventCollaborators = _eventCollaboratorsService.GetAllEventCollaborators()
                                                                      .Where(eventCollaborator =>
                                                                       eventCollaborator.EventDate.Date == nextDay.Date
                                                                       && eventCollaborator.UserId == GlobalData.user.Id)
                                                                      .ToList();

            Dictionary<int, Event> timeWithEvent = [];

            foreach (var eventCollaborator in eventCollaborators)
            {
                Event eventObj = _eventService.GetEventsById(eventCollaborator.EventId);

                AssignEventToSpecificHour(ref timeWithEvent, eventObj);
            }

            StringBuilder dailyView = new();

            dailyView.AppendLine(PrintHandler.PrintHorizontalLine());

            dailyView.AppendLine("Schedule of date :- " + nextDay + "\n");

            List<List<string>> dailyViewTableContent = [["Date", "Event Title"]];

            while (nextDay.Date <= DateTime.Today.Date)
            {
                int curHour = nextDay.Hour;

                if (timeWithEvent.ContainsKey(curHour))
                {
                    Event eventObj = timeWithEvent[curHour];
                    dailyViewTableContent.Add([DateTimeManager.GetDateWithAbbreviationFromDateTime(nextDay), eventObj.Title]);
                }
                else
                {
                    dailyViewTableContent.Add([DateTimeManager.GetDateWithAbbreviationFromDateTime(nextDay), "-"]);
                }

                nextDay = nextDay.AddHours(1);
            }

            dailyView.AppendLine(PrintHandler.GiveTable(dailyViewTableContent));

            return dailyView.ToString();
        }

        public static void AssignEventToSpecificHour(ref Dictionary<int, Event> eventRecordByHour, Event eventObj)
        {
            int startHour = eventObj.EventStartHour;
            int endHour = eventObj.EventEndHour;

            for (int i = startHour; i <= endHour; i++)
            {
                eventRecordByHour[i] = eventObj;
            }
        }

        public Dictionary<DateTime, List<int>> GetCurrentWeekEvents(DateTime startDateOfWeek, DateTime endDateOfWeek)
        {
            List<EventCollaborators> eventCollaborators = _eventCollaboratorsService.GetAllEventCollaborators();

            Dictionary<DateTime, List<int>> currentWeekEvents = eventCollaborators
                                           .Where(EventCollaborators => EventCollaborators.EventDate.Date >= startDateOfWeek.Date
                                                  && EventCollaborators.EventDate.Date <= endDateOfWeek.Date)
                                           .GroupBy(EventCollaborators => EventCollaborators.EventDate.Date)
                                           .Select(eventCollaborators => new
                                           {
                                               ScheduleDate = eventCollaborators.Key,
                                               EventCollaboratorIds = eventCollaborators
                                                    .Select(eventCollaborators => eventCollaborators.EventId)
                                           })
                                           .ToDictionary(key => key.ScheduleDate, val => val.EventCollaboratorIds
                                           .ToList());
            return currentWeekEvents;
        }

        public string GenerateWeeklyView()
        {
            StringBuilder weeklyView = new();

            DayOfWeek dayOfWeek = DateTime.Now.DayOfWeek;

            int dayTillCurrentDay = dayOfWeek - DayOfWeek.Monday;

            if (dayTillCurrentDay < 0) dayTillCurrentDay += 7;

            DateTime startDateOfWeek = DateTime.Now.AddDays(-dayTillCurrentDay);
            DateTime endDateOfWeek = startDateOfWeek.AddDays(7);

            Dictionary<DateTime, List<int>> currentWeekEvents = GetCurrentWeekEvents(startDateOfWeek, endDateOfWeek);

            weeklyView.AppendLine(PrintHandler.PrintHorizontalLine());

            weeklyView.AppendLine("Schedule from date :- " + DateTimeManager.GetDateFromDateTime(startDateOfWeek) + " to date :- " +
                                   DateTimeManager.GetDateFromDateTime(endDateOfWeek) + "\n");

            List<List<string>> weeklyViewTableContent = [["Date", "Day", "Event Title", "Start Time", "End Time"]];

            while (startDateOfWeek < endDateOfWeek)
            {

                if (currentWeekEvents.ContainsKey(startDateOfWeek.Date))
                {
                    foreach (var eventId in currentWeekEvents[startDateOfWeek.Date])
                    {

                        Event eventObj = _eventService.GetEventsById(eventId);
                        weeklyViewTableContent.Add([ DateTimeManager.GetDateFromDateTime(startDateOfWeek) ,
                                                     DateTimeManager.GetDayFromDateTime(startDateOfWeek) ,
                                                     eventObj.Title,DateTimeManager.ConvertTo12HourFormat(eventObj.EventStartHour),
                                                     DateTimeManager.ConvertTo12HourFormat(eventObj.EventEndHour)]);
                    }
                }
                else
                {
                    weeklyViewTableContent.Add([ DateTimeManager.GetDateFromDateTime(startDateOfWeek) ,
                                                                       DateTimeManager.GetDayFromDateTime(startDateOfWeek) ,
                                                                       "-",
                                                                       "-",
                                                                       "-"]);
                }
                startDateOfWeek = startDateOfWeek.AddDays(1);
            }

            weeklyView.AppendLine(PrintHandler.GiveTable(weeklyViewTableContent));

            return weeklyView.ToString();
        }

        public Dictionary<DateTime, List<int>> GetCurrentMonthEvents(DateTime startDateOfMonth, DateTime endDateOfMonth)
        {
            List<EventCollaborators> eventCollaborators = _eventCollaboratorsService.GetAllEventCollaborators();

            Dictionary<DateTime, List<int>> currentMonthEvents = eventCollaborators
                                           .Where(EventCollaborators => EventCollaborators.EventDate.Date >=
                                                                                   startDateOfMonth.Date
                                                  && EventCollaborators.EventDate.Date <= endDateOfMonth.Date)
                                           .GroupBy(EventCollaborators => EventCollaborators.EventDate.Date)
                                           .Select(EventCollaborators => new
                                           {
                                               ScheduleDate = EventCollaborators.Key,
                                               EventCollaboratorIds = EventCollaborators
                                                    .Select(eventCollaborator => eventCollaborator.EventId)
                                           })
                                           .ToDictionary(key => key.ScheduleDate, val => val.EventCollaboratorIds
                                           .ToList());

            return currentMonthEvents;
        }

        public string GenerateMonthView()
        {
            StringBuilder monthlyView = new();

            DateTime nextDay = DateTime.Now;

            DateTime startDateOfMonth = new(nextDay.Year, nextDay.Month, 1);
            DateTime endDateOfMonth = new DateTime(nextDay.Year, nextDay.Month, DateTime.DaysInMonth(nextDay.Year,
                                                    nextDay.Month));

            Dictionary<DateTime, List<int>> currentMonthEvents = GetCurrentMonthEvents(startDateOfMonth, endDateOfMonth);

            monthlyView.AppendLine(PrintHandler.PrintHorizontalLine());

            monthlyView.AppendLine("Schedule from date :- " + DateTimeManager.GetDateFromDateTime(startDateOfMonth) + " to date :- " +
                                    DateTimeManager.GetDateFromDateTime(endDateOfMonth) + "\n");

            List<List<string>> monthlyViewTableContent = [["Date", "Day", "Event Title", "Start Time", "End Time"]];

            while (startDateOfMonth.Date <= endDateOfMonth.Date)
            {

                if (currentMonthEvents.ContainsKey(startDateOfMonth.Date))
                {
                    foreach (var eventId in currentMonthEvents[startDateOfMonth.Date])
                    {

                        Event eventObj = _eventService.GetEventsById(eventId);
                        monthlyViewTableContent.Add([DateTimeManager.GetDateFromDateTime(startDateOfMonth),
                                                                      startDateOfMonth.DayOfWeek.ToString(),
                                                                      eventObj.Title,DateTimeManager.ConvertTo12HourFormat(eventObj.EventStartHour),
                                                                      DateTimeManager.ConvertTo12HourFormat(eventObj.EventEndHour)]);
                    }
                }
                else
                {
                    monthlyViewTableContent.Add([DateTimeManager.GetDateFromDateTime(startDateOfMonth),
                                                 startDateOfMonth.DayOfWeek.ToString(),
                                                                      "-",
                                                                      "-",
                                                                      "-"]);
                }

                startDateOfMonth = startDateOfMonth.AddDays(1);
            }

            monthlyView.Append(PrintHandler.GiveTable(monthlyViewTableContent));

            return monthlyView.ToString();
        }
    }
}