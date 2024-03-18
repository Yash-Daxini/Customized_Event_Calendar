using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CustomizableEventCalendar.src.CustomizableEventCalendar.ConsoleApp;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services
{
    internal static class ValidationService
    {
        public delegate bool GenericTryParse<T>(string input, out T result);

        public static bool IsValidateInput<T>(string inputFromConsole, out T choice,
                                     GenericTryParse<T> genericTryParse)
        {
            if (!genericTryParse(inputFromConsole, out choice))
            {
                return false;
            }
            return true;
        }

        public static bool IsValidListOfCommaSeparatedIntegers(string input)
        {
            string[] numbers = [..input.Split(',')
                                    .Select(number => number.Trim())
                                    .OrderBy(number => number)];

            foreach (var number in numbers)
            {
                if (!int.TryParse(number, out int validateNumber))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool IsValidEmail(string input)
        {
            string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

            Regex regex = new(pattern, RegexOptions.IgnoreCase);

            if (!regex.IsMatch(input))
            {
                return false;
            }
            return true;
        }

        public static bool IsValidWeekDay(int weekDayNumber)
        {
            return weekDayNumber >= 1 && weekDayNumber <= 7;
        }

        public static bool IsValidMonthDay(int monthDayNumber)
        {
            return monthDayNumber >= 1 && monthDayNumber <= 31;
        }

        public static bool IsValidMonth(int month)
        {
            return month >= 1 && month <= 12;
        }

        public static bool IsValid24HourTime(int hour)
        {
            return hour >= 0 && hour <= 23;
        }

        public static bool IsValid12HourTime(int hour)
        {
            return hour >= 1 && hour <= 12;
        }

        public static bool IsValidStartAndEndHour(int start, int end)
        {
            return start < end;
        }

        public static bool IsValidAbbreviation(string abbreviation)
        {
            return abbreviation.Equals("AM") || abbreviation.Equals("PM");
        }

        public static bool IsValidStartAndEndDate(DateOnly startDate, DateOnly endDate)
        {
            return startDate <= endDate;
        }

        public static bool IsNumberInRange(int start, int end, int number)
        {
            return number >= start && number <= end;
        }
    }
}
