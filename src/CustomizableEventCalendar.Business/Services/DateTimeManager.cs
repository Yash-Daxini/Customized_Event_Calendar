using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services
{
    internal static class DateTimeManager
    {
        public static string GetDateWithAbbreviationFromDateTime(DateTime dateTime)
        {
            return dateTime.ToString("hh:mm:ss tt");
        }

        public static string GetDateFromDateTime(DateTime dateTime)
        {
            return dateTime.ToString("dd-MM-yyyy");
        }

        public static string GetDayFromDateTime(DateTime dateTime)
        {
            return dateTime.ToString("dddd");
        }

        public static string GetWeekDayFromWeekNumber(int dayNumber)
        {
            if (dayNumber == 7) dayNumber = 0;

            if (dayNumber < 1 || dayNumber > 7)
            {
                return "";
            }

            DayOfWeek dayOfWeek = (DayOfWeek)(dayNumber);

            return dayOfWeek.ToString();
        }

        public static string GetMonthFromMonthNumber(int month)
        {
            if (month <= 0 || month > 12) return "";

            return CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);
        }

        public static string ConvertTo12HourFormat(int hour)
        {
            string abbreviation;

            if (hour >= 12)
            {
                abbreviation = "PM";
                if (hour > 12)
                {
                    hour -= 12;
                }
            }
            else
            {
                abbreviation = "AM";
                if (hour == 0)
                {
                    hour = 12;
                }
            }

            return $"{hour} {abbreviation}";
        }

        public static DateOnly GetStartDateOfWeek(DateOnly todayDate)
        {
            return todayDate.AddDays(-(int)(todayDate.DayOfWeek - 1));
        }

        public static DateOnly GetEndDateOfWeek(DateOnly todayDate)
        {
            return GetStartDateOfWeek(todayDate).AddDays(6);
        }
    }
}
