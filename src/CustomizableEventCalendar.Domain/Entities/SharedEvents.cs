using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities
{
    internal class SharedEvents : BaseData
    {
        public SharedEvents(int Id, int UserId, int SharedByUserId, DateOnly FromDate, DateOnly ToDate)
        {
            this.Id = Id;
            this.UserId = UserId;
            this.SharedByUserId = SharedByUserId;
            this.FromDate = FromDate;
            this.ToDate = ToDate;
        }
        public SharedEvents(int UserId, int SharedByUserId, DateOnly FromDate, DateOnly ToDate)
        {
            this.UserId = UserId;
            this.SharedByUserId = SharedByUserId;
            this.FromDate = FromDate;
            this.ToDate = ToDate;
        }
        public SharedEvents(SqlDataReader sqlDataReader)
        {
            this.Id = Convert.ToInt32(sqlDataReader["Id"]);
            this.UserId = Convert.ToInt32(sqlDataReader["UserId"]);
            this.SharedByUserId = Convert.ToInt32(sqlDataReader["SharedByUserId"]);
            this.FromDate = DateOnly.FromDateTime(Convert.ToDateTime(sqlDataReader["FromDate"].ToString()));
            this.ToDate = DateOnly.FromDateTime(Convert.ToDateTime(sqlDataReader["ToDate"].ToString()));
        }
        [NotMapped]
        public int Id { get; }
        public int SharedByUserId { get; set; }
        public int UserId { get; set; }
        public DateOnly FromDate { get; set; }
        public DateOnly ToDate { get; set; }

        public override string ToString()
        {
            return $"{SharedByUserId} , {UserId} , {FromDate} , {ToDate}";
        }
    }
}
