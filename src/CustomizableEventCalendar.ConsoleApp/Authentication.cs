using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlClient;
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

            if (GlobalData.GetUser() != null)
            {
                AskForChoiceToLoggedInUser();
            }
            else
            {
                AskForChoiceToLoggedOutUser();
            }

        }

        public static void AskForChoiceToLoggedInUser()
        {

            int choice = ValidatedInputProvider.GetValidatedIntegerBetweenRange("\nChoose the option: \n1. Logout \n0. Exit \nEnter Choice : ", 0, 1);

            LoggedinUserChoice option = (LoggedinUserChoice)choice;

            switch (option)
            {
                case LoggedinUserChoice.Logout:
                    Logout();
                    AuthenticationChoice();
                    break;
                case LoggedinUserChoice.Exit:
                    break;
            }

        }

        public static void AskForChoiceToLoggedOutUser()
        {

            int choice = ValidatedInputProvider.GetValidatedIntegerBetweenRange("\nChoose the option: \n1. Login\n2. Sign up \n0. Exit \nEnter Choice :  ", 0, 2);

            LoggedoutUserChoice option = (LoggedoutUserChoice)choice;

            switch (option)
            {
                case LoggedoutUserChoice.Login:
                    Login();
                    break;
                case LoggedoutUserChoice.Signup:
                    SignUp();
                    break;
                case LoggedoutUserChoice.Exit:
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

        public static void SignUp()
        {
            try
            {
                GetSignUpDetails(out User user);

                _userAuthenticationService.AddUser(user);

                PrintHandler.PrintSuccessMessage("Sign up completed successfully");

                Login();

                return;
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627 || ex.Number == 2601) //Check the unique key constraint
                {
                    PrintHandler.PrintErrorMessage("The user name you've entered is not available. Please choose another name.");
                }
                else
                {
                    PrintHandler.PrintErrorMessage("Some error occurred!" + " " + ex.Message);
                }
            }
            catch (Exception ex)
            {
                PrintHandler.PrintErrorMessage("Some error occurred! " + ex.Message);
            }

            AuthenticationChoice();
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
            try
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
                    PrintHandler.PrintErrorMessage("Invalid user name or password! Please try again.");
                }
            }
            catch (Exception ex)
            {
                PrintHandler.PrintErrorMessage("Some error occurred! " + ex.Message);
            }
            AuthenticationChoice();
        }

        public static void ShowUserInfo()
        {
            Console.Clear();

            PrintHandler.ShowLoadingAnimation();

            Console.SetCursorPosition(0, Console.CursorTop);

            PrintHandler.PrintSuccessMessage($"{PrintHandler.CenterText()}Welcome {GlobalData.GetUser().Name}");

            Thread.Sleep(1000);

            Console.Clear();

            PrintHandler.PrintUserName(GlobalData.GetUser().Name);

            Console.WriteLine();

            ShowNotification();
        }

        public static void ShowNotification()
        {
            NotificationService notificationService = new();

            PrintHandler.PrintNotification(notificationService.GenerateNotification());
        }

        public static void Logout()
        {
            GlobalData.SetUser(null);

            PrintHandler.PrintSuccessMessage("Successfully Logout!");
        }
    }
}