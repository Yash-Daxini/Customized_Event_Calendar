using System.Reflection;
using System.Threading.Channels;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services;
using CustomizableEventCalendar.src.CustomizableEventCalendar.ConsoleApp.InputMessageStore;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Enums;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.ConsoleApp
{
    internal class RecurrenceHandling
    {
        private readonly static RecurrenceService _recurrenceService = new RecurrenceService();

        public static RecurrencePatternCustom AskForRecurrenceChoice(int? Id)
        {
            int choice = ValidatedInputProvider.GetValidatedInteger("Are you want repeat this event ? \n 1. Yes 2. No :- ");

            RecurrencePatternChoiceEnum isRepeative = (RecurrencePatternChoiceEnum)choice;

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

            recurrencePattern.DTSTART = ValidatedInputProvider.GetValidatedDateTime(RecurrencePatternMessages.StartDate);

            recurrencePattern.UNTILL = ValidatedInputProvider.GetValidatedDateTime(RecurrencePatternMessages.EndDate);
        }

        public static RecurrencePatternCustom GetRecurrenceForSingleEvent(int? Id)
        {
            RecurrencePatternCustom recurrencePattern;

            if (Id == null) recurrencePattern = new RecurrencePatternCustom();
            else recurrencePattern = _recurrenceService.Read(Convert.ToInt32(Id));

            GetDates(ref recurrencePattern);

            return recurrencePattern;
        }

        public static RecurrencePatternCustom GetRecurrencePattern(int? Id)
        {
            RecurrencePatternCustom recurrencePattern;

            if (Id == null) recurrencePattern = new RecurrencePatternCustom();
            else recurrencePattern = _recurrenceService.Read(Convert.ToInt32(Id));

            Console.WriteLine("Fill details to make event repetitive :- ");

            GetDates(ref recurrencePattern);

            int frequency = ValidatedInputProvider.GetValidatedInteger(RecurrencePatternMessages.Frequency);

            RecurrencePatternFrequencyEnum choiceForFreq = (RecurrencePatternFrequencyEnum)frequency;

            HandleRecurrenceFrequency(choiceForFreq, ref recurrencePattern);

            recurrencePattern.INTERVAL = ValidatedInputProvider.GetValidatedInteger(RecurrencePatternMessages.Interval) + "";

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
            string days = ValidatedInputProvider.GetValidatedCommaSeparatedInput(RecurrencePatternMessages.Days);

            return days;
        }

        public static string WeeklyRecurrence()
        {
            string days = ValidatedInputProvider.GetValidatedCommaSeparatedInput(RecurrencePatternMessages.Days);

            return days;
        }

        public static string MonthlyRecurrence()
        {
            string monthDays = ValidatedInputProvider.GetValidatedCommaSeparatedInput(RecurrencePatternMessages.MonthDays);

            return monthDays;
        }

        public static string YearlyRecurrence()
        {
            string monthDays = ValidatedInputProvider.GetValidatedCommaSeparatedInput(RecurrencePatternMessages.MonthDays);

            string month = ValidatedInputProvider.GetValidatedCommaSeparatedInput(RecurrencePatternMessages.Months);

            return monthDays + "-" + month;
        }
    }
}