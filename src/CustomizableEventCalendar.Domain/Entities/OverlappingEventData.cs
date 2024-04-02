namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities
{
    internal class OverlappingEventData(Event eventInformation, DateOnly matchedDate)
    {
        public Event EventInformation { get; set; } = eventInformation;

        public DateOnly MatchedDate { get; set; } = matchedDate;
    }
}
