using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Models;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Mapping
{
    internal class EventMapper
    {
        public List<EventModel> MapEventEntityToModel(Event eventObj, List<EventCollaborator> eventCollaborators)
        {
            List<EventModel> eventModels = [];

            foreach (var eventModel in eventCollaborators.GroupBy(eventCollaborator => eventCollaborator.EventDate).Select(eventCollaborator =>
            new
            {
                EventDate = eventCollaborator.Key,
                Collaborators = eventCollaborator.Select(eventCollaborator => eventCollaborator).ToList()
            }))
            {
                DateOnly eventDate = eventModel.EventDate;

                eventModels.Add(new EventModel
                (
                    eventObj.Id,
                    eventObj.Title,
                    eventObj.Description,
                    eventObj.Location,
                    eventDate,
                    new DurationMapper().MapDurationModel(eventObj.EventStartHour, eventObj.EventEndHour),
                    new RecurrencePatternMapper().MapEventEntityToRecurrencePatternModel(eventObj),
                    GetListOfParticipant(eventModel.Collaborators)
                )
                );
            }

            return eventModels;
        }

        private List<ParticipantModel> GetListOfParticipant(List<EventCollaborator> eventCollaborators)
        {
            List<ParticipantModel> participantModels = [.. eventCollaborators.Select(eventCollaborator => new ParticipantMapper().MapEventCollaboratorToParticipantModel(eventCollaborator))];

            return participantModels;
        }

        public Event MapEventModelToEntity(EventModel eventModel)
        {
            Event eventObj = new();

            new RecurrencePatternMapper().MapRecurrencePatternModelToEventEntity(eventModel.RecurrencePattern, eventObj);

            eventObj.Id = eventModel.Id;
            eventObj.Title = eventModel.Title;
            eventObj.Description = eventModel.Description;
            eventObj.Location = eventModel.Location;
            eventObj.EventStartHour = eventModel.Duration.StartHour;
            eventObj.EventEndHour = eventModel.Duration.EndHour;

            return eventObj;

        }
    }
}
