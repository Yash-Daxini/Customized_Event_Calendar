﻿using CustomizableEventCalendar.src.CustomizableEventCalendar.ConsoleApp;
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

        public bool IsEligibleToCollaborate(EventCollaborator eventCollaborator)
        {
            if (IsEventAlreadyCollaborated(eventCollaborator))
            {
                PrintHandler.PrintWarningMessage("You already collaborated on this event");
                return false;
            }

            if (IsOverlappedCollaboration(eventCollaborator)) return false;

            return true;
        }

        private List<EventCollaborator> GetAllEventCollaborators()
        {
            return _eventCollaboratorService.GetAllEventCollaborators();
        }

        private bool IsOverlappedCollaboration(EventCollaborator eventCollaborator)
        {
            EventCollaborator overlappedCollaboration = GetCollaborationOverlap(eventCollaborator);

            if (overlappedCollaboration != null)
            {
                EventService eventService = new();
                Event eventObj = eventService.GetEventById(overlappedCollaboration.EventId);

                PrintHandler.PrintWarningMessage($"Can't collaborate ! \nThe collaboration causes overlap with \"{eventObj.Title}\""
                     + $" on {overlappedCollaboration.EventDate}, indicating that both events are scheduled concurrently.");

                return true;
            }

            return false;
        }

        private bool IsEventAlreadyCollaborated(EventCollaborator newEventCollaborator)
        {
            return GetAllEventCollaborators().Exists(eventCollaborator =>
                                                     eventCollaborator.UserId == newEventCollaborator.UserId
                                                     && eventCollaborator.EventId == newEventCollaborator.EventId
                                                     && eventCollaborator.EventDate == newEventCollaborator.EventDate);
        }

        private EventCollaborator? GetCollaborationOverlap(EventCollaborator newEventCollaborator)
        {
            return GetAllEventCollaborators().Find(eventCollaborator =>
                                                   eventCollaborator.EventDate == newEventCollaborator.EventDate
                                                   && eventCollaborator.UserId == newEventCollaborator.UserId);
        }
    }
}