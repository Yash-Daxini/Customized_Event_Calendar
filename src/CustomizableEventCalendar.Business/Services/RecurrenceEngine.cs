using System.Security.Cryptography;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;
using Ical.Net.DataTypes;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services
{
    internal class RecurrenceEngine
    {
        private readonly EventCollaboratorService eventCollaboratorsService = new();

        //public void ScheduleEventsOfThisMonth()
        //{
        //    EventService eventService = new();

        //    List<Event> events = eventService.GetAllEvents()
        //                                     .Where(eventObj => eventObj.UserId == GlobalData.user.Id)
        //                                     .ToList();

        //    foreach (var eventObj in events)
        //    {
        //        ScheduleEvents(eventObj);
        //    }
        //}

        public void ScheduleEvents(Event eventObj)
        {
            List<EventCollaborator> lastScheduledEvents = GetAllPreviousScheduledEvents(eventObj);

            EventCollaborator? lastScheduledEvent = FindLastScheduledEvent(lastScheduledEvents);

            DateTime startDateForScheduling = lastScheduledEvent == null ? DateTime.Parse(eventObj.EventStartDate.ToString()) :
                                         lastScheduledEvent.EventDate;

            if (eventObj.Frequency == null && lastScheduledEvent == null)
            {
                ScheduleNonRecurrenceEvents(eventObj, DateTime.Parse(eventObj.EventStartDate.ToString()), DateTime.Parse
                                           (eventObj.EventEndDate.ToString()));
            }

            ScheduleEventsUsingFrequency(eventObj, startDateForScheduling);

        }

        public void ScheduleEventsUsingFrequency(Event eventObj, DateTime startDate)
        {
            switch (eventObj.Frequency)
            {
                case "daily":
                    ScheduleDailyEvents(eventObj, startDate);
                    break;
                case "weekly":
                    ScheduleWeeklyEvents(eventObj, startDate);
                    break;
                case "monthly":
                    ScheduleMonthlyEvents(eventObj, startDate);
                    break;
                case "yearly":
                    ScheduleYearlyEvents(eventObj, startDate);
                    break;
            }
        }

        public List<EventCollaborator> GetAllPreviousScheduledEvents(Event eventObj)
        {
            List<EventCollaborator> lastScheduledEvents = eventCollaboratorsService.GetAllEventCollaborators()
                                                                          .Where(data => data.EventId == eventObj.Id
                                                                                 && data.UserId == GlobalData.user.Id)
                                                                          .ToList();

            return lastScheduledEvents;
        }

        public static EventCollaborator? FindLastScheduledEvent(List<EventCollaborator> eventCollaborators)
        {
            EventCollaborator? lastScheduledEvent = eventCollaborators.FirstOrDefault(data => data.EventDate ==
                                                                   (eventCollaborators.Max(data => data.EventDate)));

            return lastScheduledEvent;
        }

        public void ScheduleNonRecurrenceEvents(Event eventObj, DateTime startDate, DateTime endDate)
        {
            EventCollaborator eventCollaborator;

            if (eventObj.IsProposed)
            {
                eventCollaborator = new(eventObj.Id, GlobalData.user.Id, "organizer", null, eventObj.EventStartHour, eventObj.EventEndHour,
                                    startDate);
            }
            else
            {
                eventCollaborator = new(eventObj.Id, GlobalData.user.Id, "organizer", null, null, null, startDate);
            }


            ScheduleForSpecificHour(eventObj.EventStartHour, eventObj.EventEndHour, ref eventCollaborator);
        }

        public void ScheduleDailyEvents(Event eventObj, DateTime startDate)
        {
            if (eventObj.Interval == null) ScheduleDailyEventsWithoutInterval(eventObj, startDate);
            else ScheduleDailyEventWithInterval(eventObj, startDate);
        }

        public void ScheduleDailyEventsWithoutInterval(Event eventObj, DateTime startDateOfEvent)
        {
            HashSet<int> days = [.. eventObj.ByWeekDay.Split(",").Select(day => Convert.ToInt32(day))];

            DateOnly startDate = DateOnly.FromDateTime(startDateOfEvent);

            while (startDate < eventObj.EventEndDate)
            {
                int day = Convert.ToInt32(startDate.DayOfWeek.ToString("d"));

                if (day == 0) day = 7;

                if (days.Contains(day))
                {
                    EventCollaborator eventCollaborator = new(eventObj.Id, GlobalData.user.Id, "organizer", null, null, null,
                                                                                                DateTime.Parse(startDate.ToString()));

                    ScheduleForSpecificHour(eventObj.EventStartHour, eventObj.EventEndHour, ref eventCollaborator);
                }
                startDate = startDate.AddDays(1);
            }
        }

        public void ScheduleDailyEventWithInterval(Event eventObj, DateTime startDateOfEvent)
        {
            DateOnly startDate = DateOnly.FromDateTime(startDateOfEvent);

            while (startDate < eventObj.EventEndDate)
            {
                EventCollaborator eventCollaborator = new(eventObj.Id, GlobalData.user.Id, "organizer", null, null, null,
                                                           DateTime.Parse(startDate.ToString()));

                ScheduleForSpecificHour(eventObj.EventStartHour, eventObj.EventEndHour, ref eventCollaborator);

                startDate = startDate.AddDays(Convert.ToInt32(eventObj.Interval) + 1);
            }
        }

        public void ScheduleForSpecificHour(int startHour, int endHour, ref EventCollaborator eventCollaborators)
        {

            while (startHour < endHour)
            {
                DateTime eventDate = eventCollaborators.EventDate;

                eventCollaborators.EventDate = new DateTime(eventDate.Year, eventDate.Month, eventDate.Day, startHour, 0, 0);

                eventCollaboratorsService.InsertEventCollaborators(eventCollaborators);

                startHour++;
            }

        }

        public void ScheduleWeeklyEvents(Event eventObj, DateTime startDate)
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
                    EventCollaborator eventCollaborator = new(eventObj.Id, GlobalData.user.Id, "organizer", null, null, null,
                                                                            DateTime.Parse(curDate.ToString()));

                    ScheduleForSpecificHour(eventObj.EventStartHour, eventObj.EventEndHour, ref eventCollaborator);
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

        public void ScheduleMonthlyEvents(Event eventObj, DateTime startDate)
        {

            if (eventObj.ByMonthDay == null)
            {
                ScheduleMonthlyEventsUsingWeekOrderAndWeekDay(eventObj, startDate);
            }
            else
            {
                ScheduleMonthlyEventsUsingMonthDay(eventObj, startDate);
            }

        }

        public void ScheduleMonthlyEventsUsingMonthDay(Event eventObj, DateTime startDateOfEvent)
        {
            int day = (int)eventObj.ByMonthDay;

            DateOnly curDate = new(startDateOfEvent.Year, startDateOfEvent.Month, GetMinimumDateFromGivenMonthAndDay(day,
                                     DateOnly.FromDateTime(startDateOfEvent)));

            while (true)
            {
                if (curDate > eventObj.EventEndDate) break;

                EventCollaborator eventCollaborator = new(eventObj.Id, GlobalData.user.Id, "organizer", null, null, null,
                                                                        DateTime.Parse(curDate.ToString()));

                ScheduleForSpecificHour(eventObj.EventStartHour, eventObj.EventEndHour, ref eventCollaborator);

                curDate = curDate.AddMonths(1 * (int)eventObj.Interval);
                curDate = new DateOnly(curDate.Year, curDate.Month, GetMinimumDateFromGivenMonthAndDay(day, curDate));
            }

        }

        public static int GetMinimumDateFromGivenMonthAndDay(int day, DateOnly date)
        {
            int daysInMonth = DateTime.DaysInMonth(date.Year, date.Month);

            return Math.Min(day, daysInMonth);
        }

        public void ScheduleMonthlyEventsUsingWeekOrderAndWeekDay(Event eventObj, DateTime startDateOfEvent)
        {
            int weekOrder = (int)eventObj.WeekOrder;

            int weekDay = Convert.ToInt32(eventObj.ByWeekDay.Split(",")[0]);

            if (weekDay == 7) weekDay = 0;

            DayOfWeek dayOfWeek = (DayOfWeek)weekDay;

            DateOnly curDate = new(startDateOfEvent.Year, startDateOfEvent.Month, 1);

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
                    EventCollaborator eventCollaborator = new(eventObj.Id, GlobalData.user.Id, "organizer", null, null, null,
                                                        DateTime.Parse(nthWeekDay.ToString()));

                    ScheduleForSpecificHour(eventObj.EventStartHour, eventObj.EventEndHour, ref eventCollaborator);
                }

                curDate = curDate.AddMonths((int)eventObj.Interval);
            }

        }

        public void ScheduleYearlyEvents(Event eventObj, DateTime startDateOfEvent)
        {
            if (eventObj.ByMonthDay == null)
            {
                ScheduleYearlyEventsUsingWeekOrderAndWeekDay(eventObj, startDateOfEvent);
            }
            else
            {
                ScheduleYearlyEventsUsingMonthDay(eventObj, startDateOfEvent);
            }
        }

        public void ScheduleYearlyEventsUsingMonthDay(Event eventObj, DateTime startDateOfEvent)
        {
            int day = (int)eventObj.ByMonthDay;

            int month = (int)eventObj.ByMonth;

            DateOnly startDate = new(startDateOfEvent.Year, month, GetMinimumDateFromGivenMonthAndDay(day,
                                     DateOnly.FromDateTime(startDateOfEvent)));

            while (true)
            {
                try
                {
                    if (startDate > eventObj.EventEndDate) break;

                    EventCollaborator eventCollaborator = new(eventObj.Id, GlobalData.user.Id, "organizer", null, null, null,
                                                        DateTime.Parse(startDate.ToString()));

                    ScheduleForSpecificHour(eventObj.EventStartHour, eventObj.EventEndHour, ref eventCollaborator);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Some error occurred ! " + ex.Message);
                }

                startDate = startDate.AddYears((int)eventObj.Interval);
            }
        }

        public void ScheduleYearlyEventsUsingWeekOrderAndWeekDay(Event eventObj, DateTime startDateOfEvent)
        {
            int weekOrder = (int)eventObj.WeekOrder;

            int weekDay = Convert.ToInt32(eventObj.ByWeekDay.Split(",")[0]);

            DayOfWeek dayOfWeek = (DayOfWeek)weekDay;

            int month = (int)eventObj.ByMonth;

            DateOnly startDate = new(startDateOfEvent.Year, month, 1);

            while (startDate <= eventObj.EventEndDate)
            {
                DateOnly firstWeekDayOfMonth = new DateOnly(startDate.Year, startDate.Month, 1);

                while (firstWeekDayOfMonth.DayOfWeek != dayOfWeek)
                {
                    firstWeekDayOfMonth = firstWeekDayOfMonth.AddDays(1);
                }

                DateOnly nthWeekDay = firstWeekDayOfMonth.AddDays(7 * (weekOrder - 1));

                if (nthWeekDay.Month == startDate.Month && nthWeekDay <= eventObj.EventEndDate)
                {
                    EventCollaborator eventCollaborator = new(eventObj.Id, GlobalData.user.Id, "organizer", null, null, null,
                                                        DateTime.Parse(nthWeekDay.ToString()));

                    ScheduleForSpecificHour(eventObj.EventStartHour, eventObj.EventEndHour, ref eventCollaborator);
                }

                startDate = startDate.AddYears((int)eventObj.Interval);
            }
        }
    }
}