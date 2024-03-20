using System.Data;
using System.Text;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Enums;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.ConsoleApp
{
    internal static class EventHandling
    {
        private readonly static EventService _eventService = new();

        private readonly static ShareCalendar _shareCalendar = new();

        private readonly static OverlappingEventService _overlappingEventService = new();

        private static readonly Dictionary<EventOperation, Action> operationDictionary = new()
        {{ EventOperation.Add, GetInputToAddEvent },
        { EventOperation.Display, DisplayEvents },
        { EventOperation.Delete, DeleteEvent },
        { EventOperation.Update, GetInputToUpdateEvent },
        { EventOperation.View, CalendarView.ViewSelection },
        { EventOperation.ShareCalendar, _shareCalendar.GetDetailsToShareCalendar },
        { EventOperation.ViewSharedCalendar, _shareCalendar.ViewSharedCalendars },
        { EventOperation.SharedEventCollaboration, SharedEventCollaboration.ShowSharedEvents },
        { EventOperation.EventWithMultipleInvitees, GetInputForProposedEvent },
        { EventOperation.GiveResponseToProposedEvent, ProposedEventResponseHandler.ShowProposedEvents}};

        public static void PrintColorMessage(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        public static void ShowAllChoices()
        {
            Dictionary<string, ConsoleColor> operationWithColor = new()
            {
                { "1. Add Event",ConsoleColor.Green},{ "2. See all Events",ConsoleColor.Blue },
                { "3. Delete Event",ConsoleColor.Red },{ "4. Update Event" , ConsoleColor.White } ,
                { "5. See calendar view",ConsoleColor.Yellow} ,{ "6. Share calendar" , ConsoleColor.Cyan } ,
                { "7. View Shared calendar" , ConsoleColor.Magenta } ,
                { "8. Collaborate from shared calendar" , ConsoleColor.DarkGreen} ,
                { "9. Add event with multiple invitees" , ConsoleColor.DarkGray } ,
                { "10. Give response to proposed events" , ConsoleColor.DarkCyan} , {"0. Back" , ConsoleColor.Gray }
            };

            PrintHandler.PrintNewLine();

            foreach (var (key, value) in operationWithColor.Select(operation => (operation.Key, operation.Value)))
            {
                PrintColorMessage(key, value);
            }

            Console.Write("\nSelect Any Option :- ");
        }

        public static void AskForChoice()
        {
            int choice = GetValidatedChoice();

            EventOperation option = (EventOperation)choice;

            if (operationDictionary.TryGetValue(option, out Action? actionMethod))
            {
                actionMethod.Invoke();
                AskForChoice();
            }
            else if (option == EventOperation.Back)
            {
                Console.WriteLine("Going Back ...");
            }
            else
            {
                Console.WriteLine("Oops! Wrong choice");
                AskForChoice();
            }
        }

        public static int GetValidatedChoice()
        {
            ShowAllChoices();

            string inputFromConsole = Console.ReadLine();
            int choice;

            while (!ValidationService.IsValidateInput(inputFromConsole, out choice, int.TryParse))
            {
                ShowAllChoices();

                inputFromConsole = Console.ReadLine();
            }
            return choice;
        }

        public static void GetInputForProposedEvent()
        {
            try
            {
                if (GetInsensitiveUserInformationList().Count == 0)
                {
                    PrintHandler.PrintWarningMessage("No invitees are available !");
                    return;
                }

                Event eventObj = new();

                GetEventDetailsFromUser(eventObj);

                GetStartingAndEndingHourOfEvent(eventObj);

                DateTime proposedDate = ValidatedInputProvider.GetValidatedDateTime("Enter date for the proposed event (Enter " +
                                                                                  "date in dd-MM-yyyy) :- ");

                eventObj.EventStartDate = DateOnly.FromDateTime(proposedDate);

                eventObj.EventEndDate = DateOnly.FromDateTime(proposedDate);

                eventObj.UserId = GlobalData.GetUser().Id;

                string invitees = GetInviteesFromUser();

                eventObj.IsProposed = true;

                AddEvent(eventObj);

                MultipleInviteesEventService.AddInviteesInProposedEvent(eventObj, invitees);
            }
            catch
            {
                PrintHandler.PrintErrorMessage("Some error occurred ! Proposed event addition failed");
            }
        }

        public static T Cast<T>(object obj, T type) { return (T)obj; }

        public static void HandleOverlappedEvent(Event eventForVerify, Object overlappedEventInformationObject, bool isInsert)
        {
            var overlappedEventInformation = Cast(overlappedEventInformationObject, new { OverlappedEvent = new Event(), MatchedDate = new DateOnly() });

            string message = GetOverlapMessageFromEvents(eventForVerify, overlappedEventInformation.OverlappedEvent, overlappedEventInformation.MatchedDate);

            PrintHandler.PrintWarningMessage(message);

            if (!AskForRescheduleOverlappedEvent(eventForVerify)) return;

            if (isInsert) AddEvent(eventForVerify);
            else UpdateEvent(eventForVerify.Id, eventForVerify);
        }

        public static bool AskForRescheduleOverlappedEvent(Event eventObj)
        {
            Console.WriteLine("\nAre you want to reschedule event ? \n1. Yes \n2. No");

            int choice = ValidatedInputProvider.GetValidatedIntegerBetweenRange("\nEnter choice : ", 1, 2);

            switch (choice)
            {
                case 1:
                    GetStartingAndEndingHourOfEvent(eventObj);
                    if (eventObj.IsProposed) RecurrenceHandling.GetRecurrenceForSingleEvent(eventObj);
                    return true;
                case 2:
                    return false;
            }
            return false;
        }

        public static string GetOverlapMessageFromEvents(Event eventForVerify, Event eventToCheckOverlap, DateOnly matchedDate)
        {
            return $"\"{eventForVerify.Title}\" overlaps with \"{eventToCheckOverlap.Title}\" at {matchedDate} on following duration\n" +
                   $"1. {DateTimeManager.ConvertTo12HourFormat(eventForVerify.EventStartHour)} " +
                   $"- {DateTimeManager.ConvertTo12HourFormat(eventForVerify.EventEndHour)} \n" +
                   $"overlaps with " +
                   $"\n2. {DateTimeManager.ConvertTo12HourFormat(eventToCheckOverlap.EventStartHour)} " +
                   $"- {DateTimeManager.ConvertTo12HourFormat(eventToCheckOverlap.EventEndHour)} \n" +
                   $"\nPlease choose another date time !";
        }

        public static bool ShowAllUser()
        {
            List<User> users = GetInsensitiveUserInformationList();

            StringBuilder userInformation = new();

            if (users.Count != 0)
            {
                List<List<string>> userTableContent = [["Sr. No", "Name", "Email"]];

                foreach (var (user, index) in users.Select((user, index) => (user, index)))
                {
                    userTableContent.Add([(index + 1).ToString(), user.Name, user.Email]);
                }

                userInformation.AppendLine(PrintService.GenerateTable(userTableContent));

                Console.WriteLine(userInformation);
            }
            else
            {
                Console.WriteLine("No Users are available!");
            }
            return users.Count > 0;

        }

        public static List<User> GetInsensitiveUserInformationList()
        {
            UserService userService = new();
            List<User> users = userService.GetInsensitiveInformationOfUser();

            return users;
        }

        public static string GetInviteesFromUser()
        {
            bool isUsersAvailable = ShowAllUser();

            if (!isUsersAvailable) return "";

            string inviteesSerialNumber = ValidatedInputProvider.GetValidatedCommaSeparatedInputInRange("Enter users you want to Invite. (Enter users Sr No. comma separated Ex:- 1,2,3) : ", 1, GetInsensitiveUserInformationList().Count);

            return GetInviteesUserIdFromSerialNumber(inviteesSerialNumber);
        }

        private static string GetInviteesUserIdFromSerialNumber(string inviteesSerialNumber)
        {
            List<User> users = GetInsensitiveUserInformationList();

            StringBuilder invitees = new();

            foreach (int inviteeSerialNumber in inviteesSerialNumber.Split(",").Select(number => Convert.ToInt32(number.Trim())))
            {
                invitees.Append(users[inviteeSerialNumber - 1].Id + ",");
            }

            return invitees.ToString()[..(invitees.Length - 1)];
        }

        public static void GetEventDetailsFromUser(Event eventObj)
        {
            Console.WriteLine("\nFill Details Related to Event : ");

            Console.Write("Enter title : ");
            eventObj.Title = Console.ReadLine();

            PrintHandler.PrintNewLine();

            Console.Write("Enter description : ");
            eventObj.Description = Console.ReadLine();

            PrintHandler.PrintNewLine();

            Console.Write("Enter Location : ");
            eventObj.Location = Console.ReadLine();

            eventObj.IsProposed = false;
        }

        public static void GetStartingAndEndingHourOfEvent(Event eventObj)
        {
            Console.WriteLine("\nHow would you like to enter the time? : ");
            Console.WriteLine("\n1.Choose 24-hour format (1 to 24 hours) \n2.Choose 12-hour format (1 to 12 hours and AM/PM)");

            int choice = ValidatedInputProvider.GetValidatedInteger("Enter choice : ");

            switch (choice)
            {
                case 1:
                    PrintHandler.PrintInfoMessage("You've selected the 24-hour format.");
                    GetHourIn24HourFormat(eventObj);
                    break;
                case 2:
                    PrintHandler.PrintInfoMessage("You've selected the 12-hour format.");
                    GetHourIn12HourFormat(eventObj);
                    break;
                default:
                    GetStartingAndEndingHourOfEvent(eventObj);
                    break;
            }

        }

        public static void GetHourIn24HourFormat(Event eventObj)
        {
            PrintHandler.PrintNewLine();

            eventObj.EventStartHour = ValidatedInputProvider.GetValidated24HourFormatTime("Enter Start Hour for the event : ");

            PrintHandler.PrintNewLine();

            eventObj.EventEndHour = ValidatedInputProvider.GetValidated24HourFormatTime("Enter End Hour for the event : ");

            PrintHandler.PrintNewLine();

            if (!ValidationService.IsValidStartAndEndHour(eventObj.EventStartHour, eventObj.EventEndHour))
            {
                PrintHandler.PrintWarningMessage("Invalid input ! Start hour must less than the end hour.");
                GetHourIn24HourFormat(eventObj);
            }
        }

        public static void GetHourIn12HourFormat(Event eventObj)
        {
            PrintHandler.PrintNewLine();

            eventObj.EventStartHour = ValidatedInputProvider.GetValidated12HourFormatTime("Enter Start Hour for the event (From 1 to 12) : ");

            string startHourAbbreviation = ValidatedInputProvider.GetValidatedAbbreviations();

            PrintHandler.PrintNewLine();

            eventObj.EventEndHour = ValidatedInputProvider.GetValidated12HourFormatTime("Enter End Hour for the event (From 1 to 12) : ");

            string endHourAbbreviation = ValidatedInputProvider.GetValidatedAbbreviations();

            eventObj.EventStartHour += startHourAbbreviation.Equals("PM") && eventObj.EventStartHour != 12 ? 12 : 0;

            eventObj.EventEndHour += endHourAbbreviation.Equals("PM") && eventObj.EventEndHour != 12 ? 12 : 0;

            PrintHandler.PrintNewLine();

            if (!ValidationService.IsValidStartAndEndHour(eventObj.EventStartHour, eventObj.EventEndHour))
            {
                PrintHandler.PrintWarningMessage("Invalid input ! Start hour must less than the end hour.");
                GetHourIn12HourFormat(eventObj);
            }
        }


        public static void GetInputToAddEvent()
        {
            Event eventObj = new();

            GetEventDetailsFromUser(eventObj);

            GetStartingAndEndingHourOfEvent(eventObj);

            eventObj.UserId = GlobalData.GetUser().Id;

            RecurrenceHandling.AskForRecurrenceChoice(eventObj);

            AddEvent(eventObj);
        }

        private static void AddEvent(Event eventObj)
        {
            try
            {
                if (IsOverlappingEvent(eventObj, true)) return;

                eventObj.Id = _eventService.InsertEvent(eventObj);

                PrintHandler.PrintSuccessMessage("Event Added Successfully");

            }
            catch
            {
                PrintHandler.PrintErrorMessage("Oops Some error occurred !");
            }
        }

        private static bool IsOverlappingEvent(Event eventObj, bool isInsert)
        {
            var overlappedEventInformationObject = _overlappingEventService.GetOverlappedEventInformation(eventObj, isInsert);

            if (overlappedEventInformationObject != null)
            {
                HandleOverlappedEvent(eventObj, overlappedEventInformationObject, isInsert);
                return true;
            }
            return false;
        }

        public static void DisplayEvents()
        {
            string events = _eventService.GenerateEventTable();
            Console.WriteLine(events.Length == 0 ? "No events available !\n" : events);
        }

        public static bool IsEventsPresent()
        {
            string events = _eventService.GenerateEventTable();
            return events.Length > 0;
        }

        public static void DeleteEvent()
        {
            try
            {
                DisplayEvents();

                if (!IsEventsPresent()) return;

                int serialNumber = GetSerialNumberForUpdateOrDelete(true);

                _eventService.DeleteEvent(_eventService.GetEventIdFromSerialNumber(serialNumber));

                PrintHandler.PrintSuccessMessage("Event deleted Successfully");
            }
            catch
            {
                PrintHandler.PrintErrorMessage("Oops ! Some error occurred!");
            }

        }

        public static void GetInputToUpdateEvent()
        {

            try
            {
                DisplayEvents();

                if (!IsEventsPresent()) return;

                int serialNumber = GetSerialNumberForUpdateOrDelete(false);

                Event eventObj = _eventService.GetEventById(_eventService.GetEventIdFromSerialNumber(serialNumber));

                if (eventObj == null) return;

                if (eventObj.IsProposed)
                {
                    GetInputForProposedEvent();
                    return;
                }

                AskForItemsToUpdate(eventObj);

                eventObj.UserId = GlobalData.GetUser().Id;

                UpdateEvent(eventObj.Id, eventObj);
            }
            catch
            {
                PrintHandler.PrintErrorMessage("Oops ! Some error occurred !");
            }
        }

        private static void UpdateEvent(int id, Event eventObj)
        {
            try
            {
                if (IsOverlappingEvent(eventObj, false)) return;

                bool isUpdated = _eventService.UpdateEvent(eventObj, id);

                if (isUpdated) PrintHandler.PrintSuccessMessage("Event Updated Successfully");
            }
            catch
            {
                PrintHandler.PrintErrorMessage("Oops ! Some error occurred !");
            }
        }

        public static void AskForItemsToUpdate(Event eventObj)
        {
            string inputMessage = "\nWhat items you want to update ? \n1. Event Details (Event Title , Event Description , Event Location)" +
                                  "\n2. Event repetition details (Event dates, Event Duration, Event frequency etc ...)";

            Console.WriteLine(inputMessage);

            int choice = ValidatedInputProvider.GetValidatedIntegerBetweenRange("\nEnter choice : ", 1, 2);

            switch (choice)
            {
                case 1:
                    GetEventDetailsFromUser(eventObj);
                    break;
                case 2:
                    GetStartingAndEndingHourOfEvent(eventObj);
                    RecurrenceHandling.AskForRecurrenceChoice(eventObj);
                    break;

            }

        }

        public static int GetSerialNumberForUpdateOrDelete(bool isDelete)
        {
            string operation = isDelete ? "delete" : "update";

            int serialNumber = ValidatedInputProvider.GetValidatedIntegerBetweenRange($"From Above events give event no. that you want to {operation} :- ", 1, _eventService.GetTotalEventCount());

            return serialNumber;
        }

    }
}