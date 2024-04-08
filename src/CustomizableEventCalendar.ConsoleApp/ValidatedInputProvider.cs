using CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.ConsoleApp
{
    internal static class ValidatedInputProvider
    {

        public static DateTime GetValidaDateTime(string inputMessage)
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

        public static DateOnly GetValidDateOnly(string inputMessage)
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

        public static int GetValidInteger(string inputMessage)
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

        public static string GetValidCommaSeparatedInput(string inputMessage)
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

        public static string GetValidCommaSeparatedInputInRange(string inputMessage, int startRange, int endRange)
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

        public static string GetValidEmail(string email)
        {

            while (!ValidationService.IsValidEmail(email))
            {
                PrintHandler.PrintWarningMessage("Invalid Input ! Please enter valid email");
                Console.Write($"Enter Email: ");
                email = Console.ReadLine();
            }

            return email;
        }

        public static string GetValidWeekDays(string inputMessage)
        {
            bool isValidWeekDays = false;

            string weekDays = "";

            while (!isValidWeekDays)
            {
                weekDays = GetValidCommaSeparatedInput(inputMessage);
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

        public static int GetValidMonthDay(string inputMessage)
        {
            int monthDay = GetValidInteger(inputMessage);

            while (!ValidationService.IsValidMonthDay(monthDay))
            {
                PrintHandler.PrintWarningMessage("Invalid Input ! Please enter month day between 1 to 31");
                monthDay = GetValidInteger(inputMessage);
            }

            return monthDay;
        }

        public static int GetValidMonth(string inputMessage)
        {
            int month = GetValidInteger(inputMessage);

            while (!ValidationService.IsValidMonth(month))
            {
                PrintHandler.PrintWarningMessage("Invalid Input ! Please enter month between 1 to 12");
                month = GetValidInteger(inputMessage);
            }

            return month;
        }

        public static int GetValid24HourFormatTime(string inputMessage)
        {
            int hour = GetValidInteger(inputMessage);

            while (!ValidationService.IsValid24HourTime(hour))
            {
                PrintHandler.PrintWarningMessage("Invalid Input ! Please enter hour between 0 to 23");
                hour = GetValidInteger(inputMessage);
            }

            return hour;
        }

        public static int GetValid12HourFormatTime(string inputMessage)
        {
            int hour = GetValidInteger(inputMessage);

            while (!ValidationService.IsValid12HourTime(hour))
            {
                PrintHandler.PrintWarningMessage("Invalid Input ! Please enter Hour between 1 to 12");
                hour = GetValidInteger(inputMessage);
            }

            return hour;
        }

        public static string GetValidAbbreviations()
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

        public static int GetValidIntegerBetweenRange(string inputMessage, int startRange, int endRange)
        {
            int number = GetValidInteger(inputMessage);
            while (!ValidationService.IsNumberInRange(startRange, endRange, number))
            {
                PrintHandler.PrintWarningMessage($"Invalid Input ! Please enter number between {startRange} to {endRange}");
                number = GetValidInteger(inputMessage);
            }
            return number;
        }

        public static string GetValidString(string inputMessage)
        {
            Console.WriteLine(inputMessage);
            string? inputString = Console.ReadLine();
            while (!ValidationService.IsValidString(inputString))
            {
                PrintHandler.PrintWarningMessage($"Invalid Input ! Please enter valid input");
                Console.WriteLine(inputMessage);
                inputString = Console.ReadLine();
            }

            return inputString;
        }
    }
}
