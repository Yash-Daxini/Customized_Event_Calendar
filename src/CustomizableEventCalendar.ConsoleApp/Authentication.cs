using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using CustomizableEventCalendar.src.CustomizableEventCalendar.ConsoleApp;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services
{
    internal class Authentication
    {
        public static UserAuthenticationService userAuthenticationService = new UserAuthenticationService();
        public static void ShowUserInfo()
        {
            Console.Clear();
            Console.WriteLine($"\t\t\t\t\t\t\tWelcome {GlobalData.user.Name}");
            Thread.Sleep(2000);
            Console.Clear();
            Console.WriteLine($"\t\t\t\t\t\t\tUser Name : {GlobalData.user.Name}");
            Console.WriteLine();
        }
        public static void SignUp()
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
            userAuthenticationService.AddUser(user);
            Console.WriteLine("Sign up successfully !");
            LogIn();
        }
        public static void LogIn()
        {
            Console.WriteLine("\nEnter Login Details");
            Console.Write("Enter Name: ");
            string? name = Console.ReadLine();
            Console.Write("Enter Password: ");
            string? password = Console.ReadLine();

            bool isAuthencticate = userAuthenticationService.Authenticate(name, password);

            if (isAuthencticate)
            {
                ShowUserInfo();
                EventHandling.AskForChoice();
            }
            else
            {
                Console.WriteLine("Invalid user name or password! Please try again.");
                LogIn();
            }
        }
        public static void Logout()
        {
            GlobalData.user = null;
            Console.WriteLine("Successfully Logout!");
            LoginOrSignUp();
        }
        public static void LoginOrSignUp()
        {
            Console.WriteLine();
            Console.Write("Choose the option: \n1. Login\t2. SignUp \t3. Logout \t0. Exit :-");
            string option = Console.ReadLine();
            switch (option)
            {
                case "1":
                    LogIn();
                    break;
                case "2":
                    SignUp();
                    break;
                case "3":
                    Logout();
                    break;
                case "0": break;
                default:
                    Console.WriteLine("Please choose correct option"); LoginOrSignUp();
                    break;
            }
        }
    }
}