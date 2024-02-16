using System.Data.SqlClient;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Data.Repositories;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services
{
    internal class UserAuthenticationService
    {
        UserRepository userRepository = new UserRepository();
        public bool Authenticate(string username, string password)
        {
            User? user = null;
            try
            {
                user = userRepository.AuthenticateUser(username, password);
                GlobalData.user = user;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Some error occurred!");
            }
            return user != null;
        }
        public void AddUser(User user)
        {
            try
            {
                userRepository.Create(user);
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627 || ex.Number == 2601) //Check the unique key constraint
                {
                    Console.WriteLine("User name is not available. Please Enter another name");
                }
                else
                {
                    Console.WriteLine("Some error occurred!" + " " + ex.Message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Some error occurred!");
            }
        }
    }
}
