// Ignore Spelling: username

using System.Data.SqlClient;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Data.Repositories;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services
{
    internal class UserAuthenticationService
    {
        private readonly UserRepository userRepository = new();

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
                Console.WriteLine("Some error occurred! " + ex.Message);
            }
            if (user != null)
            {
                RecurrenceEngine recurrenceEngine = new();

                recurrenceEngine.ScheduleEventsOfThisMonth();

                MultipleInviteesEventService multipleInviteesEventService = new();

                multipleInviteesEventService.StartSchedulingProcessOfProposedEvent();
            }
            return user != null;

        }

        public bool AddUser(User user)
        {

            try
            {
                userRepository.Insert(user);
                return true;
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
                Console.WriteLine("Some error occurred! " + ex.Message);
            }
            return false;
        }
    }
}