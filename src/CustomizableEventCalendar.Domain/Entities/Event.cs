using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities
{
    internal class Event
    {
        public Event() { }

        public Event(SqlDataReader sqlDataReader)
        {
            this.Id = Convert.ToInt32(sqlDataReader["Id"]);
            this.Title = sqlDataReader["Title"].ToString();
            this.Description = sqlDataReader["Description"].ToString();
            this.Location = sqlDataReader["Location"].ToString();
            this.UserId = Convert.ToInt32(sqlDataReader["UserId"]);
            this.EventStartHour = Convert.ToInt32(sqlDataReader["EventStartHour"]);
            this.EventEndHour = Convert.ToInt32(sqlDataReader["EventEndHour"]);
            this.EventStartDate = DateOnly.FromDateTime(Convert.ToDateTime(sqlDataReader["EventStartDate"].ToString()));
            this.EventEndDate = DateOnly.FromDateTime(Convert.ToDateTime(sqlDataReader["EventEndDate"].ToString()));
            this.Frequency = sqlDataReader["Frequency"] == DBNull.Value ? null : sqlDataReader["Frequency"].ToString();
            this.Interval = sqlDataReader["Interval"] == DBNull.Value ? null : Convert.ToInt32(sqlDataReader["Interval"]);
            this.ByWeekDay = sqlDataReader["ByWeekDay"] == DBNull.Value ? null : sqlDataReader["ByWeekDay"].ToString();
            this.WeekOrder = sqlDataReader["WeekOrder"] == DBNull.Value ? null : Convert.ToInt32(sqlDataReader["WeekOrder"]);
            this.ByMonthDay = sqlDataReader["ByMonthDay"] == DBNull.Value ? null : Convert.ToInt32(sqlDataReader["ByMonthDay"]);
            this.ByMonth = sqlDataReader["ByMonth"] == DBNull.Value ? null : Convert.ToInt32(sqlDataReader["ByMonth"]);
            this.ByYear = sqlDataReader["ByYear"] == DBNull.Value ? null : Convert.ToInt32(sqlDataReader["ByYear"]);
            this.IsProposed = Convert.ToBoolean(sqlDataReader["IsProposed"]);
        }

        [NotMapped]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public int UserId { get; set; }
        public int EventStartHour { get; set; }
        public int EventEndHour { get; set; }
        public DateOnly EventStartDate { get; set; }
        public DateOnly EventEndDate { get; set; }
        public string? Frequency { get; set; }
        public int? Interval { get; set; }
        public string? ByWeekDay { get; set; }
        public int? WeekOrder { get; set; }
        public int? ByMonthDay { get; set; }
        public int? ByMonth { get; set; }
        public int? ByYear { get; set; }
        public bool IsProposed { get; set; }

        public override string ToString()
        {
            return $"{Id},\t{Title},\t{Description},\t{Location}\t{UserId}\t{EventStartHour}\t{EventEndHour}\t{EventStartDate}\t{EventEndDate}\t{Frequency}\t{Interval}\t{ByWeekDay}\t{WeekOrder}\t{ByMonthDay}\t{ByMonth}\t{ByYear}\t{IsProposed}";
        }

    }
}