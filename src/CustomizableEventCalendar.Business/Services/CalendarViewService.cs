﻿using System;
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
        private readonly EventService _eventService = new EventService();
        private readonly ScheduleEventService _scheduleEventService = new ScheduleEventService();
        public string GenerateDailyView()
        {
            DateTime nextDay = DateTime.Today;

            List<ScheduleEvent> scheduleEvents = _scheduleEventService.ReadByUserId()
                                            .Where(scheduleEvent => scheduleEvent.ScheduledDate.Date == nextDay.Date)
                                            .ToList();

            Dictionary<int, Event> timeWithEvent = new Dictionary<int, Event>();

            foreach (var scheduleEvent in scheduleEvents)
            {
                Event eventObj = _eventService.Read(_scheduleEventService.GetEventIdFromEventCollaborators(scheduleEvent.EventCollaboratorsId));

                AssignEventToSpecificHour(ref timeWithEvent, eventObj);
            }

            StringBuilder dailyView = new StringBuilder();

            dailyView.AppendLine(PrintHandler.PrintHorizontalLine());

            dailyView.AppendLine("Schedule of date :- " + GetDateFromDateTime(nextDay) + "\n");

            List<List<string>> dailyViewTableContent = new List<List<string>> { new List<string> { "Date", "Event Title" } };

            while (nextDay.Date <= DateTime.Today.Date)
            {
                int curHour = nextDay.Hour;

                if (timeWithEvent.ContainsKey(curHour))
                {
                    Event eventObj = timeWithEvent[curHour];
                    dailyViewTableContent.Add(new List<string> { GetDateWithAbbreviationFromDateTime(nextDay), eventObj.Title });
                }
                else dailyViewTableContent.Add(new List<string> { GetDateWithAbbreviationFromDateTime(nextDay), "-" });

                nextDay = nextDay.AddHours(1);
            }

            dailyView.AppendLine(PrintHandler.PrintTable(dailyViewTableContent));

            return dailyView.ToString();
        }
        public void AssignEventToSpecificHour(ref Dictionary<int, Event> eventRecordByHour, Event eventObj)
        {
            string TimeBlock = eventObj.TimeBlock;

            string startTime = TimeBlock.Split("-")[0];
            int startHour = Convert.ToInt32(startTime.Substring(0, startTime.Length - 2)) + (startTime.EndsWith("PM") ? 12 : 0);

            string endTime = TimeBlock.Split("-")[1];
            int endHour = Convert.ToInt32(endTime.Substring(0, endTime.Length - 2)) + (endTime.EndsWith("PM") ? 12 : 0);

            for (int i = startHour; i <= endHour; i++)
            {
                eventRecordByHour[i] = eventObj;
            }
        }
        public Dictionary<DateTime, List<int>> GetCurrentWeekEvents(DateTime startDateOfWeek, DateTime endDateOfWeek)
        {
            Dictionary<DateTime, List<int>> currentWeekEvents = _scheduleEventService.ReadByUserId()
                                           .Where(scheduleEvent => scheduleEvent.ScheduledDate.Date >= startDateOfWeek.Date
                                                  && scheduleEvent.ScheduledDate.Date <= endDateOfWeek.Date)
                                           .GroupBy(scheduleEvent => scheduleEvent.ScheduledDate.Date)
                                           .Select(scheduleEvent => new
                                           {
                                               ScheduleDate = scheduleEvent.Key,
                                               EventCollaboratorIds = scheduleEvent
                                                    .Select(scheduleEvent => _scheduleEventService.GetEventIdFromEventCollaborators(scheduleEvent.EventCollaboratorsId))
                                           })
                                           .ToDictionary(key => key.ScheduleDate, val => val.EventCollaboratorIds.ToList());
            return currentWeekEvents;
        }
        public string GenerateWeeklyView()
        {
            StringBuilder weeklyView = new StringBuilder();

            DayOfWeek dayOfWeek = DateTime.Now.DayOfWeek;

            int dayTillCurrentDay = dayOfWeek - DayOfWeek.Monday;

            if (dayTillCurrentDay < 0) dayTillCurrentDay += 7;

            DateTime startDateOfWeek = DateTime.Now.AddDays(-dayTillCurrentDay);
            DateTime endDateOfWeek = startDateOfWeek.AddDays(7);

            Dictionary<DateTime, List<int>> currentWeekEvents = GetCurrentWeekEvents(startDateOfWeek, endDateOfWeek);

            weeklyView.AppendLine(PrintHandler.PrintHorizontalLine());

            weeklyView.AppendLine("Schedule from date :- " + GetDateFromDateTime(startDateOfWeek) + " to date :- " + GetDateFromDateTime(endDateOfWeek) + "\n");

            List<List<string>> weeklyViewTableContent = new List<List<string>> { new List<string> { "Date", "Day" , "Event Title" ,
                                                                                                     "Time" } };

            while (startDateOfWeek < endDateOfWeek)
            {

                if (currentWeekEvents.ContainsKey(startDateOfWeek.Date))
                {
                    foreach (var eventId in currentWeekEvents[startDateOfWeek.Date])
                    {

                        Event eventObj = _eventService.Read(eventId);
                        weeklyViewTableContent.Add(new List<string> { GetDateFromDateTime(startDateOfWeek) ,
                                                                       GetDayFromDateTime(startDateOfWeek) ,
                                                                       eventObj.Title,
                                                                       {eventObj.TimeBlock} });
                    }
                }
                else
                {
                    weeklyViewTableContent.Add(new List<string> { GetDateFromDateTime(startDateOfWeek) ,
                                                                       GetDayFromDateTime(startDateOfWeek) ,
                                                                       "-",
                                                                       "-" });
                }

                startDateOfWeek = startDateOfWeek.AddDays(1);
            }

            weeklyView.AppendLine(PrintHandler.PrintTable(weeklyViewTableContent));

            return weeklyView.ToString();
        }
        public Dictionary<DateTime, List<int>> GetCurrentMonthEvents(DateTime startDateOfMonth, DateTime endDateOfMonth)
        {
            Dictionary<DateTime, List<int>> currentMonthEvents = _scheduleEventService.ReadByUserId()
                                           .Where(scheduleEvent => scheduleEvent.ScheduledDate.Date >= startDateOfMonth.Date
                                                  && scheduleEvent.ScheduledDate.Date <= endDateOfMonth.Date)
                                           .GroupBy(scheduleEvent => scheduleEvent.ScheduledDate.Date)
                                           .Select(scheduleEvent => new
                                           {
                                               ScheduleDate = scheduleEvent.Key,
                                               EventCollaboratorIds = scheduleEvent
                                                    .Select(scheduleEvent => _scheduleEventService.
                                                            GetEventIdFromEventCollaborators
                                                            (scheduleEvent.EventCollaboratorsId))
                                           })
                                           .ToDictionary(key => key.ScheduleDate, val => val.EventCollaboratorIds.ToList());
            return currentMonthEvents;
        }
        public string GenerateMonthView()
        {
            StringBuilder monthlyView = new StringBuilder();

            DateTime nextDay = DateTime.Now;

            DateTime startDateOfMonth = new(nextDay.Year, nextDay.Month, 1);
            DateTime endDateOfMonth = new DateTime(nextDay.Year, nextDay.Month, DateTime.DaysInMonth(nextDay.Year, nextDay.Month));

            Dictionary<DateTime, List<int>> currentMonthEvents = GetCurrentMonthEvents(startDateOfMonth, endDateOfMonth);

            monthlyView.AppendLine(PrintHandler.PrintHorizontalLine());

            monthlyView.AppendLine("Schedule from date :- " + GetDateFromDateTime(startDateOfMonth) + " to date :- " + GetDateFromDateTime(endDateOfMonth) + "\n");

            List<List<string>> monthlyViewTableContent = new List<List<string>> { new List<string> { "Date", "Day" , "Event Title" ,
                                                                                                     "Time" } };

            while (startDateOfMonth.Date <= endDateOfMonth.Date)
            {

                if (currentMonthEvents.ContainsKey(startDateOfMonth.Date))
                {
                    foreach (var eventId in currentMonthEvents[startDateOfMonth.Date])
                    {

                        Event eventObj = _eventService.Read(eventId);
                        monthlyViewTableContent.Add(new List<string> {GetDateFromDateTime(startDateOfMonth),
                                                                      startDateOfMonth.DayOfWeek.ToString(),
                                                                      {eventObj.Title},
                                                                      {eventObj.TimeBlock} });
                    }
                }
                else
                {
                    monthlyViewTableContent.Add(new List<string> {GetDateFromDateTime(startDateOfMonth),
                                                                      startDateOfMonth.DayOfWeek.ToString(),
                                                                      "-",
                                                                      "-" });
                }

                startDateOfMonth = startDateOfMonth.AddDays(1);
            }

            monthlyView.Append(PrintHandler.PrintTable(monthlyViewTableContent));

            return monthlyView.ToString();
        }
        public string GetDateWithAbbreviationFromDateTime(DateTime dateTime)
        {
            return dateTime.ToString("hh:mm:ss tt");
        }
        public string GetDateFromDateTime(DateTime dateTime)
        {
            return dateTime.ToString("dd-MM-yyyy");
        }
        public string GetDayFromDateTime(DateTime dateTime)
        {
            return dateTime.ToString("dddd");
        }
    }
}