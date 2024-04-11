using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Models;


namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services
{
    internal class CalendarViewService
    {
        private readonly EventService _eventService = new();

        public List<EventModel> GetEventsFroDailyView()
        {
            List<EventModel> todaysEvents = GetEventsOfToday();

            return GetEventsOrderedByDateAndHour(todaysEvents);
        }

        private bool IsProposedEvent(int eventId)
        {
            return _eventService.GetProposedEvents().Exists(eventObj => eventObj.Id == eventId);
        }

        private List<EventModel> GetEventsOfToday()
        {
            return [.. _eventService.GetAllEventsOfLoggedInUser()
                   .Where(eventModel => eventModel.EventDate == DateOnly.FromDateTime(DateTime.Today)
                                               && !IsProposedEvent(eventModel.Id))];
        }

        public List<EventModel> GetDateAndEventsForWeeklyView()
        {
            DateOnly startDateOfWeek = DateTimeManager.GetStartDateOfWeek(DateOnly.FromDateTime(DateTime.Today));
            DateOnly endDateOfWeek = DateTimeManager.GetEndDateOfWeek(DateOnly.FromDateTime(DateTime.Today));

            List<EventModel> currentWeekEvents = GetGivenWeekEventsWithDate(startDateOfWeek, endDateOfWeek);

            return currentWeekEvents;
        }

        private List<EventModel> GetGivenWeekEventsWithDate(DateOnly startDateOfWeek, DateOnly endDateOfWeek)
        {
            List<EventModel> eventModel = _eventService.GetAllEventsOfLoggedInUser();

            List<EventModel> currentWeekEvents = GetEventsOrderedByDateAndHour(GetGivenWeekEvents(eventModel, startDateOfWeek, endDateOfWeek));

            return currentWeekEvents;
        }

        private List<EventModel> GetGivenWeekEvents(List<EventModel> eventModels, DateOnly startDateOfWeek, DateOnly endDateOfWeek)
        {
            return [.. eventModels.Where(eventModel => IsEventOccurInGivenWeek(startDateOfWeek, endDateOfWeek, eventModel))];
        }

        private bool IsEventOccurInGivenWeek(DateOnly startDateOfWeek, DateOnly endDateOfWeek, EventModel eventModel)
        {
            return eventModel.EventDate >= startDateOfWeek
                   && eventModel.EventDate <= endDateOfWeek
                   && !IsProposedEvent(eventModel.Id);
        }

        public List<EventModel> GetGivenMonthEventsWithDate(DateOnly startDateOfMonth, DateOnly endDateOfMonth)
        {
            List<EventModel> eventModel = _eventService.GetAllEventsOfLoggedInUser();

            List<EventModel> currentMonthEvents = GetEventsOrderedByDateAndHour(GetGivenMonthEvents(eventModel, startDateOfMonth, endDateOfMonth));

            return currentMonthEvents;
        }

        private List<EventModel> GetEventsOrderedByDateAndHour(List<EventModel> eventModels)
        {
            return [.. eventModels.OrderBy(eventModel => eventModel.EventDate).ThenBy(eventModel => eventModel.Duration.StartHour)];
        }

        private List<EventModel> GetGivenMonthEvents(List<EventModel> eventModels, DateOnly startDateOfMonth, DateOnly endDateOfMonth)
        {
            return [.. eventModels.Where(eventModel => IsEventOccurInGivenMonth(startDateOfMonth, endDateOfMonth, eventModel))];
        }

        private bool IsEventOccurInGivenMonth(DateOnly startDateOfMonth, DateOnly endDateOfMonth,
                                             EventModel eventModel)
        {
            return eventModel.EventDate >= startDateOfMonth
                   && eventModel.EventDate <= endDateOfMonth
                   && !IsProposedEvent(eventModel.Id);
        }
    }
}