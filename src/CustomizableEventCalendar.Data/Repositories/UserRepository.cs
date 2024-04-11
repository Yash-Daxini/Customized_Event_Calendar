using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Mapping;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Models;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Data.Repositories
{
    internal class UserRepository : GenericRepository<User>
    {

        private readonly UserMapper _userMapper = new();

        public List<UserModel> GetAll()
        {
            List<User> users = GetAll(data => new User(data));

            return [.. users.Select(_userMapper.MapUserEntityToModel)];
        }

        public UserModel? GetById(int userId)
        {
            User? user = GetById(data => new User(data), userId);

            if (user == null) return null;
            return _userMapper.MapUserEntityToModel(user);
        }

        public int Insert(UserModel userModel)
        {
            User user = _userMapper.MapUserModelToEntity(userModel);

            return Insert(user);
        }

        public void Update(UserModel userModel)
        {
            User user = _userMapper.MapUserModelToEntity(userModel);

            Update(user, user.Id);
        }

        public UserModel? AuthenticateUser(string userName, string password)
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
                return _userMapper.MapUserEntityToModel(user);
            }

            Disconnect();

            return null;
        }

        public List<UserModel> ReadInsensitiveInformation()
        {
            List<UserModel> users = [];

            string query = @$"SELECT [dbo].[User].Id
                                    ,[dbo].[User].Name
                                    ,[dbo].[User].Email
                                    FROM [dbo].[User]";

            Connect();

            ExecuteQuery(query);

            while (sqlDataReader.Read())
            {
                User user = new User(Convert.ToInt32(sqlDataReader["Id"]), sqlDataReader["Name"].ToString(), sqlDataReader["Email"].ToString());
                users.Add(_userMapper.MapUserEntityToModel(user));
            }

            Disconnect();

            return users;

        }
    }
}