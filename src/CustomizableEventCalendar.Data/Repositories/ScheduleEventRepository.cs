using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Data.Repositories
{
    internal class ScheduleEventRepository : GenericRepository
    {
        public void DeleteByEventId(int eventId, int userId)
        {
            string query = @$"Delete from [dbo].[ScheduleEvent]
                              where EventCollaboratorsId in 
                                                        (
                                                            Select Id from [dbo].[EventCollaborators] 
                                                            where EventId = {eventId} and UserId = {userId}
                                                         )";
            Connect();

            ExecuteNonQuery(query);

            Disconnect();
        }
        public List<ScheduleEvent> ReadByUserId()
        {
            string query = $@"SELECT * FROM [dbo].[ScheduleEvent] 
                             WHERE EventCollaboratorsId in 
                                                            (
                                                                SELECT Id FROM [dbo].[EventCollaborators] 
                                                                WHERE UserId = {GlobalData.user.Id}
                                                            ) ";
            List<ScheduleEvent> scheduleEvents = new List<ScheduleEvent>();

            Connect();

            ExecuteQuery(query);

            while (sqlDataReader.Read())
            {
                ScheduleEvent scheduleEvent = new ScheduleEvent(sqlDataReader);
                scheduleEvents.Add(scheduleEvent);
            }

            Disconnect();

            return scheduleEvents;
        }
    }
}