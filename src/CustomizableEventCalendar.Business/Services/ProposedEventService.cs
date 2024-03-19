using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        private bool IsLoggedInUserHasPendingResponseForProposedEvent(HashSet<int> proposedEventIds, EventCollaborator eventCollaborator)
        {
            return proposedEventIds.Contains(eventCollaborator.EventId)
                   && eventCollaborator.UserId == GlobalData.GetUser().Id
                   && eventCollaborator.ParticipantRole.Equals("participant")
                   && eventCollaborator.ConfirmationStatus.Equals("pending");
        }

        public string GenerateProposedEventTable()
        {
            List<Event> proposedEvents = GetProposedEvents();

            List<List<string>> outputRows = AddProposedEventDetailsIn2DList(proposedEvents);

            string eventTable = PrintService.GenerateTable(outputRows);

            if (proposedEvents.Count > 0)
            {
                return eventTable;
            }

            return "";
        }

        private static List<List<string>> AddProposedEventDetailsIn2DList(List<Event> proposedEvents)
        {
            List<List<string>> outputRows = [["Sr No.", "Title", "Description", "Location", "StartHour", "EndHour", "StartDate"]];

            foreach (var (eventObj, index) in proposedEvents.Select((value, index) => (value, index)))
            {
                outputRows.Add([(index+1).ToString(), eventObj.Title, eventObj.Description, eventObj.Location,
                                DateTimeManager.ConvertTo12HourFormat(eventObj.EventStartHour),
                                DateTimeManager.ConvertTo12HourFormat(eventObj.EventEndHour),
                                eventObj.EventStartDate.ToString()]);
            }

            return outputRows;
        }
    }
}
