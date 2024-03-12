//using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;

//namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services
//{
//    internal class RecurrenceEngine
//    {
//        private readonly ScheduleEventService scheduleEventService = new();
//        private readonly RecurrenceService recurrenceService = new();
//        private readonly EventCollaboratorsService eventCollaboratorsService = new();

//        public void ScheduleEventsOfThisMonth()
//        {
//            EventService eventService = new();

//            List<EventCollaborators> eventCollaborators = eventCollaboratorsService.GetAllEventCollaborators()
//                                                                                   .Where(eventCollaborator =>
//                                                                                    eventCollaborator.UserId ==
//                                                                                    GlobalData.user.Id)
//                                                                                   .ToList();

//            List<Event> events = eventService.GetAllEvents()
//                                             .ToList();

//            foreach (var eventCollaborator in eventCollaborators)
//            {
//                Event? eventObj = events.FirstOrDefault(eventObj => eventObj.Id == eventCollaborator.EventId
//                                                           && eventObj.UserId == GlobalData.user.Id);

//                if (eventObj == null) continue;

//                AddEventToScheduler(eventObj, eventCollaborator.Id);
//            }
//        }

//        public void AddEventToScheduler(Event eventObj, int eventCollaboratorId)
//        {
//            int recurrenceId = eventObj.RecurrenceId;

//            RecurrencePatternCustom recurrencePattern = recurrenceService.GetRecurrencePatternById(recurrenceId);

//            ScheduleEvents(eventObj, recurrencePattern, eventCollaboratorId);
//        }

//        public void ScheduleEvents(Event eventObj, RecurrencePatternCustom recurrencePattern, int eventCollaboratorId)
//        // Event that have recurrence
//        {
//            List<ScheduleEvent> lastScheduledEvents = scheduleEventService.GetAllScheduleEvents()
//                                                                          .Where(data => scheduleEventService
//                                                                                        .GetEventIdFromEventCollaborators
//                                                                                         (data.EventCollaboratorsId)
//                                                                                         == eventObj.Id
//                                                                                         && scheduleEventService
//                                                                                        .GetUserIdFromEventCollaborators
//                                                                                         (data.EventCollaboratorsId)
//                                                                                         == GlobalData.user.Id)
//                                                                          .ToList();

//            ScheduleEvent? lastScheduledEvent = lastScheduledEvents.FirstOrDefault(data => data.ScheduledDate ==
//                                                                    (lastScheduledEvents.Max(data => data.ScheduledDate)));

//            DateTime startDate = lastScheduledEvent == null ? recurrencePattern.DTSTART : lastScheduledEvent.ScheduledDate;

//            switch (recurrencePattern.FREQ)
//            {
//                case null:
//                    if (lastScheduledEvent == null)
//                        ScheduleNonRecurrenceEvents(eventObj, recurrencePattern.DTSTART, recurrencePattern.UNTILL, eventCollaboratorId);
//                    break;
//                case "daily":
//                    ScheduleDailyEvents(eventObj, recurrencePattern, startDate, eventCollaboratorId);
//                    break;
//                case "weekly":
//                    ScheduleWeeklyEvents(eventObj, recurrencePattern, startDate, eventCollaboratorId);
//                    break;
//                case "monthly":
//                    ScheduleMonthlyEvents(eventObj, recurrencePattern, startDate, eventCollaboratorId);
//                    break;
//                case "yearly":
//                    ScheduleYearlyEvents(eventObj, recurrencePattern, startDate, eventCollaboratorId);
//                    break;
//            }

//        }

//        public void ScheduleNonRecurrenceEvents(Event eventObj, DateTime startDate, DateTime endDate, int eventCollaboratorId)
//        {
//            ScheduleEvent scheduleEvent = new(eventCollaboratorId, startDate);

//            string TimeBlock = eventObj.TimeBlock;

//            string startTime = TimeBlock.Split("-")[0];
//            int startHour = Convert.ToInt32(startTime.Substring(0, startTime.Length - 2))
//                            + (startTime.EndsWith("PM") ? 12 : 0);

//            if (startHour == 12 && startTime.EndsWith("AM")) startHour = 0;

//            string endTime = TimeBlock.Split("-")[1];
//            int endHour = Convert.ToInt32(endTime.Substring(0, endTime.Length - 2)) + (endTime.EndsWith("PM") ? 12 : 0);

//            if (endHour == 12 && endTime.EndsWith("AM")) endHour = 0;

//            SchduleForSpecificHour(startHour, endHour, ref scheduleEvent);
//        }

//        public void ScheduleDailyEvents(Event eventObj, RecurrencePatternCustom recurrencePattern, DateTime startDate, int
//                                        eventCollaboratorId)
//        {
//            HashSet<int> days = [.. recurrencePattern.BYDAY.Split(",").Select(day => Convert.ToInt32(day))];

//            if (startDate != recurrencePattern.DTSTART) startDate = startDate.AddDays(
//                                                                            Convert.ToInt32(recurrencePattern.INTERVAL + 1)
//                                                                                     );

//            while (startDate.Month <= Math.Min(DateTime.Now.Month, recurrencePattern.UNTILL.Month) && startDate.Date <= recurrencePattern.UNTILL.Date)
//            {
//                int day = Convert.ToInt32(startDate.DayOfWeek.ToString("d"));

//                if (day == 0) day = 7;

//                if (days.Contains(day))
//                {
//                    ScheduleEvent scheduleEvent = new ScheduleEvent(eventCollaboratorId, startDate);

//                    string TimeBlock = eventObj.TimeBlock;

//                    string startTime = TimeBlock.Split("-")[0];
//                    int startHour = Convert.ToInt32(startTime.Substring(0, startTime.Length - 2)) + (TimeBlock.EndsWith("PM") ? 12 : 0);

//                    string endTime = TimeBlock.Split("-")[1];
//                    int endHour = Convert.ToInt32(endTime.Substring(0, endTime.Length - 2)) + (TimeBlock.EndsWith("PM") ? 12 : 0);

//                    SchduleForSpecificHour(startHour, endHour, ref scheduleEvent);
//                }
//                startDate = startDate.AddDays(Convert.ToInt32(recurrencePattern.INTERVAL) + 1);
//            }
//        }

//        public void SchduleForSpecificHour(int startHour, int endHour, ref ScheduleEvent scheduleEvent)
//        {

//            while (startHour < endHour)
//            {
//                DateTime scheduleDate = scheduleEvent.ScheduledDate;
//                scheduleEvent.ScheduledDate = new DateTime(scheduleDate.Year, scheduleDate.Month
//                                                          , scheduleDate.Day, startHour, 0, 0);
//                scheduleEventService.Create(scheduleEvent);
//                startHour++;
//            }

//        }

//        public void ScheduleWeeklyEvents(Event eventObj, RecurrencePatternCustom recurrencePattern, DateTime startDate, int eventCollaboratorId)
//        {
//            HashSet<int> weekDays = [.. recurrencePattern.BYDAY.Split(",").Select(weekDay => Convert.ToInt32(weekDay))];

//            if (startDate != recurrencePattern.DTSTART) startDate = startDate.AddDays(Convert.ToInt32(recurrencePattern.INTERVAL + 1));

//            while (startDate.Month == Math.Min(DateTime.Now.Month, recurrencePattern.UNTILL.Month))
//            {
//                int day = Convert.ToInt32(startDate.DayOfWeek.ToString("d"));

//                if (day == 0) day = 7;

//                if (weekDays.Contains(day))
//                {
//                    ScheduleEvent scheduleEvent = new(eventCollaboratorId, startDate);

//                    string TimeBlock = eventObj.TimeBlock;

//                    string startTime = TimeBlock.Split("-")[0];
//                    int startHour = Convert.ToInt32(startTime.Substring(0, startTime.Length - 2)) + (TimeBlock.EndsWith("PM") ? 12 : 0);

//                    string endTime = TimeBlock.Split("-")[1];
//                    int endHour = Convert.ToInt32(endTime.Substring(0, endTime.Length - 2)) + (TimeBlock.EndsWith("PM") ? 12 : 0);

//                    SchduleForSpecificHour(startHour, endHour, ref scheduleEvent);
//                }
//                startDate = startDate.AddDays(Convert.ToInt32(recurrencePattern.INTERVAL) + 1);
//            }
//        }

//        public HashSet<int> CalculateProcessingMonth(DateTime startDate, DateTime endDate, string interval)
//        {
//            HashSet<int> months = [];
//            DateTime curDate = startDate;

//            while (curDate <= endDate)
//            {
//                months.Add(Convert.ToInt32(curDate.Month.ToString()));
//                curDate = curDate.AddMonths(Convert.ToInt32(interval) + 1);
//            }

//            return months;
//        }

//        public void ScheduleMonthlyEvents(Event eventObj, RecurrencePatternCustom recurrencePattern, DateTime startDate,
//                                          int eventCollaboratorId)
//        {
//            HashSet<int> monthDays = [.. recurrencePattern.BYMONTHDAY.Split(",").Select(monthDay => Convert.ToInt32(monthDay))];

//            HashSet<int> months = CalculateProcessingMonth(recurrencePattern.DTSTART, recurrencePattern.UNTILL,
//                                                              recurrencePattern.INTERVAL);

//            if (startDate != recurrencePattern.DTSTART)
//            {
//                DateTime newDate = startDate.AddMonths(1);
//                startDate = new DateTime(newDate.Year, newDate.Month, 1, newDate.Hour, newDate.Minute, newDate.Second);
//            }

//            if (months.Contains(Convert.ToInt32(startDate.Month.ToString())))
//            {

//                foreach (var day in monthDays)
//                {
//                    DateTime scheduleDate = new DateTime(startDate.Year, startDate.Month, Convert.ToInt32(day),
//                                                         startDate.Hour, startDate.Minute, startDate.Second);

//                    ScheduleEvent scheduleEvent = new(eventCollaboratorId, scheduleDate);

//                    string TimeBlock = eventObj.TimeBlock;

//                    string startTime = TimeBlock.Split("-")[0];
//                    int startHour = Convert.ToInt32(startTime.Substring(0, startTime.Length - 2)) + (TimeBlock.EndsWith("PM") ? 12 : 0);

//                    string endTime = TimeBlock.Split("-")[1];
//                    int endHour = Convert.ToInt32(endTime.Substring(0, endTime.Length - 2)) + (TimeBlock.EndsWith("PM") ? 12 : 0);

//                    SchduleForSpecificHour(startHour, endHour, ref scheduleEvent);
//                }
//            }
//        }

//        public HashSet<int> CalculateProcessingYear(DateTime startDate, DateTime endDate, string interval)
//        {

//            HashSet<int> years = [];
//            DateTime curDate = startDate;

//            while (curDate <= endDate)
//            {
//                years.Add(Convert.ToInt32(curDate.Year.ToString()));
//                curDate = curDate.AddYears(Convert.ToInt32(interval) + 1);
//            }

//            return years;

//        }

//        public void ScheduleYearlyEvents(Event eventObj, RecurrencePatternCustom recurrencePattern, DateTime startDate, int eventCollaboratorId)
//        {
//            HashSet<int> years = CalculateProcessingYear(recurrencePattern.DTSTART, recurrencePattern.UNTILL, recurrencePattern.INTERVAL);

//            HashSet<int> months = [.. recurrencePattern.BYMONTH.Split(",").Select(month => Convert.ToInt32(month))];

//            HashSet<int> monthDays = [.. recurrencePattern.BYMONTHDAY.Split(",").Select(monthDays => Convert.ToInt32(monthDays))];

//            if (startDate != recurrencePattern.DTSTART)
//            {
//                DateTime newDate = startDate.AddMonths(1);
//                startDate = new DateTime(newDate.Year, newDate.Month, 1, newDate.Hour, newDate.Minute, newDate.Second);
//            }

//            if (years.Contains(Convert.ToInt32(startDate.Year.ToString())) && months.Contains(Convert.ToInt32(startDate.Month.ToString())))
//            {
//                foreach (var day in monthDays)
//                {
//                    try
//                    {
//                        DateTime scheduleDate = new(startDate.Year, startDate.Month, Convert.ToInt32(day), startDate.Hour, startDate.Minute, startDate.Second);

//                        if (scheduleDate >= recurrencePattern.DTSTART && scheduleDate <= recurrencePattern.UNTILL)
//                        {
//                            ScheduleEvent scheduleEvent = new(eventCollaboratorId, scheduleDate);

//                            scheduleEventService.Create(scheduleEvent);
//                        }
//                    }
//                    catch (Exception e)
//                    {
//                        Console.WriteLine(e.Message);
//                    }
//                }
//            }
//        }
//    }
//}