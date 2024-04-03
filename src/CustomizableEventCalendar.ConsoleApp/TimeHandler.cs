using CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.ConsoleApp
{
    internal static class TimeHandler
    {
        public static void GetStartingAndEndingHourOfEvent(dynamic eventObj)
        {
            Console.WriteLine("\nHow would you like to enter the time? : ");
            Console.WriteLine("\n1.Choose 24-hour format (1 to 24 hours) \n2.Choose 12-hour format (1 to 12 hours and AM/PM)");

            int choice = ValidatedInputProvider.GetValidatedInteger("Enter choice : ");

            switch (choice)
            {
                case 1:
                    PrintHandler.PrintInfoMessage("You've selected the 24-hour format.");
                    GetHourIn24HourFormat(eventObj);
                    break;
                case 2:
                    PrintHandler.PrintInfoMessage("You've selected the 12-hour format.");
                    GetHourIn12HourFormat(eventObj);
                    break;
                default:
                    GetStartingAndEndingHourOfEvent(eventObj);
                    break;
            }
        }

        private static string GetChoiceOfAbbreviation()
        {
            Console.WriteLine("Enter choice for AM or PM \n1. AM \n2. PM");
            int choice = ValidatedInputProvider.GetValidatedIntegerBetweenRange("Enter choice : ", 1, 2);

            return choice == 1 ? "AM" : "PM";
        }

        private static void GetHourIn12HourFormat(dynamic obj)
        {
            GetInputFromConsoleFor12HourFormat(out int startHour, out string startHourAbbreviation, out int endHour, out string endHourAbbreviation);

            ConvertInto24HourFormat(ref startHour, startHourAbbreviation, ref endHour, endHourAbbreviation);

            PrintHandler.PrintNewLine();

            while (!ValidationService.IsValidStartAndEndHour(startHour, endHour))
            {
                PrintHandler.PrintWarningMessage("Invalid input ! Start hour must less than the end hour.");
                GetInputFromConsoleFor12HourFormat(out startHour, out startHourAbbreviation, out endHour, out endHourAbbreviation);
                ConvertInto24HourFormat(ref startHour, startHourAbbreviation, ref endHour, endHourAbbreviation);
            }

            AssignStartHourAndEndHourToSpecificObject(obj, startHour, endHour);
        }

        private static void AssignStartHourAndEndHourToSpecificObject(dynamic obj, int startHour, int endHour)
        {
            if (obj is Event)
            {
                obj.EventStartHour = startHour;
                obj.EventEndHour = endHour;

            }
            else if (obj is EventCollaborator)
            {
                obj.ProposedStartHour = startHour;
                obj.ProposedEndHour = endHour;
            }
        }

        private static void ConvertInto24HourFormat(ref int startHour, string startHourAbbreviation, ref int endHour, string endHourAbbreviation)
        {
            startHour += startHourAbbreviation.Equals("PM") && startHour != 12 ? 12 : 0;

            endHour += endHourAbbreviation.Equals("PM") && endHour != 12 ? 12 : 0;
        }

        private static void GetInputFromConsoleFor12HourFormat(out int startHour, out string startHourAbbreviation, out int endHour, out string endHourAbbreviation)
        {
            PrintHandler.PrintNewLine();

            startHour = ValidatedInputProvider.GetValidated12HourFormatTime("Enter Start Hour for the event (From 1 to 12) : ");
            startHourAbbreviation = GetChoiceOfAbbreviation();

            PrintHandler.PrintNewLine();

            endHour = ValidatedInputProvider.GetValidated12HourFormatTime("Enter End Hour for the event (From 1 to 12) : ");
            endHourAbbreviation = GetChoiceOfAbbreviation();
        }

        private static void GetHourIn24HourFormat(dynamic obj)
        {
            GetInputFromConsoleFor24HourFormat(out int startHour, out int endHour);

            while (!ValidationService.IsValidStartAndEndHour(startHour, endHour))
            {
                PrintHandler.PrintWarningMessage("Invalid input ! Start hour must less than the end hour.");
                GetInputFromConsoleFor24HourFormat(out startHour, out endHour);
            }

            AssignStartHourAndEndHourToSpecificObject(obj, startHour, endHour);
        }

        private static void GetInputFromConsoleFor24HourFormat(out int startHour, out int endHour)
        {
            PrintHandler.PrintNewLine();

            startHour = ValidatedInputProvider.GetValidated24HourFormatTime("Enter Start Hour for the event : ");
            PrintHandler.PrintNewLine();

            endHour = ValidatedInputProvider.GetValidated24HourFormatTime("Enter End Hour for the event : ");
            PrintHandler.PrintNewLine();
        }
    }
}