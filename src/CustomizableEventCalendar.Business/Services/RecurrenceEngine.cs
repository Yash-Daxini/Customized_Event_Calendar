using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services
{
    internal class RecurrenceEngine
    {
        private readonly ScheduleEventService scheduleEventService = new ScheduleEventService();
        private readonly RecurrenceService recurrenceService = new RecurrenceService();
        private readonly EventCollaboratorsService eventCollaboratorsService = new EventCollaboratorsService();
        public void ScheduleEventsOfThisMonth()
        {
            EventService eventService = new EventService();

            List<EventCollaborators> eventCollaborators = eventCollaboratorsService.Read();

            List<Event> events = eventService.Read()
                                             .ToList();

            foreach (var eventCollaborator in eventCollaborators)
            {
                Event? eventObj = events.FirstOrDefault(eventObj => eventObj.Id == eventCollaborator.EventId);
                if (eventObj == null) continue;
                AddEventToScheduler(eventObj, eventCollaborator.Id);
            }
        }
        public void AddEventToScheduler(Event eventObj, int eventCollaboratorId)
        {
            int recurrenceId = eventObj.RecurrenceId;
            RecurrencePatternCustom recurrencePattern = recurrenceService.Read(recurrenceId);

            ScheduleEvents(eventObj, recurrencePattern, eventCollaboratorId);
        }
        public void ScheduleEvents(Event eventObj, RecurrencePatternCustom recurrencePattern, int eventCollaboratorId) // Event that have recurrence
        {
            List<ScheduleEvent> lastScheduledEvents = scheduleEventService.Read()
                                                                          .Where(data => scheduleEventService.GetEventIdFromEventCollaborators(data.EventCollaboratorsId) == eventObj.Id
                                                                                 && scheduleEventService.GetUserIdFromEventCollaborators(data.EventCollaboratorsId) == GlobalData.user.Id).ToList();

            ScheduleEvent? lastScheduledEvent = lastScheduledEvents.FirstOrDefault(data => data.ScheduledDate == (lastScheduledEvents.Max(data => data.ScheduledDate)));

            DateTime startDate = lastScheduledEvent == null ? recurrencePattern.DTSTART : lastScheduledEvent.ScheduledDate;

            switch (recurrencePattern.FREQ)
            {
                case null:
                    if (lastScheduledEvent == null)
                        ScheduleNonRecurrenceEvents(eventObj, recurrencePattern.DTSTART, recurrencePattern.UNTILL, eventCollaboratorId);
                    break;
                case "daily":
                    ScheduleDailyEvents(eventObj, recurrencePattern, startDate, eventCollaboratorId);
                    break;
                case "weekly":
                    ScheduleWeeklyEvents(eventObj, recurrencePattern, startDate, eventCollaboratorId);
                    break;
                case "monthly":
                    ScheduleMonthlyEvents(eventObj, recurrencePattern, startDate, eventCollaboratorId);
                    break;
                case "yearly":
                    ScheduleYearlyEvents(eventObj, recurrencePattern, startDate, eventCollaboratorId);
                    break;
            }

        }
        public void ScheduleNonRecurrenceEvents(Event eventObj, DateTime startDate, DateTime endDate, int eventCollaboratorId)
        {
            ScheduleEvent scheduleEvent = new ScheduleEvent(eventCollaboratorId, startDate);

            string TimeBlock = eventObj.TimeBlock;

            string startTime = TimeBlock.Split("-")[0];
            int startHour = Convert.ToInt32(startTime.Substring(0, startTime.Length - 2)) + (startTime.EndsWith("PM") ? 12 : 0);

            if (startHour == 12 && startTime.EndsWith("AM")) startHour = 0;

            string endTime = TimeBlock.Split("-")[1];
            int endHour = Convert.ToInt32(endTime.Substring(0, endTime.Length - 2)) + (endTime.EndsWith("PM") ? 12 : 0);

            if (endHour == 12 && endTime.EndsWith("AM")) endHour = 0;

            SchduleForSpecificHour(startHour, endHour, ref scheduleEvent);
        }
        public void ScheduleDailyEvents(Event eventObj, RecurrencePatternCustom recurrencePattern, DateTime startDate, int eventCollaboratorId)
        {
            HashSet<string> days = recurrencePattern.BYDAY.Split(",").ToHashSet<string>();

            if (startDate != recurrencePattern.DTSTART) startDate = startDate.AddDays(Convert.ToInt32(recurrencePattern.INTERVAL + 1));

            while (startDate.Month == Math.Min(DateTime.Now.Month, recurrencePattern.UNTILL.Month))
            {
                string day = startDate.DayOfWeek.ToString("d");

                if (days.Contains(day))
                {
                    ScheduleEvent scheduleEvent = new ScheduleEvent(eventCollaboratorId, startDate);

                    string TimeBlock = eventObj.TimeBlock;

                    string startTime = TimeBlock.Split("-")[0];
                    int startHour = Convert.ToInt32(startTime.Substring(0, startTime.Length - 2)) + (TimeBlock.EndsWith("PM") ? 12 : 0);

                    string endTime = TimeBlock.Split("-")[1];
                    int endHour = Convert.ToInt32(endTime.Substring(0, endTime.Length - 2)) + (TimeBlock.EndsWith("PM") ? 12 : 0);

                    SchduleForSpecificHour(startHour, endHour, ref scheduleEvent);
                }
                startDate = startDate.AddDays(Convert.ToInt32(recurrencePattern.INTERVAL) + 1);
            }
        }
        public void SchduleForSpecificHour(int startHour, int endHour, ref ScheduleEvent scheduleEvent)
        {
            while (startHour < endHour)
            {
                DateTime scheduleDate = scheduleEvent.ScheduledDate;
                scheduleEvent.ScheduledDate = new DateTime(scheduleDate.Year, scheduleDate.Month
                                                          , scheduleDate.Day, startHour, 0, 0);
                scheduleEventService.Create(scheduleEvent);
                startHour++;
            }
        }
        public void ScheduleWeeklyEvents(Event eventObj, RecurrencePatternCustom recurrencePattern, DateTime startDate, int eventCollaboratorId)
        {
            HashSet<string> weekDays = recurrencePattern.BYDAY.Split(",").ToHashSet();

            if (startDate != recurrencePattern.DTSTART) startDate = startDate.AddDays(7 * Convert.ToInt32(recurrencePattern.INTERVAL + 1));

            while (startDate.Month == Math.Min(DateTime.Now.Month, recurrencePattern.UNTILL.Month))
            {
                string day = startDate.DayOfWeek.ToString("d");

                if (weekDays.Contains(day))
                {
                    ScheduleEvent scheduleEvent = new ScheduleEvent(eventCollaboratorId, startDate);

                    string TimeBlock = eventObj.TimeBlock;

                    string startTime = TimeBlock.Split("-")[0];
                    int startHour = Convert.ToInt32(startTime.Substring(0, startTime.Length - 2)) + (TimeBlock.EndsWith("PM") ? 12 : 0);

                    string endTime = TimeBlock.Split("-")[1];
                    int endHour = Convert.ToInt32(endTime.Substring(0, endTime.Length - 2)) + (TimeBlock.EndsWith("PM") ? 12 : 0);

                    SchduleForSpecificHour(startHour, endHour, ref scheduleEvent);
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
        public void ScheduleMonthlyEvents(Event eventObj, RecurrencePatternCustom recurrencePattern, DateTime startDate, int eventCollaboratorId)
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
                    ScheduleEvent scheduleEvent = new ScheduleEvent(eventCollaboratorId, scheduleDate);

                    string TimeBlock = eventObj.TimeBlock;

                    string startTime = TimeBlock.Split("-")[0];
                    int startHour = Convert.ToInt32(startTime.Substring(0, startTime.Length - 2)) + (TimeBlock.EndsWith("PM") ? 12 : 0);

                    string endTime = TimeBlock.Split("-")[1];
                    int endHour = Convert.ToInt32(endTime.Substring(0, endTime.Length - 2)) + (TimeBlock.EndsWith("PM") ? 12 : 0);

                    SchduleForSpecificHour(startHour, endHour, ref scheduleEvent);
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
        public void ScheduleYearlyEvents(Event eventObj, RecurrencePatternCustom recurrencePattern, DateTime startDate, int eventCollaboratorId)
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
                            ScheduleEvent scheduleEvent = new ScheduleEvent(eventCollaboratorId, scheduleDate);

                            scheduleEventService.Create(scheduleEvent);
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