using System.Text;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Data.Repositories;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.ConsoleApp
{
    internal class ShareCalendar
    {
        private readonly CalendarSharingService _calendarSharingService = new();

        private readonly static UserService _userService = new();

        public List<SharedCalendar> sharedCalendarsList = [];

        public List<EventCollaborator> sharedEventsOfSpecificCalendar = [];

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

                int serialNumberOfTableRow = ValidatedInputProvider.GetValidatedIntegerBetweenRange(inputMessage, 1, GetAvailableUsers().Count);

                User user = GetUserFromSerialNumber(serialNumberOfTableRow);

                SharedCalendar sharedCalendar = new(user.Id, GlobalData.GetUser().Id, new DateOnly(), new DateOnly());

                GetDatesFromUser(sharedCalendar);

                AddSharedCalendar(user, sharedCalendar);

            }
            catch
            {
                PrintHandler.PrintErrorMessage("Some error occurred !");
            }

        }

        private void AddSharedCalendar(User user, SharedCalendar sharedCalendar)
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

        private static void GetDatesFromUser(SharedCalendar sharedCalendar)
        {
            Console.WriteLine("Enter Time Range for which you want to share events");

            PrintHandler.PrintNewLine();

            sharedCalendar.FromDate = ValidatedInputProvider.GetValidatedDateOnly("Enter start date in dd-MM-yyyy format :-  ");

            PrintHandler.PrintNewLine();

            sharedCalendar.ToDate = ValidatedInputProvider.GetValidatedDateOnly("Enter end date in dd-MM-yyyy format :-  ");

            if (!ValidationService.IsValidStartAndEndDate(sharedCalendar.FromDate, sharedCalendar.ToDate))
            {
                PrintHandler.PrintWarningMessage("Invalid input ! Start date must less than or equal to the end date ");
                GetDatesFromUser(sharedCalendar);
            }
        }

        private static User GetUserFromSerialNumber(int serialNumber)
        {
            List<User> users = _userService.GetInsensitiveInformationOfUser();
            return users[serialNumber - 1];
        }

        private static string GetUserTableAsString()
        {
            List<User> users = GetAvailableUsers();

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

        private static List<User> GetAvailableUsers()
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
            List<SharedCalendar> sharedCalendars = _calendarSharingService.GetSharedCalendars();

            StringBuilder sharedCalendarsTable = new();

            if (sharedCalendars.Count == 0) return "";

            List<List<string>> sharedCalendarsTableContent = sharedCalendars.InsertInto2DList(["Sr. NO", "Shared by", "From", "To"],
            [
                sharedCalendar => sharedCalendars.IndexOf(sharedCalendar) + 1,
                sharedCalendar => GetUserName(sharedCalendar.SenderUserId),
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
            int choice = ValidatedInputProvider.GetValidatedIntegerBetweenRange("\nEnter choice : ", 1, 2);

            return choice == 1;
        }

        private int GetInputToShowSpecificCalendar()
        {
            string inputMessage = "Select Sr No. which calendar you want to see :- ";

            int serialNumberOfSharedCalendar = ValidatedInputProvider.GetValidatedIntegerBetweenRange(inputMessage, 1, sharedCalendarsList.Count);

            return serialNumberOfSharedCalendar;
        }

        public void PrintSpecificCalendar(int sharedCalendarId)
        {
            sharedEventsOfSpecificCalendar = _calendarSharingService.GetSharedEventsFromSharedCalendarId(sharedCalendarId);

            SharedCalendar? sharedCalendar = new CalendarSharingService().GetSharedCalendarById(sharedCalendarId);

            if (sharedCalendar == null) return;

            string sharedEventsTable = GenerateTableForSharedEvents(sharedCalendar.FromDate, sharedCalendar.ToDate, sharedEventsOfSpecificCalendar);

            Console.WriteLine(sharedEventsTable);
        }

        private static string GenerateTableForSharedEvents(DateOnly startDate, DateOnly endDate, List<EventCollaborator> sharedEvents)
        {
            List<Event> events = new EventService().GetAllEvents();

            List<List<string>> sharedEventTableContent = [["Sr No.", "Event Title", "Event Description", "Event Date", "Event Duration"]];

            int index = 0;

            DateOnly currentDate = startDate;

            while (currentDate <= endDate)
            {
                AddSharedEventsInto2DList(sharedEvents, events, sharedEventTableContent, ref index, currentDate);

                currentDate = currentDate.AddDays(1);
            }

            return PrintService.GenerateTable(sharedEventTableContent);
        }

        private static void AddSharedEventsInto2DList(List<EventCollaborator> sharedEvents, List<Event> events, List<List<string>> sharedEventTableContent, ref int index, DateOnly currentDate)
        {
            List<EventCollaborator> eventCollaboratorList = sharedEvents.FindAll(eventCollaborator => eventCollaborator.EventDate == currentDate);

            foreach (var eventCollaborator in eventCollaboratorList)
            {
                if (eventCollaborator != null)
                {
                    Event? eventObj = events.Find(eventObj => eventObj.Id == eventCollaborator.EventId);

                    sharedEventTableContent.Add([(index + 1).ToString(),
                                                     eventObj.Title, eventObj.Description,
                                                     eventCollaborator.EventDate.ToString(),
                                                     DateTimeManager.ConvertTo12HourFormat(eventObj.EventStartHour)+" - "+
                                                     DateTimeManager.ConvertTo12HourFormat(eventObj.EventEndHour)
                                                ]);
                    index++;
                }
            }
        }
    }
}