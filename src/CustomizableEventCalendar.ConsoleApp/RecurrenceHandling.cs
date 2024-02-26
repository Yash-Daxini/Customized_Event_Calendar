// Ignore Spelling: Upsert
using CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services;
using CustomizableEventCalendar.src.CustomizableEventCalendar.ConsoleApp.InputMessageStore;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Enums;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.ConsoleApp
{
    internal class RecurrenceHandling
    {
        public static RecurrenceService recurrenceService = new RecurrenceService();
        public static RecurrencePatternCustom AskForRecurrenceChoice(int? Id)
        {
            Console.WriteLine("Are you want repeat this event ? \n 1. Yes 2. No :- ");
            RecurrencePatternChoiceEnum isRepeative = (RecurrencePatternChoiceEnum)Convert.ToInt32(Console.ReadLine());

            RecurrencePatternCustom recurrencePattern = new RecurrencePatternCustom();

            switch (isRepeative)
            {
                case RecurrencePatternChoiceEnum.Yes:
                    recurrencePattern = GetRecurrencePattern(Id);
                    break;
                case RecurrencePatternChoiceEnum.No:
                    recurrencePattern = GetRecurrenceForSingleEvent(Id);
                    break;
                default:
                    Console.WriteLine("Please enter correct value : ");
                    AskForRecurrenceChoice(Id);
                    break;
            }

            return recurrencePattern;
        }
        public static void GetDates(ref RecurrencePatternCustom recurrencePattern)
        {
            Console.WriteLine("Enter dates for the event :- ");

            Console.Write(RecurrencePatternMessages.StartDate);
            recurrencePattern.DTSTART = Convert.ToDateTime(Console.ReadLine());

            Console.Write(RecurrencePatternMessages.EndDate);
            recurrencePattern.UNTILL = Convert.ToDateTime(Console.ReadLine());
        }
        public static RecurrencePatternCustom GetRecurrenceForSingleEvent(int? Id)
        {
            RecurrencePatternCustom recurrencePattern;

            if (Id == null) recurrencePattern = new RecurrencePatternCustom();
            else recurrencePattern = recurrenceService.Read(Convert.ToInt32(Id));

            GetDates(ref recurrencePattern);

            return recurrencePattern;
        }
        public static RecurrencePatternCustom GetRecurrencePattern(int? Id)
        {
            RecurrencePatternCustom recurrencePattern;

            if (Id == null) recurrencePattern = new RecurrencePatternCustom();
            else recurrencePattern = recurrenceService.Read(Convert.ToInt32(Id));

            Console.WriteLine("Fill details to make event repetitive :- ");

            GetDates(ref recurrencePattern);

            Console.Write(RecurrencePatternMessages.Frequency);
            RecurrencePatternFrequencyEnum choiceForFreq = (RecurrencePatternFrequencyEnum)Convert.ToInt32(Console.ReadLine());

            HandleRecurrenceFrequency(choiceForFreq, ref recurrencePattern);

            Console.Write(RecurrencePatternMessages.Interval);
            recurrencePattern.INTERVAL = Console.ReadLine();

            return recurrencePattern;
        }
        public static void HandleRecurrenceFrequency(RecurrencePatternFrequencyEnum choiceForFreq, ref RecurrencePatternCustom recurrencePattern)
        {
            switch (choiceForFreq)
            {
                case RecurrencePatternFrequencyEnum.Daily:
                    recurrencePattern.FREQ = "daily";
                    recurrencePattern.BYDAY = DailyRecurrence();
                    break;
                case RecurrencePatternFrequencyEnum.Weekly:
                    recurrencePattern.FREQ = "weekly";
                    recurrencePattern.BYDAY = WeeklyRecurrence();
                    break;
                case RecurrencePatternFrequencyEnum.Monthly:
                    recurrencePattern.FREQ = "monthly";
                    recurrencePattern.BYMONTHDAY = MonthlyRecurrence();
                    break;
                case RecurrencePatternFrequencyEnum.Yearly:
                    recurrencePattern.FREQ = "yearly";
                    string yearlyRecurrenceDetails = YearlyRecurrence();
                    recurrencePattern.BYMONTH = yearlyRecurrenceDetails.Split("-")[0];
                    recurrencePattern.BYMONTHDAY = yearlyRecurrenceDetails.Split("-")[1];
                    break;
                default:
                    Console.WriteLine("Please Enter correct option !");
                    GetRecurrencePattern(null);
                    break;
            }
        }
        public static string DailyRecurrence()
        {
            Console.Write(RecurrencePatternMessages.Days);

            string days = Console.ReadLine();
            return days;
        }
        public static string WeeklyRecurrence()
        {
            Console.Write(RecurrencePatternMessages.Days);
            string days = Console.ReadLine();

            return days;
        }
        public static string MonthlyRecurrence()
        {
            Console.Write(RecurrencePatternMessages.MonthDays);
            string days = Console.ReadLine();

            return days;
        }
        public static string YearlyRecurrence()
        {
            Console.WriteLine(RecurrencePatternMessages.Months);
            string days = Console.ReadLine();

            Console.WriteLine(RecurrencePatternMessages.MonthDays);
            string month = Console.ReadLine();

            return days + "-" + month;
        }
    }
}