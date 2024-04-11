using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Models;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services
{
    internal class SharedEventCollaborationService
    {
        private readonly EventCollaboratorService _eventCollaboratorService = new();

        public void AddCollaborator(ParticipantModel participantModel, int eventId)
        {
            _eventCollaboratorService.InsertParticipant(participantModel, eventId);
        }

        private List<ParticipantModel> GetAllEventCollaborators()
        {
            return _eventCollaboratorService.GetAllParticipants();
        }


        // StatFromHere how to find event id in participant

        public bool IsEventAlreadyCollaborated(ParticipantModel newParticipant)
        {
            return GetAllEventCollaborators().Exists(participant =>
                                                     participant.User.Id == newParticipant.User.Id
                                                     && participant.EventId == newEventCollaborator.EventId
                                                     && eventCollaborator.EventDate == newEventCollaborator.EventDate
                                                     && IsHourOvelapps(eventCollaborator, newEventCollaborator));
        }

        // Above method is only pending

        public ParticipantModel? GetCollaborationOverlap(ParticipantModel newParticipant)
        {
            return GetAllEventCollaborators().Find(participant =>
                                                   participant.User.Id == newParticipant.User.Id
                                                   && participant.EventDate == newParticipant.EventDate
                                                   && IsHourOvelapps(participant, newParticipant));
        }

        private static bool IsHourOvelapps(ParticipantModel eventCollaborator, ParticipantModel newEventCollaborator)
        {
            return (eventCollaborator.ProposedStartHour >= newEventCollaborator.ProposedStartHour
                    && eventCollaborator.ProposedStartHour < newEventCollaborator.ProposedEndHour)
                || (eventCollaborator.ProposedEndHour > newEventCollaborator.ProposedStartHour
                    && eventCollaborator.ProposedEndHour <= newEventCollaborator.ProposedEndHour)
                || (newEventCollaborator.ProposedStartHour >= eventCollaborator.ProposedStartHour
                    && newEventCollaborator.ProposedStartHour < eventCollaborator.ProposedEndHour)
                || (newEventCollaborator.ProposedEndHour > eventCollaborator.ProposedStartHour
                    && newEventCollaborator.ProposedEndHour <= eventCollaborator.ProposedEndHour);
        }
    }
}