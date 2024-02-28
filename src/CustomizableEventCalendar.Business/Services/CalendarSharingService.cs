using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using CustomizableEventCalendar.src.CustomizableEventCalendar.ConsoleApp;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Data.Repositories;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services
{
    internal class CalendarSharingService
    {
        SharedCalendarRepository _sharedEventsRepository = new SharedCalendarRepository();
        public void AddSharedCalendar(SharedCalendar sharedEvent)
        {
            try
            {
                _sharedEventsRepository = new SharedCalendarRepository();
                _sharedEventsRepository.Create(sharedEvent);
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
                sharedEvents = _sharedEventsRepository.Read(data => new SharedCalendar(data))
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
            StringBuilder sharedEventsDisplayString = new StringBuilder();

            UserRepository userService = new UserRepository();

            List<List<string>> sharedEventsTableContent = new List<List<string>> { new List<string> { "Sr. NO", "Shared by", "From", "To" } };

            foreach (var sharedEvent in sharedEvents)
            {
                User user = userService.Read(data => new User(data), sharedEvent.SharedByUserId);
                sharedEventsTableContent.Add(new List<string>{sharedEvent.Id.ToString(),user.Name,
                                                              sharedEvent.FromDate+"" , sharedEvent.ToDate+"" });
            }

            sharedEventsDisplayString.Append(PrintHandler.PrintTable(sharedEventsTableContent));

            return sharedEventsDisplayString.ToString();
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
            SharedCalendar sharedEvent = _sharedEventsRepository.Read(data => new SharedCalendar(data), sharedEventId);

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

            List<List<string>> sharedEventTableContent = new List<List<string>> { new List<string> { "Sr No.", "Event No.",
                                                                                 "Event Title", "Event Description", "Event Timing" } };

            while (startDate <= endDate)
            {
                ScheduleEvent? scheduleEvent = schedulers.Find(scheduleEvent =>
                                                                    DateOnly.FromDateTime(
                                                                    Convert.ToDateTime
                                                                    (scheduleEvent.ScheduledDate))
                                                                    == startDate);

                if (scheduleEvent != null)
                {
                    Event? eventObj = events.Find(eventObj => eventObj.Id == scheduleEventService
                                                            .GetEventIdFromEventCollaborators
                                                             (scheduleEvent.EventCollaboratorsId)
                                                            );

                    sharedEventTableContent.Add(new List<string> { scheduleEvent.Id.ToString() , eventObj.Id.ToString(),eventObj.Title,
                                                                   eventObj.Description,scheduleEvent.ScheduledDate.ToString() });
                }

                startDate = startDate.AddDays(1);
            }

            sharedEventInfo.AppendLine(PrintHandler.PrintTable(sharedEventTableContent));

            return sharedEventInfo.ToString();
        }
    }
}