using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.ConsoleApp
{
    internal static class UserOperationHandler
    {

        public static void AskForChoiceForUserOperations()
        {

            try
            {
                if (GlobalData.GetUser() is null)
                {
                    AskForLoginSignUpOrExit();
                }
                else
                {
                    AskForLogoutOrExit();
                }
            }
            catch (Exception ex)
            {
                PrintHandler.PrintErrorMessage(ex.Message);
                AskForChoiceForUserOperations();
            }
        }

        public static void AskForLogoutOrExit()
        {

            int choice = ValidatedInputProvider.GetValidIntegerBetweenRange("\nChoose the option: \n1. Logout \n2. Exit \nEnter Choice : ", 1, 2);

            switch (choice)
            {
                case 1:
                    LogInHandler.Logout();
                    AskForLoginSignUpOrExit();
                    break;
                case 2:
                    break;
            }

        }

        public static void AskForLoginSignUpOrExit()
        {

            int choice = ValidatedInputProvider.GetValidIntegerBetweenRange("\nChoose the option: \n1. Login\n2. Sign up \n3. Exit \nEnter Choice :  ", 1, 3);

            switch (choice)
            {
                case 1:
                    LogInHandler.LogIn();
                    break;
                case 2:
                    SignUpHandler.SignUp();
                    break;
                case 3:
                    break;
            }

        }
    }
}