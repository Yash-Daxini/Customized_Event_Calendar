using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Data.Repositories;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities
{
    internal class ScheduleEvent : BaseData
    {
        public ScheduleEvent(int Id, int EventCollaboratorsID, DateTime ScheduledDate)
        {
            this.Id = Id;
            this.EventCollaboratorsId = EventCollaboratorsID;
            this.ScheduledDate = ScheduledDate;
        }
        public ScheduleEvent(int EventCollaboratorsId, DateTime ScheduledDate)
        {
            this.EventCollaboratorsId = EventCollaboratorsId;
            this.ScheduledDate = ScheduledDate;
        }
        public ScheduleEvent(SqlDataReader sqlDataReader)
        {
            this.Id = Convert.ToInt32(sqlDataReader["Id"]);
            this.EventCollaboratorsId = Convert.ToInt32(sqlDataReader["EventCollaboratorsId"]);
            this.ScheduledDate = Convert.ToDateTime(sqlDataReader["ScheduledDate"]);
        }
        [NotMapped]
        public int Id { get; }
        public int EventCollaboratorsId { get; set; }
        public DateTime ScheduledDate { get; set; }
        public override string ToString()
        {
            return $"{EventCollaboratorsId}\t{ScheduledDate}\t";
        }
    }
}