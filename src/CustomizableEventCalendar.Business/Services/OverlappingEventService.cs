//using System.ComponentModel;
//using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;
//using Ical.Net;
//using Ical.Net.CalendarComponents;
//using Ical.Net.DataTypes;
//using Ical.Net.Evaluation;
//using Ical.Net.Serialization;

//namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services
//{
//    internal class OverlappingEventService
//    {
//        private readonly RecurrenceService recurrenceService = new();

//        private readonly EventCollaboratorsService eventCollaboratorsService = new();

//        private readonly ScheduleEventService scheduleEventService = new();


//        public bool IsOverlappingEvent(RecurrencePatternCustom recurrencePatternCustom, string TimeBlock)
//        {

//            List<DateTime> occurrences = [];

//            FindOccurrencesOfEvents(recurrencePatternCustom, TimeBlock, ref occurrences);

//            EventService eventService = new();

//            RecurrenceService recurrenceService = new();

//            List<Event> events = [.. eventService.GetAllEvents().Where(eventObj => eventObj.UserId == GlobalData.user.Id)];

//            foreach (var eventObj in events)
//            {
//                List<DateTime> occurrences1 = [];

//                RecurrencePatternCustom recurrencePatternCustom1 = recurrenceService.GetRecurrencePatternById(eventObj.RecurrenceId);

//                FindOccurrencesOfEvents(recurrencePatternCustom1, eventObj.TimeBlock, ref occurrences1);

//                foreach (var item in occurrences)
//                {
//                    DateTime matchedDate = occurrences1.Find(occurrence => occurrence == item);
//                    if (matchedDate != new DateTime())
//                    {
//                        Console.WriteLine(item + " " + matchedDate);
//                        return true;
//                    }
//                }

//            }

//            return false;
//        }

//        public void FindOccurrencesOfEvents(RecurrencePatternCustom recurrencePattern, string TimeBlock, ref List<DateTime> occurrences)
//        {
//            switch (recurrencePattern.FREQ)
//            {
//                case null:
//                    OccurrencesOfNonRecurrenceEvents(recurrencePattern, TimeBlock, ref occurrences);
//                    break;
//                case "daily":
//                    OccurrencesOfDailyEvents(recurrencePattern, TimeBlock, ref occurrences);
//                    break;
//                case "weekly":
//                    OccurrencesOfWeeklyEvents(recurrencePattern, TimeBlock, ref occurrences);
//                    break;
//                case "monthly":
//                    OccurrencesOfMonthlyEvents(recurrencePattern, TimeBlock, ref occurrences);
//                    break;
//                case "yearly":
//                    OccurrencesOfYearlyEvents(recurrencePattern, TimeBlock, ref occurrences);
//                    break;
//            }

//        }

//        public void OccurrencesOfNonRecurrenceEvents(RecurrencePatternCustom recurrencePatternCustom, string TimeBlock, ref List<DateTime> occurrences)
//        {

//            string startTime = TimeBlock.Split("-")[0];
//            int startHour = GetHourFromTimeBlock(startTime);

//            string endTime = TimeBlock.Split("-")[1];
//            int endHour = GetHourFromTimeBlock(startTime);

//            OccurrencesForSpecificHour(startHour, endHour, recurrencePatternCustom.DTSTART, ref occurrences);
//        }

//        public void OccurrencesOfDailyEvents(RecurrencePatternCustom recurrencePattern, string TimeBlock, ref List<DateTime> occurrences)
//        {
//            HashSet<int> days = [.. recurrencePattern.BYDAY.Split(",").Select(day => Convert.ToInt32(day))];

//            DateTime startDate = recurrencePattern.DTSTART;

//            while (startDate < recurrencePattern.UNTILL)
//            {
//                int day = Convert.ToInt32(startDate.DayOfWeek.ToString("d"));

//                if (day == 0) day = 7;

//                if (days.Contains(day))
//                {

//                    string startTime = TimeBlock.Split("-")[0];
//                    int startHour = GetHourFromTimeBlock(startTime);

//                    string endTime = TimeBlock.Split("-")[1];
//                    int endHour = GetHourFromTimeBlock(endTime);

//                    OccurrencesForSpecificHour(startHour, endHour, startDate, ref occurrences);
//                }
//                startDate = startDate.AddDays(Convert.ToInt32(recurrencePattern.INTERVAL) + 1);
//            }
//        }

//        public int GetHourFromTimeBlock(string timeBlock)
//        {
//            int hour = Convert.ToInt32(timeBlock.Substring(0, timeBlock.Length - 2));
//            hour += (timeBlock.EndsWith("PM") || timeBlock.EndsWith("pm")) && hour != 12 ? 12 : 0;
//            if (hour == 12 && timeBlock.EndsWith("AM")) hour = 0;
//            return hour;
//        }

//        public void OccurrencesForSpecificHour(int startHour, int endHour, DateTime dateTime, ref List<DateTime> occurrences)
//        {
//            while (startHour <= endHour)
//            {
//                occurrences.Add(new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, startHour, 0, 0));

//                startHour++;
//            }
//        }

//        public void OccurrencesOfWeeklyEvents(RecurrencePatternCustom recurrencePattern, string TimeBlock, ref List<DateTime> occurrences)
//        {
//            HashSet<int> weekDays = [.. recurrencePattern.BYDAY.Split(",").Select(weekDay => Convert.ToInt32(weekDay))];

//            DateTime startDate = recurrencePattern.DTSTART;

//            while (startDate < recurrencePattern.UNTILL)
//            {
//                int day = Convert.ToInt32(startDate.DayOfWeek.ToString("d"));

//                if (day == 0) day = 7;

//                if (weekDays.Contains(day))
//                {

//                    string startTime = TimeBlock.Split("-")[0];
//                    int startHour = GetHourFromTimeBlock(startTime);

//                    string endTime = TimeBlock.Split("-")[1];
//                    int endHour = GetHourFromTimeBlock(endTime);

//                    OccurrencesForSpecificHour(startHour, endHour, startDate, ref occurrences);
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

//        public void OccurrencesOfMonthlyEvents(RecurrencePatternCustom recurrencePattern, string TimeBlock, ref List<DateTime> occurrences)
//        {
//            HashSet<int> monthDays = [.. recurrencePattern.BYMONTHDAY.Split(",").Select(monthDay => Convert.ToInt32(monthDay))];

//            HashSet<int> months = CalculateProcessingMonth(recurrencePattern.DTSTART, recurrencePattern.UNTILL,
//                                                              recurrencePattern.INTERVAL);

//            DateTime startDate = new DateTime(recurrencePattern.DTSTART.Year, recurrencePattern.DTSTART.Month, 1);

//            while (months.Contains(startDate.Month))
//            {
//                foreach (var day in monthDays)
//                {
//                    try
//                    {
//                        DateTime scheduleDate = new DateTime(startDate.Year, startDate.Month, Convert.ToInt32(day));

//                        string startTime = TimeBlock.Split("-")[0];
//                        int startHour = GetHourFromTimeBlock(startTime);

//                        string endTime = TimeBlock.Split("-")[1];
//                        int endHour = GetHourFromTimeBlock(endTime);

//                        if (scheduleDate > recurrencePattern.UNTILL.Date) return;

//                        OccurrencesForSpecificHour(startHour, endHour, scheduleDate, ref occurrences);
//                    }
//                    catch (Exception ex)
//                    {
//                        Console.WriteLine("Some error occurred ! " + ex.Message);
//                    }
//                }
//                startDate = startDate.AddMonths(1);
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

//        public void OccurrencesOfYearlyEvents(RecurrencePatternCustom recurrencePattern, string TimeBlock, ref List<DateTime> occurrences)
//        {
//            HashSet<int> years = CalculateProcessingYear(recurrencePattern.DTSTART, recurrencePattern.UNTILL, recurrencePattern.INTERVAL);

//            HashSet<int> months = [.. recurrencePattern.BYMONTH.Split(",").Select(month => Convert.ToInt32(month))];

//            HashSet<int> monthDays = [.. recurrencePattern.BYMONTHDAY.Split(",").Select(monthDays => Convert.ToInt32(monthDays))];

//            DateTime startDate = new DateTime(recurrencePattern.DTSTART.Year, recurrencePattern.DTSTART.Month, 1);

//            while (years.Contains(Convert.ToInt32(startDate.Year.ToString())))
//            {
//                foreach (var day in monthDays)
//                {
//                    try
//                    {
//                        DateTime scheduleDate = new(startDate.Year, startDate.Month, Convert.ToInt32(day), startDate.Hour, startDate.Minute, startDate.Second);

//                        string startTime = TimeBlock.Split("-")[0];
//                        int startHour = GetHourFromTimeBlock(startTime);

//                        string endTime = TimeBlock.Split("-")[1];
//                        int endHour = GetHourFromTimeBlock(endTime);

//                        if (scheduleDate >= recurrencePattern.DTSTART && scheduleDate <= recurrencePattern.UNTILL)
//                        {
//                            OccurrencesForSpecificHour(startHour, endHour, scheduleDate, ref occurrences);
//                        }

//                        if (scheduleDate > recurrencePattern.UNTILL) return;
//                    }
//                    catch (Exception e)
//                    {
//                        Console.WriteLine(e.Message);
//                    }
//                    startDate = startDate.AddYears(1);
//                }
//            }
//        }
//    }
//}