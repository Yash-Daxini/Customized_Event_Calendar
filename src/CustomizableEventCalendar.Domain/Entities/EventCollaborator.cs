using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities
{
    internal class EventCollaborator
    {

        public EventCollaborator(int Id, int EventId, int UserId, string? ParticipantRole, string? ConfirmationStatus, int? ProposedStartHour,
                                  int? ProposedEndHour, DateTime EventDate)
        {
            this.Id = Id;
            this.UserId = UserId;
            this.EventId = EventId;
            this.ParticipantRole = ParticipantRole;
            this.ConfirmationStatus = ConfirmationStatus;
            this.ProposedStartHour = ProposedStartHour;
            this.ProposedEndHour = ProposedEndHour;
            this.EventDate = EventDate;
        }

        public EventCollaborator(int EventId, int UserId, string? ParticipantRole, string? ConfirmationStatus, int? ProposedStartHour,
                                  int? ProposedEndHour, DateTime EventDate)
        {
            this.UserId = UserId;
            this.EventId = EventId;
            this.ParticipantRole = ParticipantRole;
            this.ConfirmationStatus = ConfirmationStatus;
            this.ProposedStartHour = ProposedStartHour;
            this.ProposedEndHour = ProposedEndHour;
            this.EventDate = EventDate;
        }

        public EventCollaborator(SqlDataReader sqlDataReader)
        {
            this.Id = Convert.ToInt32(sqlDataReader["Id"]);
            this.UserId = Convert.ToInt32(sqlDataReader["UserId"]);
            this.EventId = Convert.ToInt32(sqlDataReader["EventId"]);
            this.ParticipantRole = sqlDataReader["ParticipantRole"] == DBNull.Value ? null : sqlDataReader["ParticipantRole"].ToString();
            this.ConfirmationStatus = sqlDataReader["ConfirmationStatus"] == DBNull.Value ? null :
                                                                             sqlDataReader["ConfirmationStatus"].ToString();
            this.ProposedStartHour = sqlDataReader["ProposedStartHour"] == DBNull.Value ? null :
                                                                           Convert.ToInt32(sqlDataReader["ProposedStartHour"]);
            this.ProposedEndHour = sqlDataReader["ProposedEndHour"] == DBNull.Value ? null : Convert.ToInt32(sqlDataReader["ProposedEndHour"]);
            this.EventDate = Convert.ToDateTime(sqlDataReader["EventDate"].ToString());
        }

        [NotMapped]
        public int Id { get; }
        public int EventId { get; set; }
        public int UserId { get; set; }
        public string? ParticipantRole { get; set; }
        public string? ConfirmationStatus { get; set; }
        public int? ProposedStartHour { get; set; }
        public int? ProposedEndHour { get; set; }
        public DateTime EventDate { get; set; }


        public override string ToString()
        {
            return $"{UserId}\t{EventId}\t";
        }
    }
}