using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Models;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services
{
    internal class NotificationService
    {
        private readonly EventService _eventService = new();

        public List<EventModel> GetUpcomingEvents()
        {
            return [.. _eventService.GetAllEvents().Where(eventModel => eventModel.EventDate == DateOnly.FromDateTime(DateTime.Now))
                                                        .OrderBy(eventModel=> eventModel.EventDate)
                                                        .ThenBy(eventModel=>eventModel.Duration.StartHour)];
        }

        public List<EventModel> GetProposedEvents()
        {
            return _eventService.GetProposedEvents();
        }
    }
}