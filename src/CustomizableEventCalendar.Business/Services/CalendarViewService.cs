using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;


namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services
{
    internal class CalendarViewService
    {
        private readonly EventService _eventService = new();
        private readonly EventCollaboratorService _eventCollaboratorsService = new();

        public List<EventByDate> GetEventsFroDailyView()
        {
            List<EventCollaborator> todaysEvents = GetEventsOfToday();

            return GetListOfEventByDate(todaysEvents);
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

        public List<EventByDate> GetDateAndEventsForWeeklyView()
        {
            DateOnly startDateOfWeek = DateTimeManager.GetStartDateOfWeek(DateOnly.FromDateTime(DateTime.Today));
            DateOnly endDateOfWeek = DateTimeManager.GetEndDateOfWeek(DateOnly.FromDateTime(DateTime.Today));

            List<EventByDate> currentWeekEvents = GetGivenWeekEventsWithDate(startDateOfWeek, endDateOfWeek);

            return currentWeekEvents;
        }

        private List<EventByDate> GetGivenWeekEventsWithDate(DateOnly startDateOfWeek, DateOnly endDateOfWeek)
        {
            List<EventCollaborator> eventCollaborators = _eventCollaboratorsService.GetAllEventCollaborators();

            List<EventByDate> currentWeekEvents = GetListOfEventByDate(GetGivenWeekEvents(eventCollaborators, startDateOfWeek, endDateOfWeek));

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

        public List<EventByDate> GetGivenMonthEventsWithDate(DateOnly startDateOfMonth, DateOnly endDateOfMonth)
        {
            List<EventCollaborator> eventCollaborators = _eventCollaboratorsService.GetAllEventCollaborators();

            List<EventByDate> currentMonthEvents = GetListOfEventByDate(GetGivenMonthEvents(eventCollaborators, startDateOfMonth, endDateOfMonth));

            return currentMonthEvents;
        }

        private List<EventByDate> GetListOfEventByDate(List<EventCollaborator> eventCollaborators)
        {

            List<EventByDate> eventsByDateList = [];

            List<Event> events = _eventService.GetAllEvents();

            foreach (var eventCollaborator in eventCollaborators)
            {
                Event? eventObj = events.Find(eventObj => eventObj.Id == eventCollaborator.EventId);

                if (eventObj is null) continue;

                eventsByDateList.Add(new EventByDate(eventCollaborator.EventDate, eventObj));
            }

            return [.. eventsByDateList.OrderBy(eventByDate => eventByDate.Date).ThenBy(eventByDate => eventByDate.Event.EventStartHour)];
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