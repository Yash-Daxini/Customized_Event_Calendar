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
                                                             .Where(scheduleEvent => eventIds.Contains(scheduleEventService.GetEventIdFromEventCollaborators(scheduleEvent.EventCollaboratorsId)))
                                                             .ToList();

            string completedEvents = GetCompletedEvents(scheduleEvents, events);
            string upcommingEvents = GetUpcommingEvents(scheduleEvents, events);

            notification.AppendLine(completedEvents);
            notification.AppendLine(upcommingEvents);

            return notification.ToString();
        }
        public string GetCompletedEvents(List<ScheduleEvent> scheduleEvents, List<Event> events)
        {
            StringBuilder completedEvents = new StringBuilder();

            DateTime todayDate = DateTime.Now;

            List<ScheduleEvent> missedEvents = scheduleEvents.Where(scheduleEvent => scheduleEvent.ScheduledDate.Date <

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
    }
}
