using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities
{
    internal class SharedCalendar
    {

        public SharedCalendar(int Id, int ReceiverUserId, int SenderUserId, DateOnly FromDate, DateOnly ToDate)
        {
            this.Id = Id;
            this.ReceiverUserId = ReceiverUserId;
            this.SenderUserId = SenderUserId;
            this.FromDate = FromDate;
            this.ToDate = ToDate;
        }

        public SharedCalendar(int ReceiverUserId, int SenderUserId, DateOnly FromDate, DateOnly ToDate)
        {
            this.ReceiverUserId = ReceiverUserId;
            this.SenderUserId = SenderUserId;
            this.FromDate = FromDate;
            this.ToDate = ToDate;
        }

        public SharedCalendar(SqlDataReader sqlDataReader)
        {
            this.Id = Convert.ToInt32(sqlDataReader["Id"]);
            this.ReceiverUserId = Convert.ToInt32(sqlDataReader["ReceiverUserId"]);
            this.SenderUserId = Convert.ToInt32(sqlDataReader["SenderUserId"]);
            this.FromDate = DateOnly.FromDateTime(Convert.ToDateTime(sqlDataReader["FromDate"].ToString()));
            this.ToDate = DateOnly.FromDateTime(Convert.ToDateTime(sqlDataReader["ToDate"].ToString()));
        }

        [NotMapped]
        public int Id { get; }
        public int SenderUserId { get; set; }
        public int ReceiverUserId { get; set; }
        public DateOnly FromDate { get; set; }
        public DateOnly ToDate { get; set; }

        public override string ToString()
        {
            return $"{SenderUserId} , {ReceiverUserId} , {FromDate} , {ToDate}";
        }
    }
}