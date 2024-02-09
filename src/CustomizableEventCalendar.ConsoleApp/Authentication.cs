using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Data.Repositories;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.ConsoleApp
{
    internal class Authentication
    {
        public void SignUp()
        {
            Console.Write("Enter Name: ");
            string? name = Console.ReadLine();
            Console.Write("Enter Email: ");
            string? email = Console.ReadLine();
            Console.Write("Enter Password: ");
            string? password = Console.ReadLine();
            GenericRepository genericRepository = new GenericRepository();
            User user = new User(name, email, password);
            genericRepository.Create<User>(new UserQuerySupplier(), user.generateDictionary());
        }
        public void LogIn()
        {
            Console.Write("Enter Name: ");
            string? name = Console.ReadLine();
            Console.Write("Enter Password: ");
            string? password = Console.ReadLine();

            GenericRepository genericRepository = new GenericRepository();
            List<User> users = genericRepository.Read<User>(new UserQuerySupplier(), data => new User(data));

            foreach (User user in users)
            {
                if (user.Name.Equals(name) && user.Password.Equals(password))
                {
                    Console.WriteLine("Login Succesfully");
                    GlobalData.user = user;
                    EventHandling.askForChoice();
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
