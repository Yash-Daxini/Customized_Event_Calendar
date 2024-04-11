using CustomizableEventCalendar.src.CustomizableEventCalendar.Data.Repositories;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Models;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services
{
    internal class EventCollaboratorService
    {
        private readonly EventCollaboratorRepository _eventCollaboratorsRepository = new();

        public List<ParticipantModel> GetAllParticipants()
        {
            List<ParticipantModel> participants = [.._eventCollaboratorsRepository.GetAll()
                                                         .OrderBy(participant => participant.EventDate)
                                                         .ThenBy(participant => participant.ProposedStartHour)];
            return participants;
        }

        public ParticipantModel? GetParticipantById(int eventCollaboratorId)
        {
            ParticipantModel? participantModel = _eventCollaboratorsRepository.GetById(eventCollaboratorId);
            return participantModel;
        }

        public int InsertParticipant(ParticipantModel participantModel,int eventId)
        {
            int eventCollaboratorsId = _eventCollaboratorsRepository.Insert(participantModel,eventId);
            return eventCollaboratorsId;
        }

        public void UpdateParticipant(ParticipantModel participantModel, int eventId)
        {
            _eventCollaboratorsRepository.Update(participantModel, eventId);
        }

        public void DeleteParticipantByEventId(int eventId)
        {
            _eventCollaboratorsRepository.DeleteByEventId(eventId);
        }
    }
}