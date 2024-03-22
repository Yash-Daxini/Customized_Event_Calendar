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
                PrintHandler.PrintWarningMessage("Invalid Input ! Please enter valid date format (dd-MM-yyyy hh:mm:ss)");
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
                PrintHandler.PrintWarningMessage("Invalid Input ! Please enter valid date format (dd-MM-yyyy)");
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
                PrintHandler.PrintWarningMessage("Invalid Input ! Please enter valid input");

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
                PrintHandler.PrintWarningMessage("Invalid Input ! Please enter valid comma separated values");
                Console.Write(inputMessage);
                input = Console.ReadLine();
            }

            return input;
        }

        public static string GetValidatedCommaSeparatedInputInRange(string inputMessage, int startRange, int endRange)
        {
            Console.Write(inputMessage);
            string input = Console.ReadLine();

            while (!ValidationService.IsValidListOfCommaSeparatedIntegersInRange(input, startRange, endRange))
            {
                PrintHandler.PrintWarningMessage("Invalid Input ! Please enter valid comma separated values");
                Console.Write(inputMessage);
                input = Console.ReadLine();
            }

            return input;
        }

        public static string GetValidateEmail(string email)
        {

            while (!ValidationService.IsValidEmail(email))
            {
                PrintHandler.PrintWarningMessage("Invalid Input ! Please enter valid email");
                Console.Write($"Enter Email: ");
                email = Console.ReadLine();
            }

            return email;
        }

        public static string GetValidatedWeekDays(string inputMessage)
        {
            bool isValidWeekDays = false;

            string weekDays = "";

            while (!isValidWeekDays)
            {
                weekDays = GetValidatedCommaSeparatedInput(inputMessage);
                isValidWeekDays = true;
                foreach (var weekDay in weekDays.Split(","))
                {
                    if (!ValidationService.IsValidWeekDay(Convert.ToInt32(weekDay)))
                    {
                        isValidWeekDays = false;
                        PrintHandler.PrintWarningMessage("Invalid Input ! Please enter week days between 1 to 7");
                        break;
                    }
                }
            }

            return weekDays;

        }

        public static int GetValidatedMonthDay(string inputMessage)
        {
            int monthDay = GetValidatedInteger(inputMessage);

            while (!ValidationService.IsValidMonthDay(monthDay))
            {
                PrintHandler.PrintWarningMessage("Invalid Input ! Please enter month day between 1 to 31");
                monthDay = GetValidatedInteger(inputMessage);
            }

            return monthDay;
        }

        public static int GetValidatedMonth(string inputMessage)
        {
            int month = GetValidatedInteger(inputMessage);

            while (!ValidationService.IsValidMonth(month))
            {
                PrintHandler.PrintWarningMessage("Invalid Input ! Please enter month between 1 to 12");
                month = GetValidatedInteger(inputMessage);
            }

            return month;
        }

        public static int GetValidated24HourFormatTime(string inputMessage)
        {
            int hour = GetValidatedInteger(inputMessage);

            while (!ValidationService.IsValid24HourTime(hour))
            {
                PrintHandler.PrintWarningMessage("Invalid Input ! Please enter hour between 0 to 23");
                hour = GetValidatedInteger(inputMessage);
            }

            return hour;
        }

        public static int GetValidated12HourFormatTime(string inputMessage)
        {
            int hour = GetValidatedInteger(inputMessage);

            while (!ValidationService.IsValid12HourTime(hour))
            {
                PrintHandler.PrintWarningMessage("Invalid Input ! Please enter Hour between 1 to 12");
                hour = GetValidatedInteger(inputMessage);
            }

            return hour;
        }

        public static string GetValidatedAbbreviations()
        {
            string inputMessage = "Enter AM or PM : ";

            Console.WriteLine(inputMessage);
            string abbreviation = Console.ReadLine().ToUpper();

            while (!ValidationService.IsValidAbbreviation(abbreviation))
            {
                PrintHandler.PrintWarningMessage("Invalid Input ! Please enter AM or PM");
                Console.WriteLine(inputMessage);
                abbreviation = Console.ReadLine().ToUpper();
            }

            return abbreviation;

        }

        public static int GetValidatedIntegerBetweenRange(string inputMessage, int startRange, int endRange)
        {
            int number = GetValidatedInteger(inputMessage);
            while (!ValidationService.IsNumberInRange(startRange, endRange, number))
            {
                PrintHandler.PrintWarningMessage($"Invalid Input ! Please enter number between {startRange} to {endRange}");
                number = GetValidatedInteger(inputMessage);
            }
            return number;
        }
    }
}
