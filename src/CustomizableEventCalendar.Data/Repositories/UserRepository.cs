using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Data.Repositories
{
    internal class UserRepository : GenericRepository
    {
        public User? AuthenticateUser(string userName, string password)
        {
            string query = @$"SELECT [dbo].[User].Id
                                    ,[dbo].[User].Name
                                    ,[dbo].[User].Email
                                    ,[dbo].[User].Password
                                    FROM [dbo].[User]
                                    WHERE [dbo].[User].Name = {userName}
                                    and [dbo].[User].Password = {password}";

            Connect();

            ExecuteQuery(query);

            User user;

            if (sqlDataReader.Read())
            {
                user = new User(sqlDataReader);
                return user;
            }

            Disconnect();

            return null;
        }
    }
}
