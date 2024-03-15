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

            HashSet<int> loginUsersProposedEventIds = [.._eventCollaboratorService.GetAllEventCollaborators()
                                                                                     .Where(eventCollaborator => proposedEventIds
                                                                                           .Contains(eventCollaborator.EventId)
                                                                                            && eventCollaborator.UserId ==
                                                                                               GlobalData.user.Id
                                                                                            && eventCollaborator.ParticipantRole
                                                                                                            .Equals("participant")
                                                                                            && eventCollaborator.ConfirmationStatus
                                                                                                            .Equals("pending"))
                                                                                     .Select(eventCollaborator=>eventCollaborator.EventId)];

            return [.. events.Where(eventObj => loginUsersProposedEventIds.Contains(eventObj.Id))];

        }

        public string GenerateProposedEventTable()
        {
            List<Event> proposedEvents = GetProposedEvents();

            List<List<string>> outputRows = [["Sr No.", "Title", "Description", "Location", "StartHour", "EndHour", "StartDate"]];

            AddProposedEventDetailsIn2DList(proposedEvents, ref outputRows);

            string eventTable = PrintHandler.GiveTable(outputRows);

            if (proposedEvents.Count > 0)
            {
                return eventTable;
            }

            return "";
        }

        public static void AddProposedEventDetailsIn2DList(List<Event> proposedEvents, ref List<List<string>> outputRows)
        {
            foreach (var (eventObj, index) in proposedEvents.Select((value, index) => (value, index)))
            {
                outputRows.Add([(index+1).ToString(), eventObj.Title, eventObj.Description, eventObj.Location,
                                DateTimeManager.ConvertTo12HourFormat(eventObj.EventStartHour),
                                DateTimeManager.ConvertTo12HourFormat(eventObj.EventEndHour),
                                eventObj.EventStartDate.ToString()]);
            }
        }
    }
}
