using CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Enums;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Models;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.ConsoleApp
{
    internal static class RecurrenceHandling
    {
        public static void AskForRecurrenceChoice(EventModel eventModel)
        {
            PrintHandler.PrintNewLine();

            int choice = ValidatedInputProvider.GetValidIntegerBetweenRange("Are you want repeat this event ? \n 1. Yes 2. No :- ", 1, 2);

            switch (choice)
            {
                case 1:
                    GetRecurrencePattern(eventModel);
                    PrintHandler.PrintInfoMessage("You decided to repeat event " + RecurrencePatternMessageGenerator.GenerateRecurrenceMessage(eventModel));
                    break;
                case 2:
                    GetRecurrenceForSingleEvent(eventModel);
                    PrintHandler.PrintInfoMessage("Event will be " + RecurrencePatternMessageGenerator.GenerateRecurrenceMessage(eventModel));
                    break;
                default:
                    Console.WriteLine("Please enter correct value : ");
                    AskForRecurrenceChoice(eventModel);
                    break;
            }
        }

        private static void GetDates(EventModel eventModel)
        {
            Console.WriteLine("Enter dates for the event :- ");

            PrintHandler.PrintNewLine();

            eventModel.RecurrencePattern.StartDate = DateOnly.FromDateTime(ValidatedInputProvider.GetValidaDateTime("Enter Start Date :-  (Please enter date in dd-mm-yyyy) :- "));

            PrintHandler.PrintNewLine();

            eventModel.RecurrencePattern.EndDate = DateOnly.FromDateTime(ValidatedInputProvider.GetValidaDateTime("Enter End Date :-  (Please enter date in dd-mm-yyyy) :- "));

            if (!ValidationService.IsValidStartAndEndDate(eventModel.RecurrencePattern.StartDate, eventModel.RecurrencePattern.EndDate))
            {
                PrintHandler.PrintWarningMessage("Invalid input ! Start date must less than or equal to the end date ");
                GetDates(eventModel);
            }
        }

        public static void GetRecurrenceForSingleEvent(EventModel eventModel)
        {
            PrintHandler.PrintNewLine();
            eventModel.RecurrencePattern.StartDate = DateOnly.FromDateTime(ValidatedInputProvider.GetValidaDateTime("Enter event date : "));
            eventModel.RecurrencePattern.EndDate = eventModel.RecurrencePattern.StartDate;
        }

        private static void GetRecurrencePattern(EventModel eventModel)
        {

            PrintHandler.PrintNewLine();

            Console.WriteLine("Fill details to make event repetitive :- ");

            GetDates(eventModel);

            int frequency = ValidatedInputProvider.GetValidIntegerBetweenRange("How frequent you want to repeat the event: \n1. Daily\t2. Weekly\t3. Monthly\t4. Yearly: ", 1, 5);

            RecurrencePatternFrequency choiceForFreq = (RecurrencePatternFrequency)frequency;

            HandleRecurrenceFrequency(choiceForFreq, eventModel);
        }

        private static void HandleRecurrenceFrequency(RecurrencePatternFrequency choiceForFreq, EventModel eventModel)
        {
            switch (choiceForFreq)
            {

                case RecurrencePatternFrequency.Daily:
                    eventModel.RecurrencePattern.Frequency = Frequency.Daily;
                    DailyRecurrence(eventModel);
                    break;

                case RecurrencePatternFrequency.Weekly:
                    eventModel.RecurrencePattern.Frequency = Frequency.Weekly;
                    WeeklyRecurrence(eventModel);
                    break;

                case RecurrencePatternFrequency.Monthly:
                    eventModel.RecurrencePattern.Frequency = Frequency.Monthly;
                    MonthlyRecurrence(eventModel);
                    break;

                case RecurrencePatternFrequency.Yearly:
                    eventModel.RecurrencePattern.Frequency = Frequency.Yearly;
                    YearlyRecurrence(eventModel);
                    break;

                default:
                    Console.WriteLine("Please Enter correct option !");
                    GetRecurrencePattern(null);
                    break;
            }
        }

        private static void DailyRecurrence(EventModel eventModel)
        {
            PrintHandler.PrintNewLine();

            Console.WriteLine("How often does this event occur? \n1.Every day \n2.Every Weekday \n3.Every n days (You need to specify the value of n)");
            int choice = ValidatedInputProvider.GetValidIntegerBetweenRange("\nEnter choice : ", 1, 2);

            string days;

            switch (choice)
            {
                case 1:
                    eventModel.RecurrencePattern.ByWeekDay = [1, 2, 3, 4, 5, 6, 7];
                    eventModel.RecurrencePattern.Interval = null;
                    break;
                case 2:
                    eventModel.RecurrencePattern.ByWeekDay = [1, 2, 3, 4, 5];
                    eventModel.RecurrencePattern.Interval = null;
                    break;
                case 3:
                    int interval = ValidatedInputProvider.GetValidIntegerBetweenRange("Please specify how often you'd like to repeat the event" +
                                                                              "(in days) : ", 1, 5);
                    eventModel.RecurrencePattern.Interval = interval;
                    eventModel.RecurrencePattern.ByWeekDay = null;
                    break;

            }

            eventModel.RecurrencePattern.ByMonthDay = null;
            eventModel.RecurrencePattern.ByMonth = null;
            eventModel.RecurrencePattern.WeekOrder = null;

        }

        private static void WeeklyRecurrence(EventModel eventModel)
        {
            PrintHandler.PrintNewLine();

            int interval = ValidatedInputProvider.GetValidIntegerBetweenRange("Please specify how often you'd like to repeat the event(in weeks) : ", 1, 5);

            eventModel.RecurrencePattern.Interval = interval;

            List<int> days = GetValidWeekDays();

            eventModel.RecurrencePattern.ByWeekDay = days;
            eventModel.RecurrencePattern.WeekOrder = null;
            eventModel.RecurrencePattern.ByMonthDay = null;
            eventModel.RecurrencePattern.ByMonth = null;
        }

        private static List<int> GetValidWeekDays()
        {
            PrintHandler.PrintNewLine();

            List<int> weekDays = [];

            while (true)
            {
                int day = ValidatedInputProvider.GetValidIntegerBetweenRange("Which weekdays would you like the event to occur on? (i.e. :- 1 for Monday,4 for Thursday,7 for Sunday) (Enter zero when you want to stop. : ", 0, 7);

                if (day == 0) break;

                weekDays.Add(day);
            }


            return weekDays;

        }

        private static void MonthlyRecurrence(EventModel eventModel)
        {
            PrintHandler.PrintNewLine();

            int interval = ValidatedInputProvider.GetValidIntegerBetweenRange("Please specify how often you'd like to repeat the " +
                                                                      "event (in months) : ", 1, 5);

            eventModel.RecurrencePattern.Interval = interval;

            GetMonthlyFrequencyChoices(eventModel);

        }

        private static void GetMonthlyFrequencyChoices(EventModel eventModel)
        {
            PrintHandler.PrintNewLine();

            Console.WriteLine("1.Select specific Day of the Month\n2.Select week day and week number ");

            int choice = ValidatedInputProvider.GetValidIntegerBetweenRange("\nEnter your choice : ", 1, 2);

            switch (choice)
            {
                case 1:
                    GetSpecificMonthDay(eventModel);
                    eventModel.RecurrencePattern.WeekOrder = null;
                    eventModel.RecurrencePattern.ByWeekDay = null;
                    break;
                case 2:
                    GetDayOfWeekAndWeekOrderNumber(eventModel);
                    eventModel.RecurrencePattern.ByMonthDay = null;
                    break;
            }

            eventModel.RecurrencePattern.ByMonth = null;
        }

        private static void GetSpecificMonthDay(EventModel eventModel)
        {
            eventModel.RecurrencePattern.ByMonthDay = ValidatedInputProvider.GetValidIntegerBetweenRange("On which day of the month would you like the "
                                                                               + "event to occur? (From 1 to 31) : ", 1, 31);
        }

        private static void GetDayOfWeekAndWeekOrderNumber(EventModel eventModel)
        {

            GetWeekOrder(eventModel);

            GetWeekDay(eventModel);

        }

        private static void GetWeekDay(EventModel eventModel)
        {
            PrintHandler.PrintNewLine();

            Console.WriteLine("Enter the day of the week (e.g., 'Monday' or 'Tuesday' or 'Wednesday' or 'Thursday' or" +
                              "'Friday' or 'Saturday' or 'Sunday')");
            Console.WriteLine("\n1. Monday \n2. Tuesday \n3. Wednesday \n4. Thursday \n5. Friday \n6. Saturday \n7. " +
                              "Sunday");

            WeekDay choice = (WeekDay)ValidatedInputProvider.GetValidIntegerBetweenRange("\nEnter choice :", 1, 7);
            switch (choice)
            {
                case WeekDay.Monday:
                    eventModel.RecurrencePattern.ByWeekDay = [1];
                    break;
                case WeekDay.Tuesday:
                    eventModel.RecurrencePattern.ByWeekDay = [2];
                    break;
                case WeekDay.Wednesday:
                    eventModel.RecurrencePattern.ByWeekDay = [3];
                    break;
                case WeekDay.Thursday:
                    eventModel.RecurrencePattern.ByWeekDay = [4];
                    break;
                case WeekDay.Friday:
                    eventModel.RecurrencePattern.ByWeekDay = [5];
                    break;
                case WeekDay.Saturday:
                    eventModel.RecurrencePattern.ByWeekDay = [6];
                    break;
                case WeekDay.Sunday:
                    eventModel.RecurrencePattern.ByWeekDay = [7];
                    break;
                default:
                    GetWeekDay(eventModel);
                    PrintHandler.PrintErrorMessage("Invalid input !");
                    break;
            }

        }

        private static void GetWeekOrder(EventModel eventModel)
        {
            PrintHandler.PrintNewLine();

            Console.WriteLine("Enter the order number (e.g., 'first' or 'second' or 'third' or 'fourth' or 'last')");
            Console.WriteLine("\n1. First \n2. Second \n3. Third \n4. Fourth \n5. Last");

            WeekOrder choice = (WeekOrder)ValidatedInputProvider.GetValidIntegerBetweenRange("\nEnter choice :", 1, 5);

            switch (choice)
            {
                case WeekOrder.First:
                    eventModel.RecurrencePattern.WeekOrder = 1;
                    break;
                case WeekOrder.Second:
                    eventModel.RecurrencePattern.WeekOrder = 2;
                    break;
                case WeekOrder.Third:
                    eventModel.RecurrencePattern.WeekOrder = 3;
                    break;
                case WeekOrder.Fourth:
                    eventModel.RecurrencePattern.WeekOrder = 4;
                    break;
                case WeekOrder.Last:
                    eventModel.RecurrencePattern.WeekOrder = 5;
                    break;
                default:
                    GetWeekOrder(eventModel);
                    PrintHandler.PrintErrorMessage("Invalid input !");
                    break;
            }
        }

        private static void YearlyRecurrence(EventModel eventModel)
        {
            PrintHandler.PrintNewLine();

            eventModel.RecurrencePattern.Interval = ValidatedInputProvider.GetValidIntegerBetweenRange("Please specify how often you'd like to repeat " +
                                                                           "the event (in years) : ", 1, 5);

            Console.WriteLine("1.Select specific Day of the Month \n2.Select week day and week number ");

            int choice = ValidatedInputProvider.GetValidIntegerBetweenRange("\nEnter your choice : ", 1, 2);

            GetValidMonth(eventModel);

            switch (choice)
            {
                case 1:
                    GetSpecificMonthDay(eventModel);
                    eventModel.RecurrencePattern.WeekOrder = null;
                    eventModel.RecurrencePattern.ByWeekDay = null;
                    break;
                case 2:
                    GetDayOfWeekAndWeekOrderNumber(eventModel);
                    eventModel.RecurrencePattern.ByMonthDay = null;
                    break;
            }

        }

        private static void GetValidMonth(EventModel eventObj)
        {
            PrintHandler.PrintNewLine();

            Console.WriteLine("Enter Months \n1. January \n2. February \n3. March \n4. April \n5. May \n6. June \n7. July " +
                              "\n8. August \n9. September \n10. October \n11. November \n12. December");

            eventObj.RecurrencePattern.ByMonth = ValidatedInputProvider.GetValidMonth("Enter the month :");

        }
    }
}