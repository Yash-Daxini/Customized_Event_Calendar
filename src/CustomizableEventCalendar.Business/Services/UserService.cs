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
        public string GetInsensitiveInformationOfUser()
        {
            List<User> users = userRepository.ReadInsensitiveInformation(data => new User(data));

            StringBuilder userInformation = new StringBuilder();

            List<List<string>> userTableContent = new List<List<string>> { new List<string> { "User Sr. No", "Name", "Email" } };

            foreach (var user in users)
            {
                userTableContent.Add(new List<string> { user.Id.ToString(), user.Name, user.Email });
            }

            userInformation.AppendLine(PrintHandler.PrintTable(userTableContent));

            return userInformation.ToString();
        }
        public User? Read(int userId)
        {
            User? user = userRepository.Read(data => new User(data), userId);
            return user;
        }
    }
}