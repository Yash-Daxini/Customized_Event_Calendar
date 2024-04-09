using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Enums;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Model
{
    internal class Participant
    {
        public int Id { get; set; } 

        public ParticipantRole ParticipantRole { get; set; }

        public ConfirmationStatus ConfirmationStatus { get; set; }

        public Duration Duration { get; set; }

        public DateOnly EventDate { get; set; }

        public User User { get; set; }
    }
}
