using System.Data.SqlClient;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Data.Repositories
{
    internal class UserRepository : GenericRepository<User>
    {

        public User? AuthenticateUser(string userName, string password)
        {
            string query = @$"SELECT [dbo].[User].Id
                                    ,[dbo].[User].Name
                                    ,[dbo].[User].Email
                                    ,[dbo].[User].Password
                                    FROM [dbo].[User]
                                    WHERE [dbo].[User].Name = '{userName}'
                                    and [dbo].[User].Password = '{password}' ";

            Connect();

            ExecuteQuery(query);

            User user;

            if (sqlDataReader.Read())
            {
                user = new User(sqlDataReader);
                Disconnect();
                return user;
            }

            Disconnect();

            return null;
        }

        public List<User> ReadInsensitiveInformation(Func<SqlDataReader, User> createObject)
        {
            List<User> users = new List<User>();

            string query = @$"SELECT [dbo].[User].Id
                                    ,[dbo].[User].Name
                                    ,[dbo].[User].Email
                                    FROM [dbo].[User]";

            Connect();

            ExecuteQuery(query);

            while (sqlDataReader.Read())
            {
                User user = new User(Convert.ToInt32(sqlDataReader["Id"]), sqlDataReader["Name"].ToString(), sqlDataReader["Email"].ToString());
                users.Add(user);
            }

            Disconnect();

            return users;

        }
    }
}