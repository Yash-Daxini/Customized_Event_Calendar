using System.Text;
using CustomizableEventCalendar.src.CustomizableEventCalendar.ConsoleApp;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Data.Repositories;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services
{
    internal class CalendarSharingService
    {
        private readonly SharedCalendarRepository _sharedEventsRepository = new();
        public void AddSharedCalendar(SharedCalendar sharedEvent)
        {
            try
            {
                _sharedEventsRepository.Insert(sharedEvent);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Some error occurred ! " + ex.Message);
            }
        }

        public List<SharedCalendar> GetSharedEvents()
        {
            List<SharedCalendar> sharedEvents = [];

            try
            {
                sharedEvents = _sharedEventsRepository.GetAll(data => new SharedCalendar(data))
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

            StringBuilder sharedEventsDisplayString = new();

            UserRepository userService = new();

            List<List<string>> sharedEventsTableContent = [["Sr. NO", "Shared by", "From", "To"]];

            foreach (var sharedEvent in sharedEvents)
            {
                User? user = userService.GetById(data => new User(data), sharedEvent.SharedByUserId);
                sharedEventsTableContent.Add([sharedEvent.Id.ToString(),user.Name,sharedEvent.FromDate+""
                                              ,sharedEvent.ToDate+"" ]);
            }

            sharedEventsDisplayString.Append(PrintHandler.GiveTable(sharedEventsTableContent));

            return sharedEventsDisplayString.ToString();
        }

        public List<ScheduleEvent> GetSharedScheduleEvents(SharedCalendar sharedEvent, HashSet<int> sharedEventIds)
        {
            ScheduleEventRepository scheduleEventRepository = new();

            ScheduleEventService scheduleEventService = new();

            List<ScheduleEvent> schedulers = scheduleEventRepository.GetAll(data => new ScheduleEvent(data))
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
            SharedCalendar? sharedEvent = _sharedEventsRepository.GetById(data => new SharedCalendar(data), sharedEventId);

            if (sharedEvent == null) return "";

            EventRepository eventRepository = new();
            List<Event> events = eventRepository.GetAll(data => new Event(data));

            HashSet<int> sharedEventIds = events.Where(eventObj => eventObj.UserId == sharedEvent.SharedByUserId)
                                                .Select(eventObj => eventObj.Id)
                                                .ToHashSet();

            List<ScheduleEvent> schedulers = GetSharedScheduleEvents(sharedEvent, sharedEventIds);

            StringBuilder sharedEventInfo = new();

            DateOnly startDate = sharedEvent.FromDate;
            DateOnly endDate = sharedEvent.ToDate;

            ScheduleEventService scheduleEventService = new ScheduleEventService();

            List<List<string>> sharedEventTableContent = [[ "Sr No.", "Event No.","Event Title","Event Description",
                                                            "Event Timing"]];

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

                    sharedEventTableContent.Add([scheduleEvent.Id.ToString() , eventObj.Id.ToString(),
                                                 eventObj.Title,eventObj.Description,
                                                 scheduleEvent.ScheduledDate.ToString()
                                                ]);
                }

                startDate = startDate.AddDays(1);
            }

            sharedEventInfo.AppendLine(PrintHandler.GiveTable(sharedEventTableContent));

            return sharedEventInfo.ToString();
        }
    }
}