using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using CustomizableEventCalendar.src.CustomizableEventCalendar.ConsoleApp;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Enums;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services
{
    internal class Authentication
    {
        public static UserAuthenticationService userAuthenticationService = new UserAuthenticationService();
        static ValidationService validationService = new ValidationService();
        public static void AskForChoice()
        {
            int choice = ValidatedInputProvider.GetValidatedInteger("\nChoose the option: \n1. Login\t2. Sign up " +
                                                                    "\t3. Logout \t0. Exit :-");

            UserActionEnum option = (UserActionEnum)choice;

            switch (option)
            {
                case UserActionEnum.Login:
                    Login();
                    break;
                case UserActionEnum.Signup:
                    Signup();
                    Login();
                    break;
                case UserActionEnum.Logout:
                    Logout();
                    AskForChoice();
                    break;
                case UserActionEnum.Exit:
                    break;
                default:
                    Console.WriteLine("Please choose correct option");
                    AskForChoice();
                    break;
            }
        }
        public static void GetSignupDetails(out User user)
        {
            user = new User();

            PropertyInfo[] properties = user.GetType().GetProperties().Where(property => !Attribute.IsDefined(property, typeof(NotMappedAttribute))).ToArray();

            Console.WriteLine("\nEnter Sign Up Details");

            foreach (PropertyInfo property in properties)
            {
                Console.Write($"Enter value for {property.Name}: ");
                string value = Console.ReadLine();

                if (property.Name.Equals("Email"))
                {
                    value = ValidatedInputProvider.GetValidateEmail(value);
                }

                object typedValue = Convert.ChangeType(value, property.PropertyType);
                property.SetValue(user, typedValue);
            }
        }
        public static void Signup()
        {
            GetSignupDetails(out User user);

            userAuthenticationService.AddUser(user);
        }
        public static void GetLoginDetails(out string userName, out string password)
        {
            Console.WriteLine("\nEnter Login Details");

            Console.Write("Enter Name: ");
            userName = Console.ReadLine();

            Console.Write("Enter Password: ");
            password = Console.ReadLine();
        }
        public static void Login()
        {
            GetLoginDetails(out string usrename, out string password);

            bool isAuthencticate = userAuthenticationService.Authenticate(usrename, password);

            if (isAuthencticate)
            {
                ShowUserInfo();
                EventHandling.AskForChoice();
            }
            else
            {
                Console.WriteLine("Invalid user name or password! Please try again.");
                Login();
            }
        }
        public static void ShowUserInfo()
        {
            Console.Clear();

            PrintHandler.ShowLoadingAnimation();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.WriteLine($"{PrintHandler.CenterText()}Welcome {GlobalData.user.Name}");
            Thread.Sleep(1000);
            Console.ResetColor();
            Console.Clear();

            PrintHandler.PrintUserName(GlobalData.user.Name);
            Console.WriteLine();

            ShowNotification();

        }
        public static void ShowNotification()
        {
            NotificationService notificationService = new NotificationService();
            Console.WriteLine(notificationService.GenerateNotification());
        }
        public static void Logout()
        {
            GlobalData.user = null;
            Console.WriteLine("Successfully Logout!");
        }
    }
}