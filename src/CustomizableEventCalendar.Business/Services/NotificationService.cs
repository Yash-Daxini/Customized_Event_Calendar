using System.Text;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services
{
    internal class NotificationService
    {
        private readonly EventCollaboratorService _eventCollaboratorsService = new();

        private readonly EventService _eventService = new();

        private readonly List<Event> events;

        public NotificationService()
        {
            events = _eventService.GetAllEvents();
        }

        private static bool IsEventOrganizer(EventCollaborator eventCollaborator)
        {
            return eventCollaborator.ParticipantRole != null && eventCollaborator.ParticipantRole.Equals("organizer");
        }
        private List<EventCollaborator> GetConsiderableEventCollaborators()
        {
            return [.. _eventCollaboratorsService.GetAllEventCollaborators()
                                                 .Where(eventCollaborator => eventCollaborator.UserId == GlobalData.GetUser().Id &&
                                                 (IsEventOrganizer(eventCollaborator)
                                                 || (!IsEventOrganizer(eventCollaborator)
                                                      && eventCollaborator.ConfirmationStatus != null
                                                      && !(eventCollaborator.ConfirmationStatus.Equals("reject")
                                                            && eventCollaborator.ProposedStartHour == null
                                                            && eventCollaborator.ProposedEndHour == null
                                                          )
                                                    )
                                                    && !_eventService.GetProposedEvents().Exists(eventObj=>eventObj.Id == eventCollaborator.EventId)
                                                 ))];
        }

        public List<EventCollaborator> GetUpcomingEvents()
        {
            List<EventCollaborator> scheduleEvents = [.. GetConsiderableEventCollaborators().OrderBy(eventCollaborator => eventCollaborator.EventDate)];

            List<EventCollaborator> upcommingEvents = [..scheduleEvents.Where(eventCollaborators =>
                                                                            eventCollaborators.EventDate==DateOnly.FromDateTime(DateTime.Now))];

            return upcommingEvents;
        }



        public List<EventCollaborator> GetProposedEvents()
        {
            HashSet<int> proposedEventIds = events.Where(eventObj => eventObj.IsProposed
                                                         && eventObj.EventStartDate >= DateOnly.FromDateTime(DateTime.Now))
                                                  .Select(eventObj => eventObj.Id)
                                                  .ToHashSet();

            List<EventCollaborator> proposedEventCollabprators = [.. _eventCollaboratorsService.GetAllEventCollaborators().OrderBy(eventCollaborator => eventCollaborator.EventDate)];

            proposedEventCollabprators = [..proposedEventCollabprators.Where(eventCollaborator => proposedEventIds
                                                                      .Contains(eventCollaborator.EventId) && eventCollaborator.UserId == GlobalData.GetUser().Id)];

            return proposedEventCollabprators;
        }


    }
}