using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services
{
    internal class RecurrenceEngine
    {
        SchedulerService schedulerService = new SchedulerService();
        RecurrenceService recurrenceService = new RecurrenceService();

        public void ScheduleEventsOfThisMonth()
        {
            EventService eventService = new EventService();
            List<Event> events = eventService.Read()
                                             .Where(eventObj => eventObj.UserId == GlobalData.user.Id)
                                             .ToList();
            foreach (var eventObj in events)
            {
                AddEventToScheduler(eventObj);
            }
        }
        public void AddEventToScheduler(Event eventObj)
        {
            int recurrenceId = eventObj.RecurrenceId;
            RecurrencePattern recurrencePattern = recurrenceService.Read(recurrenceId);

            ScheduleEvents(eventObj, recurrencePattern);
        }
        public void ScheduleEvents(Event eventObj, RecurrencePattern recurrencePattern) // Event that have recurrence
        {
            List<Scheduler> lastScheduledEvents = schedulerService.Read().Where(data => data.EventId == eventObj.Id).ToList();
            Scheduler? lastScheduledEvent = lastScheduledEvents.FirstOrDefault(data => data.ScheduledDate == (lastScheduledEvents.Max(data => data.ScheduledDate)));
            DateTime startDate = lastScheduledEvent == null ? recurrencePattern.DTSTART : lastScheduledEvent.ScheduledDate;

            switch (recurrencePattern.FREQ)
            {
                case null:
                    if (lastScheduledEvent == null)
                        ScheduleNonRecurrenceEvents(eventObj, recurrencePattern.DTSTART, recurrencePattern.UNTILL);
                    break;
                case "daily":
                    ScheduleDailyEvents(eventObj, recurrencePattern, startDate);
                    break;
                case "weekly":
                    ScheduleWeeklyEvents(eventObj, recurrencePattern, startDate);
                    break;
                case "monthly":
                    ScheduleMonthlyEvents(eventObj, recurrencePattern, startDate);
                    break;
                case "yearly":
                    ScheduleYearlyEvents(eventObj, recurrencePattern, startDate);
                    break;
            }

        }
        public void ScheduleNonRecurrenceEvents(Event eventObj, DateTime startDate, DateTime endDate)
        {
            Scheduler scheduler = new Scheduler(eventObj.Id, startDate);
            schedulerService.Create(scheduler);
        }
        public void ScheduleDailyEvents(Event eventObj, RecurrencePattern recurrencePattern, DateTime startDate)
        {
            HashSet<string> days = recurrencePattern.BYDAY.Split(",").ToHashSet<string>();

            if (startDate != recurrencePattern.DTSTART) startDate = startDate.AddDays(Convert.ToInt32(recurrencePattern.INTERVAL + 1));

            while (startDate.Month == Math.Min(DateTime.Now.Month, recurrencePattern.UNTILL.Month))
            {
                string day = startDate.DayOfWeek.ToString("d");

                if (days.Contains(day))
                {
                    Scheduler scheduler = new Scheduler(eventObj.Id, startDate);
                    schedulerService.Create(scheduler);
                }
                startDate = startDate.AddDays(Convert.ToInt32(recurrencePattern.INTERVAL) + 1);
            }
        }
        public void ScheduleWeeklyEvents(Event eventObj, RecurrencePattern recurrencePattern, DateTime startDate)
        {
            HashSet<string> weekDays = recurrencePattern.BYDAY.Split(",").ToHashSet();

            if (startDate != recurrencePattern.DTSTART) startDate = startDate.AddDays(7 * Convert.ToInt32(recurrencePattern.INTERVAL + 1));

            while (startDate.Month == Math.Min(DateTime.Now.Month, recurrencePattern.UNTILL.Month))
            {
                string day = startDate.DayOfWeek.ToString("d");

                if (weekDays.Contains(day))
                {
                    Scheduler scheduler = new Scheduler(eventObj.Id, startDate);
                    schedulerService.Create(scheduler);
                }
                startDate = startDate.AddDays(7 * Convert.ToInt32(recurrencePattern.INTERVAL) + 1);
            }
        }
        public HashSet<string> CalculateProcessingMonth(DateTime startDate, DateTime endDate, string interval)
        {
            HashSet<string> months = new HashSet<string>();
            DateTime curDate = startDate;

            while (curDate <= endDate)
            {
                months.Add(curDate.Month.ToString());
                curDate = curDate.AddMonths(Convert.ToInt32(interval) + 1);
            }

            return months;
        }
        public void ScheduleMonthlyEvents(Event eventObj, RecurrencePattern recurrencePattern, DateTime startDate)
        {
            HashSet<string> monthDays = recurrencePattern.BYMONTHDAY.Split(",").Where(data => data.Length > 0).ToHashSet();

            HashSet<string> months = CalculateProcessingMonth(recurrencePattern.DTSTART, recurrencePattern.UNTILL, recurrencePattern.INTERVAL);

            if (startDate != recurrencePattern.DTSTART)
            {
                DateTime newDate = startDate.AddMonths(1);
                startDate = new DateTime(newDate.Year, newDate.Month, 1, newDate.Hour, newDate.Minute, newDate.Second);
            }

            if (months.Contains(startDate.Month.ToString()))
            {
                foreach (var day in monthDays)
                {
                    DateTime scheduleDate = new DateTime(startDate.Year, startDate.Month, Convert.ToInt32(day), startDate.Hour, startDate.Minute, startDate.Second);
                    Scheduler scheduler = new Scheduler(eventObj.Id, scheduleDate);

                    schedulerService.Create(scheduler);
                }
            }
        }
        public HashSet<string> CalculateProcessingYear(DateTime startDate, DateTime endDate, string interval)
        {
            HashSet<string> years = new HashSet<string>();
            DateTime curDate = startDate;

            while (curDate <= endDate)
            {
                years.Add(curDate.Year.ToString());
                curDate = curDate.AddYears(Convert.ToInt32(interval) + 1);
            }

            return years;
        }
        public void ScheduleYearlyEvents(Event eventObj, RecurrencePattern recurrencePattern, DateTime startDate)
        {
            HashSet<string> years = CalculateProcessingYear(recurrencePattern.DTSTART, recurrencePattern.UNTILL, recurrencePattern.INTERVAL);

            HashSet<string> months = recurrencePattern.BYMONTH.Split(",").Where(data => data.Length > 0).ToHashSet();

            HashSet<string> monthDays = recurrencePattern.BYMONTHDAY.Split(",").Where(data => data.Length > 0).ToHashSet();

            if (startDate != recurrencePattern.DTSTART)
            {
                DateTime newDate = startDate.AddMonths(1);
                startDate = new DateTime(newDate.Year, newDate.Month, 1, newDate.Hour, newDate.Minute, newDate.Second);
            }

            if (years.Contains(startDate.Year.ToString()) && months.Contains(startDate.Month.ToString()))
            {
                foreach (var day in monthDays)
                {
                    try
                    {
                        DateTime scheduleDate = new DateTime(startDate.Year, startDate.Month, Convert.ToInt32(day), startDate.Hour, startDate.Minute, startDate.Second);

                        if (scheduleDate >= recurrencePattern.DTSTART && scheduleDate <= recurrencePattern.UNTILL)
                        {
                            Scheduler scheduler = new Scheduler(eventObj.Id, scheduleDate);

                            schedulerService.Create(scheduler);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            }
        }
    }
}
