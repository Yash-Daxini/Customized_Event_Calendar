using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities
{
    internal class Scheduler : BaseData
    {
        public Scheduler(int Id, int EventId, string TimeBlock, DateTime Date)
        {
            this.Id = Id;
            this.EventId = EventId;
            this.TimeBlock = TimeBlock;
            this.Date = Date;
        }
        public Scheduler(int EventId, string TimeBlock, DateTime Date)
        {
            this.EventId = EventId;
            this.TimeBlock = TimeBlock;
            this.Date = Date;
        }
        public Scheduler(SqlDataReader sqlDataReader)
        {
            this.EventId = Convert.ToInt32(sqlDataReader["EventId"]);
            this.TimeBlock = sqlDataReader["TimeBlock"].ToString();
            this.Date = Convert.ToDateTime(sqlDataReader["Date"]);
        }
        public int Id { get; set; }
        public int EventId { get; set; }
        public string TimeBlock { get; set; }
        public DateTime Date { get; set; }
        public Dictionary<string, object> generateDictionary()
        {
            Dictionary<string, object> keyValuePairs = new Dictionary<string, object>();
            keyValuePairs.Add("@EventId", EventId);
            keyValuePairs.Add("@TimeBlock", TimeBlock);
            keyValuePairs.Add("@Date", Date);

            return keyValuePairs;
        }
        public override string ToString()
        {
            return string.Format("{0}\t{1}\t{2}\t", EventId, TimeBlock, Date);
        }
    }
}