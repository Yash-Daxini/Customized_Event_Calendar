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
            _sharedEventsRepository.Insert(sharedEvent);
        }

        public List<SharedCalendar> GetSharedEvents()
        {
            return [.._sharedEventsRepository.GetAll(data => new SharedCalendar(data))
                                                                        .Where(sharedEvent =>
                                                                         sharedEvent.ReceiverUserId ==
                                                                         GlobalData.GetUser().Id)];

        }

        public int GetSharedEventsCount()
        {
            return GetSharedEvents().Count;
        }

        public string DesignSharedEventDisplayFormat()
        {
            List<SharedCalendar> sharedCalendars = GetSharedEvents();

            StringBuilder sharedEventsDisplayString = new();

            List<List<string>> sharedEventsTableContent = InsertInto2DList(sharedCalendars);

            sharedEventsDisplayString.Append(PrintService.GenerateTable(sharedEventsTableContent));

            return sharedEventsDisplayString.ToString();
        }

        public static List<List<string>> InsertInto2DList(List<SharedCalendar> sharedCalendars)
        {
            List<List<string>> sharedEventsTableContent = [["Sr. NO", "Shared by", "From", "To"]];

            UserRepository userService = new();

            foreach (var (sharedCalendar, index) in sharedCalendars.Select((sharedCalendar, index) => (sharedCalendar, index)))
            {
                User? user = userService.GetById(data => new User(data), sharedCalendar.SenderUserId);

                sharedEventsTableContent.Add([(index + 1).ToString(), user.Name, sharedCalendar.FromDate + ""
                                              , sharedCalendar.ToDate + ""]);
            };

            return sharedEventsTableContent;
        }

        public string GenerateSharedCalendar(int sharedEventId)
        {
            SharedCalendar? sharedCalendar = _sharedEventsRepository.GetById(data => new SharedCalendar(data), sharedEventId);

            if (sharedCalendar == null) return "";

            EventRepository eventRepository = new();

            List<Event> events = eventRepository.GetAll(data => new Event(data));

            HashSet<int> sharedEventIds = GetSharedEventIdsFromSharedCalendar(events, sharedCalendar);

            List<EventCollaborator> eventCollaborators = GetSharedEventsFromSharedCalendars(sharedCalendar, sharedEventIds);

            Dictionary<DateOnly, EventCollaborator?> dateAndEvent = GenerateListOfAvailableCollaborationEvents(sharedCalendar.FromDate,
                                                                   sharedCalendar.ToDate, eventCollaborators);

            availableEventsToCollaborate.Clear();

            return GenerateTableForSharedCalendar(events, dateAndEvent);
        }

        private static string GenerateTableForSharedCalendar(List<Event> events, Dictionary<DateOnly, EventCollaborator?> dateAndEvent)
        {
            StringBuilder sharedEventInfo = new();

            List<List<string>> sharedEventTableContent = [["Sr No.", "Event No.", "Event Title", "Event Description",
                                                            "Event Timing"]];

            int index = 0;
            foreach (var (date, eventCollaborator) in dateAndEvent.Select(x => (x.Key, x.Value)))
            {
                if (eventCollaborator != null)
                {
                    Event? eventObj = events.Find(eventObj => eventObj.Id == eventCollaborator.EventId);

                    sharedEventTableContent.Add([(index + 1).ToString(), eventObj.Id.ToString(),
                                                 eventObj.Title, eventObj.Description,
                                                 eventCollaborator.EventDate.ToString()
                                                ]);

                    availableEventsToCollaborate.Add(eventCollaborator);
                    index++;
                }
                else
                {
                    sharedEventTableContent.Add(["-", "-", "-", "-", date.ToString()]);
                }
            }

            sharedEventInfo.AppendLine(PrintService.GenerateTable(sharedEventTableContent));

            return sharedEventInfo.ToString();
        }

        private static HashSet<int> GetSharedEventIdsFromSharedCalendar(List<Event> events, SharedCalendar sharedCalendar)
        {
            HashSet<int> sharedEventIds = events.Where(eventObj => eventObj.UserId == sharedCalendar.SenderUserId)
                                                .Select(eventObj => eventObj.Id)
                                                .ToHashSet();

            return sharedEventIds;
        }

        private static List<EventCollaborator> GetSharedEventsFromSharedCalendars(SharedCalendar sharedCalendar, HashSet<int> sharedEventIds)
        {
            List<EventCollaborator> sharedEvents = GetAllSharedEventsBetweenGivenDate(sharedCalendar.FromDate, sharedCalendar.ToDate, sharedEventIds);

            return sharedEvents;
        }

        private static List<EventCollaborator> GetAllSharedEventsBetweenGivenDate(DateOnly fromDate, DateOnly toDate, HashSet<int> sharedEventIds)
        {

            EventCollaboratorRepository eventCollaboratorsRepository = new();

            List<EventCollaborator> sharedEvents = [.. eventCollaboratorsRepository.GetAll(data => new EventCollaborator(data))
                                                                    .Where(sharedEvent =>
                                                                      IsDateBetweenRange(fromDate,toDate,
                                                                      DateOnly.FromDateTime(sharedEvent.EventDate)) &&
                                                                      IsSharedEvent(sharedEventIds, sharedEvent.EventId))
                                                                    ];

            return sharedEvents;

        }

        private static bool IsDateBetweenRange(DateOnly startDate, DateOnly endDate, DateOnly checkingDate)
        {
            return checkingDate >= startDate && checkingDate <= endDate;
        }

        private static bool IsSharedEvent(HashSet<int> sharedEventIds, int eventId)
        {
            return sharedEventIds.Contains(eventId);
        }

        private static Dictionary<DateOnly, EventCollaborator?> GenerateListOfAvailableCollaborationEvents(DateOnly startDate, DateOnly endDate, List<EventCollaborator> eventCollaborators)
        {
            Dictionary<DateOnly, EventCollaborator?> availableEventCollaboratorsonSpecificDate = [];

            while (startDate <= endDate)
            {
                EventCollaborator? eventCollaborator = eventCollaborators.Find(eventCollaborator =>
                                                       DateOnly.FromDateTime(eventCollaborator.EventDate) == startDate);

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

        public static List<EventCollaborator> GetAvailableEventCollaborations()
        {

            return availableEventsToCollaborate;
        }
    }
}