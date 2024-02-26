using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities
{
    internal class User : BaseData
    {
        public User() { }
        public User(int Id, string Name, string Email, string Password)
        {
            this.Id = Id;
            this.Name = Name;
            this.Email = Email;
            this.Password = Password;
        }
        public User(string Name, string Email, string Password)
        {
            this.Name = Name;
            this.Email = Email;
            this.Password = Password;
        }
        public User(int Id, string Name, string Email)
        {
            this.Id = Id;
            this.Name = Name;
            this.Email = Email;
        }
        public User(SqlDataReader sqlDataReader)
        {
            this.Id = Convert.ToInt32(sqlDataReader["Id"]);
            this.Name = sqlDataReader["Name"].ToString();
            this.Email = sqlDataReader["Email"].ToString();
            this.Password = sqlDataReader["Password"].ToString();
        }
        [NotMapped]
        public int Id { get; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public override string ToString()
        {
            return $"Name:{Name}\tEmail:{Email}\tPassword:{Password}";
        }
    }
}