using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities
{
    internal class User : BaseData
    {
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
        public User(SqlDataReader sqlDataReader)
        {
            this.Id = Convert.ToInt32(sqlDataReader["Id"]);
            this.Name = sqlDataReader["Name"].ToString();
            this.Email = sqlDataReader["Email"].ToString();
            this.Password = sqlDataReader["Password"].ToString();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public Dictionary<string, object> generateDictionary()
        {
            Dictionary<string, object> keyValuePairs = new Dictionary<string, object>();
            keyValuePairs.Add("@Name", Name);
            keyValuePairs.Add("@Email", Email);
            keyValuePairs.Add("@Password", Password);

            return keyValuePairs;
        }
        public override string ToString()
        {
            return string.Format("Name:{0}\tEmail:{1}\tPassword:{2}", Name, Email, Password);
        }
    }
}