using CustomizableEventCalendar.src.CustomizableEventCalendar.ConsoleApp;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services
{
    internal class ProposedEventService
    {
        private readonly EventService _eventService = new();
        private readonly EventCollaboratorService _eventCollaboratorService = new();

        public List<Event> GetProposedEvents()
        {

            List<Event> events = _eventService.GetProposedEvents();

            HashSet<int> proposedEventIds = [.. events.Select(eventObj => eventObj.Id)];

            HashSet<int> proposedEventIdsForLoggedInUser = [..GetEventCollaboratorWhichHasPendingResponse(proposedEventIds)
                                                           .Select(eventCollaborator => eventCollaborator.EventId)];

            return [.. events.Where(eventObj => proposedEventIdsForLoggedInUser.Contains(eventObj.Id))];

        }

        private List<EventCollaborator> GetEventCollaboratorWhichHasPendingResponse(HashSet<int> proposedEventIds)
        {
            return [.._eventCollaboratorService.GetAllEventCollaborators()
                                               .Where(eventCollaborator => IsLoggedInUserHasPendingResponseForProposedEvent
                                                                           (proposedEventIds,eventCollaborator))];
        }

        private static bool IsLoggedInUserHasPendingResponseForProposedEvent(HashSet<int> proposedEventIds, EventCollaborator eventCollaborator)
        {
            return proposedEventIds.Contains(eventCollaborator.EventId)
                   && eventCollaborator.UserId == GlobalData.GetUser().Id
                   && eventCollaborator.ParticipantRole != null
                   && eventCollaborator.ParticipantRole.Equals("participant")
                   && eventCollaborator.ConfirmationStatus != null
                   && eventCollaborator.ConfirmationStatus.Equals("pending");
        }
    }
}
