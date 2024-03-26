using CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.ConsoleApp
{
    internal static class SharedEventCollaboration
    {
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
            SharedEventCollaborationService sharedEventCollaborationService = new();

            if (selectedEvent == null) return;

            int eventId = selectedEvent.EventId;

            EventCollaborator newEventCollaborator = new(eventId, GlobalData.GetUser().Id, "participant", null, null, null,
                                                        selectedEvent.EventDate);

            if (!sharedEventCollaborationService.IsEligibleToCollaborate(newEventCollaborator)) return;

            sharedEventCollaborationService.AddCollaborator(newEventCollaborator);

            PrintHandler.PrintSuccessMessage($"Successfully collaborated on event");
        }
    }
}