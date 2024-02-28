﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomizableEventCalendar.src.CustomizableEventCalendar.ConsoleApp;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services
{
    internal class NotificationService
    {
        private readonly ScheduleEventService scheduleEventService = new ScheduleEventService();
        public string GenerateNotification()
        {
            StringBuilder notification = new StringBuilder();

            EventService eventService = new EventService();

            List<Event> events = eventService.Read();

            HashSet<int> eventIds = events.Select(eventObj => eventObj.Id)
                                          .ToHashSet();

            List<ScheduleEvent> scheduleEvents = scheduleEventService.Read()
                                                            .Where(scheduleEvent => eventIds.Contains
                                                            (scheduleEventService.GetEventIdFromEventCollaborators
                                                            (scheduleEvent.EventCollaboratorsId)) &&
                                                            scheduleEventService.GetUserIdFromEventCollaborators
                                                            (scheduleEvent.EventCollaboratorsId) ==
                                                            GlobalData.user.Id)
                                                            .ToList();

            string completedEvents = GetCompletedEvents(scheduleEvents, events);
            string upcommingEvents = GetUpcomingEvents(scheduleEvents, events);
            string proposedEvents = GetProposedEvents(events);

            if (completedEvents.Length > 0) notification.AppendLine(completedEvents);
            if (upcommingEvents.Length > 0) notification.AppendLine(upcommingEvents);
            if (proposedEvents.Length > 0) notification.AppendLine(proposedEvents);

            return notification.ToString();
        }
        public string GetCompletedEvents(List<ScheduleEvent> scheduleEvents, List<Event> events)
        {
            StringBuilder completedEvents = new StringBuilder();

            DateTime todayDate = DateTime.Now;

            List<ScheduleEvent> missedEvents = scheduleEvents.Where(scheduleEvent =>
                                                                    scheduleEvent.ScheduledDate.Date <
                                                                    todayDate.Date)
                                                             .ToList();

            if (missedEvents.Count == 0) return "";

            completedEvents.AppendLine($"Completed events :- ");
            completedEvents.AppendLine($"{PrintHandler.PrintHorizontalLine()}");

            EventCollaboratorsService eventCollaboratorsService = new EventCollaboratorsService();

            foreach (ScheduleEvent scheduleEvent in missedEvents)
            {
                int eventIdFromEventCollaboratorId = scheduleEventService
                                                    .GetEventIdFromEventCollaborators
                                                     (scheduleEvent.EventCollaboratorsId);

                EventCollaborators? eventCollaborator = eventCollaboratorsService.ReadByEventId
                                                                                 (eventIdFromEventCollaboratorId);

                if (eventCollaborator != null && eventCollaborator.UserId != GlobalData.user.Id) continue;

                Event? eventObj = events.FirstOrDefault(eventObj => eventObj.Id == eventIdFromEventCollaboratorId);
                completedEvents.AppendLine($"Event :- {eventObj.Title}, " +
                                           $"Description :- {eventObj.Description}, " +
                                           $"Date : {scheduleEvent.ScheduledDate}");
            }

            return completedEvents.ToString();
        }
        public string GetUpcomingEvents(List<ScheduleEvent> scheduleEvents, List<Event> events)
        {
            StringBuilder upcommingEvents = new StringBuilder();

            DateTime todayDate = DateTime.Now;

            List<ScheduleEvent> upcommingEventsList = scheduleEvents.Where(scheduleEvent =>
                                                                    scheduleEvent.ScheduledDate.Date ==
                                                                                  todayDate.Date)
                                                                    .ToList();

            if (upcommingEventsList.Count == 0) return "";

            upcommingEvents.AppendLine("Your today's events :- ");
            upcommingEvents.AppendLine($"{PrintHandler.PrintHorizontalLine()}");

            EventCollaboratorsService eventCollaboratorsService = new EventCollaboratorsService();

            foreach (ScheduleEvent scheduleEvent in upcommingEventsList)
            {

                int eventIdFromEventCollaboratorId = scheduleEventService
                                                    .GetEventIdFromEventCollaborators
                                                     (scheduleEvent.EventCollaboratorsId);

                EventCollaborators? eventCollaborator = eventCollaboratorsService.ReadByEventId
                                                                                 (eventIdFromEventCollaboratorId);

                if (eventCollaborator != null && eventCollaborator.UserId != GlobalData.user.Id) continue;

                Event? eventObj = events.FirstOrDefault(eventObj => eventObj.Id ==
                                                       scheduleEventService.GetEventIdFromEventCollaborators
                                                                            (scheduleEvent.EventCollaboratorsId));

                upcommingEvents.AppendLine($"Event :- {eventObj.Title}, " +
                                           $"Description :- {eventObj.Description}, " +
                                           $"Date : {scheduleEvent.ScheduledDate}" +
                                           $"Time :- {eventObj.TimeBlock}");
            }

            return upcommingEvents.ToString();
        }
        public string GetProposedEvents(List<Event> events)
        {
            StringBuilder proposedEvents = new StringBuilder();

            HashSet<int> proposedEventIds = events.Where(eventObj => eventObj.IsProposed)
                                                  .Select(eventObj => eventObj.Id)
                                                  .ToHashSet();

            EventCollaboratorsService eventCollaboratorsService = new EventCollaboratorsService();

            List<EventCollaborators> eventCollaborators = eventCollaboratorsService.Read();

            eventCollaborators = eventCollaborators.Where(eventCollaborator => proposedEventIds
                                                          .Contains(eventCollaborator.EventId) &&
                                                           eventCollaborator.UserId == GlobalData.user.Id)
                                                   .ToList();

            bool isUserHasProposedEvent = eventCollaborators.Exists(eventCollaborator =>
                                                             eventCollaborator.UserId == GlobalData.user.Id);


            if (events.Count == 0 || !isUserHasProposedEvent) return "";

            proposedEvents.AppendLine("Proposed Events");
            proposedEvents.AppendLine($"{PrintHandler.PrintHorizontalLine()}");

            UserService userService = new UserService();
            RecurrenceService recurrenceService = new RecurrenceService();
            EventService eventService = new EventService();


            foreach (var eventCollaborator in eventCollaborators)
            {
                Event eventObj = eventService.Read(eventCollaborator.EventId);
                if (eventObj.UserId == GlobalData.user.Id) continue;
                User? eventProposer = userService.Read(eventObj.UserId);
                RecurrencePatternCustom recurrencePattern = recurrenceService.Read(eventObj.RecurrenceId);
                proposedEvents.AppendLine($"Event Proposed by {eventProposer.Name} on " +
                                          $"{recurrencePattern.DTSTART}between {eventObj.TimeBlock}. " +
                                          $"Event Title :- {eventObj.Title} , " +
                                          $"Event Description :-{eventObj.Description}");
            }

            return proposedEvents.ToString();
        }
    }
}