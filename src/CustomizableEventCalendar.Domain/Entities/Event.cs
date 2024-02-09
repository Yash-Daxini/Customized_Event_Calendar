using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities
{
    internal class Event : BaseData
    {
        public Event(int Id, string Title, string Description, string Location, int UserId, int RecurrenceId)
        {
            this.Id = Id;
            this.Title = Title;
            this.Description = Description;
            this.Location = Location;
            this.UserId = UserId;
            this.RecurrenceId = RecurrenceId;
        }
        public Event(string Title, string Description, string Location, int UserId, int RecurrenceId)
        {
            this.Title = Title;
            this.Description = Description;
            this.Location = Location;
            this.UserId = UserId;
            this.RecurrenceId = RecurrenceId;
        }
        public Event(SqlDataReader sqlDataReader)
        {
            this.Title = sqlDataReader["Title"].ToString();
            this.Description = sqlDataReader["Description"].ToString();
            this.Location = sqlDataReader["Location"].ToString();
            this.UserId = Convert.ToInt32(sqlDataReader["UserId"]);
            this.RecurrenceId = Convert.ToInt32(sqlDataReader["RecurrenceId"]);
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public int UserId { get; set; }
        public int RecurrenceId { get; set; }

        public Dictionary<string, object> generateDictionary()
        {
            Dictionary<string, object> keyValuePairs = new Dictionary<string, object>();
            keyValuePairs.Add("@Title", Title);
            keyValuePairs.Add("@Location", Location);
            keyValuePairs.Add("@Description", Description);
            keyValuePairs.Add("@UserId", UserId);
            keyValuePairs.Add("@RecurrenceId", RecurrenceId);

            return keyValuePairs;
        }
        public override string ToString()
        {
            return string.Format("{0}\t{1}\t{2}\t{3}\t{4}", Title, Description, Location, UserId, RecurrenceId);
        }

    }
}