using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services
{
    internal class CalendarViewService
    {
        public string GenerateDailyView()
        {
            DateTime todayDate = DateTime.Today;

            StringBuilder dailyView = new StringBuilder();
            dailyView.AppendLine("\n\t\t\t\t\tSchedule of date :- " + todayDate.ToString("dd-MM-yyyy"));

            while (todayDate.Date <= DateTime.Today.Date)
            {
                dailyView.AppendLine("\t\t\t\t\t\t" + todayDate.ToString("hh:mm:ss tt"));
                todayDate = todayDate.AddHours(1);
            }

            return dailyView.ToString();
        }
        public string GenerateWeeklyView()
        {
            StringBuilder weeklyView = new StringBuilder();

            DayOfWeek dayOfWeek = DateTime.Now.DayOfWeek;

            int dayTillCurrentDay = dayOfWeek - DayOfWeek.Monday;

            DateTime startDateOfWeek = DateTime.Now.AddDays(-dayTillCurrentDay);
            DateTime endDateOfWeek = startDateOfWeek.AddDays(7);

            weeklyView.AppendLine("\n\t\t\tSchedule from date :- " + startDateOfWeek.ToString("dd-MM-yyyy") + " to date :- " + endDateOfWeek.ToString("dd-MM-yyyy"));

            while (startDateOfWeek < endDateOfWeek)
            {
                weeklyView.AppendLine("\t\t\t\t\t\t" + startDateOfWeek.ToString("dd-MM-yyyy") + " " + startDateOfWeek.ToString("dddd"));
                startDateOfWeek = startDateOfWeek.AddDays(1);
            }

            return weeklyView.ToString();
        }
        public string GenerateMonthView()
        {
            StringBuilder monthlyView = new StringBuilder();

            DateTime todayDate = DateTime.Now;

            DateTime startDateOfMonth = new DateTime(todayDate.Year, todayDate.Month, 1);
            DateTime endDateOfMonth = new DateTime(todayDate.Year, todayDate.Month, DateTime.DaysInMonth(todayDate.Year, todayDate.Month));

            monthlyView.AppendLine("\n\t\t\tSchedule from date :- " + startDateOfMonth.ToString("dd-MM-yyyy") + " to date :- " + endDateOfMonth.ToString("dd-MM-yyyy"));

            while (startDateOfMonth.Date <= endDateOfMonth.Date)
            {
                monthlyView.AppendLine("\t\t\t\t\t\t" + startDateOfMonth.ToString("dd-MM-yyyy") + " " + startDateOfMonth.DayOfWeek);
                startDateOfMonth = startDateOfMonth.AddDays(1);
            }

            return monthlyView.ToString();
        }
    }
}
