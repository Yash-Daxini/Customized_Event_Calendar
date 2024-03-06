using System;
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
        private readonly ScheduleEventService scheduleEventService = new();

        public string GenerateNotification()
        {
            StringBuilder notification = new();

            EventService eventService = new();

            List<Event> events = eventService.GetAllEvents();

            HashSet<int> eventIds = events.Select(eventObj => eventObj.Id)
                                          .ToHashSet();

            List<ScheduleEvent> scheduleEvents = scheduleEventService.GetAllScheduleEvents()
                                                            .Where(scheduleEvent => eventIds.Contains
                                                            (scheduleEventService.GetEventIdFromEventCollaborators
                                                            (scheduleEvent.EventCollaboratorsId)) &&
                                                            scheduleEventService.GetUserIdFromEventCollaborators
                                                            (scheduleEvent.EventCollaboratorsId) ==
                                                            GlobalData.user.Id)
                                                            .ToList();

            notification.AppendLine(new string('═', Console.WindowWidth));

            notification.AppendLine();

            notification.AppendLine(new string(' ', (Console.WindowWidth - 20) / 2) + "Notifications\n");

            string completedEvents = GetCompletedEvents(scheduleEvents, events);
            string upcommingEvents = GetUpcomingEvents(scheduleEvents, events);
            string proposedEvents = GetProposedEvents(events);

            if (completedEvents.Length > 0) notification.AppendLine(completedEvents);
            if (upcommingEvents.Length > 0) notification.AppendLine(upcommingEvents);
            if (proposedEvents.Length > 0) notification.AppendLine(proposedEvents);

            notification.AppendLine(new string('═', Console.WindowWidth));

            return notification.ToString();
        }

        public string GetCompletedEvents(List<ScheduleEvent> scheduleEvents, List<Event> events)
        {
            StringBuilder completedEvents = new();

            DateTime todayDate = DateTime.Now;

            List<ScheduleEvent> missedEvents = scheduleEvents.Where(scheduleEvent =>
                                                                    scheduleEvent.ScheduledDate <=
                                                                    todayDate)
                                                             .ToList();

            if (missedEvents.Count == 0) return "";

            completedEvents.AppendLine($"Completed events :- ");
            completedEvents.AppendLine($"{PrintHandler.PrintHorizontalLine()}");

            EventCollaboratorsService eventCollaboratorsService = new();

            List<List<string>> completedEventNotificationsTableContent = [["Event", "Description", "Date"]];

            foreach (ScheduleEvent scheduleEvent in missedEvents)
            {
                int eventIdFromEventCollaboratorId = scheduleEventService
                                                    .GetEventIdFromEventCollaborators
                                                     (scheduleEvent.EventCollaboratorsId);

                EventCollaborators? eventCollaborator = eventCollaboratorsService.GetEventCollaboratorsByEventId
                                                                                 (eventIdFromEventCollaboratorId);

                if (eventCollaborator != null && eventCollaborator.UserId != GlobalData.user.Id) continue;

                Event? eventObj = events.FirstOrDefault(eventObj => eventObj.Id == eventIdFromEventCollaboratorId);
                completedEventNotificationsTableContent.Add(new List<string> {eventObj.Title,
                                                                         eventObj.Description,
                                                                         scheduleEvent.ScheduledDate.ToString() });
            }

            completedEvents.AppendLine(PrintHandler.GiveTableForNotification(completedEventNotificationsTableContent));

            return completedEvents.ToString();
        }

        public string GetUpcomingEvents(List<ScheduleEvent> scheduleEvents, List<Event> events)
        {
            StringBuilder upcommingEvents = new();

            DateTime todayDate = DateTime.Now;

            List<ScheduleEvent> upcommingEventsList = scheduleEvents.Where(scheduleEvent =>
                                                                            scheduleEvent.ScheduledDate.Date == todayDate.Date)
                                                                    .ToList();

            if (upcommingEventsList.Count == 0) return "";

            upcommingEvents.AppendLine("Your today's events :- ");
            upcommingEvents.AppendLine($"{PrintHandler.PrintHorizontalLine()}");

            List<List<string>> upcommingEventNotificationsTableContent = [["Event", "Description", "Date", "Time"]];

            EventCollaboratorsService eventCollaboratorsService = new();

            foreach (ScheduleEvent scheduleEvent in upcommingEventsList)
            {

                int eventIdFromEventCollaboratorId = scheduleEventService
                                                    .GetEventIdFromEventCollaborators
                                                     (scheduleEvent.EventCollaboratorsId);

                EventCollaborators? eventCollaborator = eventCollaboratorsService.GetEventCollaboratorsByEventId
                                                                                 (eventIdFromEventCollaboratorId);

                if (eventCollaborator != null && eventCollaborator.UserId != GlobalData.user.Id) continue;

                Event? eventObj = events.FirstOrDefault(eventObj => eventObj.Id ==
                                                       scheduleEventService.GetEventIdFromEventCollaborators
                                                                            (scheduleEvent.EventCollaboratorsId));

                upcommingEventNotificationsTableContent.Add([eventObj.Title,eventObj.Description,scheduleEvent.ScheduledDate.ToString(),
                                                             eventObj.TimeBlock]);
            }

            upcommingEvents.AppendLine(PrintHandler.GiveTableForNotification(upcommingEventNotificationsTableContent));

            return upcommingEvents.ToString();
        }

        public string GetProposedEvents(List<Event> events)
        {
            StringBuilder proposedEvents = new();

            HashSet<int> proposedEventIds = events.Where(eventObj => eventObj.IsProposed)
                                                  .Select(eventObj => eventObj.Id)
                                                  .ToHashSet();

            EventCollaboratorsService eventCollaboratorsService = new();

            List<EventCollaborators> eventCollaborators = eventCollaboratorsService.GetAllEventCollaborators();

            eventCollaborators = eventCollaborators.Where(eventCollaborator => proposedEventIds
                                                          .Contains(eventCollaborator.EventId) &&
                                                           eventCollaborator.UserId == GlobalData.user.Id)
                                                   .ToList();

            bool isUserHasProposedEvent = eventCollaborators.Exists(eventCollaborator =>
                                                             eventCollaborator.UserId == GlobalData.user.Id);


            if (events.Count == 0 || !isUserHasProposedEvent) return "";

            proposedEvents.AppendLine("Proposed Events");
            proposedEvents.AppendLine($"{PrintHandler.PrintHorizontalLine()}");

            List<List<string>> propsedEventNotificationsTableContent = [[ "Event Proposed by","Date","Time Block","Event",
                                                                           "Description"]];

            UserService userService = new();
            RecurrenceService recurrenceService = new();
            EventService eventService = new();


            foreach (var eventCollaborator in eventCollaborators)
            {
                Event eventObj = eventService.GetEventsById(eventCollaborator.EventId);
                if (eventObj.UserId == GlobalData.user.Id) continue;
                User? eventProposer = userService.Read(eventObj.UserId);
                RecurrencePatternCustom recurrencePattern = recurrenceService.GetRecurrencePatternById(eventObj.RecurrenceId);
                propsedEventNotificationsTableContent.Add([ eventProposer.Name,
                                                                             recurrencePattern.DTSTART.ToString(),
                                                                             eventObj.TimeBlock,
                                                                             eventObj.Title,eventObj.Description
                                                                           ]
                                                         );
            }

            proposedEvents.AppendLine(PrintHandler.GiveTableForNotification(propsedEventNotificationsTableContent));

            return proposedEvents.ToString();
        }
    }
}