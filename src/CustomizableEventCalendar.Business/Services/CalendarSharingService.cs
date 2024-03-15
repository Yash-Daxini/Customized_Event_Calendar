using System.Text;
using CustomizableEventCalendar.src.CustomizableEventCalendar.ConsoleApp;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Data.Repositories;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services
{
    internal class CalendarSharingService
    {
        private readonly SharedCalendarRepository _sharedEventsRepository = new();

        public static List<EventCollaborator> availableEventsToCollaborate = [];
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
                                                                         sharedEvent.ReceiverUserId == GlobalData.user.Id)
                                                                        .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Some error occurred ! " + ex.Message);
            }

            return sharedEvents;
        }

        public int GetSharedEventsCount()
        {
            return GetSharedEvents().Count();
        }

        public string GenerateDisplayFormatForSharedEvents()
        {
            List<SharedCalendar> sharedEvents = GetSharedEvents();

            StringBuilder sharedEventsDisplayString = new();

            UserRepository userService = new();

            List<List<string>> sharedEventsTableContent = [["Sr. NO", "Shared by", "From", "To"]];

            foreach (var (sharedEvent, index) in sharedEvents.Select((sharedEvent, index) => (sharedEvent, index)))
            {
                User? user = userService.GetById(data => new User(data), sharedEvent.SenderUserId);
                sharedEventsTableContent.Add([(index+1).ToString(),user.Name,sharedEvent.FromDate+""
                                              ,sharedEvent.ToDate+"" ]);
            }

            sharedEventsDisplayString.Append(PrintHandler.GiveTable(sharedEventsTableContent));

            return sharedEventsDisplayString.ToString();
        }

        public List<EventCollaborator> GetSharedScheduleEvents(SharedCalendar sharedEvent, HashSet<int> sharedEventIds)
        {
            EventCollaboratorRepository eventCollaboratorsRepository = new();

            List<EventCollaborator> eventCollaborators = eventCollaboratorsRepository.GetAll(data => new EventCollaborator(data))
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

            HashSet<int> sharedEventIds = events.Where(eventObj => eventObj.UserId == sharedEvent.SenderUserId)
                                                .Select(eventObj => eventObj.Id)
                                                .ToHashSet();

            List<EventCollaborator> eventCollaborators = GetSharedScheduleEvents(sharedEvent, sharedEventIds);

            StringBuilder sharedEventInfo = new();

            DateOnly startDate = sharedEvent.FromDate;
            DateOnly endDate = sharedEvent.ToDate;

            EventCollaboratorService eventCollaboratorsService = new();

            List<List<string>> sharedEventTableContent = [[ "Sr No.", "Event No.","Event Title","Event Description",
                                                            "Event Timing"]];

            Dictionary<DateOnly, EventCollaborator> dateAndEvent = GenerateListOfAvailableCollaborationEvents(startDate, endDate, eventCollaborators);

            availableEventsToCollaborate = [];


            int index = 0;
            foreach (var (date, eventCollaborator) in dateAndEvent.Select(x => (x.Key, x.Value)))
            {
                if (eventCollaborator != null)
                {
                    Event? eventObj = events.Find(eventObj => eventObj.Id == eventCollaborator.EventId);

                    sharedEventTableContent.Add([(index+1).ToString() , eventObj.Id.ToString(),
                                                 eventObj.Title,eventObj.Description,
                                                 eventCollaborator.EventDate.ToString()
                                                ]);

                    availableEventsToCollaborate.Add(eventCollaborator);
                    index++;
                }
                else
                {
                    sharedEventTableContent.Add(["-", "-", "-", "-", date.ToString()]);
                }

                startDate = startDate.AddDays(1);
            }

            sharedEventInfo.AppendLine(PrintHandler.GiveTable(sharedEventTableContent));

            return sharedEventInfo.ToString();
        }

        public static Dictionary<DateOnly, EventCollaborator> GenerateListOfAvailableCollaborationEvents(DateOnly startDate, DateOnly endDate, List<EventCollaborator> eventCollaborators)
        {
            Dictionary<DateOnly, EventCollaborator?> availableEventCollaboratorsonSpecificDate = [];

            while (startDate <= endDate)
            {
                EventCollaborator? eventCollaborator = eventCollaborators.Find(eventCollaborator =>
                                                                                DateOnly.FromDateTime(eventCollaborator.EventDate) ==
                                                                                                      startDate);

                if (eventCollaborator != null)
                {
                    availableEventCollaboratorsonSpecificDate.Add(startDate, eventCollaborator);
                }
                else
                {
                    availableEventCollaboratorsonSpecificDate.Add(startDate, null); 
                }

                startDate = startDate.AddDays(1);
            }

            return availableEventCollaboratorsonSpecificDate;
        }

        public List<EventCollaborator> GetAvailableEventCollaborations()
        {
            
            return availableEventsToCollaborate;
        }
    }
}