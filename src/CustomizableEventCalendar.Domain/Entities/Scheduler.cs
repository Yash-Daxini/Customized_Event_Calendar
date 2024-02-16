using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities
{
    internal class Scheduler : BaseData
    {
        public Scheduler(int Id, int EventId, DateTime ScheduledDate)
        {
            this.Id = Id;
            this.EventId = EventId;
            this.ScheduledDate = ScheduledDate;
        }
        public Scheduler(int EventId, DateTime ScheduledDate)
        {
            this.EventId = EventId;
            this.ScheduledDate = ScheduledDate;
        }
        public Scheduler(SqlDataReader sqlDataReader)
        {
            this.EventId = Convert.ToInt32(sqlDataReader["EventId"]);
            this.ScheduledDate = Convert.ToDateTime(sqlDataReader["ScheduledDate"]);
        }
        [NotMapped]
        public int Id { get; }
        public int EventId { get; set; }
        public DateTime ScheduledDate { get; set; }
        public override string ToString()
        {
            return $"{EventId}\t{ScheduledDate}\t";
        }
    }
}