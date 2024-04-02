using CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.ConsoleApp
{
    internal static class ProposedEventResponseHandler
    {
        private readonly static ProposedEventService _proposedEventService = new();

        public static void ShowProposedEvents()
        {
            try
            {
                List<Event> proposedEvents = _proposedEventService.GetProposedEvents();

                string tableOfProposedEvents = GenerateProposedEventTable();

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
                        GetInputToGiveResponse(eventCollaborator, eventObj);

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

        private static string GenerateProposedEventTable()
        {
            List<Event> proposedEvents = _proposedEventService.GetProposedEvents();

            List<List<string>> outputRows = proposedEvents.InsertInto2DList(["Sr No.", "Title", "Description", "Location", "StartHour", "EndHour", "StartDate"],
                [
                    eventObj => proposedEvents.IndexOf(eventObj) + 1,
                    eventObj => eventObj.Title,
                    eventObj => eventObj.Description,
                    eventObj => eventObj.Location,
                    eventObj => DateTimeManager.ConvertTo12HourFormat(eventObj.EventStartHour),
                    eventObj => DateTimeManager.ConvertTo12HourFormat(eventObj.EventEndHour),
                    eventObj => eventObj.EventStartDate.ToString()
                ]);

            string eventTable = PrintService.GenerateTable(outputRows);

            if (proposedEvents.Count > 0)
            {
                return eventTable;
            }

            return "";
        }

        private static int SelectEventForResponse(int endRange)
        {
            Console.WriteLine("\nChoose an event to respond (Please enter Sr no. )");

            int serialNumberOfEvent = ValidatedInputProvider.GetValidatedIntegerBetweenRange("Enter Sr. No : ", 1, endRange);

            return serialNumberOfEvent;
        }

        private static void GetInputToGiveResponse(EventCollaborator eventCollaborator, Event eventObj)
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
                    eventCollaborator.ProposedStartHour = null;
                    eventCollaborator.ProposedEndHour = null;
                    break;
                case 3:
                    eventCollaborator.ConfirmationStatus = "maybe";
                    eventCollaborator.ProposedStartHour = eventObj.EventStartHour;
                    eventCollaborator.ProposedEndHour = eventObj.EventEndHour;
                    GetInputToGetProposedTime(eventCollaborator);
                    break;
            }
        }

        private static void GetInputToGetProposedTime(EventCollaborator eventCollaborator)
        {
            Console.WriteLine("\nDo you want to propose time ? \n1. Yes \n2. No");

            int choice = ValidatedInputProvider.GetValidatedIntegerBetweenRange("Enter choice : ", 1, 2);

            switch (choice)
            {
                case 1:
                    Console.WriteLine($"\nEnter your proposed timings for {eventCollaborator.EventDate}");
                    GetStartingAndEndingHourOfProposedEvent(eventCollaborator);
                    break;
                case 2:
                    break;

            }
        }

        private static void GetStartingAndEndingHourOfProposedEvent(EventCollaborator eventCollaborator)
        {
            Console.WriteLine("\nHow would you like to enter the time? : ");
            Console.WriteLine("\n1.Choose 24-hour format (1 to 24 hours) \n2.Choose 12-hour format (1 to 12 hours and AM/PM)");

            int choice = ValidatedInputProvider.GetValidatedInteger("Enter choice : ");

            switch (choice)
            {
                case 1:
                    PrintHandler.PrintInfoMessage("You've selected the 24-hour format.");
                    GetHourIn24HourFormat(eventCollaborator);
                    break;
                case 2:
                    PrintHandler.PrintInfoMessage("You've selected the 12-hour format.");
                    GetHourIn12HourFormat(eventCollaborator);
                    break;
                default:
                    GetStartingAndEndingHourOfProposedEvent(eventCollaborator);
                    break;
            }

        }

        private static void GetHourIn24HourFormat(EventCollaborator eventCollaborator)
        {
            PrintHandler.PrintNewLine();

            eventCollaborator.ProposedStartHour = ValidatedInputProvider.GetValidated24HourFormatTime("Enter Start Hour for the event : ");

            PrintHandler.PrintNewLine();

            eventCollaborator.ProposedEndHour = ValidatedInputProvider.GetValidated24HourFormatTime("Enter End Hour for the event : ");

            PrintHandler.PrintNewLine();

            if (!ValidationService.IsValidStartAndEndHour((int)eventCollaborator.ProposedStartHour, (int)eventCollaborator.ProposedEndHour))
            {
                PrintHandler.PrintWarningMessage("Invalid input ! Start hour must less than the end hour.");
                GetHourIn24HourFormat(eventCollaborator);
            }
        }

        private static void GetHourIn12HourFormat(EventCollaborator eventCollaborator)
        {
            PrintHandler.PrintNewLine();

            eventCollaborator.ProposedStartHour = ValidatedInputProvider.GetValidated12HourFormatTime("Enter Start Hour for the event (From 1 to 12) : ");

            string startHourAbbreviation = GetChoiceOfAbbreviation();

            PrintHandler.PrintNewLine();

            eventCollaborator.ProposedEndHour = ValidatedInputProvider.GetValidated12HourFormatTime("Enter End Hour for the event (From 1 to 12) : ");

            string endHourAbbreviation = GetChoiceOfAbbreviation();

            eventCollaborator.ProposedStartHour += startHourAbbreviation.Equals("PM") && eventCollaborator.ProposedStartHour != 12 ? 12 : 0;

            eventCollaborator.ProposedEndHour += endHourAbbreviation.Equals("PM") && eventCollaborator.ProposedEndHour != 12 ? 12 : 0;

            PrintHandler.PrintNewLine();

            if (!ValidationService.IsValidStartAndEndHour((int)eventCollaborator.ProposedStartHour, (int)eventCollaborator.ProposedEndHour))
            {
                PrintHandler.PrintWarningMessage("Invalid input ! Start hour must less than the end hour.");
                GetHourIn12HourFormat(eventCollaborator);
            }
        }

        private static string GetChoiceOfAbbreviation()
        {
            Console.WriteLine("Enter choice for AM or PM \n1. AM \n2. PM");
            int choice = ValidatedInputProvider.GetValidatedIntegerBetweenRange("Enter choice : ", 1, 2);

            return choice == 1 ? "AM" : "PM";
        }
    }
}
