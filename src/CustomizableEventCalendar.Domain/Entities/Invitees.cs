using System;
using System.Collections.Generic;
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
        public int Id { get; set; }
        public int UserId { get; set; }
        public int EventId { get; set; }

        public Dictionary<string, object> generateDictionary()
        {
            Dictionary<string, object> keyValuePairs = new Dictionary<string, object>();
            keyValuePairs.Add("@EventId", EventId);
            keyValuePairs.Add("@UserId", UserId);

            return keyValuePairs;
        }
        public override string ToString()
        {
            return string.Format("{0}\t{1}\t", UserId, EventId);
        }
    }
}
