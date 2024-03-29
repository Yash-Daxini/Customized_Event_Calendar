using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;


namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services
{
    internal class CalendarViewService
    {
        private readonly EventService _eventService = new();
        private readonly EventCollaboratorService _eventCollaboratorsService = new();

        public Dictionary<int, Event> GetHourAndEventsForDailyView()
        {
            return MapEachHourWithEvent();
        }

        private Dictionary<int, Event> MapEachHourWithEvent()
        {
            List<EventCollaborator> eventCollaborators = GetEventsOfToday();

            Dictionary<int, Event> hourEventMapping = [];

            foreach (var eventCollaborator in eventCollaborators)
            {
                Event eventObj = _eventService.GetEventById(eventCollaborator.EventId);

                AssignEventToSpecificHour(ref hourEventMapping, eventObj);
            }

            return hourEventMapping;
        }

        private bool IsProposedEvent(int eventId)
        {
            return _eventService.GetProposedEvents().Exists(eventObj => eventObj.Id == eventId);
        }

        private List<EventCollaborator> GetEventsOfToday()
        {
            return [.. _eventCollaboratorsService.GetAllEventCollaborators()
                   .Where(eventCollaborator => eventCollaborator.EventDate == DateOnly.FromDateTime(DateTime.Today)
                                               && IsConsiderableEventCollaborators(eventCollaborator)
                                               && !IsProposedEvent(eventCollaborator.EventId))];
        }

        private static bool IsConsiderableEventCollaborators(EventCollaborator eventCollaborator)
        {
            return eventCollaborator.UserId == GlobalData.GetUser().Id
                    && !(eventCollaborator.ConfirmationStatus != null && eventCollaborator.ConfirmationStatus.Equals("reject")
                        && eventCollaborator.ProposedStartHour == null
                        && eventCollaborator.ProposedEndHour == null
                       );
        }

        private static void AssignEventToSpecificHour(ref Dictionary<int, Event> eventRecordByHour, Event eventObj)
        {
            int startHour = eventObj.EventStartHour;
            int endHour = eventObj.EventEndHour;

            for (int i = startHour; i < endHour; i++)
            {
                eventRecordByHour[i] = eventObj;
            }
        }

        public Dictionary<DateOnly, List<Event>> GetDateAndEventsForWeeklyView()
        {
            DateOnly startDateOfWeek = DateTimeManager.GetStartDateOfWeek(DateOnly.FromDateTime(DateTime.Today));
            DateOnly endDateOfWeek = DateTimeManager.GetEndDateOfWeek(DateOnly.FromDateTime(DateTime.Today));

            Dictionary<DateOnly, List<Event>> currentWeekEvents = GetGivenWeekEventsWithDate(startDateOfWeek, endDateOfWeek);

            return currentWeekEvents;
        }

        private Dictionary<DateOnly, List<Event>> GetGivenWeekEventsWithDate(DateOnly startDateOfWeek, DateOnly endDateOfWeek)
        {
            List<EventCollaborator> eventCollaborators = _eventCollaboratorsService.GetAllEventCollaborators();

            Dictionary<DateOnly, List<Event>> currentWeekEvents = GenerateDictionaryOfDateOnlyAndEventFromList(GetGivenWeekEvents(eventCollaborators, startDateOfWeek, endDateOfWeek));

            return currentWeekEvents;
        }

        private List<EventCollaborator> GetGivenWeekEvents(List<EventCollaborator> eventCollaborators, DateOnly startDateOfWeek, DateOnly endDateOfWeek)
        {
            return [.. eventCollaborators.Where(eventCollaborator => IsEventOccurInGivenWeek(startDateOfWeek, endDateOfWeek, eventCollaborator))];
        }

        private bool IsEventOccurInGivenWeek(DateOnly startDateOfWeek, DateOnly endDateOfWeek, EventCollaborator eventCollaborator)
        {
            return eventCollaborator.EventDate >= startDateOfWeek
                   && eventCollaborator.EventDate <= endDateOfWeek
                   && IsConsiderableEventCollaborators(eventCollaborator)
                   && !IsProposedEvent(eventCollaborator.EventId);
        }

        public Dictionary<DateOnly, List<Event>> GetGivenMonthEventsWithDate(DateOnly startDateOfMonth, DateOnly endDateOfMonth)
        {
            List<EventCollaborator> eventCollaborators = _eventCollaboratorsService.GetAllEventCollaborators();

            Dictionary<DateOnly, List<Event>> currentMonthEvents = GenerateDictionaryOfDateOnlyAndEventFromList(GetGivenMonthEvents(eventCollaborators, startDateOfMonth, endDateOfMonth));

            return currentMonthEvents;
        }

        private Dictionary<DateOnly, List<Event>> GenerateDictionaryOfDateOnlyAndEventFromList(List<EventCollaborator> eventCollaborators)
        {
            return eventCollaborators.GroupBy(eventCollaborator => eventCollaborator.EventDate)
                                     .Select(eventCollaborator => new
                                     {
                                         ScheduleDate = eventCollaborator.Key,
                                         eventObj = eventCollaborator.Select(eventCollaborator => _eventService.GetEventById(eventCollaborator.EventId))
                                                                     .OrderBy(eventObj => eventObj.EventStartHour)
                                                                     .ToList()
                                     })
                                     .ToDictionary(key => key.ScheduleDate, val => val.eventObj);
        }

        private List<EventCollaborator> GetGivenMonthEvents(List<EventCollaborator> eventCollaborators, DateOnly startDateOfMonth, DateOnly endDateOfMonth)
        {
            return [..eventCollaborators.Where(eventCollaborator => IsEventOccurInGivenMonth
                                                                    (startDateOfMonth, endDateOfMonth, eventCollaborator))];
        }

        private bool IsEventOccurInGivenMonth(DateOnly startDateOfMonth, DateOnly endDateOfMonth,
                                             EventCollaborator eventCollaborator)
        {
            return eventCollaborator.EventDate >= startDateOfMonth
                   && eventCollaborator.EventDate <= endDateOfMonth
                   && IsConsiderableEventCollaborators(eventCollaborator)
                   && !IsProposedEvent(eventCollaborator.EventId);
        }
    }
}