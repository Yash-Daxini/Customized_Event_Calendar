using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Enums;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Model
{
    internal class ParticipantModel
    {
        public int Id { get; set; } 

        public ParticipantRole ParticipantRole { get; set; }

        public ConfirmationStatus ConfirmationStatus { get; set; }

        public int? ProposedStartHour { get; set; }

        public int? ProposedEndHour { get; set; }

        public DateOnly EventDate { get; set; }

        public UserModel User { get; set; }
    }
}
