using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Data.Repositories;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services
{
    internal class RecurrenceEngine
    {
        public void AddEventToScheduler(Event eventObj)
        {
            GenericRepository genericRepository = new GenericRepository();
            int recurrenceId = eventObj.RecurrenceId == null ? 0 : eventObj.RecurrenceId.Value;
            if (eventObj.RecurrenceId == null) ScheduleEvents(eventObj);
            else
            {
                RecurrencePattern recurrencePattern = genericRepository.Read<RecurrencePattern>(data => new RecurrencePattern(data), recurrenceId);
                ScheduleEvents(eventObj, recurrencePattern);
            }
        }
        public void ScheduleEvents(Event eventObj)
        {

        }
        public void ScheduleEvents(Event eventObj, RecurrencePattern recurrencePattern)
        {
            GenericRepository repository = new GenericRepository();
            List<Scheduler> lastScheduledEvents = repository.Read(data => new Scheduler(data));
            Scheduler? lastScheduledEvent = lastScheduledEvents.FirstOrDefault(data => data.Date == (lastScheduledEvents.Max(data => data.Date)));
            DateTime startDate = lastScheduledEvent == null ? recurrencePattern.DTSTART : lastScheduledEvent.Date;
            switch (recurrencePattern.FREQ)
            {
                case "daily":
                    ScheduleDailyEvents(eventObj, recurrencePattern, startDate);
                    break;
                case "weekly":
                    break;
                case "monthly":
                    break;
                case "yearly":
                    break;
            }

        }
        public void ScheduleDailyEvents(Event eventObj, RecurrencePattern recurrencePattern, DateTime startDate)
        {
            HashSet<string> days = recurrencePattern.BYDAY.Split(",").ToHashSet<string>();
            if (startDate != recurrencePattern.DTSTART) startDate.AddDays(Convert.ToInt32(recurrencePattern.INTERVAL + 1));
            while (startDate.Month == Math.Min(DateTime.Now.Month, recurrencePattern.UNTILL.Month))
            {
                string day = startDate.DayOfWeek.ToString("d");
                if (days.Contains(day))
                {
                    Scheduler scheduler = new Scheduler(eventObj.Id, startDate);
                    Console.WriteLine(scheduler);
                }
                startDate = startDate.AddDays(Convert.ToInt32(recurrencePattern.INTERVAL + 1));
            }
        }
        public void ScheduleWeeklyEvents(Event eventObj, RecurrencePattern recurrencePattern, DateTime startDate)
        {
            HashSet<string> weekDays = recurrencePattern.BYDAY.Split(",").ToHashSet();
            if (startDate != recurrencePattern.DTSTART) startDate.AddDays(7 * Convert.ToInt32(recurrencePattern.INTERVAL + 1));
            while (startDate.Month == Math.Min(DateTime.Now.Month, recurrencePattern.UNTILL.Month))
            {
                string day = startDate.DayOfWeek.ToString("d");
                if (weekDays.Contains(day))
                {
                    Scheduler scheduler = new Scheduler(eventObj.Id, startDate);
                    Console.WriteLine(scheduler);
                }
                startDate = startDate.AddDays(7 * Convert.ToInt32(recurrencePattern.INTERVAL + 1));
            }
        }
        public void ScheduleMonthlyEvents(Event eventObj, RecurrencePattern recurrencePattern, DateTime startDate)
        {
            HashSet<string> monthDays = recurrencePattern.BYMONTHDAY.Split(",").Where(data => data.Length > 0).ToHashSet();
            if (startDate != recurrencePattern.DTSTART) startDate.AddMonths(Convert.ToInt32(recurrencePattern.INTERVAL + 1));
            if (startDate.Month == Math.Min(DateTime.Now.Month, recurrencePattern.UNTILL.Month))
            {
                foreach (var day in monthDays)
                {
                    Console.WriteLine("COME");
                    Console.WriteLine(day.Length);
                    DateTime scheduleDate = new DateTime(startDate.Year, startDate.Month, Convert.ToInt32(day));
                    Scheduler scheduler = new Scheduler(eventObj.Id, scheduleDate);
                    Console.WriteLine(scheduler);
                }
                startDate = startDate.AddMonths(Convert.ToInt32(recurrencePattern.INTERVAL + 1));
            }
        }
        public void ScheduleYearlyEvents(Event eventObj, RecurrencePattern recurrencePattern, DateTime startDate)
        {
            HashSet<string> month = recurrencePattern.BYMONTH.Split(",").Where(data => data.Length > 0).ToHashSet();
            HashSet<string> monthDays = recurrencePattern.BYMONTHDAY.Split(",").Where(data => data.Length > 0).ToHashSet();
            if (startDate != recurrencePattern.DTSTART) startDate.AddMonths(Convert.ToInt32(recurrencePattern.INTERVAL + 1));
            if (startDate.Year == Math.Min(DateTime.Now.Year, recurrencePattern.UNTILL.Year))
            {
                foreach (var day in monthDays)
                {
                    Console.WriteLine("COME");
                    Console.WriteLine(day.Length);
                    DateTime scheduleDate = new DateTime(startDate.Year, startDate.Month, Convert.ToInt32(day));
                    Scheduler scheduler = new Scheduler(eventObj.Id, scheduleDate);
                    Console.WriteLine(scheduler);
                }
                startDate = startDate.AddMonths(Convert.ToInt32(recurrencePattern.INTERVAL + 1));
            }
        }
    }
}
