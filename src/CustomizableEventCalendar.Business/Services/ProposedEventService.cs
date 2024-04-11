using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Enums;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Models;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services
{
    internal class ProposedEventService
    {
        private readonly EventService _eventService = new();

        public List<EventModel> GetProposedEvents()
        {

            List<EventModel> eventModels = _eventService.GetProposedEvents();

            return GetEventCollaboratorWhichHasPendingResponse(eventModels);

        }

        private List<EventModel> GetEventCollaboratorWhichHasPendingResponse(List<EventModel> eventModels)
        {
            return [.. eventModels.Where(eventModel => eventModel.Participants.Exists(participantModel => IsLoggedInUserHasPendingResponseForProposedEvent(participantModel)))];
        }

        private static bool IsLoggedInUserHasPendingResponseForProposedEvent(ParticipantModel participantModel)
        {
            return participantModel.ConfirmationStatus == ConfirmationStatus.Pending;
        }
    }
}
