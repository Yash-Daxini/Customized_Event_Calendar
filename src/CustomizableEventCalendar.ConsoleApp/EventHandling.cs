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
        {
            { EventOperation.Add, GetInputToAddEvent },
            { EventOperation.Display, DisplayEvents },
            { EventOperation.Delete, DeleteEvent },
            { EventOperation.Update, GetInputToUpdateEvent },
            { EventOperation.View, CalendarView.ChooseView },
            { EventOperation.ShareCalendar, _shareCalendar.GetDetailsToShareCalendar },
            { EventOperation.ViewSharedCalendar, _shareCalendar.ShowSharedCalendars },
            { EventOperation.EventWithMultipleInvitees, () => GetInputForProposedEvent(null) },
            { EventOperation.GiveResponseToProposedEvent, ProposedEventResponseHandler.ShowProposedEvents},
            { EventOperation.EventsTimeline , PrintEventWithTimeline}
        };

        public static void PrintColorMessage(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        private static void ShowAllChoices()
        {
            Dictionary<string, ConsoleColor> operationWithColor = new()
            {
                { "1. Add Event",ConsoleColor.Green},{ "2. Events Overview",ConsoleColor.Blue },
                { "3. Delete Event",ConsoleColor.Red },{ "4. Update Event" , ConsoleColor.White } ,
                { "5. See calendar view",ConsoleColor.Yellow} ,{ "6. Share calendar" , ConsoleColor.Cyan } ,
                { "7. View Shared calendar" , ConsoleColor.Magenta } ,
                { "8. Add event with multiple invitees" , ConsoleColor.DarkGray } ,
                { "9. Give response to proposed events" , ConsoleColor.DarkCyan} ,
                { "10. See Events time line",ConsoleColor.DarkGreen},
                { "0.  Back" , ConsoleColor.Gray }
            };

            PrintHandler.PrintNewLine();

            foreach (var (key, value) in operationWithColor.Select(operation => (operation.Key, operation.Value)))
            {
                PrintColorMessage(key, value);
            }
        }

        public static void AskForChoice()
        {
            try
            {
                ShowAllChoices();

                int choice = ValidatedInputProvider.GetValidatedIntegerBetweenRange("\nEnter choice :- ", 0, 10);

                EventOperation option = (EventOperation)choice;

                if (operationDictionary.TryGetValue(option, out Action? actionMethod))
                {
                    actionMethod.Invoke();
                }

                if (option != EventOperation.Back)
                {
                    AskForChoice();
                }
            }
            catch (Exception ex)
            {
                PrintHandler.PrintErrorMessage(ex.Message);
                AskForChoice();
            }
        }

        private static void GetInputForProposedEvent(Event? eventObj)
        {
            if (GetInsensitiveUserInformationList().Count == 0)
            {
                PrintHandler.PrintWarningMessage("No invitees are available !");
                return;
            }

            eventObj ??= new();

            GetEventDetailsFromUser(eventObj);

            GetStartingAndEndingHourOfEvent(eventObj);

            DateTime proposedDate = ValidatedInputProvider.GetValidatedDateTime("Enter date for the proposed event (Enter " +
                                                                              "date in dd-MM-yyyy) :- ");

            eventObj.EventStartDate = DateOnly.FromDateTime(proposedDate);

            eventObj.EventEndDate = DateOnly.FromDateTime(proposedDate);

            eventObj.UserId = GlobalData.GetUser().Id;

            string invitees = GetInviteesFromUser();

            eventObj.IsProposed = true;

            if (eventObj.Id > 0)
                UpdateEvent(eventObj.Id, eventObj);
            else
                AddEvent(eventObj);

            MultipleInviteesEventService.AddInviteesInProposedEvent(eventObj, invitees);
        }

        private static T CastAnonymousObject<T>(object obj, T type) { return (T)obj; }

        private static void HandleOverlappedEvent(Event eventForVerify, Object overlappedEventInformationObject, bool isInsert)
        {
            var overlappedEventInformation = CastAnonymousObject(overlappedEventInformationObject, new { OverlappedEvent = new Event(), MatchedDate = new DateOnly() });

            string overlapEventMessage = GetOverlapMessageFromEvents(eventForVerify, overlappedEventInformation.OverlappedEvent, overlappedEventInformation.MatchedDate);

            PrintHandler.PrintWarningMessage(overlapEventMessage);

            if (!AskForRescheduleOverlappedEvent(eventForVerify)) return;

            if (isInsert) AddEvent(eventForVerify);
            else UpdateEvent(eventForVerify.Id, eventForVerify);
        }

        private static bool AskForRescheduleOverlappedEvent(Event eventObj)
        {
            Console.WriteLine("\nAre you want to reschedule event ? \n1. Yes \n2. No");

            int choice = ValidatedInputProvider.GetValidatedIntegerBetweenRange("\nEnter choice : ", 1, 2);

            switch (choice)
            {
                case 1:
                    GetStartingAndEndingHourOfEvent(eventObj);
                    if (eventObj.IsProposed) RecurrenceHandling.GetRecurrenceForSingleEvent(eventObj);
                    else RecurrenceHandling.AskForRecurrenceChoice(eventObj);
                    return true;
                case 2:
                    return false;
            }
            return false;
        }

        private static string GetOverlapMessageFromEvents(Event eventForVerify, Event eventToCheckOverlap, DateOnly matchedDate)
        {
            return $"\"{eventForVerify.Title}\" overlaps with \"{eventToCheckOverlap.Title}\" at {matchedDate} on following duration\n" +
                   $"1. {DateTimeManager.ConvertTo12HourFormat(eventForVerify.EventStartHour)} " +
                   $"- {DateTimeManager.ConvertTo12HourFormat(eventForVerify.EventEndHour)} \n" +
                   $"overlaps with " +
                   $"\n2. {DateTimeManager.ConvertTo12HourFormat(eventToCheckOverlap.EventStartHour)} " +
                   $"- {DateTimeManager.ConvertTo12HourFormat(eventToCheckOverlap.EventEndHour)} \n" +
                   $"\nPlease choose another date time !";
        }

        private static bool ShowAllUser()
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

        private static List<User> GetInsensitiveUserInformationList()
        {
            UserService userService = new();
            List<User> users = userService.GetInsensitiveInformationOfUser();

            return users;
        }

        private static string GetInviteesFromUser()
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

        private static DateOnly GetDate(string inputMessage)
        {
            return ValidatedInputProvider.GetValidatedDateOnly(inputMessage);
        }

        private static void GetDatesToPrintEventWithTimeline(out DateOnly startDate, out DateOnly endDate)
        {
            startDate = GetDate("Please enter start date from you want to see events : ");
            endDate = GetDate("Please enter end date from you want to see events : ");
        }

        private static void PrintEventWithTimeline()
        {

            GetDatesToPrintEventWithTimeline(out DateOnly startDate, out DateOnly endDate);

            EventCollaboratorService eventCollaboratorService = new();
            List<EventCollaborator> eventCollaborators = eventCollaboratorService.GetAllEventCollaborators()
                                                                                 .FindAll(eventCollaborator => eventCollaborator.UserId
                                                                                  == GlobalData.GetUser().Id);
            List<Event> events = _eventService.GetAllEvents();

            List<List<string>> tableContentOfEventTimeLine = [["Date", "Day", "Event Name", "Start Time", "End Time"]];

            Dictionary<DateOnly, List<EventCollaborator>> dateWiseEventCollaborators = GetDateWiseEventCollaborators(startDate, endDate, eventCollaborators);

            foreach (var date in dateWiseEventCollaborators.Keys)
            {
                foreach (var eventCollaborator in dateWiseEventCollaborators[date])
                {
                    Event? eventObj = events.Find(eventObj => eventObj.Id == eventCollaborator.EventId);

                    if (eventObj == null) continue;

                    tableContentOfEventTimeLine.Add([date.ToString(), DateTimeManager.GetDayFromDateOnly(date),
                                                     eventObj.Title, eventObj.EventStartHour.ToString(),
                                                     eventObj.EventEndHour.ToString()]);
                }
                if (dateWiseEventCollaborators[date].Count == 0) tableContentOfEventTimeLine.Add([date.ToString(),
                                                                                                  DateTimeManager.GetDayFromDateOnly(date),
                                                                                                  "-", "-", "-"]);
            }

            Console.WriteLine("\n" + PrintService.GenerateTable(tableContentOfEventTimeLine));

        }

        private static Dictionary<DateOnly, List<EventCollaborator>> GetDateWiseEventCollaborators(DateOnly startDate, DateOnly endDate, List<EventCollaborator> eventCollaborators)
        {
            Dictionary<DateOnly, List<EventCollaborator>> dateWiseEventCollaborators = [];

            DateOnly currentDate = startDate;

            while (currentDate <= endDate)
            {
                List<EventCollaborator> eventCollaboratorBetweenGivenRange = eventCollaborators.FindAll(eventCollaborator =>
                                                                             IsDateInRange(startDate, endDate, eventCollaborator.EventDate) &&
                                                                             currentDate == eventCollaborator.EventDate);

                dateWiseEventCollaborators[currentDate] = eventCollaboratorBetweenGivenRange;

                currentDate = currentDate.AddDays(1);
            }

            return dateWiseEventCollaborators;
        }

        private static bool IsDateInRange(DateOnly startDate, DateOnly endDate, DateOnly dateToCheck)
        {
            return dateToCheck >= startDate && dateToCheck <= endDate;
        }

        private static void GetEventDetailsFromUser(Event eventObj)
        {
            Console.WriteLine("\nFill Details Related to Event : ");

            eventObj.Title = ValidatedInputProvider.GetValidatedString("Enter title : ");

            PrintHandler.PrintNewLine();

            eventObj.Description = ValidatedInputProvider.GetValidatedString("Enter description : ");

            PrintHandler.PrintNewLine();

            eventObj.Location = ValidatedInputProvider.GetValidatedString("Enter Location : ");

            eventObj.IsProposed = false;
        }

        private static void GetStartingAndEndingHourOfEvent(Event eventObj)
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

        private static void GetHourIn24HourFormat(Event eventObj)
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

        private static void GetHourIn12HourFormat(Event eventObj)
        {
            PrintHandler.PrintNewLine();

            eventObj.EventStartHour = ValidatedInputProvider.GetValidated12HourFormatTime("Enter Start Hour for the event (From 1 to 12) : ");

            string startHourAbbreviation = GetChoiceOfAbbreviation();

            PrintHandler.PrintNewLine();

            eventObj.EventEndHour = ValidatedInputProvider.GetValidated12HourFormatTime("Enter End Hour for the event (From 1 to 12) : ");

            string endHourAbbreviation = GetChoiceOfAbbreviation();

            eventObj.EventStartHour += startHourAbbreviation.Equals("PM") && eventObj.EventStartHour != 12 ? 12 : 0;

            eventObj.EventEndHour += endHourAbbreviation.Equals("PM") && eventObj.EventEndHour != 12 ? 12 : 0;

            PrintHandler.PrintNewLine();

            if (!ValidationService.IsValidStartAndEndHour(eventObj.EventStartHour, eventObj.EventEndHour))
            {
                PrintHandler.PrintWarningMessage("Invalid input ! Start hour must less than the end hour.");
                GetHourIn12HourFormat(eventObj);
            }
        }

        private static string GetChoiceOfAbbreviation()
        {
            Console.WriteLine("Enter choice for AM or PM \n1. AM \n2. PM");
            int choice = ValidatedInputProvider.GetValidatedIntegerBetweenRange("Enter choice : ", 1, 2);

            return choice == 1 ? "AM" : "PM";
        }


        private static void GetInputToAddEvent()
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

        private static void DisplayEvents()
        {
            string events = GetEventTable();
            Console.WriteLine(events.Length == 0 ? "No events available !\n" : events);
        }

        private static bool IsEventsPresent()
        {
            string events = GetEventTable();
            return events.Length > 0;
        }

        private static string GetEventTable()
        {
            List<Event> events = _eventService.GetAllEventsOfLoggedInUser();

            List<List<string>> outputRows = events.InsertInto2DList(["Event NO.", "Title", "Description", "Location", "Event Repetition", "Start Date", "End Date", "Duration"],
                [
                    eventObj => events.IndexOf(eventObj) + 1,
                    eventObj => eventObj.Title,
                    eventObj => eventObj.Description,
                    eventObj => eventObj.Location,
                    eventObj => RecurrencePatternMessageGenerator.GenerateRecurrenceMessage(eventObj),
                    eventObj => eventObj.EventStartDate.ToString(),
                    eventObj => eventObj.EventEndDate.ToString(),
                    eventObj => DateTimeManager.ConvertTo12HourFormat(eventObj.EventStartHour)+" - "+
                                DateTimeManager.ConvertTo12HourFormat(eventObj.EventEndHour)
                ]);

            string eventTable = PrintService.GenerateTable(outputRows);

            if (events.Count > 0)
            {
                return eventTable;
            }

            return "";
        }

        private static void DeleteEvent()
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

        private static void GetInputToUpdateEvent()
        {

            DisplayEvents();

            if (!IsEventsPresent()) return;

            int serialNumber = GetSerialNumberForUpdateOrDelete(false);

            Event eventObj = _eventService.GetEventById(_eventService.GetEventIdFromSerialNumber(serialNumber));

            if (eventObj == null) return;

            if (eventObj.IsProposed)
            {
                GetInputForProposedEvent(eventObj);
                return;
            }

            AskForItemsToUpdate(eventObj);

            eventObj.UserId = GlobalData.GetUser().Id;

            UpdateEvent(eventObj.Id, eventObj);
        }

        private static void UpdateEvent(int id, Event eventObj)
        {
            try
            {
                if (IsOverlappingEvent(eventObj, false)) return;

                _eventService.UpdateEvent(eventObj, id);

                PrintHandler.PrintSuccessMessage("Event Updated Successfully");
            }
            catch
            {
                PrintHandler.PrintErrorMessage("Oops ! Some error occurred !");
            }
        }

        private static void AskForItemsToUpdate(Event eventObj)
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

        private static int GetSerialNumberForUpdateOrDelete(bool isDelete)
        {
            string operation = isDelete ? "delete" : "update";

            int serialNumber = ValidatedInputProvider.GetValidatedIntegerBetweenRange($"From Above events give event no. that you want to {operation} :- ", 1, _eventService.GetTotalEventCount());

            return serialNumber;
        }

    }
}