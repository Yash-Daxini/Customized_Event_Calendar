using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomizableEventCalendar.src.CustomizableEventCalendar.ConsoleApp;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Data.Repositories;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services
{
    internal class UserService
    {
        private readonly UserRepository userRepository = new UserRepository();

        public List<User> GetInsensitiveInformationOfUser()
        {

            List<User> users = userRepository.ReadInsensitiveInformation(data => new User(data))
                                             .Where(user => user.Id != GlobalData.user.Id)
                                             .ToList();

            return users;
            
        }

        public User? Read(int userId)
        {
            User? user = userRepository.GetById(data => new User(data), userId);
            return user;
        }
    }
}