// Ignore Spelling: username
using CustomizableEventCalendar.src.CustomizableEventCalendar.Data.Repositories;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services
{
    internal class UserAuthenticationService
    {
        private readonly UserRepository userRepository = new();

        public bool Authenticate(string username, string password)
        {
            User? user = userRepository.AuthenticateUser(username, password);

            GlobalData.SetUser(user);

            if (user != null)
            {
                ScheduleProposedEventsForLoggedInUser();
            }

            return user != null;
        }

        private static void ScheduleProposedEventsForLoggedInUser()
        {
            MultipleInviteesEventService multipleInviteesEventService = new();

            multipleInviteesEventService.StartSchedulingProcessOfProposedEvent();
        }
    }
}