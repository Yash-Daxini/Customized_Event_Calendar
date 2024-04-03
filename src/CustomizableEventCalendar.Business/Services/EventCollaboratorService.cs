using CustomizableEventCalendar.src.CustomizableEventCalendar.Data.Repositories;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services
{
    internal class EventCollaboratorService
    {
        private readonly EventCollaboratorRepository _eventCollaboratorsRepository = new();

        public List<EventCollaborator> GetAllEventCollaborators()
        {
            List<EventCollaborator> eventCollaborators = [.._eventCollaboratorsRepository.GetAll(data => new EventCollaborator(data))
                                                         .OrderBy(eventCollaborator => eventCollaborator.EventDate)
                                                         .ThenBy(eventCollaborator => eventCollaborator.ProposedStartHour)];
            return eventCollaborators;
        }

        public EventCollaborator? GetEventCollaboratorById(int eventCollaboratorId)
        {
            EventCollaborator? eventCollaborators = _eventCollaboratorsRepository.GetById(data => new EventCollaborator(data), eventCollaboratorId);
            return eventCollaborators;
        }

        public int InsertEventCollaborators(EventCollaborator eventCollaborators)
        {
            int eventCollaboratorsId = _eventCollaboratorsRepository.Insert(eventCollaborators);
            return eventCollaboratorsId;
        }

        public void UpdateEventCollaborators(EventCollaborator eventCollaborator, int eventCollaboratorId)
        {
            _eventCollaboratorsRepository.Update(eventCollaborator, eventCollaboratorId);
        }

        public void DeleteEventCollaboratorsByEventId(int eventId)
        {
            _eventCollaboratorsRepository.DeleteByEventId(eventId);
        }

        public EventCollaborator? GetEventCollaboratorFromEventIdAndUserId(int eventId)
        {
            EventCollaborator? eventCollaborator = GetAllEventCollaborators()
                                                   .Find(eventCollaborator =>
                                                                   eventCollaborator.UserId == GlobalData.GetUser().Id
                                                                   && eventCollaborator.EventId == eventId);
            return eventCollaborator;
        }
    }
}