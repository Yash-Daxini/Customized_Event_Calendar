using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Models
{
    internal class EventByDate(DateOnly date, Event eventObj)
    {
        public DateOnly Date { get; set; } = date;

        public Event Event { get; set; } = eventObj;
    }
}
