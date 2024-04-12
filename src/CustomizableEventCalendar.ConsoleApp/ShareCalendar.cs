using System.Text;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Data.Repositories;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Models;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.ConsoleApp
{
    internal class ShareCalendar
    {
        private readonly CalendarSharingService _calendarSharingService = new();

        private readonly static UserService _userService = new();

        public List<SharedCalendarModel> sharedCalendarsList = [];

        public List<EventModel> sharedEventsOfSpecificCalendar = [];

        public void GetDetailsToShareCalendar()
        {
            try
            {
                string userTable = GetUserTableAsString();

                if (userTable.Length == 0)
                {
                    Console.WriteLine("No user available to share calendar !");
                    return;
                }

                Console.WriteLine(userTable);

                string inputMessage = "Enter Sr No. whom you want to share calendar :- ";

                int serialNumberOfTableRow = ValidatedInputProvider.GetValidIntegerBetweenRange(inputMessage, 1, GetAvailableUsers().Count);

                UserModel user = GetUserFromSerialNumber(serialNumberOfTableRow);

                SharedCalendarModel sharedCalendarModel = new(GlobalData.GetUser(), user, new DateOnly(), new DateOnly());

                GetDatesFromUser(sharedCalendarModel);

                AddSharedCalendar(user, sharedCalendarModel);

            }
            catch
            {
                PrintHandler.PrintErrorMessage("Some error occurred !");
            }

        }

        private void AddSharedCalendar(UserModel user, SharedCalendarModel sharedCalendar)
        {
            try
            {
                _calendarSharingService.AddSharedCalendar(sharedCalendar);

                PrintHandler.PrintSuccessMessage($"\nYour Calendar shared with {user.Name} from {sharedCalendar.FromDate} to {sharedCalendar.ToDate}");
            }
            catch
            {
                throw new Exception("Some error occurred !");
            }
        }

        private static void GetDatesFromUser(SharedCalendarModel sharedCalendarModel)
        {
            Console.WriteLine("Enter Time Range for which you want to share events");

            PrintHandler.PrintNewLine();

            sharedCalendarModel.FromDate = ValidatedInputProvider.GetValidDateOnly("Enter start date in dd-MM-yyyy format :-  ");

            PrintHandler.PrintNewLine();

            sharedCalendarModel.ToDate = ValidatedInputProvider.GetValidDateOnly("Enter end date in dd-MM-yyyy format :-  ");

            if (!ValidationService.IsValidStartAndEndDate(sharedCalendarModel.FromDate, sharedCalendarModel.ToDate))
            {
                PrintHandler.PrintWarningMessage("Invalid input ! Start date must less than or equal to the end date ");
                GetDatesFromUser(sharedCalendarModel);
            }
        }

        private static UserModel GetUserFromSerialNumber(int serialNumber)
        {
            List<UserModel> users = _userService.GetInsensitiveInformationOfUser();
            return users[serialNumber - 1];
        }

        private static string GetUserTableAsString()
        {
            List<UserModel> users = GetAvailableUsers();

            if (users.Count == 0) return "";

            StringBuilder userInformation = new();

            userInformation.AppendLine(PrintService.GenerateTable(users.InsertInto2DList(["Sr. No", "Name", "Email"],
                [
                    user => users.IndexOf(user)+1,
                    user => user.Name,
                    user => user.Email,
                ])));

            return userInformation.ToString();
        }

        private static List<UserModel> GetAvailableUsers()
        {
            return _userService.GetInsensitiveInformationOfUser();
        }

        public void ShowSharedCalendars()
        {

            sharedCalendarsList = _calendarSharingService.GetSharedCalendars();

            try
            {
                string sharedCalendarsTable = GetSharedCalendarTableAsString();

                if (sharedCalendarsTable.Length > 0)
                {
                    PrintHandler.PrintInfoMessage("Calendars shared to you !");
                    Console.WriteLine(sharedCalendarsTable);
                }
                else
                {
                    PrintHandler.PrintWarningMessage("No calendars available !");
                    return;
                }

                PerformOperationToShowSpecificCalendar();
            }
            catch
            {
                PrintHandler.PrintErrorMessage("Some error occurred !");
            }
        }

        public string GetSharedCalendarTableAsString()
        {
            List<SharedCalendarModel> sharedCalendars = _calendarSharingService.GetSharedCalendars();

            StringBuilder sharedCalendarsTable = new();

            if (sharedCalendars.Count == 0) return "";

            List<List<string>> sharedCalendarsTableContent = sharedCalendars.InsertInto2DList(["Sr. NO", "Shared by", "From", "To"],
            [
                sharedCalendar => sharedCalendars.IndexOf(sharedCalendar) + 1,
                sharedCalendar => sharedCalendar.SenderUser.Name,
                sharedCalendar => sharedCalendar.FromDate,
                sharedCalendar => sharedCalendar.ToDate
            ]);

            sharedCalendarsTable.Append(PrintService.GenerateTable(sharedCalendarsTableContent));

            return sharedCalendarsTable.ToString();
        }

        private static string GetUserName(int userId)
        {
            UserRepository userService = new();

            User? user = userService.GetById(data => new User(data), userId);

            return user != null ? user.Name : "-";
        }

        private void PerformOperationToShowSpecificCalendar()
        {
            int serialNumberOfSharedCalendar = GetInputToShowSpecificCalendar();

            int sharedCalendarId = sharedCalendarsList[serialNumberOfSharedCalendar - 1].Id;

            PrintSpecificCalendar(sharedCalendarId);

            if (sharedEventsOfSpecificCalendar.Count == 0) return;

            if (IsWantToCollaborate()) SharedEventCollaboration.GetInputToCollaborateInEvent(sharedEventsOfSpecificCalendar);
        }

        private static bool IsWantToCollaborate()
        {
            Console.WriteLine("\nAre you want to collaborate on any event ? \n1.Yes \n2.No");
            int choice = ValidatedInputProvider.GetValidIntegerBetweenRange("\nEnter choice : ", 1, 2);

            return choice == 1;
        }

        private int GetInputToShowSpecificCalendar()
        {
            string inputMessage = "Select Sr No. which calendar you want to see :- ";

            int serialNumberOfSharedCalendar = ValidatedInputProvider.GetValidIntegerBetweenRange(inputMessage, 1, sharedCalendarsList.Count);

            return serialNumberOfSharedCalendar;
        }

        public void PrintSpecificCalendar(int sharedCalendarId)
        {
            sharedEventsOfSpecificCalendar = _calendarSharingService.GetSharedEventsFromSharedCalendarId(sharedCalendarId);

            SharedCalendarModel? sharedCalendar = new CalendarSharingService().GetSharedCalendarById(sharedCalendarId);

            if (sharedCalendar == null) return;

            string sharedEventsTable = GenerateTableForSharedEvents(sharedCalendar.FromDate, sharedCalendar.ToDate, sharedEventsOfSpecificCalendar);

            Console.WriteLine(sharedEventsTable);
        }

        private static string GenerateTableForSharedEvents(DateOnly startDate, DateOnly endDate, List<EventModel> sharedEvents)
        {
            List<List<string>> sharedEventTableContent = [["Sr No.", "Event Title", "Event Description", "Event Date", "Event Duration"]];

            int index = 0;

            DateOnly currentDate = startDate;

            while (currentDate <= endDate)
            {
                AddSharedEventsInto2DList(sharedEvents, sharedEventTableContent, ref index, currentDate);

                currentDate = currentDate.AddDays(1);
            }

            return PrintService.GenerateTable(sharedEventTableContent);
        }

        private static void AddSharedEventsInto2DList(List<EventModel> sharedEvents, List<List<string>> sharedEventTableContent, ref int index, DateOnly currentDate)
        {
            sharedEvents = sharedEvents.Where(sharedEvent => sharedEvent.EventDate == currentDate).ToList();

            foreach (var sharedEvent in sharedEvents)
            {

                sharedEventTableContent.Add([(index + 1).ToString(),
                                                     sharedEvent.Title, sharedEvent.Description,
                                                     sharedEvent.EventDate.ToString(),
                                                     DateTimeManager.ConvertTo12HourFormat(sharedEvent.Duration.StartHour)+" - "+
                                                     DateTimeManager.ConvertTo12HourFormat(sharedEvent.Duration. EndHour)
                                            ]);
                index++;
            }
        }
    }
}