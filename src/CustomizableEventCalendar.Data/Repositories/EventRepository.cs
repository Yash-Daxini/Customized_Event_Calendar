using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Data.Repositories
{

    internal class EventRepository : GenericRepository<Event>
    {

        public void ConvertProposedEventToScheduleEvent(int eventId)
        {
            string query = @$"Update [dbo].[Event]
                              set IsProposed = 0
                              where Id = {eventId}";

            Connect();

            ExecuteNonQuery(query);

            Disconnect();
        }
    }
}