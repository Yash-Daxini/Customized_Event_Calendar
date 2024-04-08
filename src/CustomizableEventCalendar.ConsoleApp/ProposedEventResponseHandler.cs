using CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.ConsoleApp
{
    internal static class ProposedEventResponseHandler
    {
        private readonly static ProposedEventService _proposedEventService = new();

        private readonly static EventCollaboratorService _eventCollaboratorService = new();

        public static void ShowProposedEvents()
        {
            try
            {
                List<Event> proposedEvents = _proposedEventService.GetProposedEvents();

                string tableOfProposedEvents = GenerateProposedEventTable();

                if (IsMessagePrintedOnUnavailabilityOfProposedEvents(proposedEvents)) return;

                Console.WriteLine(tableOfProposedEvents);

                HandleResponse(proposedEvents);
            }
            catch
            {
                PrintHandler.PrintErrorMessage("Some error occurred ! Can't share your response.");
            }
        }

        private static void HandleResponse(List<Event> proposedEvents)
        {
            int serialNumber = GetEventSerialNumberToGiveResponse(proposedEvents.Count);

            Event eventObj = GetEventFromSerialNumber(proposedEvents, serialNumber);

            GetCollaboratorsInformation(eventObj, out EventCollaborator? eventCollaborator);

            if (eventCollaborator is null) return;

            GetAndSaveResponse(eventObj, eventCollaborator);

            PrintHandler.PrintSuccessMessage("Your response successfully shared with organizer of event.");
        }

        private static void GetAndSaveResponse(Event eventObj, EventCollaborator eventCollaborator)
        {
            GetInputToGiveResponse(eventCollaborator, eventObj);

            _eventCollaboratorService.UpdateEventCollaborators(eventCollaborator, eventCollaborator.Id);
        }

        private static void GetCollaboratorsInformation(Event eventObj, out EventCollaborator? eventCollaborator)
        {
            eventCollaborator = _eventCollaboratorService.GetEventCollaboratorFromEventIdAndUserId(eventObj.Id);
        }

        private static Event GetEventFromSerialNumber(List<Event> proposedEvents, int serialNumber)
        {
            return proposedEvents[serialNumber - 1];
        }

        private static bool IsMessagePrintedOnUnavailabilityOfProposedEvents(List<Event> proposedEvents)
        {
            if (proposedEvents.Count == 0)
            {
                Console.WriteLine("No events available !");
                return true;
            }
            return false;
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

        private static int GetEventSerialNumberToGiveResponse(int endRange)
        {
            Console.WriteLine("\nChoose an event to respond (Please enter Sr no. )");

            int serialNumberOfEvent = ValidatedInputProvider.GetValidIntegerBetweenRange("Enter Sr. No : ", 1, endRange);

            return serialNumberOfEvent;
        }

        private static void GetInputToGiveResponse(EventCollaborator eventCollaborator, Event eventObj)
        {
            Console.WriteLine("\nEnter your response : ");
            Console.WriteLine("1. Accept 2. Reject 3. May be ");

            int choice = ValidatedInputProvider.GetValidIntegerBetweenRange("Enter choice : ", 1, 3);

            switch (choice)
            {
                case 1:
                    eventCollaborator.ConfirmationStatus = "accept";
                    SetProposedHours(eventCollaborator, eventObj.EventStartHour, eventObj.EventEndHour);
                    break;
                case 2:
                    eventCollaborator.ConfirmationStatus = "reject";
                    SetProposedHours(eventCollaborator, null, null);
                    break;
                case 3:
                    eventCollaborator.ConfirmationStatus = "maybe";
                    SetProposedHours(eventCollaborator, eventObj.EventStartHour, eventObj.EventEndHour);
                    GetInputToGetProposedTime(eventCollaborator);
                    break;
            }
        }

        private static void SetProposedHours(EventCollaborator eventCollaborator, int? startHour, int? endHour)
        {
            eventCollaborator.ProposedStartHour = startHour;
            eventCollaborator.ProposedEndHour = endHour;
        }

        private static void GetInputToGetProposedTime(EventCollaborator eventCollaborator)
        {
            Console.WriteLine("\nDo you want to propose time ? \n1. Yes \n2. No");

            int choice = ValidatedInputProvider.GetValidIntegerBetweenRange("Enter choice : ", 1, 2);

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
