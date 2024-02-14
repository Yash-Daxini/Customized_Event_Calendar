using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities
{
    internal class Event : BaseData
    {
        public Event() { }
        public Event(int Id, string Title, string Description, string Location, string TimeBlock, int UserId, int? RecurrenceId)
        {
            this.Id = Id;
            this.Title = Title;
            this.Description = Description;
            this.Location = Location;
            this.TimeBlock = TimeBlock;
            this.UserId = UserId;
            this.RecurrenceId = RecurrenceId;
        }
        public Event(string Title, string Description, string Location, int UserId, string TimeBlock, int? RecurrenceId)
        {
            this.Title = Title;
            this.Description = Description;
            this.Location = Location;
            this.TimeBlock = TimeBlock;
            this.UserId = UserId;
            this.RecurrenceId = RecurrenceId;
        }
        public Event(SqlDataReader sqlDataReader)
        {
            this.Id = Convert.ToInt32(sqlDataReader["Id"]);
            this.Title = sqlDataReader["Title"].ToString();
            this.Description = sqlDataReader["Description"].ToString();
            this.Location = sqlDataReader["Location"].ToString();
            this.TimeBlock = sqlDataReader["TimeBlock"].ToString();
            this.UserId = Convert.ToInt32(sqlDataReader["UserId"]);
            this.RecurrenceId = Convert.ToInt32(sqlDataReader["RecurrenceId"]);
        }
        [NotMapped]
        public int Id { get; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public string TimeBlock { get; set; }
        public int UserId { get; set; }
        public int? RecurrenceId { get; set; }
        public override string ToString()
        {
            StringBuilder events = new StringBuilder();
            events.AppendLine($"{Id},\t{Title},\t{Description},\t{Location},\t{TimeBlock}");
            return events.ToString();
        }

    }
}