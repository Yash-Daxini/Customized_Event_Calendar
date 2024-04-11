using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Mapping;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Models;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Data.Repositories
{

    internal class EventCollaboratorRepository : GenericRepository<EventCollaborator>
    {
        private readonly ParticipantMapper _participantMapper = new();

        public List<ParticipantModel> GetAll()
        {
            List<EventCollaborator> eventCollaborators = GetAll(data => new EventCollaborator(data));

            return [.. eventCollaborators.Select(_participantMapper.MapEventCollaboratorToParticipantModel)];
        }

        public ParticipantModel? GetById(int eventCollaboratorId)
        {
            EventCollaborator? eventCollaborator = GetById(data => new EventCollaborator(data), eventCollaboratorId);

            if (eventCollaborator == null) return null;
            return _participantMapper.MapEventCollaboratorToParticipantModel(eventCollaborator);
        }

        public void DeleteByEventId(int eventId)
        {
            string query = @$"Delete from [dbo].[EventCollaborator]
                              where EventId = {eventId}";
            Connect();

            ExecuteNonQuery(query);

            Disconnect();
        }

        public int Insert(ParticipantModel participantModel, int eventId)
        {
            EventCollaborator eventCollaborator = _participantMapper.MapParticipantModelToEventCollaborator(participantModel, eventId);

            return Insert(eventCollaborator);
        }

        public void Update(ParticipantModel participantModel, int eventId)
        {
            EventCollaborator eventCollaborator = _participantMapper.MapParticipantModelToEventCollaborator(participantModel, eventId);

            Update(eventCollaborator, eventCollaborator.Id);
        }

        public List<EventCollaborator> GetByEventId(int eventId)
        {
            string query = @$"Select * from [dbo].[EventCollaborator]
                              where EventId = {eventId}";
            Connect();

            ExecuteQuery(query);

            List<EventCollaborator> eventCollaborators = [];

            while (sqlDataReader.Read())
            {
                eventCollaborators.Add(new EventCollaborator(sqlDataReader));
            }

            Disconnect();

            return eventCollaborators;
        }
    }
}