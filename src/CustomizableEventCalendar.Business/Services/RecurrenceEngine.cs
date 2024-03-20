using System;
using System.Security.Cryptography;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;
using Ical.Net.DataTypes;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services
{
    internal class RecurrenceEngine
    {
        private readonly EventCollaboratorService eventCollaboratorsService = new();

        public void ScheduleEvents(Event eventObj)
        {
            if (eventObj.Frequency == null)
            {
                ScheduleNonRecurrenceEvents(eventObj);
            }

            ScheduleEventsUsingFrequency(eventObj);

        }

        private void ScheduleEventsUsingFrequency(Event eventObj)
        {
            switch (eventObj.Frequency)
            {
                case "daily":
                    ScheduleDailyEvents(eventObj);
                    break;
                case "weekly":
                    ScheduleWeeklyEvents(eventObj);
                    break;
                case "monthly":
                    ScheduleMonthlyEvents(eventObj);
                    break;
                case "yearly":
                    ScheduleYearlyEvents(eventObj);
                    break;
            }
        }

        private void ScheduleNonRecurrenceEvents(Event eventObj)
        {
            EventCollaborator eventCollaborator = new(eventObj.Id, GlobalData.GetUser().Id, "organizer", "accept", eventObj.EventStartHour, eventObj.EventEndHour, eventObj.EventStartDate);

            ScheduleEvent(eventCollaborator);

        }

        private void ScheduleDailyEvents(Event eventObj)
        {
            if (eventObj.Interval == null) ScheduleDailyEventsWithoutInterval(eventObj);
            else ScheduleDailyEventWithInterval(eventObj);
        }

        private void ScheduleDailyEventsWithoutInterval(Event eventObj)
        {
            HashSet<int> days = [.. eventObj.ByWeekDay.Split(",").Select(day => Convert.ToInt32(day))];

            DateOnly startDate = eventObj.EventStartDate;

            while (startDate <= eventObj.EventEndDate)
            {
                int day = DateTimeManager.GetDayNumberFromWeekDay(startDate);

                if (days.Contains(day))
                {
                    EventCollaborator eventCollaborator = new(eventObj.Id, GlobalData.GetUser().Id, "organizer", "accept", eventObj.EventStartHour, eventObj.EventEndHour, startDate);

                    ScheduleEvent(eventCollaborator);
                }
                startDate = startDate.AddDays(1);
            }
        }

        private void ScheduleDailyEventWithInterval(Event eventObj)
        {
            DateOnly startDate = eventObj.EventStartDate;

            while (startDate <= eventObj.EventEndDate)
            {
                EventCollaborator eventCollaborator = new(eventObj.Id, GlobalData.GetUser().Id, "organizer", "accept", eventObj.EventStartHour, eventObj.EventEndHour, startDate);

                ScheduleEvent(eventCollaborator);

                startDate = startDate.AddDays(Convert.ToInt32(eventObj.Interval));
            }
        }

        private void ScheduleEvent(EventCollaborator eventCollaborators)
        {
            eventCollaboratorsService.InsertEventCollaborators(eventCollaborators);
        }

        private void ScheduleWeeklyEvents(Event eventObj)
        {
            HashSet<int> weekDays = [.. eventObj.ByWeekDay.Split(",").Select(weekDay => Convert.ToInt32(weekDay))];

            DateOnly startDateOfEvent = eventObj.EventStartDate;
            DateOnly endDateOfEvent = eventObj.EventEndDate;

            DateOnly startDateOfWeek = DateTimeManager.GetStartDateOfWeek(startDateOfEvent);
            DateOnly endDateOfWeek = DateTimeManager.GetEndDateOfWeek(startDateOfEvent);

            DateOnly startDate = startDateOfWeek;

            while (startDate <= eventObj.EventEndDate)
            {
                int day = DateTimeManager.GetDayNumberFromWeekDay(startDate);

                if (weekDays.Contains(day) && IsDateInRange(startDateOfEvent, endDateOfEvent, startDate))
                {
                    EventCollaborator eventCollaborator = new(eventObj.Id, GlobalData.GetUser().Id, "organizer", "accept", eventObj.EventStartHour, eventObj.EventEndHour, startDate);

                    ScheduleEvent(eventCollaborator);
                }
                startDate = startDate.AddDays(1);

                if (startDate > endDateOfWeek)
                {
                    startDate = startDateOfWeek.AddDays(7 * (int)eventObj.Interval);
                    startDateOfWeek = startDate;
                    endDateOfWeek = DateTimeManager.GetEndDateOfWeek(startDate);
                }
            }
        }

        private static bool IsDateInRange(DateOnly startDate, DateOnly endDate, DateOnly dateToCheck)
        {
            return dateToCheck >= startDate && dateToCheck <= endDate;
        }

        private void ScheduleMonthlyEvents(Event eventObj)
        {

            if (eventObj.ByMonthDay == null)
            {
                ScheduleMonthlyEventsUsingWeekOrderAndWeekDay(eventObj);
            }
            else
            {
                ScheduleMonthlyEventsUsingMonthDay(eventObj);
            }

        }

        private void ScheduleMonthlyEventsUsingMonthDay(Event eventObj)
        {
            int day = (int)eventObj.ByMonthDay;

            DateOnly startDateOfEvent = eventObj.EventStartDate;

            DateOnly startDate = new(startDateOfEvent.Year, startDateOfEvent.Month, GetMinimumDateFromGivenMonthAndDay(day, startDateOfEvent));

            while (true)
            {
                if (startDate > eventObj.EventEndDate) break;

                EventCollaborator eventCollaborator = new(eventObj.Id, GlobalData.GetUser().Id, "organizer", "accept", eventObj.EventStartHour, eventObj.EventEndHour, startDate);

                ScheduleEvent(eventCollaborator);

                startDate = startDate.AddMonths((int)eventObj.Interval);
                startDate = new DateOnly(startDate.Year, startDate.Month, GetMinimumDateFromGivenMonthAndDay(day, startDate));
            }

        }

        private static int GetMinimumDateFromGivenMonthAndDay(int day, DateOnly date)
        {
            int daysInMonth = DateTime.DaysInMonth(date.Year, date.Month);

            return Math.Min(day, daysInMonth);
        }

        private void ScheduleMonthlyEventsUsingWeekOrderAndWeekDay(Event eventObj)
        {
            int weekOrder = (int)eventObj.WeekOrder;

            int weekDay = Convert.ToInt32(eventObj.ByWeekDay.Split(",")[0]);

            if (weekDay == 7) weekDay = 0;

            DayOfWeek dayOfWeek = (DayOfWeek)weekDay;

            DateOnly startDate = new(eventObj.EventStartDate.Year, eventObj.EventStartDate.Month, 1);

            while (startDate <= eventObj.EventEndDate)
            {
                DateOnly firstWeekDayOfMonth = new(startDate.Year, startDate.Month, 1);

                DateOnly nthWeekDay = GetFirstGivenWeekDay(firstWeekDayOfMonth, dayOfWeek).AddDays(7 * (weekOrder - 1));

                if (nthWeekDay.Month == startDate.Month && nthWeekDay <= eventObj.EventEndDate)
                {
                    EventCollaborator eventCollaborator = new(eventObj.Id, GlobalData.GetUser().Id, "organizer", "accept", eventObj.EventStartHour, eventObj.EventEndHour, startDate);

                    ScheduleEvent(eventCollaborator);
                }

                startDate = startDate.AddMonths((int)eventObj.Interval);
            }

        }

        private static DateOnly GetFirstGivenWeekDay(DateOnly firstWeekDayOfMonth, DayOfWeek dayOfWeek)
        {
            while (firstWeekDayOfMonth.DayOfWeek != dayOfWeek)
            {
                firstWeekDayOfMonth = firstWeekDayOfMonth.AddDays(1);
            }

            return firstWeekDayOfMonth;
        }

        private void ScheduleYearlyEvents(Event eventObj)
        {
            if (eventObj.ByMonthDay == null)
            {
                ScheduleYearlyEventsUsingWeekOrderAndWeekDay(eventObj);
            }
            else
            {
                ScheduleYearlyEventsUsingMonthDay(eventObj);
            }
        }

        private void ScheduleYearlyEventsUsingMonthDay(Event eventObj)
        {
            int day = (int)eventObj.ByMonthDay;

            int month = (int)eventObj.ByMonth;

            DateOnly startDate = new(eventObj.EventStartDate.Year, month, GetMinimumDateFromGivenMonthAndDay(day, eventObj.EventStartDate));

            while (true)
            {
                try
                {
                    if (startDate > eventObj.EventEndDate) break;

                    EventCollaborator eventCollaborator = new(eventObj.Id, GlobalData.GetUser().Id, "organizer", "accept", eventObj.EventStartHour, eventObj.EventEndHour, startDate);

                    ScheduleEvent(eventCollaborator);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Some error occurred ! " + ex.Message);
                }

                startDate = startDate.AddYears((int)eventObj.Interval);
            }
        }

        private void ScheduleYearlyEventsUsingWeekOrderAndWeekDay(Event eventObj)
        {
            int weekOrder = (int)eventObj.WeekOrder;

            int weekDay = Convert.ToInt32(eventObj.ByWeekDay.Split(",")[0]);

            DayOfWeek dayOfWeek = (DayOfWeek)weekDay;

            int month = (int)eventObj.ByMonth;

            DateOnly startDate = new(eventObj.EventStartDate.Year, month, 1);

            while (startDate <= eventObj.EventEndDate)
            {
                DateOnly firstWeekDayOfMonth = new DateOnly(startDate.Year, startDate.Month, 1);

                DateOnly nthWeekDay = GetFirstGivenWeekDay(firstWeekDayOfMonth, dayOfWeek).AddDays(7 * (weekOrder - 1));

                if (nthWeekDay.Month == startDate.Month && nthWeekDay <= eventObj.EventEndDate)
                {
                    EventCollaborator eventCollaborator = new(eventObj.Id, GlobalData.GetUser().Id, "organizer", "accept", eventObj.EventStartHour, eventObj.EventEndHour, startDate);

                    ScheduleEvent(eventCollaborator);
                }

                startDate = startDate.AddYears((int)eventObj.Interval);
            }
        }
    }
}