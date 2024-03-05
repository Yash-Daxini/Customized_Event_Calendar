using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services;
using CustomizableEventCalendar.src.CustomizableEventCalendar.ConsoleApp.InputMessageStore;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Data.Repositories;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.ConsoleApp
{
    internal class ShareCalendar
    {
        private readonly CalendarSharingService _calendarSharingService = new();

        private readonly ValidationService _validationService = new();

        public void GetDetailsToShareCalendar()
        {
            bool isUsersAvailble = ShowAllUser();

            if (!isUsersAvailble) return;

            int UserId = ValidatedInputProvider.GetValidatedInteger("Enter User No. whom you want to share calendar :- ");

            Console.WriteLine("Enter Time Range for which you want to share events");

            DateOnly startDate = ValidatedInputProvider.GetValidatedDateOnly("Enter start date in dd-MM-yyyy format :-  ");

            DateOnly endDate = ValidatedInputProvider.GetValidatedDateOnly("Enter end date in dd-MM-yyyy format :-  ");

            UserRepository userRepository = new();

            User user = userRepository.GetById<User>(data => new User(data), UserId);

            SharedCalendar sharedEvents = new(UserId, GlobalData.user == null ? 0 : GlobalData.user.Id, startDate, endDate);

            _calendarSharingService.AddSharedCalendar(sharedEvents);

            Console.WriteLine($"Your Calendar shared with {user.Name} from {startDate} to {endDate}");

        }

        public int GetValidatedInteger(string inputMessage)
        {

            Console.Write(inputMessage);
            string inputFromConsole = Console.ReadLine();

            int Id;

            while (!_validationService.ValidateInput(inputFromConsole, out Id, int.TryParse))
            {
                Console.Write(inputMessage);
                inputFromConsole = Console.ReadLine();
            }

            return Id;
        }

        public bool ShowAllUser()
        {
            UserService userService = new();

            List<User> users = userService.GetInsensitiveInformationOfUser();

            if(users.Count > 0) 
            {
                StringBuilder userInformation = new();

                List<List<string>> userTableContent = [["User Sr. No", "Name", "Email"]];

                foreach (var user in users)
                {
                    userTableContent.Add([user.Id.ToString(), user.Name, user.Email]);
                }

                userInformation.AppendLine(PrintHandler.GiveTable(userTableContent));

                Console.WriteLine(userInformation);
            }
            else
            {
                Console.WriteLine("No users are available !");
            }
            return users.Count > 0;

        }

        public void ViewSharedCalendars()

        {
            Console.WriteLine("Calendars shared to you !");

            string sharedEvents = _calendarSharingService.GenerateDisplayFormatForSharedEvents();

            Console.WriteLine(sharedEvents);

            int sharedCalendarId = ValidatedInputProvider.GetValidatedInteger("Select Sr No. which calendar you want to see :- ");

            ViewSpecificCalendar(sharedCalendarId);
        }

        public void ViewSpecificCalendar(int sharedCalendarId)
        {
            string calendar = _calendarSharingService.GenerateSharedCalendar(sharedCalendarId);
            Console.WriteLine(calendar);
        }
    }
}