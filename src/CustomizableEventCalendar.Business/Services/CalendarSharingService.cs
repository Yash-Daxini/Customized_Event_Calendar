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
        SharedEventsRepository sharedEventsRepository = new SharedEventsRepository();
        public void AddSharedCalendar(SharedEvents sharedEvent)
        {
            try
            {
                sharedEventsRepository = new SharedEventsRepository();
                sharedEventsRepository.Create(sharedEvent);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Some error occurred ! " + ex.Message);
            }
        }
        public List<SharedEvents> GetSharedEvents()
        {
            List<SharedEvents> sharedEvents = new List<SharedEvents>();
            try
            {
                sharedEvents = sharedEventsRepository.Read(data => new SharedEvents(data))
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
            List<SharedEvents> sharedEvents = GetSharedEvents();
            StringBuilder sharedEventUsers = new StringBuilder();

            UserRepository userService = new UserRepository();

            foreach (var sharedEvent in sharedEvents)
            {
                User user = userService.Read(data => new User(data), sharedEvent.UserId);
                sharedEventUsers.Append($" Sr. NO :- {sharedEvent.Id} , User Sr No. :- {user.Id} " +
                                        $", Name :- {user.Name} " +
                                        $", From :- {sharedEvent.FromDate} , To :- {sharedEvent.ToDate}");
            }

            return sharedEventUsers.ToString();
        }
        public List<Scheduler> GetSharedScheduleEvents(SharedEvents sharedEvent, HashSet<int> sharedEventIds)
        {
            ScheduleRepository scheduleRepository = new ScheduleRepository();

            List<Scheduler> schedulers = scheduleRepository.Read(data => new Scheduler(data))
                                                           .Where(scheduleEvent =>
                                                                sharedEventIds.Contains(scheduleEvent.EventId) &&
                                                                DateOnly.FromDateTime(scheduleEvent.ScheduledDate)
                                                                >= sharedEvent.FromDate &&
                                                                DateOnly.FromDateTime(scheduleEvent.ScheduledDate)
                                                                <= sharedEvent.ToDate)
                                                           .ToList();
            return schedulers;
        }
        public string GenerateSharedCalendar(int sharedEventId)
        {
            SharedEvents sharedEvent = sharedEventsRepository.Read(data => new SharedEvents(data), sharedEventId);

            EventRepository eventRepository = new EventRepository();
            List<Event> events = eventRepository.Read(data => new Event(data));

            HashSet<int> sharedEventIds = events.Where(eventObj => eventObj.UserId == sharedEvent.SharedByUserId)
                                          .Select(eventObj => eventObj.Id)
                                          .ToHashSet();

            List<Scheduler> schedulers = GetSharedScheduleEvents(sharedEvent, sharedEventIds);

            StringBuilder sharedEventInfo = new StringBuilder();

            DateOnly startDate = sharedEvent.FromDate;
            DateOnly endDate = sharedEvent.ToDate;

            sharedEventInfo.AppendLine("\tEvent No.\tEvent Title\tEvent Description\tEvent Timing");

            while (startDate <= endDate)
            {
                Scheduler? scheduleEvent = schedulers.FirstOrDefault(scheduleEvent =>
                                                                    DateOnly.FromDateTime(
                                                                    Convert.ToDateTime(scheduleEvent.ScheduledDate))
                                                                    == startDate);

                if (scheduleEvent != null)
                {
                    Event? eventObj = events.FirstOrDefault(eventObj => eventObj.Id == scheduleEvent.EventId);

                    sharedEventInfo.AppendLine($"\t{eventObj.Id}\t{eventObj.Title}\t{eventObj.Description}" +
                                               $"\t{scheduleEvent.ScheduledDate}\t");
                }

                startDate = startDate.AddDays(1);
            }

            return sharedEventInfo.ToString();
        }
    }
}
