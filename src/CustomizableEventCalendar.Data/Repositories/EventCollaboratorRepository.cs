using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Data.Repositories
{

    internal class EventCollaboratorRepository : GenericRepository<EventCollaborator>
    {

        public void DeleteByEventId(int eventId)
        {
            string query = @$"Delete from [dbo].[EventCollaborator]
                              where EventId = {eventId}";
            Connect();

            ExecuteNonQuery(query);

            Disconnect();
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