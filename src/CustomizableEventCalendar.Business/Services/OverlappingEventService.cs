using System.ComponentModel;
using CustomizableEventCalendar.src.CustomizableEventCalendar.ConsoleApp;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;
using Ical.Net;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using Ical.Net.Evaluation;
using Ical.Net.Serialization;
using Microsoft.VisualBasic;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services
{
    internal class OverlappingEventService
    {
        public bool IsOverlappingEvent(Event eventForVerify)
        {

            List<DateTime> occurrences = [];

            FindOccurrencesOfEvents(eventForVerify, ref occurrences);

            EventService eventService = new();

            List<Event> events = [.. eventService.GetAllEvents().Where(eventObj => eventObj.UserId == GlobalData.user.Id)];

            foreach (var eventObj in events)
            {
                List<DateTime> occurrences1 = [];

                FindOccurrencesOfEvents(eventObj, ref occurrences1);

                foreach (var occurrence in occurrences)
                {
                    DateTime matchedDate = occurrences1.Find(singlOccurrence => singlOccurrence == occurrence);
                    if (matchedDate != new DateTime())
                    {

                        string message = GetOverlapMessageFromEvents(eventForVerify, eventObj, occurrence, matchedDate);
                        PrintHandler.PrintWarningMessage(message);
                        return true;
                    }
                }

            }

            return false;
        }

        public static string GetOverlapMessageFromEvents(Event eventForVerify, Event eventObj, DateTime occurrence,
                                                  DateTime matchedDate)
        {
            return $"{eventForVerify.Title} overlaps with {eventObj.Title} at following date and time.\n" +
                   $"{occurrence.Date} from {DateTimeManager.ConvertTo12HourFormat(eventForVerify.EventStartHour)} " +
                   $" to {DateTimeManager.ConvertTo12HourFormat(eventForVerify.EventEndHour)} " +
                   $"overlaps with {matchedDate.Date} from {DateTimeManager.ConvertTo12HourFormat(eventObj.EventStartHour)}" +
                   $" to {DateTimeManager.ConvertTo12HourFormat(eventObj.EventEndHour)}" +
                   $"\nPlease choose another date time !";
        }

        public void FindOccurrencesOfEvents(Event eventObj, ref List<DateTime> occurrences)
        {
            switch (eventObj.Frequency)
            {
                case null:
                    OccurrencesOfNonRecurrenceEvents(eventObj, ref occurrences);
                    break;
                case "daily":
                    OccurrencesOfDailyEvents(eventObj, ref occurrences);
                    break;
                case "weekly":
                    OccurrencesOfWeeklyEvents(eventObj, ref occurrences);
                    break;
                case "monthly":
                    OccurrencesOfMonthlyEvents(eventObj, ref occurrences);
                    break;
                case "yearly":
                    OccurrencesOfYearlyEvents(eventObj, ref occurrences);
                    break;
            }

        }

        public void OccurrencesOfNonRecurrenceEvents(Event eventObj, ref List<DateTime> occurrences)
        {
            OccurrencesForSpecificHour(eventObj.EventStartHour, eventObj.EventEndHour, eventObj.EventStartDate, ref occurrences);
        }

        public void OccurrencesOfDailyEvents(Event eventObj, ref List<DateTime> occurrences)
        {
            if (eventObj.Interval == null) OccurrencesOfDailyEventsWithoutInterval(eventObj, ref occurrences);
            else OccurrencesOfDailyEventWithInterval(eventObj, ref occurrences);
        }

        public void OccurrencesOfDailyEventsWithoutInterval(Event eventObj, ref List<DateTime> occurrences)
        {
            HashSet<int> days = [.. eventObj.ByWeekDay.Split(",").Select(day => Convert.ToInt32(day))];

            DateOnly startDate = eventObj.EventStartDate;

            while (startDate < eventObj.EventEndDate)
            {
                int day = Convert.ToInt32(startDate.DayOfWeek.ToString("d"));

                if (day == 0) day = 7;

                if (days.Contains(day))
                {
                    OccurrencesForSpecificHour(eventObj.EventStartHour, eventObj.EventEndHour, startDate, ref occurrences);
                }
                startDate = startDate.AddDays(1);
            }
        }

        public void OccurrencesOfDailyEventWithInterval(Event eventObj, ref List<DateTime> occurrences)
        {
            DateOnly startDate = eventObj.EventStartDate;

            while (startDate < eventObj.EventEndDate)
            {
                OccurrencesForSpecificHour(eventObj.EventStartHour, eventObj.EventEndHour, startDate, ref occurrences);

                startDate = startDate.AddDays(Convert.ToInt32(eventObj.Interval) + 1);
            }
        }

        public void OccurrencesForSpecificHour(int startHour, int endHour, DateOnly date, ref List<DateTime> occurrences)
        {
            while (startHour < endHour)
            {
                occurrences.Add(new DateTime(date.Year, date.Month, date.Day, startHour, 0, 0));

                startHour++;
            }
        }

        public void OccurrencesOfWeeklyEvents(Event eventObj, ref List<DateTime> occurrences)
        {
            HashSet<int> weekDays = [.. eventObj.ByWeekDay.Split(",").Select(weekDay => Convert.ToInt32(weekDay))];

            DateOnly startDateOfEvent = eventObj.EventStartDate;
            DateOnly endDateOfEvent = eventObj.EventEndDate;

            DateOnly startDateOfWeek = DateTimeManager.GetStartDateOfWeek(startDateOfEvent);
            DateOnly endDateOfWeek = DateTimeManager.GetEndDateOfWeek(startDateOfEvent);

            DateOnly curDate = startDateOfWeek;

            while (curDate < eventObj.EventEndDate)
            {
                int day = Convert.ToInt32(curDate.DayOfWeek.ToString("d"));

                if (day == 0) day = 7;

                if (weekDays.Contains(day) && IsDateInRange(startDateOfEvent, endDateOfEvent, curDate))
                {
                    OccurrencesForSpecificHour(eventObj.EventStartHour, eventObj.EventEndHour, startDateOfEvent, ref occurrences);
                }
                curDate = curDate.AddDays(1);

                if (curDate > endDateOfWeek)
                {
                    curDate = startDateOfWeek.AddDays(7 * (int)eventObj.Interval);
                    startDateOfWeek = curDate;
                    endDateOfWeek = DateTimeManager.GetEndDateOfWeek(curDate);
                }
            }
        }

        public static bool IsDateInRange(DateOnly startDate, DateOnly endDate, DateOnly dateToCheck)
        {
            return dateToCheck >= startDate && dateToCheck <= endDate;
        }

        public HashSet<int> CalculateProcessingMonth(DateTime startDate, DateTime endDate, string interval)
        {
            HashSet<int> months = [];
            DateTime curDate = startDate;

            while (curDate <= endDate)
            {
                months.Add(Convert.ToInt32(curDate.Month.ToString()));
                curDate = curDate.AddMonths(Convert.ToInt32(interval) + 1);
            }

            return months;
        }

        public void OccurrencesOfMonthlyEvents(Event eventObj, ref List<DateTime> occurrences)
        {

            if (eventObj.ByMonthDay == null)
            {
                OccurrencesOfMonthlyEventsUsingWeekOrderAndWeekDay(eventObj, ref occurrences);
            }
            else
            {
                OccurrencesOfMonthlyEventsUsingMonthDay(eventObj, ref occurrences);
            }

        }

        public void OccurrencesOfMonthlyEventsUsingMonthDay(Event eventObj, ref List<DateTime> occurrences)
        {
            int day = (int)eventObj.ByMonthDay;

            DateOnly startDate = new();

            startDate = new(eventObj.EventStartDate.Year, eventObj.EventStartDate.Month, GetMinimumDateFromGivenMonthAndDay(day,
                                     eventObj.EventStartDate));


            while (true)
            {
                try
                {
                    if (startDate > eventObj.EventEndDate) break;

                    OccurrencesForSpecificHour(eventObj.EventStartHour, eventObj.EventEndHour, startDate, ref occurrences);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Some error occurred ! " + ex.Message);
                }

                startDate = startDate.AddMonths(1 * (int)eventObj.Interval);
                startDate = new DateOnly(startDate.Year, startDate.Month, GetMinimumDateFromGivenMonthAndDay(day, startDate));
            }

        }

        public int GetMinimumDateFromGivenMonthAndDay(int day, DateOnly date)
        {
            int daysInMonth = DateTime.DaysInMonth(date.Year, date.Month);

            return Math.Min(day, daysInMonth);
        }

        public void OccurrencesOfMonthlyEventsUsingWeekOrderAndWeekDay(Event eventObj, ref List<DateTime> occurrences)
        {
            int weekOrder = (int)eventObj.WeekOrder;

            int weekDay = Convert.ToInt32(eventObj.ByWeekDay.Split(",")[0]);

            DayOfWeek dayOfWeek = (DayOfWeek)weekDay;

            DateOnly curDate = new(eventObj.EventStartDate.Year, eventObj.EventStartDate.Month, 1);

            while (curDate <= eventObj.EventEndDate)
            {
                DateOnly firstWeekDayOfMonth = new(curDate.Year, curDate.Month, 1);

                while (firstWeekDayOfMonth.DayOfWeek != dayOfWeek)
                {
                    firstWeekDayOfMonth = firstWeekDayOfMonth.AddDays(1);
                }

                DateOnly nthWeekDay = firstWeekDayOfMonth.AddDays(7 * (weekOrder - 1));

                if (nthWeekDay.Month == curDate.Month && nthWeekDay <= eventObj.EventEndDate)
                {
                    OccurrencesForSpecificHour(eventObj.EventStartHour, eventObj.EventEndHour, nthWeekDay, ref occurrences);
                }

                curDate = curDate.AddMonths((int)eventObj.Interval);
            }

        }

        public void OccurrencesOfYearlyEvents(Event eventObj, ref List<DateTime> occurrences)
        {
            if (eventObj.ByMonthDay == null)
            {
                OccurrencesOfYearlyEventsUsingWeekOrderAndWeekDay(eventObj, ref occurrences);
            }
            else
            {
                OccurrencesOfYearlyEventsUsingMonthDay(eventObj, ref occurrences);
            }
        }

        public void OccurrencesOfYearlyEventsUsingMonthDay(Event eventObj, ref List<DateTime> occurrences)
        {
            int day = (int)eventObj.ByMonthDay;

            int month = (int)eventObj.ByMonth;

            DateOnly startDate = new(eventObj.EventStartDate.Year, month, GetMinimumDateFromGivenMonthAndDay(day,
                                     eventObj.EventStartDate));

            while (true)
            {
                try
                {
                    if (startDate > eventObj.EventEndDate) break;

                    OccurrencesForSpecificHour(eventObj.EventStartHour, eventObj.EventEndHour, startDate, ref occurrences);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Some error occurred ! " + ex.Message);
                }

                startDate = startDate.AddYears(1 * (int)eventObj.Interval);
            }
        }

        public void OccurrencesOfYearlyEventsUsingWeekOrderAndWeekDay(Event eventObj, ref List<DateTime> occurrences)
        {
            int weekOrder = (int)eventObj.WeekOrder;

            int weekDay = Convert.ToInt32(eventObj.ByWeekDay.Split(",")[0]);

            DayOfWeek dayOfWeek = (DayOfWeek)weekDay;

            int month = (int)eventObj.ByMonth;

            DateOnly curDate = new(eventObj.EventStartDate.Year, month, 1);

            while (curDate <= eventObj.EventEndDate)
            {
                DateOnly firstWeekDayOfMonth = new DateOnly(curDate.Year, curDate.Month, 1);

                while (firstWeekDayOfMonth.DayOfWeek != dayOfWeek)
                {
                    firstWeekDayOfMonth = firstWeekDayOfMonth.AddDays(1);
                }

                DateOnly nthWeekDay = firstWeekDayOfMonth.AddDays(7 * (weekOrder - 1));

                if (nthWeekDay.Month == curDate.Month && nthWeekDay <= eventObj.EventEndDate)
                {
                    OccurrencesForSpecificHour(eventObj.EventStartHour, eventObj.EventEndHour, nthWeekDay, ref occurrences);
                }

                curDate = curDate.AddYears((int)eventObj.Interval);
            }
        }
    }
}