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

        private bool IsLoggedInUserHasPendingResponseForProposedEvent(HashSet<int> proposedEventIds, EventCollaborator eventCollaborator)
        {
            return proposedEventIds.Contains(eventCollaborator.EventId)
                   && eventCollaborator.UserId == GlobalData.GetUser().Id
                   && eventCollaborator.ParticipantRole != null
                   && eventCollaborator.ParticipantRole.Equals("participant")
                   && eventCollaborator.ConfirmationStatus != null
                   && eventCollaborator.ConfirmationStatus.Equals("pending");
        }

        public string GenerateProposedEventTable()
        {
            List<Event> proposedEvents = GetProposedEvents();

            List<List<string>> outputRows = proposedEvents.InsertInto2DList(["Sr No.", "Title", "Description", "Location", "StartHour", "EndHour", "StartDate"],
                [
                    eventObj => proposedEvents.IndexOf(eventObj) + 1,
                    eventObj => eventObj.Title,
                    eventObj => eventObj.Description,
                    eventObj => eventObj.Location,
                    eventObj => DateTimeManager.ConvertTo12HourFormat(eventObj.EventStartHour),
                    eventObj => DateTimeManager.ConvertTo12HourFormat(eventObj.EventEndHour),
                    eventObj => eventObj.EventStartDate.ToString()
                ]);

            string eventTable = PrintService.GenerateTable(outputRows);

            if (proposedEvents.Count > 0)
            {
                return eventTable;
            }

            return "";
        }
    }
}
