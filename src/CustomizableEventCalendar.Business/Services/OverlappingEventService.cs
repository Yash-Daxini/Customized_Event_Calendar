using CustomizableEventCalendar.src.CustomizableEventCalendar.ConsoleApp;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services
{
    internal class OverlappingEventService
    {
        public Object? GetOverlappedEventInformation(Event eventForVerify, bool isInsert)
        {

            List<DateTime> occurrencesOfEventForVerify = [];

            FindOccurrencesOfEvents(eventForVerify, ref occurrencesOfEventForVerify);

            EventService eventService = new();

            List<Event> events = eventService.GetAllEventsOfLoggedInUser();

            foreach (var eventToCheckOverlap in events)
            {

                if (!isInsert && eventToCheckOverlap.Id == eventForVerify.Id) continue;

                List<DateTime> occurrencesOfEventToCheckOverlap = [];

                FindOccurrencesOfEvents(eventToCheckOverlap, ref occurrencesOfEventToCheckOverlap);

                foreach (var occurrence in occurrencesOfEventForVerify)
                {
                    DateTime matchedDate = occurrencesOfEventToCheckOverlap.Find(singlOccurrence => singlOccurrence == occurrence);
                    if (matchedDate != new DateTime())
                    {
                        return new { OverlappedEvent = eventToCheckOverlap, MatchedDate = DateOnly.FromDateTime(matchedDate) };
                    }
                }

            }

            return null;
        }

        private void FindOccurrencesOfEvents(Event eventObj, ref List<DateTime> occurrences)
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

        public static void OccurrencesOfNonRecurrenceEvents(Event eventObj, ref List<DateTime> occurrences)
        {
            OccurrencesForSpecificHour(eventObj.EventStartHour, eventObj.EventEndHour, eventObj.EventStartDate, ref occurrences);
        }

        private static void OccurrencesOfDailyEvents(Event eventObj, ref List<DateTime> occurrences)
        {
            if (eventObj.Interval == null) OccurrencesOfDailyEventsWithoutInterval(eventObj, ref occurrences);
            else OccurrencesOfDailyEventWithInterval(eventObj, ref occurrences);
        }

        private static void OccurrencesOfDailyEventsWithoutInterval(Event eventObj, ref List<DateTime> occurrences)
        {
            HashSet<int> days = [.. eventObj.ByWeekDay.Split(",").Select(day => Convert.ToInt32(day))];

            DateOnly startDate = eventObj.EventStartDate;

            while (startDate <= eventObj.EventEndDate)
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

        private static void OccurrencesOfDailyEventWithInterval(Event eventObj, ref List<DateTime> occurrences)
        {
            DateOnly startDate = eventObj.EventStartDate;

            while (startDate <= eventObj.EventEndDate)
            {
                OccurrencesForSpecificHour(eventObj.EventStartHour, eventObj.EventEndHour, startDate, ref occurrences);

                startDate = startDate.AddDays(Convert.ToInt32(eventObj.Interval));
            }
        }

        private static void OccurrencesForSpecificHour(int startHour, int endHour, DateOnly date, ref List<DateTime> occurrences)
        {
            while (startHour < endHour)
            {
                occurrences.Add(new DateTime(date.Year, date.Month, date.Day, startHour, 0, 0));

                startHour++;
            }
        }

        private static void OccurrencesOfWeeklyEvents(Event eventObj, ref List<DateTime> occurrences)
        {
            HashSet<int> weekDays = [.. eventObj.ByWeekDay.Split(",").Select(weekDay => Convert.ToInt32(weekDay))];

            DateOnly startDateOfEvent = eventObj.EventStartDate;
            DateOnly endDateOfEvent = eventObj.EventEndDate;

            DateOnly startDateOfWeek = DateTimeManager.GetStartDateOfWeek(startDateOfEvent);
            DateOnly endDateOfWeek = DateTimeManager.GetEndDateOfWeek(startDateOfEvent);

            DateOnly startDate = startDateOfWeek;

            while (startDate <= eventObj.EventEndDate)
            {
                int day = Convert.ToInt32(startDate.DayOfWeek.ToString("d"));

                if (day == 0) day = 7;

                if (weekDays.Contains(day) && IsDateInRange(startDateOfEvent, endDateOfEvent, startDate))
                {
                    OccurrencesForSpecificHour(eventObj.EventStartHour, eventObj.EventEndHour, startDate, ref occurrences);
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

        private static void OccurrencesOfMonthlyEvents(Event eventObj, ref List<DateTime> occurrences)
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

        private static void OccurrencesOfMonthlyEventsUsingMonthDay(Event eventObj, ref List<DateTime> occurrences)
        {
            int day = (int)eventObj.ByMonthDay;

            DateOnly startDate = new(eventObj.EventStartDate.Year, eventObj.EventStartDate.Month, GetMinimumDateFromGivenMonthAndDay(day,
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

                startDate = startDate.AddMonths((int)eventObj.Interval);
                startDate = new DateOnly(startDate.Year, startDate.Month, GetMinimumDateFromGivenMonthAndDay(day, startDate));
            }

        }

        private static int GetMinimumDateFromGivenMonthAndDay(int day, DateOnly date)
        {
            int daysInMonth = DateTime.DaysInMonth(date.Year, date.Month);

            return Math.Min(day, daysInMonth);
        }

        private static void OccurrencesOfMonthlyEventsUsingWeekOrderAndWeekDay(Event eventObj, ref List<DateTime> occurrences)
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

        private static void OccurrencesOfYearlyEventsUsingMonthDay(Event eventObj, ref List<DateTime> occurrences)
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

                startDate = startDate.AddYears((int)eventObj.Interval);
            }
        }

        private static void OccurrencesOfYearlyEventsUsingWeekOrderAndWeekDay(Event eventObj, ref List<DateTime> occurrences)
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