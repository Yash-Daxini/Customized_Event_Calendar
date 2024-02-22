using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services
{
    internal class NotificationService
    {
        ScheduleEventService scheduleEventService = new ScheduleEventService();
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
                                                             (scheduleEvent.EventCollaboratorsId)))
                                                             .ToList();

            string completedEvents = GetCompletedEvents(scheduleEvents, events);
            string upcommingEvents = GetUpcommingEvents(scheduleEvents, events);
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

            completedEvents.AppendLine("You missed this events :- ");


            foreach (ScheduleEvent scheduleEvent in missedEvents)
            {
                Event eventObj = events.FirstOrDefault(eventObj => eventObj.Id == scheduleEventService.GetEventIdFromEventCollaborators(scheduleEvent.EventCollaboratorsId));
                completedEvents.AppendLine($"Event :- {eventObj.Title}, " +
                                           $"Description :- {eventObj.Description}, " +
                                           $"Time :- {eventObj.TimeBlock}");
            }

            return completedEvents.ToString();
        }
        public string GetUpcommingEvents(List<ScheduleEvent> scheduleEvents, List<Event> events)
        {
            StringBuilder upcommingEvents = new StringBuilder();

            DateTime todayDate = DateTime.Now;

            List<ScheduleEvent> upcommingEventsList = scheduleEvents.Where(scheduleEvent =>
                                                                    scheduleEvent.ScheduledDate.Date == todayDate.Date)
                                                                    .ToList();

            if (upcommingEventsList.Count == 0) return "";

            upcommingEvents.AppendLine("Your today's events :- ");

            foreach (ScheduleEvent scheduleEvent in upcommingEventsList)
            {
                Event eventObj = events.FirstOrDefault(eventObj => eventObj.Id == scheduleEventService.GetEventIdFromEventCollaborators(scheduleEvent.EventCollaboratorsId));
                upcommingEvents.AppendLine($"Event :- {eventObj.Title}, " +
                                           $"Description :- {eventObj.Description}, " +
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
                                                          .Contains(eventCollaborator.EventId))
                                                   .ToList();

            bool isUserHasProposedEvent = eventCollaborators.Count(eventCollaborator =>
                                                             eventCollaborator.UserId == GlobalData.user.Id) != 0;


            if (events.Count == 0 || !isUserHasProposedEvent) return "";

            proposedEvents.AppendLine("Proposed Events");

            UserService userService = new UserService();
            RecurrenceService recurrenceService = new RecurrenceService();
            EventService eventService = new EventService();


            foreach (var eventCollaborator in eventCollaborators)
            {
                Event eventObj = eventService.Read(eventCollaborator.EventId);
                if (eventObj.UserId == GlobalData.user.Id) continue;
                User? eventProposer = userService.Read(eventObj.UserId);
                RecurrencePattern recurrencePattern = recurrenceService.Read(eventObj.RecurrenceId);
                proposedEvents.AppendLine($"Event Proposed by {eventProposer.Name} on " +
                                          $"{recurrencePattern.DTSTART}between {eventObj.TimeBlock}. " +
                                          $"Event Title :- {eventObj.Title} , " +
                                          $"Event Description :-{eventObj.Description}");
            }

            return proposedEvents.ToString();
        }
    }
}
