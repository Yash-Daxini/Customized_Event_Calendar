using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services
{
    internal class CalendarViewService
    {
        EventService eventService = new EventService();
        SchedulerService schedulerService = new SchedulerService();
        public string GenerateDailyView()
        {
            DateTime todayDate = DateTime.Today;

            List<Scheduler> scheduleEvents = schedulerService.Read()
                                            .Where(scheduleEvent => scheduleEvent.ScheduledDate.Date == todayDate.Date)
                                            .ToList();

            Dictionary<int, Event> timeWithEvent = new Dictionary<int, Event>();

            foreach (var scheduleEvent in scheduleEvents)
            {
                Event eventObj = eventService.Read(scheduleEvent.EventId);

                AssignEventToSpecificHour(ref timeWithEvent, eventObj);
            }

            StringBuilder dailyView = new StringBuilder();

            dailyView.AppendLine("\n\t\t\t\t\tSchedule of date :- " + todayDate.ToString("dd-MM-yyyy"));

            while (todayDate.Date <= DateTime.Today.Date)
            {
                int curHour = todayDate.Hour;

                if (timeWithEvent.ContainsKey(curHour))
                {
                    Event eventObj = timeWithEvent[curHour];
                    dailyView.AppendLine("\t\t\t\t\t\t" + todayDate.ToString("hh:mm:ss tt") +
                                        $" Event Name :- {eventObj.Title} , Event Description :- {eventObj.Description}");
                }
                else dailyView.AppendLine("\t\t\t\t\t\t" + todayDate.ToString("hh:mm:ss tt"));

                todayDate = todayDate.AddHours(1);
            }

            return dailyView.ToString();
        }
        public void AssignEventToSpecificHour(ref Dictionary<int, Event> eventRecordByHour, Event eventObj)
        {
            string TimeBlock = eventObj.TimeBlock;

            string startTime = TimeBlock.Split("-")[0];
            int startHour = Convert.ToInt32(startTime.Substring(0, startTime.Length - 2)) + (TimeBlock.EndsWith("PM") ? 12 : 0);

            string endTime = TimeBlock.Split("-")[1];
            int endHour = Convert.ToInt32(endTime.Substring(0, endTime.Length - 2)) + (TimeBlock.EndsWith("PM") ? 12 : 0);

            for (int i = startHour; i <= endHour; i++)
            {
                eventRecordByHour[i] = eventObj;
            }
        }
        public Dictionary<DateTime, int> GetCurrentWeekEvents(DateTime startDateOfWeek, DateTime endDateOfWeek)
        {
            Dictionary<DateTime, int> currentWeekEvents = schedulerService.Read()
                                           .Where(scheduleEvent => scheduleEvent.ScheduledDate.Date >= startDateOfWeek.Date
                                                  && scheduleEvent.ScheduledDate.Date <= endDateOfWeek.Date)
                                           .ToDictionary(key => key.ScheduledDate.Date, val => val.EventId);
            return currentWeekEvents;
        }
        public string GenerateWeeklyView()
        {
            StringBuilder weeklyView = new StringBuilder();

            DayOfWeek dayOfWeek = DateTime.Now.DayOfWeek;

            int dayTillCurrentDay = dayOfWeek - DayOfWeek.Monday;

            DateTime startDateOfWeek = DateTime.Now.AddDays(-dayTillCurrentDay);
            DateTime endDateOfWeek = startDateOfWeek.AddDays(7);

            Dictionary<DateTime, int> currentWeekEvents = GetCurrentWeekEvents(startDateOfWeek, endDateOfWeek);

            weeklyView.AppendLine("\n\t\t\tSchedule from date :- " + startDateOfWeek.ToString("dd-MM-yyyy") + " to date :- " + endDateOfWeek.ToString("dd-MM-yyyy"));

            while (startDateOfWeek < endDateOfWeek)
            {

                if (currentWeekEvents.ContainsKey(startDateOfWeek.Date))
                {
                    Event eventObj = eventService.Read(currentWeekEvents[startDateOfWeek.Date]);
                    weeklyView.AppendLine("\t\t\t\t\t\t" + startDateOfWeek.ToString("dd-MM-yyyy") + " "
                                            + startDateOfWeek.ToString("dddd") +
                                            $" Event :- {eventObj.Title} , Description :- {eventObj.Description}"
                                            + $" , Time :- {eventObj.TimeBlock}");
                }
                else weeklyView.AppendLine("\t\t\t\t\t\t" + startDateOfWeek.ToString("dd-MM-yyyy") + " " + startDateOfWeek.ToString("dddd"));

                startDateOfWeek = startDateOfWeek.AddDays(1);
            }

            return weeklyView.ToString();
        }
        public Dictionary<DateTime, int> GetCurrentMonthEvents(DateTime startDateOfMonth, DateTime endDateOfMonth)
        {
            Dictionary<DateTime, int> currentMonthEvents = schedulerService.Read()
                                           .Where(scheduleEvent => scheduleEvent.ScheduledDate.Date >= startDateOfMonth.Date
                                                  && scheduleEvent.ScheduledDate.Date <= endDateOfMonth.Date)
                                           .ToDictionary(key => key.ScheduledDate.Date, val => val.EventId);
            return currentMonthEvents;
        }
        public string GenerateMonthView()
        {
            StringBuilder monthlyView = new StringBuilder();

            DateTime todayDate = DateTime.Now;

            DateTime startDateOfMonth = new DateTime(todayDate.Year, todayDate.Month, 1);
            DateTime endDateOfMonth = new DateTime(todayDate.Year, todayDate.Month, DateTime.DaysInMonth(todayDate.Year, todayDate.Month));

            Dictionary<DateTime, int> currentMonthEvents = GetCurrentMonthEvents(startDateOfMonth, endDateOfMonth);

            monthlyView.AppendLine("\n\t\t\tSchedule from date :- " + startDateOfMonth.ToString("dd-MM-yyyy") + " to date :- " + endDateOfMonth.ToString("dd-MM-yyyy"));

            while (startDateOfMonth.Date <= endDateOfMonth.Date)
            {

                if (currentMonthEvents.ContainsKey(startDateOfMonth.Date))
                {
                    Event eventObj = eventService.Read(currentMonthEvents[startDateOfMonth.Date]);
                    monthlyView.AppendLine("\t\t\t\t\t\t" + startDateOfMonth.ToString("dd-MM-yyyy") + " "
                                            + startDateOfMonth.DayOfWeek
                                            + $" Event :- {eventObj.Title} , Description :- {eventObj.Description}"
                                            + $" , Time :- {eventObj.TimeBlock}");
                }
                else
                    monthlyView.AppendLine("\t\t\t\t\t\t" + startDateOfMonth.ToString("dd-MM-yyyy") + " " + startDateOfMonth.DayOfWeek);

                startDateOfMonth = startDateOfMonth.AddDays(1);
            }

            return monthlyView.ToString();
        }
    }
}
