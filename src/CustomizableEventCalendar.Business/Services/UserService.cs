using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Data.Repositories;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services
{
    internal class UserService
    {
        public List<User> GetInsensitiveInformationOfUser()
        {
            UserRepository userRepository = new UserRepository();
            List<User> users = userRepository.ReadInsensitiveInformation(data => new User(data));
            StringBuilder userInformation = new StringBuilder();
            foreach (var user in users)
            {
                userInformation.AppendLine($"")
            }
            return users;
        }
    }
}
