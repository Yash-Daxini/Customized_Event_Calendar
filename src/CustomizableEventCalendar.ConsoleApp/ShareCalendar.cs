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
        public void GetDetailsToShareCalendar()
        {
            try
            {
                List<User> availableUsersToShareCalendar = GetAvailableUsers();

                int availalbeUserCountToShareCalendar = availableUsersToShareCalendar.Count;

                if (availalbeUserCountToShareCalendar <= 0) return;

                ShowAllUserTable(availableUsersToShareCalendar);

                string inputMessage = "Enter Sr No. whom you want to share calendar :- ";

                int serialNumberOfTableRow = ValidatedInputProvider.GetValidatedIntegerBetweenRange(inputMessage, 1,
                                             availalbeUserCountToShareCalendar);

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

        private static void ShowAllUserTable(List<User> users)
        {
            if (users.Count == 0)
            {
                Console.WriteLine("No users are available !");
                return;
            }

            StringBuilder userInformation = new();

            userInformation.AppendLine(PrintService.GenerateTable(users.InsertInto2DList(["Sr. No", "Name", "Email"],
                [
                    user => users.IndexOf(user)+1,
                    user => user.Name,
                    user => user.Email,
                ])));

            Console.WriteLine(userInformation);
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
                string sharedEventsTable = DesignSharedCalendarTable();

                if (sharedCalendarsList.Count > 0)
                {
                    PrintHandler.PrintInfoMessage("Calendars shared to you !");
                    Console.WriteLine(sharedEventsTable);
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

        private void PerformOperationToShowSpecificCalendar()
        {
            int serialNumberOfSharedCalendar = GetInputToShowSpecificCalendar();

            int sharedCalendarId = sharedCalendarsList[serialNumberOfSharedCalendar - 1].Id;

            ShowSpecificCalendar(sharedCalendarId);

            List<EventCollaborator> sharedEvents = _calendarSharingService.GetSharedEventsFromSharedCalendarId(sharedCalendarId);

            if (sharedEvents.Count == 0) return;

            if (IsWantToCollaborate()) SharedEventCollaboration.GetInputToEventCollaboration(sharedEvents);
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

        public void ShowSpecificCalendar(int sharedCalendarId)
        {
            List<EventCollaborator> sharedEvents = _calendarSharingService.GetSharedEventsFromSharedCalendarId(sharedCalendarId);

            CalendarSharingService calendarSharingService = new();

            SharedCalendar? sharedCalendar = calendarSharingService.GetSharedCalendarById(sharedCalendarId);

            if (sharedCalendar == null) return;

            string sharedEventsTable = GenerateTableForSharedEvents(sharedCalendar.FromDate, sharedCalendar.ToDate, sharedEvents);

            Console.WriteLine(sharedEventsTable);
        }

        public string DesignSharedCalendarTable()
        {
            List<SharedCalendar> sharedCalendars = _calendarSharingService.GetSharedCalendars();

            StringBuilder sharedEventsDisplayString = new();

            List<List<string>> sharedEventsTableContent = sharedCalendars.InsertInto2DList(["Sr. NO", "Shared by", "From", "To"],
            [
                sharedCalendar => sharedCalendars.IndexOf(sharedCalendar) + 1,
                sharedCalendar => GetUserName(sharedCalendar.SenderUserId),
                sharedCalendar => sharedCalendar.FromDate,
                sharedCalendar => sharedCalendar.ToDate
            ]);

            sharedEventsDisplayString.Append(PrintService.GenerateTable(sharedEventsTableContent));

            return sharedEventsDisplayString.ToString();
        }

        private static string GetUserName(int userId)
        {
            UserRepository userService = new();

            User? user = userService.GetById(data => new User(data), userId);

            return user != null ? user.Name : "-";
        }

        private static string GenerateTableForSharedEvents(DateOnly startDate, DateOnly endDate, List<EventCollaborator> sharedEvents)
        {
            List<Event> events = new EventService().GetAllEvents();

            List<List<string>> sharedEventTableContent = [["Sr No.", "Event Title", "Event Description", "Event Date", "Event Duration"]];

            int index = 0;

            DateOnly currentDate = startDate;

            while (currentDate <= endDate)
            {
                AddEventsOf(sharedEvents, events, sharedEventTableContent, ref index, currentDate);

                currentDate = currentDate.AddDays(1);
            }

            return PrintService.GenerateTable(sharedEventTableContent);
        }

        private static void AddEventsOf(List<EventCollaborator> sharedEvents, List<Event> events, List<List<string>> sharedEventTableContent, ref int index, DateOnly currentDate)
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