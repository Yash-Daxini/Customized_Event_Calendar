namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Model
{
    internal class Event
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public string Location { get; set; }

        public DateOnly EventDate { get; set; }

        public Duration Duration { get; set; }

        public RecurrencePattern RecurrencePattern { get; set; }

        public List<Participant> Participants { get; set; }
    }
}
