using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Data.Repositories
{
    internal class ScheduleRepository : GenericRepository
    {
        public void DeleteByEventId(int EventId)
        {
            string query = @$"DELETE FROM [dbo].[Scheduler]
                              WHERE [dbo].[Scheduler].[EventId] = {EventId}";
            Connect();

            ExecuteNonQuery(query);

            Disconnect();
        }
    }
}
