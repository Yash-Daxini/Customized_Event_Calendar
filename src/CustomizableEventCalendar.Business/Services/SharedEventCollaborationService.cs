using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomizableEventCalendar.src.CustomizableEventCalendar.ConsoleApp;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services
{
    internal class SharedEventCollaborationService
    {
        private readonly EventCollaboratorService _eventCollaboratorsService = new();

        public void AddCollaborator(int eventCollaboratorId)
        {

            EventCollaborator? eventCollaborator = _eventCollaboratorsService.GetEventCollaboratorsById(eventCollaboratorId);

            if (eventCollaborator == null) return;

            int eventId = eventCollaborator.EventId;

            EventCollaborator newEventCollaborator = new(eventId, GlobalData.user.Id, "participant", null, null, null,
                                                        eventCollaborator.EventDate);

            if (IsEventAlreadyCollaborated(newEventCollaborator))
            {
                PrintHandler.PrintWarningMessage("You already collaborated on this event");
                return;
            }

            EventCollaborator overlappedCollaboration = GetCollaborationOverlap(newEventCollaborator);

            if (overlappedCollaboration != null)
            {
                EventService eventService = new EventService();
                Event eventObj = eventService.GetEventsById(overlappedCollaboration.EventId);   

                PrintHandler.PrintWarningMessage($"Can't collaborate ! \nThe collaboration causes overlap with {eventObj.Title} on " +
                                                 $"{overlappedCollaboration.EventDate}, indicating that both events are scheduled concurrently.");

                return;
            }

            try
            {
                _eventCollaboratorsService.InsertEventCollaborators(newEventCollaborator);
                PrintHandler.PrintSuccessMessage($"Successfully collaborated on {eventCollaborator.EventDate} event");
            }
            catch
            {
                PrintHandler.PrintErrorMessage("Some error occurred ! Can't collaborate on event");
            }

        }

        public List<EventCollaborator> GetAllEventCollaborators()
        {
            return _eventCollaboratorsService.GetAllEventCollaborators();
        }

        public bool IsEventAlreadyCollaborated(EventCollaborator newEventCollaborator)
        {

            return GetAllEventCollaborators().Exists(eventCollaborator => eventCollaborator.UserId == newEventCollaborator.UserId
                                          && eventCollaborator.EventId == newEventCollaborator.EventId
                                          && eventCollaborator.EventDate == newEventCollaborator.EventDate);
        }

        public EventCollaborator? GetCollaborationOverlap(EventCollaborator newEventCollaborator)
        {
            return GetAllEventCollaborators().Find(eventCollaborator => eventCollaborator.EventDate == newEventCollaborator.EventDate
                                                  && eventCollaborator.UserId == newEventCollaborator.UserId);
        }
    }
}