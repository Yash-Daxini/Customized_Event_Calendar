﻿using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Models;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services
{
    internal class NotificationService
    {
        private readonly EventCollaboratorService _eventCollaboratorsService = new();

        private readonly EventService _eventService = new();

        private readonly List<Event> events;

        public NotificationService()
        {
            //events = _eventService.GetAllEvents();
        }

        private static bool IsEventOrganizer(Domain.Entities.EventCollaborator eventCollaborator)
        {
            return eventCollaborator.ParticipantRole is not null && eventCollaborator.ParticipantRole.Equals("organizer");
        }
        private List<Domain.Entities.EventCollaborator> GetConsiderableEventCollaborators()
        {
            return [.. _eventCollaboratorsService.GetAllParticipants()
                                                 .Where(eventCollaborator => eventCollaborator.UserId == GlobalData.GetUser().Id &&
                                                 (IsEventOrganizer(eventCollaborator)
                                                 || (!IsEventOrganizer(eventCollaborator)
                                                      && eventCollaborator.ConfirmationStatus is not null
                                                      && !(eventCollaborator.ConfirmationStatus.Equals("reject")
                                                            && eventCollaborator.ProposedStartHour is null
                                                            && eventCollaborator.ProposedEndHour is null
                                                          )
                                                    )
                                                    && !_eventService.GetProposedEvents().Exists(eventObj=>eventObj.Id == eventCollaborator.EventId)
                                                 ))];
        }

        public List<EventByDate> GetUpcomingEvents()
        {
            List<Domain.Entities.EventCollaborator> scheduleEvents = [.. GetConsiderableEventCollaborators()
                                                         .Where(eventCollaborator => eventCollaborator.EventDate == DateOnly.FromDateTime(DateTime.Now))
                                                         .OrderBy(eventCollaborator => eventCollaborator.EventDate)
                                                         .ThenBy(eventCollaborator =>eventCollaborator.ProposedStartHour)];

            List<EventByDate> upcommingEvents = [];

            foreach (var scheduleEvent in scheduleEvents)
            {
                Event? eventObj = events.Find(eventObj => eventObj.Id == scheduleEvent.EventId);

                if (eventObj is null) continue;

                upcommingEvents.Add(new EventByDate(scheduleEvent.EventDate, eventObj));
            }

            return upcommingEvents;
        }

        public List<EventByDate> GetProposedEvents()
        {
            HashSet<int> proposedEventIds = GetProposedEventIds();

            List<Domain.Entities.EventCollaborator> proposedEventCollaborators = GetProposedEventCollaborators(proposedEventIds);

            List<EventByDate> proposedEventsWithDate = [];

            foreach (var proposedEventCollaborator in proposedEventCollaborators)
            {
                Event? eventObj = events.Find(eventObj => eventObj.Id == proposedEventCollaborator.EventId);

                if (eventObj is null) continue;

                proposedEventsWithDate.Add(new EventByDate(proposedEventCollaborator.EventDate, eventObj));
            }

            return proposedEventsWithDate;
        }

        private List<Domain.Entities.EventCollaborator> GetProposedEventCollaborators(HashSet<int> proposedEventIds)
        {
            List<Domain.Entities.EventCollaborator> proposedEventCollaborators = [.. _eventCollaboratorsService.GetAllParticipants()
                                                                 .Where(eventCollaborator => proposedEventIds.Contains(eventCollaborator.EventId)
                                                                        && eventCollaborator.UserId == GlobalData.GetUser().Id)
                                                                 .OrderBy(eventCollaborator => eventCollaborator.EventDate)
                                                                 .ThenBy(eventCollaborator =>eventCollaborator.ProposedStartHour)];
            return proposedEventCollaborators;
        }

        private HashSet<int> GetProposedEventIds()
        {
            return events.Where(eventObj => eventObj.IsProposed && eventObj.EventStartDate >= DateOnly.FromDateTime(DateTime.Now))
                         .Select(eventObj => eventObj.Id)
                         .ToHashSet();
        }
    }
}