using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CustomizableEventCalendar.src.CustomizableEventCalendar.ConsoleApp;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services
{
    internal class ValidationService
    {
        public delegate bool GenericTryParse<T>(string input, out T result);

        public bool ValidateInput<T>(string inputFromConsole, out T choice,
                                     GenericTryParse<T> genericTryParse)
        {
            if (!genericTryParse(inputFromConsole, out choice))
            {
                PrintHandler.PrintInvalidMessage("Invalid Input!");
                return false;
            }
            return true;
        }

        public bool ValidateListOfCommaSeparatedIntegers(string input)
        {
            string[] numbers = input.Split(',')
                                    .Select(number => number.Trim())
                                    .ToArray();

            foreach (var number in numbers)
            {
                if (!int.TryParse(number, out int validateNumber))
                {
                    PrintHandler.PrintInvalidMessage("Invalid Input!");
                    return false;
                }
            }
            return true;
        }

        public bool ValidateTimeBlock(string input)
        {
            string pattern = @"^\s*(?:([1-9]|0[1-9]|1[0-2])\s*(AM|PM))\s*-\s*(?:([1-9]|0[1-9]|1[0-2])\s*(AM|PM))\s*$";

            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);

            if (!regex.IsMatch(input))
            {
                PrintHandler.PrintInvalidMessage("Invalid time block format ! Enter like this format :- 2AM-7PM");
                return false;
            }
            return true;
        }

        public bool ValidateEmail(string input)
        {
            string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);

            if (!regex.IsMatch(input))
            {
                PrintHandler.PrintInvalidMessage("Invalid email address! ");
                return false;
            }
            return true;
        }
    }
}
