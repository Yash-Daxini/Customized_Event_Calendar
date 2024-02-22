﻿using System;
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
        UserRepository userRepository = new UserRepository();
        public string GetInsensitiveInformationOfUser()
        {
            List<User> users = userRepository.ReadInsensitiveInformation(data => new User(data));

            StringBuilder userInformation = new StringBuilder();

            foreach (var user in users)
            {
                userInformation.AppendLine($"User Sr. No :- {user.Id} , Name :-  {user.Name} , Email :- {user.Email}");
            }

            return userInformation.ToString();
        }
        public User? Read(int userId)
        {
            User? user = userRepository.Read(data => new User(data), userId);
            return user;
        }
    }
}
