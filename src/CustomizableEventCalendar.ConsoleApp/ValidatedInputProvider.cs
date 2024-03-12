using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.ConsoleApp
{
    internal static class ValidatedInputProvider
    {

        public static DateTime GetValidatedDateTime(string inputMessage)
        {
            Console.Write(inputMessage);

            string dateTime = Console.ReadLine();

            DateTime validatedDateTime;

            while (!ValidationService.IsValidateInput(dateTime, out validatedDateTime, DateTime.TryParse))
            {
                Console.Write(inputMessage);
                dateTime = Console.ReadLine();
            }

            return validatedDateTime;
        }

        public static DateOnly GetValidatedDateOnly(string inputMessage)
        {
            Console.Write(inputMessage);
            string dateTime = Console.ReadLine();

            DateOnly validatedDateTime;

            while (!ValidationService.IsValidateInput(dateTime, out validatedDateTime, DateOnly.TryParse))
            {
                Console.Write(inputMessage);
                dateTime = Console.ReadLine();
            }

            return validatedDateTime;
        }

        public static int GetValidatedInteger(string inputMessage)
        {
            Console.Write(inputMessage);

            string inputFromConsole = Console.ReadLine();

            int choice;

            while (!ValidationService.IsValidateInput(inputFromConsole, out choice, int.TryParse))
            {
                Console.Write(inputMessage);

                inputFromConsole = Console.ReadLine();
            }

            return choice;
        }

        public static string GetValidatedCommaSeparatedInput(string inputMessage)
        {
            Console.Write(inputMessage);
            string input = Console.ReadLine();

            while (!ValidationService.IsValidListOfCommaSeparatedIntegers(input))
            {
                Console.Write(inputMessage);
                input = Console.ReadLine();
            }

            return input;
        }

        public static string GetValidateEmail(string email)
        {

            while (!ValidationService.IsValidEmail(email))
            {
                Console.Write($"Enter Email: ");
                email = Console.ReadLine();
            }

            return email;
        }

        public static string GetValidatedWeekDays(string inputMessage)
        {

            string weekDays = GetValidatedCommaSeparatedInput(inputMessage);

            bool isValidWeekDays = true;

            foreach (var weekDay in weekDays.Split(","))
            {
                if (!ValidationService.IsValidWeekDay(Convert.ToInt32(weekDay)))
                {
                    isValidWeekDays = false;
                    break;
                }
            }

            if (!isValidWeekDays) GetValidatedWeekDays(inputMessage);
            return weekDays;

        }

        public static int GetValidatedMonthDay(string inputMessage)
        {
            int monthDay = GetValidatedInteger(inputMessage);

            while (!ValidationService.IsValidMonthDay(monthDay))
            {
                monthDay = GetValidatedInteger(inputMessage);
            }

            return monthDay;
        }

        public static int GetValidatedMonth(string inputMessage)
        {
            int month = GetValidatedInteger(inputMessage);

            while (!ValidationService.IsValidMonth(month))
            {
                month = GetValidatedInteger(inputMessage);
            }

            return month;
        }

        public static int GetValidated24HourFormatTime(string inputMessage)
        {
            int hour = GetValidatedInteger(inputMessage);

            while (!ValidationService.IsValid24HourTime(hour))
            {
                hour = GetValidatedInteger(inputMessage);
            }

            return hour;
        }

        public static int GetValidated12HourFormatTime(string inputMessage)
        {
            int hour = GetValidatedInteger(inputMessage);

            while (!ValidationService.IsValid24HourTime(hour))
            {
                hour = GetValidatedInteger(inputMessage);
            }

            return hour;
        }

        public static string GetValidatedAbbreviations()
        {
            string inputMessage = "Enter AM or PM";

            Console.WriteLine();
            string abbreviation = Console.ReadLine().ToUpper();

            while (!ValidationService.IsValidAbbreviation(abbreviation))
            {
                Console.WriteLine(inputMessage);
                abbreviation = Console.ReadLine().ToUpper();
            }

            return abbreviation;

        }
    }
}
