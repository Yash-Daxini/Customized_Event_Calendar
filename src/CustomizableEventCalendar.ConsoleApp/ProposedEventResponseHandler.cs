using CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.ConsoleApp
{
    internal class ProposedEventResponseHandler
    {
        private readonly static ProposedEventService _proposedEventService = new();

        public static void ShowProposedEvents()
        {
            try
            {
                List<Event> proposedEvents = _proposedEventService.GetProposedEvents();

                string tableOfProposedEvents = _proposedEventService.GenerateProposedEventTable();

                if (proposedEvents.Count == 0)
                {
                    Console.WriteLine("No events available !");
                }
                else
                {
                    Console.WriteLine(tableOfProposedEvents);

                    int serialNumber = SelectEventForResponse(proposedEvents.Count);

                    Event eventObj = proposedEvents[serialNumber - 1];

                    EventCollaboratorService eventCollaboratorService = new();

                    EventCollaborator eventCollaborator = eventCollaboratorService.GetEventCollaboratorFromEventIdAndUserId(eventObj.Id);

                    if (eventCollaborator != null)
                    {
                        TakeInputToGiveResponse(eventCollaborator, eventObj);

                        eventCollaboratorService.UpdateEventCollaborators(eventCollaborator, eventCollaborator.Id);

                    }

                    PrintHandler.PrintSuccessMessage("Your response successfully shared with organizer of event.");

                }


            }
            catch
            {
                PrintHandler.PrintErrorMessage("Some error occurred ! Can't share your response.");
            }
        }

        public static int SelectEventForResponse(int endRange)
        {
            Console.WriteLine("\nChoose an event to respond (Please enter Sr no. )");

            int serialNumberOfEvent = ValidatedInputProvider.GetValidatedIntegerBetweenRange("Enter Sr. No : ", 1, endRange);

            return serialNumberOfEvent;
        }

        public static void TakeInputToGiveResponse(EventCollaborator eventCollaborator, Event eventObj)
        {
            Console.WriteLine("\nEnter your response : ");
            Console.WriteLine("1. Accept 2. Reject 3. May be ");

            int choice = ValidatedInputProvider.GetValidatedIntegerBetweenRange("Enter choice : ", 1, 3);

            switch (choice)
            {
                case 1:
                    eventCollaborator.ConfirmationStatus = "accept";
                    eventCollaborator.ProposedStartHour = eventObj.EventStartHour;
                    eventCollaborator.ProposedEndHour = eventObj.EventEndHour;
                    break;
                case 2:
                    eventCollaborator.ConfirmationStatus = "reject";
                    TakeInputToGetProposedTime(eventCollaborator);
                    break;
                case 3:
                    eventCollaborator.ConfirmationStatus = "maybe";
                    eventCollaborator.ProposedStartHour = eventObj.EventStartHour;
                    eventCollaborator.ProposedEndHour = eventObj.EventEndHour;
                    break;
            }
        }

        public static void TakeInputToGetProposedTime(EventCollaborator eventCollaborator)
        {
            Console.WriteLine("\nDo you want to propose time ? \n1. Yes \n0. No");

            int choice = ValidatedInputProvider.GetValidatedIntegerBetweenRange("Enter choice : ", 0, 1);

            switch (choice)
            {
                case 1:
                    Console.WriteLine($"\nEnter your proposed timings for {DateTimeManager.GetDateFromDateTime(eventCollaborator.EventDate)}");
                    TakeStartingAndEndingHourOfProposedEvent(eventCollaborator);
                    break;
                case 0: break;

            }
        }

        public static void TakeStartingAndEndingHourOfProposedEvent(EventCollaborator eventCollaborator)
        {
            Console.WriteLine("\nHow would you like to enter the time? : ");
            Console.WriteLine("\n1.Choose 24-hour format (1 to 24 hours) \n2.Choose 12-hour format (1 to 12 hours and AM/PM)");

            int choice = ValidatedInputProvider.GetValidatedInteger("Enter choice : ");

            switch (choice)
            {
                case 1:
                    PrintHandler.PrintInfoMessage("You've selected the 24-hour format.");
                    TakeHourIn24HourFormat(eventCollaborator);
                    break;
                case 2:
                    PrintHandler.PrintInfoMessage("You've selected the 12-hour format.");
                    TakeHourIn12HourFormat(eventCollaborator);
                    break;
                default:
                    TakeStartingAndEndingHourOfProposedEvent(eventCollaborator);
                    break;
            }

        }

        public static void TakeHourIn24HourFormat(EventCollaborator eventCollaborator)
        {
            PrintHandler.PrintNewLine();

            eventCollaborator.ProposedStartHour = ValidatedInputProvider.GetValidated24HourFormatTime("Enter Start Hour for the event : ");

            PrintHandler.PrintNewLine();

            eventCollaborator.ProposedEndHour = ValidatedInputProvider.GetValidated24HourFormatTime("Enter End Hour for the event : ");

            PrintHandler.PrintNewLine();

            if (!ValidationService.IsValidStartAndEndHour((int)eventCollaborator.ProposedStartHour, (int)eventCollaborator.ProposedEndHour))
            {
                PrintHandler.PrintWarningMessage("Invalid input ! Start hour must less than the end hour.");
                TakeHourIn24HourFormat(eventCollaborator);
            }
        }

        public static void TakeHourIn12HourFormat(EventCollaborator eventCollaborator)
        {
            PrintHandler.PrintNewLine();

            eventCollaborator.ProposedStartHour = ValidatedInputProvider.GetValidated12HourFormatTime("Enter Start Hour for the event (From 1 to 12) : ");

            string startHourAbbreviation = ValidatedInputProvider.GetValidatedAbbreviations();

            PrintHandler.PrintNewLine();

            eventCollaborator.ProposedEndHour = ValidatedInputProvider.GetValidated12HourFormatTime("Enter End Hour for the event (From 1 to 12) : ");

            string endHourAbbreviation = ValidatedInputProvider.GetValidatedAbbreviations();

            eventCollaborator.ProposedStartHour += startHourAbbreviation.Equals("PM") && eventCollaborator.ProposedStartHour != 12 ? 12 : 0;

            eventCollaborator.ProposedEndHour += endHourAbbreviation.Equals("PM") && eventCollaborator.ProposedEndHour != 12 ? 12 : 0;

            PrintHandler.PrintNewLine();

            if (!ValidationService.IsValidStartAndEndHour((int)eventCollaborator.ProposedStartHour, (int)eventCollaborator.ProposedEndHour))
            {
                PrintHandler.PrintWarningMessage("Invalid input ! Start hour must less than the end hour.");
                TakeHourIn12HourFormat(eventCollaborator);
            }
        }
    }
}
