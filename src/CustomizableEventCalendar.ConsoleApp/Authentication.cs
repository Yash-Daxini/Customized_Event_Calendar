using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using CustomizableEventCalendar.src.CustomizableEventCalendar.ConsoleApp;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Enums;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services
{
    internal static class Authentication
    {
        private readonly static UserAuthenticationService _userAuthenticationService = new();

        public static void AuthenticationChoice()
        {

            if (GlobalData.user != null)
            {
                AskForChoiceToLoginUser();
            }
            else
            {
                AskForChoiceToLogoutUser();
            }

        }

        public static void AskForChoiceToLoginUser()
        {

            int choice = ValidatedInputProvider.GetValidatedInteger("\nChoose the option: \n1. Logout \n0. Exit \nEnter Choice : ");

            LoginUserChoices option = (LoginUserChoices)choice;

            switch (option)
            {
                case LoginUserChoices.Logout:
                    Logout();
                    AuthenticationChoice();
                    break;
                case LoginUserChoices.Exit:
                    break;
                default:
                    Console.WriteLine("Please choose correct option");
                    AuthenticationChoice();
                    break;
            }

        }

        public static void AskForChoiceToLogoutUser()
        {

            int choice = ValidatedInputProvider.GetValidatedInteger("\nChoose the option: \n1. Login\n2. Sign up \n0. Exit \nEnter Choice :  ");

            LogoutUserChoices option = (LogoutUserChoices)choice;

            switch (option)
            {
                case LogoutUserChoices.Login:
                    bool isLoggedin = Login();
                    if (!isLoggedin) AuthenticationChoice();
                    break;
                case LogoutUserChoices.Signup:
                    bool isSignUp = SignUp();
                    if (isSignUp) Login();
                    else AuthenticationChoice();
                    break;
                case LogoutUserChoices.Exit:
                    break;
                default:
                    Console.WriteLine("Please choose correct option");
                    AuthenticationChoice();
                    break;
            }

        }

        public static void GetSignUpDetails(out User user)
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

        public static bool SignUp()
        {
            GetSignUpDetails(out User user);

            bool isSignUp = _userAuthenticationService.AddUser(user);

            if (isSignUp)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Sign up completed successfully");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Some error occurred !");
                Console.ResetColor();
            }

            return isSignUp;

        }

        public static void GetLoginDetails(out string userName, out string password)
        {
            Console.WriteLine("\nEnter Login Details");

            Console.Write("Enter Name: ");
            userName = Console.ReadLine();

            Console.Write("Enter Password: ");
            password = Console.ReadLine();
        }

        public static bool Login()
        {
            GetLoginDetails(out string usrename, out string password);

            bool isAuthencticate = _userAuthenticationService.Authenticate(usrename, password);

            if (isAuthencticate)
            {
                ShowUserInfo();
                EventHandling.AskForChoice();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid user name or password! Please try again.");
                Console.ResetColor();
                return false;
            }
            return true;
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