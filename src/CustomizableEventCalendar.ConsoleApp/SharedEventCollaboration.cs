using CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.ConsoleApp
{
    internal static class SharedEventCollaboration
    {

        private static readonly SharedEventCollaborationService _sharedEventCollaborationService = new();

        public static void GetInputToEventCollaboration(int sharedEventsCount, List<EventCollaborator> sharedEvents)
        {
            try
            {
                int serialNumberOfSharedEvent = ValidatedInputProvider.GetValidatedIntegerBetweenRange("Enter Sr.No of the event which you want to collaborate :- ", 1, sharedEventsCount);

                EventCollaborator selectedEvent = sharedEvents[serialNumberOfSharedEvent - 1];

                CollaborateOnSharedEvent(selectedEvent);
            }
            catch (Exception)
            {
                PrintHandler.PrintErrorMessage("Some error occurred ! Can't collaborate on event");
            }

        }

        private static void CollaborateOnSharedEvent(EventCollaborator selectedEvent)
        {
            if (selectedEvent == null) return;

            int eventId = selectedEvent.EventId;

            EventCollaborator newEventCollaborator = new(eventId, GlobalData.GetUser().Id, "participant", null, null, null,
                                                        selectedEvent.EventDate);

            if (IsEligibleToCollaborate(newEventCollaborator)) return;

            _sharedEventCollaborationService.AddCollaborator(newEventCollaborator);

            PrintHandler.PrintSuccessMessage($"Successfully collaborated on event");
        }

        public static bool IsEligibleToCollaborate(EventCollaborator eventCollaborator)
        {
            if (_sharedEventCollaborationService.IsEventAlreadyCollaborated(eventCollaborator))
            {
                PrintHandler.PrintWarningMessage("You already collaborated on this event");
                return false;
            }

            EventCollaborator overlappedCollaboration = _sharedEventCollaborationService.GetCollaborationOverlap(eventCollaborator);

            if (overlappedCollaboration != null)
            {
                EventService eventService = new();
                Event eventObj = eventService.GetEventById(overlappedCollaboration.EventId);

                PrintHandler.PrintWarningMessage($"Can't collaborate ! \nThe collaboration causes overlap with \"{eventObj.Title}\""
                     + $" on {overlappedCollaboration.EventDate}, indicating that both events are scheduled concurrently.");

                return false;
            }

            return true;
        }
    }
}