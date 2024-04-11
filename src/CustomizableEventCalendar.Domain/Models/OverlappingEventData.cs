namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Models
{
    internal class OverlappingEventData(EventModel eventToVerifyOverlap, EventModel overlappedEvent, DateOnly matchedDate)
    {
        public EventModel EventInformation { get; set; } = eventToVerifyOverlap;

        public EventModel OverlappedEvent { get; set; } = overlappedEvent;

        public DateOnly MatchedDate { get; set; } = matchedDate;
    }
}
