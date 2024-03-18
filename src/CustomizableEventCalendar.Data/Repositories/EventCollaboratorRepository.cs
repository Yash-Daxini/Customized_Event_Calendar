using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public EventCollaborator? ReadByEventId(int eventId)
        {
            string query = @$"Select * from [dbo].[EventCollaborator]
                              where EventId = {eventId} and UserId = {GlobalData.GetUser().Id}";
            Connect();

            ExecuteQuery(query);

            EventCollaborator eventCollaborators = null;

            if (sqlDataReader.Read())
            {
                eventCollaborators = new EventCollaborator(sqlDataReader);
            }

            Disconnect();

            return eventCollaborators;
        }
    }
}