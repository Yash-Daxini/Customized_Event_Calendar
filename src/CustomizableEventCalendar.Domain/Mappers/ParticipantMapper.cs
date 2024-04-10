using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Enums;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Model;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Mapping
{
    internal class ParticipantMapper
    {
        public ParticipantModel MapEventCollaboratorToParticipantModel(EventCollaborator eventCollaborator)
        {
            return new ParticipantModel
            {
                Id = eventCollaborator.Id,
                ParticipantRole = MapParticipantRoleToEnum(eventCollaborator.ParticipantRole),
                ConfirmationStatus = MapConfirmationStatusToEnum(eventCollaborator.ConfirmationStatus),
                ProposedStartHour = eventCollaborator.ProposedStartHour,
                ProposedEndHour = eventCollaborator.ProposedEndHour,
                EventDate = eventCollaborator.EventDate,
            };
        }

        public EventCollaborator MapParticipantModelToEventCollaborator(ParticipantModel participantModel, int eventId, int userId)
        {
            return new EventCollaborator
            {
                Id = participantModel.Id,
                EventId = eventId,
                UserId = userId,
                ParticipantRole = MapEnumToParticipantRole(participantModel.ParticipantRole),
                ConfirmationStatus = MapEnumToConfirmationStatus(participantModel.ConfirmationStatus),
                ProposedStartHour = participantModel.ProposedStartHour,
                ProposedEndHour = participantModel.ProposedEndHour,
                EventDate = participantModel.EventDate,
            };
        }

        private ParticipantRole MapParticipantRoleToEnum(string participantRole)
        {
            return participantRole.Equals("organizer") ? ParticipantRole.Organizer : ParticipantRole.Participant;
        }

        private string MapEnumToParticipantRole(ParticipantRole participantRole)
        {
            return participantRole == ParticipantRole.Organizer ? "organizer" : "participant";
        }

        private ConfirmationStatus MapConfirmationStatusToEnum(string confirmationStatus)
        {
            return confirmationStatus switch
            {
                "accept" => ConfirmationStatus.Accept,
                "reject" => ConfirmationStatus.Reject,
                "pending" => ConfirmationStatus.Pending,
                "maybe" => ConfirmationStatus.Maybe,
                "proposed" => ConfirmationStatus.Proposed,
                _ => ConfirmationStatus.Pending,
            };
        }

        private string MapEnumToConfirmationStatus(ConfirmationStatus confirmationStatus)
        {
            return confirmationStatus switch
            {
                ConfirmationStatus.Accept => "accept",
                ConfirmationStatus.Reject => "reject",
                ConfirmationStatus.Pending => "pending",
                ConfirmationStatus.Maybe => "maybe",
                ConfirmationStatus.Proposed => "proposed",
                _ => "pending",
            };
        }
    }
}
