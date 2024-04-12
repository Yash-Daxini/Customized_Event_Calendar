namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Models
{
    internal class EventModel
    {
        public EventModel()
        {
        }

        public EventModel(int Id, string Title, string Description, string Location, DateOnly EventDate, DurationModel Duration, RecurrencePatternModel RecurrencePattern, List<ParticipantModel> Participants)
        {
            this.Id = Id;
            this.Title = Title;
            this.Description = Description;
            this.Location = Location;
            this.EventDate = EventDate;
            this.Duration = Duration;
            this.RecurrencePattern = RecurrencePattern;
            this.Participants = Participants;
        }

        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Location { get; set; }

        public DateOnly EventDate { get; set; }

        public DurationModel Duration { get; set; }

        public RecurrencePatternModel RecurrencePattern { get; set; }

        public List<ParticipantModel> Participants { get; set; }

        public override string ToString() => $"Title : {Title},Description : {Description},Location : {Location},Event Date : {EventDate},\nDuration : {Duration},\nRecurrence Pattern : {RecurrencePattern},\nParticipants : {string.Join(" ", Participants)}\n";

    }
}
