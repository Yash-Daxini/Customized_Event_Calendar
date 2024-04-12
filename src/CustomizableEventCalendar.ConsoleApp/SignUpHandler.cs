using System.Data.SqlClient;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Models;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.ConsoleApp
{
    internal static class SignUpHandler
    {

        private static void GetSignUpDetails(out UserModel user)
        {
            user = new()
            {
                Name = ValidatedInputProvider.GetValidString($"Enter Name: "),
                Email = ValidatedInputProvider.GetValidEmail($"Enter Email: "),
                Password = ValidatedInputProvider.GetValidString($"Enter Password: ")
            };
        }

        public static void SignUp()
        {
            try
            {
                GetSignUpDetails(out UserModel user);

                new UserService().AddUser(user);

                PrintHandler.PrintSuccessMessage("Sign up completed successfully");

                LogInHandler.LogIn();
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627 || ex.Number == 2601) //Check the unique key constraint
                {
                    throw new Exception("The user name you've entered is not available. Please choose another name.");
                }
                else
                {
                    throw new Exception("Some error occurred!");
                }
            }
        }
    }
}