using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Enums;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Models
{
    internal class ParticipantModel
    {
        public ParticipantModel()
        {
            
        }
        public ParticipantModel(int Id,ParticipantRole ParticipantRole, ConfirmationStatus ConfirmationStatus, int? ProposedStartHour, int? ProposedEndHour, DateOnly EventDate, UserModel User)
        {
            this.Id = Id;
            this.ParticipantRole = ParticipantRole;
            this.ConfirmationStatus = ConfirmationStatus;
            this.EventDate = EventDate;
            this.ProposedStartHour = ProposedStartHour;
            this.ProposedEndHour = ProposedEndHour;
            this.User = User;
        }
        
        public ParticipantModel(ParticipantRole ParticipantRole, ConfirmationStatus ConfirmationStatus, int? ProposedStartHour, int? ProposedEndHour, DateOnly EventDate, UserModel User)
        {
            this.ParticipantRole = ParticipantRole;
            this.ConfirmationStatus = ConfirmationStatus;
            this.EventDate = EventDate;
            this.ProposedStartHour = ProposedStartHour;
            this.ProposedEndHour = ProposedEndHour;
            this.User = User;
        }

        public int Id { get; set; }

        public ParticipantRole ParticipantRole { get; set; }

        public ConfirmationStatus ConfirmationStatus { get; set; }

        public int? ProposedStartHour { get; set; }

        public int? ProposedEndHour { get; set; }

        public DateOnly EventDate { get; set; }

        public UserModel User { get; set; }

        public override string ToString()
        {
            return $"Participant Role : {ParticipantRole}\tConfirmation Status : {ConfirmationStatus}\tProposed Start Hour : {ProposedStartHour}\t Proposed End Hour : {ProposedEndHour}\tEvent Date : {EventDate}\tUser : {User.ToString()}";
        }
    }
}
