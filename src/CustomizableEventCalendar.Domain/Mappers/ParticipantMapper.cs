using CustomizableEventCalendar.src.CustomizableEventCalendar.Data.Repositories;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Enums;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Models;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Mapping
{
    internal class ParticipantMapper
    {
        public ParticipantModel MapEventCollaboratorToParticipantModel(EventCollaborator eventCollaborator)
        {
            return new ParticipantModel
            (
                eventCollaborator.Id,
                MapParticipantRoleToEnum(eventCollaborator.ParticipantRole),
                MapConfirmationStatusToEnum(eventCollaborator.ConfirmationStatus),
                eventCollaborator.ProposedStartHour,
                eventCollaborator.ProposedEndHour,
                eventCollaborator.EventDate,
                MapUserIdToUserModel(eventCollaborator.UserId)
            );
        }

        public EventCollaborator MapParticipantModelToEventCollaborator(ParticipantModel participantModel, int eventId)
        {
            return new EventCollaborator
            {
                Id = participantModel.Id,
                EventId = eventId,
                UserId = participantModel.User.Id,
                ParticipantRole = MapEnumToParticipantRole(participantModel.ParticipantRole),
                ConfirmationStatus = MapEnumToConfirmationStatus(participantModel.ConfirmationStatus),
                ProposedStartHour = participantModel.ProposedStartHour,
                ProposedEndHour = participantModel.ProposedEndHour,
                EventDate = participantModel.EventDate,
            };
        }

        private UserModel MapUserIdToUserModel(int userId)
        {
            return new UserMapper().MapUserEntityToModel(new UserRepository().GetById(data => new User(data), userId));
        }

        private ParticipantRole MapParticipantRoleToEnum(string participantRole)
        {
            return participantRole switch
            {
                "organizer" => ParticipantRole.Organizer,
                "participant" => ParticipantRole.Participant,
                "collaborator" => ParticipantRole.Collaborator,
            };
        }

        private string MapEnumToParticipantRole(ParticipantRole participantRole)
        {
            return participantRole switch
            {
                ParticipantRole.Organizer => "organizer",
                ParticipantRole.Participant => "participant",
                ParticipantRole.Collaborator => "collaborator",
            };
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
