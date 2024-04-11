// Ignore Spelling: username
using CustomizableEventCalendar.src.CustomizableEventCalendar.Data.Repositories;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Models;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services
{
    internal class UserAuthenticationService
    {
        private readonly UserRepository userRepository = new();

        public bool Authenticate(string username, string password)
        {
            UserModel? userModel = userRepository.AuthenticateUser(username, password);

            GlobalData.SetUser(userModel);

            if (userModel != null)
            {
                ScheduleProposedEventsForLoggedInUser();
            }

            return userModel != null;
        }

        private static void ScheduleProposedEventsForLoggedInUser()
        {
            MultipleInviteesEventService multipleInviteesEventService = new();

            multipleInviteesEventService.StartSchedulingProcessOfProposedEvent();
        }
    }
}