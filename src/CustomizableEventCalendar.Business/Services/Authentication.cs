using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CustomizableEventCalendar.src.CustomizableEventCalendar.ConsoleApp;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Data.Repositories;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services
{
    internal class Authentication
    {
        public void SignUp()
        {
            Console.WriteLine("\nEnter Sign Up Details");
            User user = new User();
            PropertyInfo[] properties = user.GetType().GetProperties().Where(property => !Attribute.IsDefined(property, typeof(NotMappedAttribute))).ToArray();
            foreach (PropertyInfo property in properties)
            {
                Console.Write($"Enter value for {property.Name}: ");
                string value = Console.ReadLine();
                object typedValue = Convert.ChangeType(value, property.PropertyType);
                property.SetValue(user, typedValue);
            }
            GenericRepository genericRepository = new GenericRepository();
            genericRepository.Create<User>(user);
            Console.WriteLine("Sign up successfull !");
            LogIn();
        }
        public void LogIn()
        {
            Console.WriteLine("\nEnter Login Details");
            Console.Write("Enter Name: ");
            string? name = Console.ReadLine();
            Console.Write("Enter Password: ");
            string? password = Console.ReadLine();

            GenericRepository genericRepository = new GenericRepository();
            List<User> users = genericRepository.Read(data => new User(data));

            foreach (User user in users)
            {
                if (user.Name.Equals(name) && user.Password.Equals(password))
                {
                    Console.WriteLine("Login Succesfully");
                    GlobalData.user = user;
                    EventHandling.AskForChoice();
                    return;
                }
            }
            Console.WriteLine("Invalid user name or password! Please try again.");
            LogIn();

        }
        public void Logout()
        {
            Console.WriteLine("Successfully Logout!");
            GlobalData.user = null;
            LoginOrSignUp();
        }
        public void LoginOrSignUp()
        {
            Console.WriteLine("Choose the option: \n1. Login\t2. SignUp ");
            string option = Console.ReadLine();
            switch (option)
            {
                case "1":
                    LogIn();
                    break;
                case "2":
                    SignUp();
                    break;
                case "0": break;
                default:
                    Console.WriteLine("Please choose correct option"); LoginOrSignUp();
                    break;
            }
        }
    }
}