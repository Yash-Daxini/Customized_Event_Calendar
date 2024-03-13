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

        public List<EventCollaborators> GetSharedScheduleEvents(SharedCalendar sharedEvent, HashSet<int> sharedEventIds)
        {
            EventCollaboratorsRepository eventCollaboratorsRepository = new();

            List<EventCollaborators> eventCollaborators = eventCollaboratorsRepository.GetAll(data => new EventCollaborators(data))
                                                                    .Where(scheduleEvent =>
                                                                        DateOnly.FromDateTime(scheduleEvent.EventDate) >=
                                                                        sharedEvent.FromDate &&
                                                                        DateOnly.FromDateTime(scheduleEvent.EventDate)
                                                                        <= sharedEvent.ToDate)
                                                                    .ToList();

            eventCollaborators = eventCollaborators.Where(eventCollaborator => sharedEventIds.Contains(eventCollaborator.EventId))
                                                   .ToList();

            return eventCollaborators;
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

            List<EventCollaborators> eventCollaborators = GetSharedScheduleEvents(sharedEvent, sharedEventIds);

            StringBuilder sharedEventInfo = new();

            DateOnly startDate = sharedEvent.FromDate;
            DateOnly endDate = sharedEvent.ToDate;

            EventCollaboratorsService eventCollaboratorsService = new();

            List<List<string>> sharedEventTableContent = [[ "Sr No.", "Event No.","Event Title","Event Description",
                                                            "Event Timing"]];

            while (startDate <= endDate)
            {
                EventCollaborators? eventCollaborator = eventCollaborators.Find(eventCollaborator =>
                                                                                DateOnly.FromDateTime(eventCollaborator.EventDate) ==
                                                                                                      startDate);

                if (eventCollaborator != null)
                {
                    Event? eventObj = events.Find(eventObj => eventObj.Id == eventCollaborator.EventId);

                    sharedEventTableContent.Add([eventCollaborator.Id.ToString() , eventObj.Id.ToString(),
                                                 eventObj.Title,eventObj.Description,
                                                 eventCollaborator.EventDate.ToString()
                                                ]);
                }

                startDate = startDate.AddDays(1);
            }

            sharedEventInfo.AppendLine(PrintHandler.GiveTable(sharedEventTableContent));

            return sharedEventInfo.ToString();
        }
    }
}