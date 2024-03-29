﻿using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Channels;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services;
using CustomizableEventCalendar.src.CustomizableEventCalendar.ConsoleApp.InputMessageStore;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Enums;
using Ical.Net.DataTypes;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.ConsoleApp
{
    internal static class RecurrenceHandling
    {
        public static void AskForRecurrenceChoice(Event eventObj)
        {
            PrintHandler.PrintNewLine();

            int choice = ValidatedInputProvider.GetValidatedIntegerBetweenRange("Are you want repeat this event ? \n 1. Yes 2. No :- ", 1, 2);

            RecurrencePatternChoice isRepeative = (RecurrencePatternChoice)choice;

            switch (isRepeative)
            {
                case RecurrencePatternChoice.Yes:
                    GetRecurrencePattern(eventObj);
                    break;
                case RecurrencePatternChoice.No:
                    GetRecurrenceForSingleEvent(eventObj);
                    break;
                default:
                    Console.WriteLine("Please enter correct value : ");
                    AskForRecurrenceChoice(eventObj);
                    break;
            }

        }

        private static void GetDates(Event eventObj)
        {
            Console.WriteLine("Enter dates for the event :- ");

            PrintHandler.PrintNewLine();

            eventObj.EventStartDate = DateOnly.FromDateTime(ValidatedInputProvider.GetValidatedDateTime(RecurrencePatternMessages.StartDate));

            PrintHandler.PrintNewLine();

            eventObj.EventEndDate = DateOnly.FromDateTime(ValidatedInputProvider.GetValidatedDateTime(RecurrencePatternMessages.EndDate));

            if (!ValidationService.IsValidStartAndEndDate(eventObj.EventStartDate, eventObj.EventEndDate))
            {
                PrintHandler.PrintWarningMessage("Invalid input ! Start date must less than or equal to the end date ");
                GetDates(eventObj);
            }
        }

        public static void GetRecurrenceForSingleEvent(Event eventObj)
        {
            PrintHandler.PrintNewLine();
            eventObj.EventStartDate = DateOnly.FromDateTime(ValidatedInputProvider.GetValidatedDateTime("Enter event date : "));
            eventObj.EventEndDate = eventObj.EventStartDate;
        }

        private static void GetRecurrencePattern(Event eventObj)
        {

            PrintHandler.PrintNewLine();

            Console.WriteLine("Fill details to make event repetitive :- ");

            GetDates(eventObj);

            int frequency = ValidatedInputProvider.GetValidatedIntegerBetweenRange(RecurrencePatternMessages.Frequency, 1, 5);

            RecurrencePatternFrequency choiceForFreq = (RecurrencePatternFrequency)frequency;

            HandleRecurrenceFrequency(choiceForFreq, eventObj);
        }

        private static void HandleRecurrenceFrequency(RecurrencePatternFrequency choiceForFreq, Event eventObj)
        {
            switch (choiceForFreq)
            {

                case RecurrencePatternFrequency.Daily:
                    eventObj.Frequency = "daily";
                    DailyRecurrence(eventObj);
                    break;

                case RecurrencePatternFrequency.Weekly:
                    eventObj.Frequency = "weekly";
                    WeeklyRecurrence(eventObj);
                    break;

                case RecurrencePatternFrequency.Monthly:
                    eventObj.Frequency = "monthly";
                    MonthlyRecurrence(eventObj);
                    break;

                case RecurrencePatternFrequency.Yearly:
                    eventObj.Frequency = "yearly";
                    YearlyRecurrence(eventObj);
                    break;

                default:
                    Console.WriteLine("Please Enter correct option !");
                    GetRecurrencePattern(null);
                    break;
            }
        }

        private static void DailyRecurrence(Event eventObj)
        {
            PrintHandler.PrintNewLine();

            Console.WriteLine("How often does this event occur? \n1.Every Weekday \n2.Every n days (You need to specify the value of n)");
            int choice = ValidatedInputProvider.GetValidatedIntegerBetweenRange("\nEnter choice : ", 1, 2);

            string days;

            switch (choice)
            {
                case 1:
                    PrintHandler.PrintInfoMessage("Great! You've selected to repeat the event every weekday.");
                    eventObj.ByWeekDay = "1,2,3,4,5";
                    eventObj.Interval = null;
                    break;
                case 2:
                    int interval = ValidatedInputProvider.GetValidatedIntegerBetweenRange("Please specify how often you'd like to repeat the event" +
                                                                              "(in days) : ", 1, 5);
                    eventObj.Interval = interval;
                    eventObj.ByWeekDay = null;
                    PrintHandler.PrintInfoMessage($"You've chosen to repeat the event every {eventObj.Interval} days.");
                    break;

            }

            eventObj.ByMonthDay = null;
            eventObj.ByMonth = null;
            eventObj.ByYear = null;
            eventObj.WeekOrder = null;

        }

        private static void WeeklyRecurrence(Event eventObj)
        {
            PrintHandler.PrintNewLine();

            int interval = ValidatedInputProvider.GetValidatedIntegerBetweenRange("Please specify how often you'd like to repeat the event(in weeks) : ", 1, 5);

            eventObj.Interval = interval;

            string days = GetValidWeekDays();

            eventObj.ByWeekDay = days;
            eventObj.WeekOrder = null;
            eventObj.ByMonthDay = null;
            eventObj.ByMonth = null;
            eventObj.ByYear = null;

            PrintHandler.PrintInfoMessage($"You've chosen to repeat the event every {eventObj.Interval} weeks on the following weekdays: " +
                $"{DateTimeManager.GetWeekDaysFromWeekDayNumbers(days)}.");
        }

        private static string GetValidWeekDays()
        {
            PrintHandler.PrintNewLine();

            string days = ValidatedInputProvider.GetValidatedWeekDays("Which weekdays would you like the event to occur on? "
                                                                      + "(Please provide weekdays separated by commas) (i.e. :- 1,4,7 for " +
                                                                        "Monday,Thursday,Sunday) : ");

            return days;

        }

        private static void MonthlyRecurrence(Event eventObj)
        {
            PrintHandler.PrintNewLine();

            int interval = ValidatedInputProvider.GetValidatedIntegerBetweenRange("Please specify how often you'd like to repeat the " +
                                                                      "event (in months) : ", 1, 5);

            eventObj.Interval = interval;

            GetMonthlyFrequencyChoices(eventObj);

        }

        private static void GetMonthlyFrequencyChoices(Event eventObj)
        {
            PrintHandler.PrintNewLine();

            Console.WriteLine("1.Select specific Day of the Month\n2.Select day of the month and its position ");

            int choice = ValidatedInputProvider.GetValidatedIntegerBetweenRange("\nEnter your choice : ", 1, 2);

            switch (choice)
            {
                case 1:
                    GetSpecificMonthDay(eventObj);
                    PrintHandler.PrintInfoMessage($"You've chosen to repeat the event every {eventObj.Interval} months on the {eventObj.ByMonthDay} day of" +
                                                  $" the month");
                    eventObj.WeekOrder = null;
                    eventObj.ByWeekDay = null;
                    break;
                case 2:
                    GetDayOfWeekAndWeekOrderNumber(eventObj);
                    PrintHandler.PrintInfoMessage($"You've chosen to repeat the event every {eventObj.Interval} months on the {eventObj.WeekOrder}th" +
                                                  $" {DateTimeManager.GetWeekDaysFromWeekDayNumbers(eventObj.ByWeekDay)} day of the month");
                    eventObj.ByMonthDay = null;
                    break;
            }

            eventObj.ByMonth = null;
            eventObj.ByYear = null;
        }

        private static void GetSpecificMonthDay(Event eventObj)
        {
            eventObj.ByMonthDay = ValidatedInputProvider.GetValidatedIntegerBetweenRange("On which day of the month would you like the "
                                                                               + "event to occur? (From 1 to 31) : ", 1, 31);
        }

        private static void GetDayOfWeekAndWeekOrderNumber(Event eventObj)
        {

            GetWeekOrder(eventObj);

            GetWeekDay(eventObj);

        }

        private static void GetWeekDay(Event eventObj)
        {
            PrintHandler.PrintNewLine();

            Console.WriteLine("Enter the day of the week (e.g., 'Monday' or 'Tuesday' or 'Wednesday' or 'Thursday' or" +
                              "'Friday' or 'Saturday' or 'Sunday')");
            Console.WriteLine("\n1. Monday \n2. Tuesday \n3. Wednesday \n4. Thursday \n5. Friday \n6. Saturday \n7. " +
                              "Sunday");

            Domain.Enums.WeekDay choice = (Domain.Enums.WeekDay)ValidatedInputProvider.GetValidatedIntegerBetweenRange("\nEnter choice :", 1, 7);
            switch (choice)
            {
                case Domain.Enums.WeekDay.Monday:
                    eventObj.ByWeekDay = "1";
                    break;
                case Domain.Enums.WeekDay.Tuesday:
                    eventObj.ByWeekDay = "2";
                    break;
                case Domain.Enums.WeekDay.Wednesday:
                    eventObj.ByWeekDay = "3";
                    break;
                case Domain.Enums.WeekDay.Thursday:
                    eventObj.ByWeekDay = "4";
                    break;
                case Domain.Enums.WeekDay.Friday:
                    eventObj.ByWeekDay = "5";
                    break;
                case Domain.Enums.WeekDay.Saturday:
                    eventObj.ByWeekDay = "6";
                    break;
                case Domain.Enums.WeekDay.Sunday:
                    eventObj.ByWeekDay = "7";
                    break;
                default:
                    GetWeekDay(eventObj);
                    PrintHandler.PrintErrorMessage("Invalid input !");
                    break;
            }

        }

        private static void GetWeekOrder(Event eventObj)
        {
            PrintHandler.PrintNewLine();

            Console.WriteLine("Enter the order number (e.g., 'first' or 'second' or 'third' or 'fourth' or 'last')");
            Console.WriteLine("\n1. First \n2. Second \n3. Third \n4. Fourth \n5. Fifth");

            WeekOrder choice = (WeekOrder)ValidatedInputProvider.GetValidatedIntegerBetweenRange("\nEnter choice :", 1, 5);

            switch (choice)
            {
                case WeekOrder.First:
                    eventObj.WeekOrder = 1;
                    break;
                case WeekOrder.Second:
                    eventObj.WeekOrder = 2;
                    break;
                case WeekOrder.Third:
                    eventObj.WeekOrder = 3;
                    break;
                case WeekOrder.Fourth:
                    eventObj.WeekOrder = 4;
                    break;
                case WeekOrder.Fifth:
                    eventObj.WeekOrder = 5;
                    break;
                default:
                    GetWeekOrder(eventObj);
                    PrintHandler.PrintErrorMessage("Invalid input !");
                    break;
            }
        }

        private static void YearlyRecurrence(Event eventObj)
        {
            PrintHandler.PrintNewLine();

            eventObj.Interval = ValidatedInputProvider.GetValidatedIntegerBetweenRange("Please specify how often you'd like to repeat " +
                                                                           "the event (in years) : ", 1, 5);

            Console.WriteLine("1.Select specific Day of the Month \n2.Select day of the month and its position ");

            int choice = ValidatedInputProvider.GetValidatedIntegerBetweenRange("\nEnter your choice : ", 1, 2);

            GetValidMonth(eventObj);

            switch (choice)
            {
                case 1:
                    GetSpecificMonthDay(eventObj);
                    PrintHandler.PrintSuccessMessage($"You've chosen to repeat the event every {eventObj.Interval} years on the {eventObj.ByMonthDay} day" +
                                                     $" of the month of {eventObj.ByMonth}");
                    eventObj.WeekOrder = null;
                    eventObj.ByWeekDay = null;
                    break;
                case 2:
                    GetDayOfWeekAndWeekOrderNumber(eventObj);
                    PrintHandler.PrintSuccessMessage($"You've chosen to repeat the event every {eventObj.Interval} years on the {eventObj.WeekOrder}th" +
                        $" {DateTimeManager.GetWeekDaysFromWeekDayNumbers(eventObj.ByWeekDay)} day of the month of {eventObj.ByMonth}");
                    eventObj.ByMonthDay = null;
                    break;
            }

            eventObj.ByYear = null;

        }

        private static void GetValidMonth(Event eventObj)
        {
            PrintHandler.PrintNewLine();

            Console.WriteLine("Enter Months \n1. January \n2. February \n3. March \n4. April \n5. May \n6. June \n7. July " +
                              "\n8. August \n9. September \n10. October \n11. November \n12. December");

            eventObj.ByMonth = ValidatedInputProvider.GetValidatedMonth("Enter the month :");

        }
    }
}