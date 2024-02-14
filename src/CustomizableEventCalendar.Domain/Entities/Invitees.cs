using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities
{
    internal class Invitees : BaseData
    {
        public Invitees(int Id, int UserId, int EventId)
        {
            this.Id = Id;
            this.UserId = UserId;
            this.EventId = EventId;
        }
        public Invitees(int UserId, int EventId)
        {
            this.UserId = UserId;
            this.EventId = EventId;
        }
        public Invitees(SqlDataReader sqlDataReader)
        {
            this.UserId = Convert.ToInt32(sqlDataReader["UserId"]);
            this.EventId = Convert.ToInt32(sqlDataReader["EventId"]);
        }
        [NotMapped]
        public int Id { get; }
        public int UserId { get; set; }
        public int EventId { get; set; }
        public override string ToString()
        {
            return string.Format("{0}\t{1}\t", UserId, EventId);
        }
    }
}