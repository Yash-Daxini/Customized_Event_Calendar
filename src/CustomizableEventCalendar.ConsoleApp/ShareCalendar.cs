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

        private static int ShowAllUser()
        {
            List<User> users = _userService.GetInsensitiveInformationOfUser();

            if (users.Count > 0)
            {
                StringBuilder userInformation = new();

                userInformation.AppendLine(PrintService.GenerateTable(users.InsertInto2DList(["Sr. No", "Name", "Email"],
                    [
                        user => users.IndexOf(user)+1,
                        user => user.Name,
                        user => user.Email,
                    ])));

                Console.WriteLine(userInformation);
            }
            else
            {
                Console.WriteLine("No users are available !");
            }
            return users.Count;

        }

        public void ShowSharedCalendars()
        {

            sharedCalendarsList = _calendarSharingService.GetSharedCalendars();

            _ = sharedCalendarsList.OrderBy(sharedCalendar => sharedCalendar.ToDate);

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

                int serialNumberOfSharedCalendar = GetInputToShowSpecificCalendar();

                int sharedCalendarId = sharedCalendarsList[serialNumberOfSharedCalendar - 1].Id;

                ShowSpecificCalendar(sharedCalendarId);

                List<EventCollaborator> sharedEvents = [.._calendarSharingService.GetSharedEventsFromSharedCalendarId(sharedCalendarId)
                                                                              .OrderBy(sharedEvent => sharedEvent.EventDate)];

                if (sharedEvents.Count == 0) return;

                if (IsWantToCollaborate()) SharedEventCollaboration.GetInputToEventCollaboration(sharedEvents.Count, sharedEvents);
            }
            catch
            {
                PrintHandler.PrintErrorMessage("Some error occurred !");
            }
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

            int serialNumberOfSharedCalendar = ValidatedInputProvider.GetValidatedIntegerBetweenRange(inputMessage
                                                , 1, sharedCalendarsList.Count);

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
            EventService eventService = new();
            List<Event> events = eventService.GetAllEvents();

            List<List<string>> sharedEventTableContent = [["Sr No.", "Event Title", "Event Description", "Event Date", "Event Duration"]];

            int index = 0;

            DateOnly currentDate = startDate;

            while (currentDate <= endDate)
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

                if (eventCollaboratorList.Count == 0)
                {
                    sharedEventTableContent.Add(["-", "-", "-", currentDate.ToString(), "-"]);
                }

                currentDate = currentDate.AddDays(1);
            }

            return PrintService.GenerateTable(sharedEventTableContent);
        }
    }
}