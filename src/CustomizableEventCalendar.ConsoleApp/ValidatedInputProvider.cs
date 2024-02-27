﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.ConsoleApp
{
    internal class ValidatedInputProvider
    {
        static ValidationService validationService = new ValidationService();
        public static DateTime GetValidatedDateTime(string inputMessage)
        {
            Console.Write(inputMessage);
            string dateTime = Console.ReadLine();

            DateTime validatedDateTime;

            while (!validationService.ValidateInput(dateTime, out validatedDateTime, DateTime.TryParse))
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

            while (!validationService.ValidateInput(dateTime, out validatedDateTime, DateOnly.TryParse))
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

            while (!validationService.ValidateInput(inputFromConsole, out choice, int.TryParse))
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

            while (!validationService.ValidateListOfCommaSeparatedIntegers(input))
            {
                Console.Write(inputMessage);
                input = Console.ReadLine();
            }

            return input;
        }
        public static string GetValidateEmail(string email)
        {

            while (!validationService.ValidateEmail(email))
            {
                Console.Write($"Enter value for Email: ");
                string value = Console.ReadLine();
            }

            return email;
        }
        public static string GetValidatedTimeBlock(string timeBlock)
        {
            while (!validationService.ValidateTimeBlock(timeBlock))
            {
                Console.Write($"Enter value for Time Block: ");
                timeBlock = Console.ReadLine();
            }
            return timeBlock;
        }
    }
}