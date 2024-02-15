using CustomizableEventCalendar.src.CustomizableEventCalendar.Data.Repositories;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services
{
    internal class UserAuthenticationService
    {
        UserRepository userRepository = new UserRepository();
        public bool Authenticate(string username, string password)
        {
            User? user = userRepository.AuthenticateUser(username, password);
            GlobalData.user = user;
            return user == null;
        }
        public void AddUser(User user)
        {
            userRepository.Create(user);
        }
    }
}
