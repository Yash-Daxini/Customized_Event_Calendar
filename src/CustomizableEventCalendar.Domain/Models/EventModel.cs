namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Model
{
    internal class EventModel
    { 
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Location { get; set; }

        public DateOnly EventDate { get; set; }

        public DurationModel Duration { get; set; }

        public RecurrencePatternModel RecurrencePattern { get; set; }

        public List<ParticipantModel> Participants { get; set; }
    }
}
