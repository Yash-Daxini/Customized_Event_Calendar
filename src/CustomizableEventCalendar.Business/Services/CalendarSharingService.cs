using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Data.Repositories;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services
{
    internal class CalendarSharingService
    {
        SharedCalendarRepository sharedEventsRepository = new SharedCalendarRepository();
        public void AddSharedCalendar(SharedCalendar sharedEvent)
        {
            try
            {
                sharedEventsRepository = new SharedCalendarRepository();
                sharedEventsRepository.Create(sharedEvent);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Some error occurred ! " + ex.Message);
            }
        }
        public List<SharedCalendar> GetSharedEvents()
        {
            List<SharedCalendar> sharedEvents = new List<SharedCalendar>();
            try
            {
                sharedEvents = sharedEventsRepository.Read(data => new SharedCalendar(data))
                                                                        .Where(sharedEvent =>
                                                                         sharedEvent.UserId == GlobalData.user.Id)
                                                                        .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Some error occurred ! " + ex.Message);
            }
            return sharedEvents;
        }
        public string GenerateDisplayFormatForSharedEvents()
        {
            List<SharedCalendar> sharedEvents = GetSharedEvents();
            StringBuilder sharedEventUsers = new StringBuilder();

            UserRepository userService = new UserRepository();

            foreach (var sharedEvent in sharedEvents)
            {
                User user = userService.Read(data => new User(data), sharedEvent.SharedByUserId);
                sharedEventUsers.AppendLine($" Sr. NO :- {sharedEvent.Id}" +
                                        $", Shared by {user.Name} " +
                                        $", From :- {sharedEvent.FromDate} , To :- {sharedEvent.ToDate}");
            }

            return sharedEventUsers.ToString();
        }
        public List<ScheduleEvent> GetSharedScheduleEvents(SharedCalendar sharedEvent, HashSet<int> sharedEventIds)
        {
            ScheduleEventRepository scheduleEventRepository = new ScheduleEventRepository();

            ScheduleEventService scheduleEventService = new ScheduleEventService();

            List<ScheduleEvent> schedulers = scheduleEventRepository.Read(data => new ScheduleEvent(data))
                                                           .Where(scheduleEvent =>
                                                                DateOnly.FromDateTime(scheduleEvent.ScheduledDate)
                                                                >= sharedEvent.FromDate &&
                                                                DateOnly.FromDateTime(scheduleEvent.ScheduledDate)
                                                                <= sharedEvent.ToDate)
                                                           .ToList();

            schedulers = schedulers.Where(scheduleEvent => sharedEventIds.Contains
                                    (scheduleEventService.GetEventIdFromEventCollaborators(
                                                                  scheduleEvent.EventCollaboratorsId)))
                                    .ToList();

            return schedulers;
        }
        public string GenerateSharedCalendar(int sharedEventId)
        {
            SharedCalendar sharedEvent = sharedEventsRepository.Read(data => new SharedCalendar(data), sharedEventId);

            EventRepository eventRepository = new EventRepository();
            List<Event> events = eventRepository.Read(data => new Event(data));

            HashSet<int> sharedEventIds = events.Where(eventObj => eventObj.UserId == sharedEvent.SharedByUserId)
                                          .Select(eventObj => eventObj.Id)
                                          .ToHashSet();

            List<ScheduleEvent> schedulers = GetSharedScheduleEvents(sharedEvent, sharedEventIds);

            StringBuilder sharedEventInfo = new StringBuilder();

            DateOnly startDate = sharedEvent.FromDate;
            DateOnly endDate = sharedEvent.ToDate;

            ScheduleEventService scheduleEventService = new ScheduleEventService();

            sharedEventInfo.AppendLine("\tSr No.\tEvent No.\tEvent Title\tEvent Description\tEvent Timing");

            while (startDate <= endDate)
            {
                ScheduleEvent? scheduleEvent = schedulers.FirstOrDefault(scheduleEvent =>
                                                                    DateOnly.FromDateTime(
                                                                    Convert.ToDateTime
                                                                    (scheduleEvent.ScheduledDate))
                                                                    == startDate);

                if (scheduleEvent != null)
                {
                    Event? eventObj = events.FirstOrDefault(eventObj => eventObj.Id == scheduleEventService
                                                            .GetEventIdFromEventCollaborators
                                                             (scheduleEvent.EventCollaboratorsId)
                                                            );

                    sharedEventInfo.AppendLine($"\t{scheduleEvent.Id}\t{eventObj.Id}\t{eventObj.Title}" +
                                               $"\t{eventObj.Description}\t{scheduleEvent.ScheduledDate}\t");
                }

                startDate = startDate.AddDays(1);
            }

            return sharedEventInfo.ToString();
        }
    }
}
