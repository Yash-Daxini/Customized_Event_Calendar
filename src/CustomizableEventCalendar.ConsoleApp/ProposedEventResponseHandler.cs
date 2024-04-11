using CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Enums;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Models;

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
                List<EventModel> proposedEvents = _proposedEventService.GetProposedEvents();

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

        private static void HandleResponse(List<EventModel> proposedEvents)
        {
            int serialNumber = GetEventSerialNumberToGiveResponse(proposedEvents.Count);

            EventModel eventModel = GetEventFromSerialNumber(proposedEvents, serialNumber);

            GetAndSaveResponse(eventModel);

            PrintHandler.PrintSuccessMessage("Your response successfully shared with organizer of event.");
        }

        private static void GetAndSaveResponse(EventModel eventModel)
        {
            GetInputToGiveResponse(eventModel);

            //Do changes here ........................
            //_eventCollaboratorService.UpdateEventCollaborators(eventCollaborator, eventCollaborator.Id);
        }

        private static EventModel GetEventFromSerialNumber(List<EventModel> proposedEvents, int serialNumber)
        {
            return proposedEvents[serialNumber - 1];
        }

        private static bool IsMessagePrintedOnUnavailabilityOfProposedEvents(List<EventModel> proposedEvents)
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
            List<EventModel> proposedEvents = _proposedEventService.GetProposedEvents();

            List<List<string>> outputRows = proposedEvents.InsertInto2DList(["Sr No.", "Title", "Description", "Location", "StartHour", "EndHour", "StartDate"],
                [
                    eventModel => proposedEvents.IndexOf(eventModel) + 1,
                    eventModel => eventModel.Title,
                    eventModel => eventModel.Description,
                    eventModel => eventModel.Location,
                    eventModel => DateTimeManager.ConvertTo12HourFormat(eventModel.Duration.StartHour),
                    eventModel => DateTimeManager.ConvertTo12HourFormat(eventModel.Duration.EndHour),
                    eventModel => eventModel.EventDate.ToString()
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

        private static void GetInputToGiveResponse(EventModel eventModel)
        {
            Console.WriteLine("\nEnter your response : ");
            Console.WriteLine("1. Accept 2. Reject 3. May be ");

            int choice = ValidatedInputProvider.GetValidIntegerBetweenRange("Enter choice : ", 1, 3);

            switch (choice)
            {
                case 1:
                    eventModel.Participants[0].ConfirmationStatus = ConfirmationStatus.Accept;
                    SetProposedHours(eventModel.Participants[0], eventModel.Duration.StartHour, eventModel.Duration.EndHour);
                    break;
                case 2:
                    eventModel.Participants[0].ConfirmationStatus = ConfirmationStatus.Reject;
                    SetProposedHours(eventModel.Participants[0], null, null);
                    break;
                case 3:
                    eventModel.Participants[0].ConfirmationStatus = ConfirmationStatus.Maybe;
                    SetProposedHours(eventModel.Participants[0], eventModel.Duration.StartHour, eventModel.Duration.EndHour);
                    GetInputToGetProposedTime(eventModel);
                    break;
            }
        }

        private static void SetProposedHours(ParticipantModel participantModel, int? startHour, int? endHour)
        {
            participantModel.ProposedStartHour = startHour;
            participantModel.ProposedEndHour = endHour;
        }

        private static void GetInputToGetProposedTime(EventModel eventModel)
        {
            Console.WriteLine("\nDo you want to propose time ? \n1. Yes \n2. No");

            int choice = ValidatedInputProvider.GetValidIntegerBetweenRange("Enter choice : ", 1, 2);

            switch (choice)
            {
                case 1:
                    eventModel.Participants[0].ConfirmationStatus = ConfirmationStatus.Proposed;
                    Console.WriteLine($"\nEnter your proposed timings for {eventModel.EventDate}");
                    TimeHandler.GetStartingAndEndingHourOfEvent(eventModel);
                    break;
                case 2:
                    break;

            }
        }

    }
}
