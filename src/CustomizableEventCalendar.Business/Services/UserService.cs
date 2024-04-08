using CustomizableEventCalendar.src.CustomizableEventCalendar.Data.Repositories;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services
{
    internal class UserService
    {
        private readonly UserRepository userRepository = new();

        public List<User> GetInsensitiveInformationOfUser()
        {

            List<User> users = userRepository.ReadInsensitiveInformation(data => new User(data))
                                             .Where(user => user.Id != GlobalData.GetUser().Id)
                                             .ToList();

            return users;
            
        }

        public User? Read(int userId)
        {
            User? user = userRepository.GetById(data => new User(data), userId);
            return user;
        }

        public void AddUser(User user)
        {
            userRepository.Insert(user);
        }
    }
}