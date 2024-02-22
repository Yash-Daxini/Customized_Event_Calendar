using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Data.Repositories
{
    internal class EventCollaboratorsRepository : GenericRepository
    {
        public void DeleteByEventId(int eventId)
        {
            string query = @$"Delete from [dbo].[EventCollaborators]
                              where EventId = {eventId}";
            Connect();

            ExecuteNonQuery(query);

            Disconnect();
        }
    }
}
