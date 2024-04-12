using CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Enums;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Models;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.ConsoleApp
{
    internal static class SharedEventCollaboration
    {

        private static readonly SharedEventCollaborationService _sharedEventCollaborationService = new();

        public static void GetInputToCollaborateInEvent(List<EventModel> sharedEvents)
        {
            try
            {
                int serialNumberOfSharedEvent = ValidatedInputProvider.GetValidIntegerBetweenRange("Enter Sr.No of the event which you want to collaborate :- ", 1, sharedEvents.Count);

                EventModel selectedEvent = sharedEvents[serialNumberOfSharedEvent - 1];

                CollaborateInEvent(selectedEvent);
            }
            catch (Exception)
            {
                PrintHandler.PrintErrorMessage("Some error occurred ! Can't collaborate on event");
            }
        }

        private static void CollaborateInEvent(EventModel selectedEvent)
        {
            if (selectedEvent is null) return;

            int eventId = selectedEvent.Id;

            List<EventModel>? eventModels = new EventService().GetEventById(eventId);

            if (eventModels is null) return;

            ParticipantModel newParticipant = new(ParticipantRole.Collaborator, ConfirmationStatus.Accept, null, null, selectedEvent.EventDate, GlobalData.GetUser());

            if (!IsEligibleToCollaborate(eventModels.First())) return;

            _sharedEventCollaborationService.AddCollaborator(newParticipant, eventId);

            PrintHandler.PrintSuccessMessage($"Successfully collaborated on event");
        }

        public static bool IsEligibleToCollaborate(EventModel eventModel)
        {
            return !(IsAlreadyCollaborated(eventModel) || IsCollaborationOverlap(eventModel));

        }

        private static bool IsAlreadyCollaborated(EventModel eventModel)
        {
            if (_sharedEventCollaborationService.IsEventAlreadyCollaborated(eventModel))
            {
                PrintHandler.PrintWarningMessage("You already collaborated on this event");
                return true;
            }

            return false;
        }

        private static bool IsCollaborationOverlap(EventModel eventModel)
        {
            EventModel? overlappedCollaboration = _sharedEventCollaborationService.GetCollaborationOverlap(eventModel);

            if (overlappedCollaboration is null) return false;

            PrintHandler.PrintWarningMessage($"Can't collaborate ! \nThe collaboration causes overlap with \"{eventModel.Title}\" on {overlappedCollaboration.EventDate} at {overlappedCollaboration.Duration.StartHour} to {overlappedCollaboration.Duration.EndHour}, indicating that both events are scheduled concurrently.");

            return true;
        }
    }
}