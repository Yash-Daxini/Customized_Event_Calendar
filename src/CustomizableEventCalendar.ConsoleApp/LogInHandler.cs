using CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.ConsoleApp
{
    internal static class LogInHandler
    {

        private readonly static UserAuthenticationService _userAuthenticationService = new();

        public static void GetLoginDetails(out string userName, out string password)
        {
            Console.WriteLine("\nEnter Login Details");
            userName = ValidatedInputProvider.GetValidString("Enter Name: ");
            password = ValidatedInputProvider.GetValidString("Enter Password: ");
        }

        public static void LogIn()
        {
            try
            {
                GetLoginDetails(out string usrename, out string password);

                bool isAuthencticate = _userAuthenticationService.Authenticate(usrename, password);

                if (isAuthencticate)
                {
                    NotificationPrinter.DisplayUserInfo();
                    EventHandling.AskForChoice();
                }
                else
                {
                    PrintHandler.PrintErrorMessage("Invalid user name or password! Please try again.");
                }
            }
            catch
            {
                throw new Exception("Some error occurred! ");
            }
            UserOperationHandler.AskForUserOperationChoice();
        }

        public static void Logout()
        {
            GlobalData.SetUser(null);

            PrintHandler.PrintSuccessMessage("Successfully Logout!");
        }
    }
}