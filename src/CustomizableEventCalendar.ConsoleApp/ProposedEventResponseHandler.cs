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
                    TimeHandler.GetStartingAndEndingHourOfEvent(eventCollaborator);
                    break;
                case 2:
                    break;

            }
        }

    }
}
