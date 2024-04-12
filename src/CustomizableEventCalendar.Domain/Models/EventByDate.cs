using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Models
{
    internal class EventByDate(DateOnly date, Entities.Event eventObj)
    {
        public DateOnly Date { get; set; } = date;

        public Entities.Event Event { get; set; } = eventObj;
    }
}
