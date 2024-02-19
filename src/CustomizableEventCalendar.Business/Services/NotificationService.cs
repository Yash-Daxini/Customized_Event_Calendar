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
        public string GenerateNotification()
        {
            StringBuilder notification = new StringBuilder();

            SchedulerService schedulerService = new SchedulerService();

            EventService eventService = new EventService();

            List<Event> events = eventService.Read()
                                             .Where(eventObj => eventObj.UserId == GlobalData.user.Id)
                                             .ToList();

            HashSet<int> eventIds = events.Select(eventObj => eventObj.Id)
                                          .ToHashSet();

            List<Scheduler> scheduleEvents = schedulerService.Read()
                                                             .Where(scheduleEvent => eventIds.Contains(scheduleEvent.EventId))
                                                             .ToList();

            string completedEvents = GetCompletedEvents(scheduleEvents, events);
            string upcommingEvents = GetUpcommingEvents(scheduleEvents, events);

            notification.AppendLine(completedEvents);
            notification.AppendLine(upcommingEvents);

            return notification.ToString();
        }
        public string GetCompletedEvents(List<Scheduler> scheduleEvents, List<Event> events)
        {
            StringBuilder completedEvents = new StringBuilder();

            DateTime todayDate = DateTime.Now;

            completedEvents.AppendLine("You missed this events :- ");

            foreach (Scheduler scheduleEvent in scheduleEvents.Where(scheduleEvent => scheduleEvent.ScheduledDate.Date < todayDate.Date))
            {
                Event eventObj = events.FirstOrDefault(eventObj => eventObj.Id == scheduleEvent.EventId);
                completedEvents.AppendLine($"Event :- {eventObj.Title}, " +
                                           $"Description :- {eventObj.Description}, " +
                                           $"Time :- {eventObj.TimeBlock}");
            }

            return completedEvents.ToString();
        }
        public string GetUpcommingEvents(List<Scheduler> scheduleEvents, List<Event> events)
        {
            StringBuilder upcommingEvents = new StringBuilder();

            DateTime todayDate = DateTime.Now;

            upcommingEvents.AppendLine("Your today's events :- ");

            foreach (Scheduler scheduleEvent in scheduleEvents.Where(scheduleEvent => scheduleEvent.ScheduledDate.Date == todayDate.Date))
            {
                Event eventObj = events.FirstOrDefault(eventObj => eventObj.Id == scheduleEvent.EventId);
                upcommingEvents.AppendLine($"Event :- {eventObj.Title}, " +
                                           $"Description :- {eventObj.Description}, " +
                                           $"Time :- {eventObj.TimeBlock}");
            }

            return upcommingEvents.ToString();
        }
    }
}
