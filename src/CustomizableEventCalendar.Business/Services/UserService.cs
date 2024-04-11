using CustomizableEventCalendar.src.CustomizableEventCalendar.Data.Repositories;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Models;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services
{
    internal class UserService
    {
        private readonly UserRepository userRepository = new();

        public List<UserModel> GetInsensitiveInformationOfUser()
        {

            List<UserModel> users = userRepository.ReadInsensitiveInformation()
                                             .Where(user => user.Id != GlobalData.GetUser().Id)
                                             .ToList();

            return users;
            
        }

        public UserModel? GetUserById(int userId)
        {
            UserModel? user = userRepository.GetById(userId);
            return user;
        }

        public void AddUser(UserModel user)
        {
            userRepository.Insert(user);
        }
    }
}