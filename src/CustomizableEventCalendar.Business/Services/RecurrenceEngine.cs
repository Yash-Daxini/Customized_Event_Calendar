using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;
using Ical.Net.DataTypes;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services
{
    internal class RecurrenceEngine
    {
        private readonly EventCollaboratorsService eventCollaboratorsService = new();

        public void ScheduleEventsOfThisMonth()
        {
            EventService eventService = new();

            List<Event> events = eventService.GetAllEvents()
                                             .Where(eventObj => eventObj.UserId == GlobalData.user.Id)
                                             .ToList();

            foreach (var eventObj in events)
            {
                ScheduleEvents(eventObj);
            }
        }

        public void AddEventToScheduler(Event eventObj)
        {
            ScheduleEvents(eventObj);
        }

        public void ScheduleEvents(Event eventObj)
        {
            List<EventCollaborators> lastScheduledEvents = eventCollaboratorsService.GetAllEventCollaborators()
                                                                          .Where(data => data.EventId == eventObj.Id
                                                                                 && data.UserId == GlobalData.user.Id)
                                                                          .ToList();

            EventCollaborators? lastScheduledEvent = lastScheduledEvents.FirstOrDefault(data => data.EventDate ==
                                                                    (lastScheduledEvents.Max(data => data.EventDate)));

            DateTime startDate = lastScheduledEvent == null ? DateTime.Parse(eventObj.EventStartDate.ToString()) : lastScheduledEvent.EventDate;

            switch (eventObj.Frequency)
            {
                case null:
                    if (lastScheduledEvent == null)
                        ScheduleNonRecurrenceEvents(eventObj, DateTime.Parse(eventObj.EventStartDate.ToString()), DateTime.Parse
                                                                                                            (eventObj.EventEndDate.ToString()));
                    break;
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

        public void ScheduleNonRecurrenceEvents(Event eventObj, DateTime startDate, DateTime endDate)
        {
            EventCollaborators eventCollaborator = new(eventObj.Id, GlobalData.user.Id, "organizer", null, null, null, startDate);

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
                    EventCollaborators eventCollaborator = new(eventObj.Id, GlobalData.user.Id, "organizer", null, null, null,
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
                EventCollaborators eventCollaborator = new(eventObj.Id, GlobalData.user.Id, "organizer", null, null, null, DateTime.Parse
                                                                                                                            (startDate.ToString()));

                ScheduleForSpecificHour(eventObj.EventStartHour, eventObj.EventEndHour, ref eventCollaborator);

                startDate = startDate.AddDays(Convert.ToInt32(eventObj.Interval) + 1);
            }
        }

        public void ScheduleForSpecificHour(int startHour, int endHour, ref EventCollaborators eventCollaborators)
        {

            while (startHour < endHour)
            {
                DateTime eventDate = eventCollaborators.EventDate;
                eventCollaborators.EventDate = new DateTime(eventDate.Year, eventDate.Month
                                                          , eventDate.Day, startHour, 0, 0);
                eventCollaboratorsService.InsertEventCollaborators(eventCollaborators);
                startHour++;
            }

        }

        public void ScheduleWeeklyEvents(Event eventObj, DateTime startDate)
        {
            HashSet<int> weekDays = [.. eventObj.ByWeekDay.Split(",").Select(weekDay => Convert.ToInt32(weekDay))];

            DateOnly startDateOfEvent = eventObj.EventStartDate;
            DateOnly endDateOfEvent = eventObj.EventEndDate;

            DateOnly startDateOfWeek = GetStartDateOfWeek(startDateOfEvent);
            DateOnly endDateOfWeek = GetEndDateOfWeek(startDateOfEvent);

            DateOnly curDate = startDateOfWeek;

            while (curDate < eventObj.EventEndDate)
            {
                int day = Convert.ToInt32(startDateOfEvent.DayOfWeek.ToString("d"));

                if (weekDays.Contains(day) && IsDateInRange(startDateOfEvent, endDateOfEvent, curDate))
                {
                    EventCollaborators eventCollaborator = new(eventObj.Id, GlobalData.user.Id, "organizer", null, null, null,
                                                                            DateTime.Parse(startDate.ToString()));

                    ScheduleForSpecificHour(eventObj.EventStartHour, eventObj.EventEndHour, ref eventCollaborator);
                }

                startDateOfEvent = startDateOfEvent.AddDays(Convert.ToInt32(eventObj.Interval) + 1);

                if (curDate > endDateOfEvent)
                {
                    curDate = startDateOfWeek.AddDays(7 * (int)eventObj.Interval);
                    startDateOfWeek = GetStartDateOfWeek(curDate);
                    endDateOfWeek = GetEndDateOfWeek(curDate);
                }
            }
        }

        public bool IsDateInRange(DateOnly startDate, DateOnly endDate, DateOnly dateToCheck)
        {
            return dateToCheck >= startDate && dateToCheck <= endDate;
        }

        public DateOnly GetStartDateOfWeek(DateOnly todayDate)
        {
            return todayDate.AddDays(-(int)(todayDate.DayOfWeek - 1));
        }

        public DateOnly GetEndDateOfWeek(DateOnly todayDate)
        {
            return GetStartDateOfWeek(todayDate).AddDays(6);
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

            DateOnly startDate = new(startDateOfEvent.Year, startDateOfEvent.Month, GetMinimumDateFromGivenMonthAndDay(day,
                                     DateOnly.FromDateTime(startDateOfEvent)));

            while (true)
            {
                try
                {
                    if (startDate > eventObj.EventEndDate) break;

                    EventCollaborators eventCollaborator = new(eventObj.Id, GlobalData.user.Id, "organizer", null, null, null,
                                                                            DateTime.Parse(startDate.ToString()));

                    ScheduleForSpecificHour(eventObj.EventStartHour, eventObj.EventEndHour, ref eventCollaborator);

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

        public void ScheduleMonthlyEventsUsingWeekOrderAndWeekDay(Event eventObj, DateTime startDateOfEvent)
        {
            int weekOrder = (int)eventObj.WeekOrder;

            int weekDay = Convert.ToInt32(eventObj.ByWeekDay.Split(",")[0]);

            if (weekDay == 7) weekDay = 0;

            DayOfWeek dayOfWeek = (DayOfWeek)weekDay;

            DateOnly curDate = new(eventObj.EventStartDate.Year, eventObj.EventEndDate.Month, 1);

            while (curDate <= eventObj.EventEndDate)
            {
                DateOnly firstWeekDayOfMonth = new DateOnly(curDate.Year, curDate.Month, 1);

                while (firstWeekDayOfMonth.DayOfWeek != dayOfWeek)
                {
                    firstWeekDayOfMonth = firstWeekDayOfMonth.AddDays(1);
                }

                DateOnly nthWeekDay = firstWeekDayOfMonth.AddDays(7 * (weekOrder - 1));

                if (nthWeekDay.Month == curDate.Month)
                {
                    EventCollaborators eventCollaborator = new(eventObj.Id, GlobalData.user.Id, "organizer", null, null, null,
                                                        DateTime.Parse(curDate.ToString()));

                    ScheduleForSpecificHour(eventObj.EventStartHour, eventObj.EventEndHour, ref eventCollaborator);
                }

                curDate = curDate.AddMonths((int)eventObj.Interval);
            }

        }

        public HashSet<int> CalculateProcessingYear(DateTime startDate, DateTime endDate, string interval)
        {

            HashSet<int> years = [];
            DateTime curDate = startDate;

            while (curDate <= endDate)
            {
                years.Add(Convert.ToInt32(curDate.Year.ToString()));
                curDate = curDate.AddYears(Convert.ToInt32(interval) + 1);
            }

            return years;

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

                    EventCollaborators eventCollaborator = new(eventObj.Id, GlobalData.user.Id, "organizer", null, null, null,
                                                        DateTime.Parse(startDate.ToString()));

                    ScheduleForSpecificHour(eventObj.EventStartHour, eventObj.EventEndHour, ref eventCollaborator);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Some error occurred ! " + ex.Message);
                }

                startDate = startDate.AddYears(1 * (int)eventObj.Interval);
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

                if (nthWeekDay.Month == startDate.Month)
                {
                    EventCollaborators eventCollaborator = new(eventObj.Id, GlobalData.user.Id, "organizer", null, null, null,
                                                        DateTime.Parse(startDate.ToString()));

                    ScheduleForSpecificHour(eventObj.EventStartHour, eventObj.EventEndHour, ref eventCollaborator);
                }

                startDate = startDate.AddMonths((int)eventObj.Interval);
            }
        }
    }
}