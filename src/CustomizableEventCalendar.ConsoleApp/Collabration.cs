using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Data.Repositories;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.ConsoleApp
{
    internal class Collabration
    {
        public void GetDetailsToShareCalendar()
        {
            ShowAllUser();
            Console.WriteLine("Enter User No. whom you want to share calendar :- ");
            int UserId = Convert.ToInt32(Console.ReadLine());
            UserRepository userRepository = new UserRepository();
            User user = userRepository.Read<User>(data => new User(data), UserId);
            Console.WriteLine($"Your Calendar shared with {user.Name}");
        }
        public void ShowAllUser()
        {
            UserService userService = new UserService();
            List<User> users = userService.GetInsensitiveInformationOfUser();
            foreach (User user in users)
            {
                Console.WriteLine($"Name : {user.Name} , Email :- {user.Email} , Password :- {user.Password}");
            }
        }
    }
}
