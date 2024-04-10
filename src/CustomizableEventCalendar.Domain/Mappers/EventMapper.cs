using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Model;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Mapping
{
    internal class EventMapper
    {
        public List<EventModel> MapEventEntityToModel(Event eventObj, List<EventCollaborator> eventCollaborators)
        {
            List<EventModel> eventModels = [];

            foreach (var eventModel in eventCollaborators.GroupBy(eventCollaborator => eventCollaborator.EventDate))
            {
                DateOnly eventDate = eventModel.Key;

                eventModels.Add(new EventModel
                {
                    Id = eventObj.Id,
                    Title = eventObj.Title,
                    Description = eventObj.Description,
                    Location = eventObj.Location,
                    EventDate = eventDate,
                    Duration = new DurationMapper().MapDurationModel(eventObj.EventStartHour, eventObj.EventEndHour),
                    RecurrencePattern = new RecurrencePatternMapper().MapEventEntityToRecurrencePatternModel(eventObj),
                    Participants = [.. eventModel.Select(eventCollaborator => new ParticipantMapper().MapEventCollaboratorToParticipantModel(eventCollaborator))]
                }
                );
            }

            return eventModels;
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
