using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;

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
    }
}
