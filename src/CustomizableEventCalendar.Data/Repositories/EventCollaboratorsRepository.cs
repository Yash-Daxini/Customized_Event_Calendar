using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Data.Repositories
{

    internal class EventCollaboratorsRepository : GenericRepository<EventCollaborators>
    {

        public void DeleteByEventId(int eventId)
        {
            string query = @$"Delete from [dbo].[EventCollaborators]
                              where EventId = {eventId}";
            Connect();

            ExecuteNonQuery(query);

            Disconnect();
        }

        public EventCollaborators? ReadByEventId(int eventId)
        {
            string query = @$"Select * from [dbo].[EventCollaborators]
                              where EventId = {eventId} and UserId = {GlobalData.user.Id}";
            Connect();

            ExecuteQuery(query);

            EventCollaborators eventCollaborators = null;

            if (sqlDataReader.Read())
            {
                eventCollaborators = new EventCollaborators(sqlDataReader);
            }

            Disconnect();

            return eventCollaborators;
        }
    }
}