using System.Globalization;
using System.Text;

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

        public static string GetDayFromDateTime(DateOnly dateOnly)
        {
            return dateOnly.ToString("dddd");
        }

        public static string GetWeekDayFromWeekNumber(int dayNumber)
        {
            if (dayNumber < 1 || dayNumber > 7)
            {
                return "";
            }

            if (dayNumber == 7) dayNumber = 0;

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

        public static DateTime GetStartDateOfWeek(DateTime todayDate)
        {
            return todayDate.AddDays(-(int)(todayDate.DayOfWeek - 1));
        }

        public static DateTime GetEndDateOfWeek(DateTime todayDate)
        {
            return GetStartDateOfWeek(todayDate).AddDays(6);
        }

        public static DateOnly GetStartDateOfMonth(DateTime todayDate)
        {
            return ConvertToDateOnly(new(todayDate.Year, todayDate.Month, 1));
        }

        public static DateOnly GetEndDateOfMonth(DateTime todayDate)
        {
            return ConvertToDateOnly(new(todayDate.Year, todayDate.Month, DateTime.DaysInMonth(todayDate.Year, todayDate.Month)));
        }

        public static DateOnly ConvertToDateOnly(DateTime dateTime)
        {
            return DateOnly.FromDateTime(dateTime);
        }

        public static string GetWeekDaysFromWeekDayNumbers(string days)
        {
            List<string> listOfDays = [.. days.Split(",").Select(day => day.Trim())];

            StringBuilder daysOfWeek = new();

            foreach (string day in listOfDays)
            {
                if (day.Length == 0) continue;
                daysOfWeek.Append(GetWeekDayFromWeekNumber(Convert.ToInt32(day)) + ",");
            }

            if (daysOfWeek.Length == 0) return "-";
            return daysOfWeek.ToString()[..(daysOfWeek.Length - 1)];
        }

        public static int GetDayNumberFromWeekDay(DateOnly date)
        {
            int dayNumber = Convert.ToInt32(date.DayOfWeek.ToString("d"));
            return dayNumber == 0 ? 7 : dayNumber;
        }
    }
}
