using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomizableEventCalendar.src.CustomizableEventCalendar.ConsoleApp;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Data.Repositories;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;
using System.Xml.Linq;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services
{
    internal class UserAuthenticationService
    {
        public bool Authenticate(string username, string password)
        {
            GenericRepository genericRepository = new GenericRepository();
            List<User> users = genericRepository.Read(data => new User(data));

            foreach (User user in users)
            {
                if (user.Name.Equals(username) && user.Password.Equals(password))
                {
                    Console.WriteLine("Login Successfully");
                    GlobalData.user = user;
                    return true;
                }
            }
            return false;
        }
        public void AddUser(User user)
        {
            GenericRepository genericRepository = new GenericRepository();
            genericRepository.Create(user);
        }
    }
}
