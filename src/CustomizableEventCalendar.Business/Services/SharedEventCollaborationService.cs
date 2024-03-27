using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services
{
    internal class SharedEventCollaborationService
    {
        private readonly EventCollaboratorService _eventCollaboratorService = new();

        public void AddCollaborator(EventCollaborator eventCollaborator)
        {
            _eventCollaboratorService.InsertEventCollaborators(eventCollaborator);
        }

        private List<EventCollaborator> GetAllEventCollaborators()
        {
            return _eventCollaboratorService.GetAllEventCollaborators();
        }

        public bool IsEventAlreadyCollaborated(EventCollaborator newEventCollaborator)
        {
            return GetAllEventCollaborators().Exists(eventCollaborator =>
                                                     eventCollaborator.UserId == newEventCollaborator.UserId
                                                     && eventCollaborator.EventId == newEventCollaborator.EventId
                                                     && eventCollaborator.EventDate == newEventCollaborator.EventDate);
        }

        public EventCollaborator? GetCollaborationOverlap(EventCollaborator newEventCollaborator)
        {
            return GetAllEventCollaborators().Find(eventCollaborator =>
                                                   eventCollaborator.UserId == newEventCollaborator.UserId
                                                   && eventCollaborator.EventDate == newEventCollaborator.EventDate
                                                   && eventCollaborator.UserId == newEventCollaborator.UserId);
        }
    }
}