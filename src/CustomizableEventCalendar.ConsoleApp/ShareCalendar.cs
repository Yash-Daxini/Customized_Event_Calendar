using System.Text;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.ConsoleApp
{
    internal class ShareCalendar
    {
        private readonly CalendarSharingService _calendarSharingService = new();
        private readonly static UserService _userService = new();

        public void GetDetailsToShareCalendar()
        {
            try
            {
                int availableUsersToShareCalendar = ShowAllUser();

                if (availableUsersToShareCalendar <= 0) return;

                string inputMessage = "Enter Sr No. whom you want to share calendar :- ";

                int serialNumberOfTableRow = ValidatedInputProvider.GetValidatedIntegerBetweenRange(inputMessage, 1,
                                             availableUsersToShareCalendar);

                User user = GetUserFromSerialNumber(serialNumberOfTableRow);

                SharedCalendar sharedCalendar = new(user.Id, GlobalData.GetUser() == null ? 0 : GlobalData.GetUser().Id, new DateOnly(), new DateOnly());

                GetDatesFromUser(sharedCalendar);

                _calendarSharingService.AddSharedCalendar(sharedCalendar);

                PrintHandler.PrintNewLine();

                PrintHandler.PrintSuccessMessage($"Your Calendar shared with {user.Name} from {sharedCalendar.FromDate} to {sharedCalendar.ToDate}");

            }
            catch
            {
                PrintHandler.PrintErrorMessage("Some error occurred !");
            }

        }

        public static void GetDatesFromUser(SharedCalendar sharedCalendar)
        {
            Console.WriteLine("Enter Time Range for which you want to share events");

            PrintHandler.PrintNewLine();

            sharedCalendar.FromDate = ValidatedInputProvider.GetValidatedDateOnly("Enter start date in dd-MM-yyyy format :-  ");

            PrintHandler.PrintNewLine();

            sharedCalendar.ToDate = ValidatedInputProvider.GetValidatedDateOnly("Enter end date in dd-MM-yyyy format :-  ");
        }

        public static User GetUserFromSerialNumber(int serialNumber)
        {
            List<User> users = _userService.GetInsensitiveInformationOfUser();

            return users[serialNumber - 1];
        }

        public static int ShowAllUser()
        {
            List<User> users = _userService.GetInsensitiveInformationOfUser();

            if (users.Count > 0)
            {
                StringBuilder userInformation = new();

                userInformation.AppendLine(PrintService.GenerateTable(Get2DListToGenerateTable(users)));

                Console.WriteLine(userInformation);
            }
            else
            {
                Console.WriteLine("No users are available !");
            }
            return users.Count;

        }

        public static List<List<string>> Get2DListToGenerateTable(List<User> users)
        {
            List<List<string>> userTableContent = [["Sr. No", "Name", "Email"]];

            foreach (var (user, index) in users.Select((user, index) => (user, index)))
            {
                userTableContent.Add([(index + 1).ToString(), user.Name, user.Email]);
            }

            return userTableContent;

        }

        public void ViewSharedCalendars()
        {
            try
            {
                List<SharedCalendar> sharedCalendarsList = _calendarSharingService.GetSharedEvents();

                string sharedEvents = _calendarSharingService.DesignSharedEventDisplayFormat();

                if (_calendarSharingService.GetSharedEventsCount() > 0)
                {
                    Console.WriteLine("Calendars shared to you !");
                    Console.WriteLine(sharedEvents);
                }
                else
                {
                    Console.WriteLine("No calendars available !");
                    return;
                }

                string inputMessage = "Select Sr No. which calendar you want to see :- ";

                int serialNumberOfSharedCalendar = ValidatedInputProvider.GetValidatedIntegerBetweenRange(inputMessage
                                                    , 1, sharedCalendarsList.Count);

                ViewSpecificCalendar(sharedCalendarsList[serialNumberOfSharedCalendar - 1].Id);

            }
            catch
            {
                PrintHandler.PrintErrorMessage("Some error occurred !");
            }
        }

        public void ViewSpecificCalendar(int sharedCalendarId)
        {
            string calendar = _calendarSharingService.GenerateSharedCalendar(sharedCalendarId);
            Console.WriteLine(calendar);
        }
    }
}