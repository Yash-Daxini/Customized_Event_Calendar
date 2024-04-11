using CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Models;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.ConsoleApp
{
    internal static class SharedEventCollaboration
    {

        private static readonly SharedEventCollaborationService _sharedEventCollaborationService = new();

        public static void GetInputToCollaborateInEvent(List<Domain.Entities.EventCollaborator> sharedEvents)
        {
            try
            {
                int serialNumberOfSharedEvent = ValidatedInputProvider.GetValidIntegerBetweenRange("Enter Sr.No of the event which you want to collaborate :- ", 1, sharedEvents.Count);

                Domain.Entities.EventCollaborator selectedEvent = sharedEvents[serialNumberOfSharedEvent - 1];

                CollaborateInEvent(selectedEvent);
            }
            catch (Exception)
            {
                PrintHandler.PrintErrorMessage("Some error occurred ! Can't collaborate on event");
            }
        }

        private static void CollaborateInEvent(Domain.Entities.EventCollaborator selectedEvent)
        {
            if (selectedEvent is null) return;

            int eventId = selectedEvent.EventId;

            Event? eventObj = new EventService().GetEventById(eventId);

            if (eventObj is null) return;

            Domain.Entities.EventCollaborator newEventCollaborator = new(eventId, GlobalData.GetUser().Id, "participant", null, eventObj.EventStartHour, eventObj.EventEndHour, selectedEvent.EventDate);

            if (!IsEligibleToCollaborate(newEventCollaborator)) return;

            _sharedEventCollaborationService.AddCollaborator(newEventCollaborator);

            PrintHandler.PrintSuccessMessage($"Successfully collaborated on event");
        }

        public static bool IsEligibleToCollaborate(Domain.Entities.EventCollaborator eventCollaborator)
        {
            return !(IsAlreadyCollaborated(eventCollaborator) || IsCollaborationOverlap(eventCollaborator));

        }

        private static bool IsAlreadyCollaborated(Domain.Entities.EventCollaborator eventCollaborator)
        {
            if (_sharedEventCollaborationService.IsEventAlreadyCollaborated(eventCollaborator))
            {
                PrintHandler.PrintWarningMessage("You already collaborated on this event");
                return true;
            }

            return false;
        }

        private static bool IsCollaborationOverlap(Domain.Entities.EventCollaborator eventCollaborator)
        {
            Domain.Entities.EventCollaborator? overlappedCollaboration = _sharedEventCollaborationService.GetCollaborationOverlap(eventCollaborator);

            if (overlappedCollaboration is null) return false;

            Event? eventObj = new EventService().GetEventById(overlappedCollaboration.EventId);

            if (eventObj is null) return true;

            PrintHandler.PrintWarningMessage($"Can't collaborate ! \nThe collaboration causes overlap with \"{eventObj.Title}\" on {overlappedCollaboration.EventDate} at {overlappedCollaboration.ProposedStartHour} to {overlappedCollaboration.ProposedEndHour}, indicating that both events are scheduled concurrently.");

            return true;
        }
    }
}