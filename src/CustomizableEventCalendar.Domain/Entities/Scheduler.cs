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
        public Scheduler(int Id, int EventId, DateTime Date)
        {
            this.Id = Id;
            this.EventId = EventId;
            this.Date = Date;
        }
        public Scheduler(int EventId, DateTime Date)
        {
            this.EventId = EventId;
            this.Date = Date;
        }
        public Scheduler(SqlDataReader sqlDataReader)
        {
            this.EventId = Convert.ToInt32(sqlDataReader["EventId"]);
            this.Date = Convert.ToDateTime(sqlDataReader["Date"]);
        }
        [NotMapped]
        public int Id { get; }
        public int EventId { get; set; }
        public DateTime Date { get; set; }
        public override string ToString()
        {
            return string.Format("{0}\t{1}\t", EventId, Date);
        }
    }
}