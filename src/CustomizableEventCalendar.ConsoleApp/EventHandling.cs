using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services;
using CustomizableEventCalendar.src.CustomizableEventCalendar.ConsoleApp.InputMessageStore;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Data.Repositories;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Enums;
using Ical.Net.DataTypes;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.ConsoleApp
{
    internal static class EventHandling
    {
        private readonly static EventService _eventService = new();

        private readonly static ShareCalendar _shareCalendar = new();

        private static readonly Dictionary<EventOperation, Action> operationDictionary = new()
        {{ EventOperation.Add, TakeInputToAddEvent },
        { EventOperation.Display, DisplayEvents },
        { EventOperation.Delete, TakeInputToDeleteEvent },
        { EventOperation.Update, TakeInputToUpdateEvent },
        { EventOperation.View, CalendarView.ViewSelection },
        { EventOperation.ShareCalendar, _shareCalendar.GetDetailsToShareCalendar },
        { EventOperation.ViewSharedCalendar, _shareCalendar.ViewSharedCalendars },
        { EventOperation.SharedEventCollaboration, SharedEventCollaboration.ShowSharedEvents },
        { EventOperation.EventWithMultipleInvitees, TakeInputForProposedEvent },
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
                Authentication.AuthenticationChoice();
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

        public static void TakeInputForProposedEvent()
        {
            try
            {
                Event eventObj = new();

                GetEventDetailsFromUser(eventObj);

                DateTime proposedDate = ValidatedInputProvider.GetValidatedDateTime("Enter date for the proposed event (Enter " +
                                                                                  "date in dd-MM-yyyy) :- ");

                eventObj.EventStartDate = DateOnly.FromDateTime(proposedDate);
                eventObj.EventEndDate = DateOnly.FromDateTime(proposedDate);

                eventObj.UserId = GlobalData.GetUser().Id;

                eventObj.IsProposed = true;

                int eventId = _eventService.InsertEvent(eventObj);

                string invitees = GetInviteesFromUser();

                if (invitees.Length == 0) return;

                MultipleInviteesEventService.AddInviteesInProposedEvent(eventObj, invitees);

                PrintHandler.PrintSuccessMessage("Proposed event added successfully.");
            }
            catch
            {
                PrintHandler.PrintErrorMessage("Some error occurred ! Proposed event addition failed");
            }
        }

        public static bool ShowAllUser()
        {
            UserService userService = new();
            List<User> users = userService.GetInsensitiveInformationOfUser();

            StringBuilder userInformation = new();

            if (users.Count != 0)
            {
                List<List<string>> userTableContent = [["User Sr. No", "Name", "Email"]];

                foreach (var user in users)
                {
                    userTableContent.Add([user.Id.ToString(), user.Name, user.Email]);
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

        public static string GetInviteesFromUser()
        {
            bool isUsersAvailable = ShowAllUser();

            if (!isUsersAvailable) return "";

            string invitees = ValidatedInputProvider.GetValidatedCommaSeparatedInput("Enter users you want to Invite. " +
                                                                       "Enter users Sr No. comma separated Ex:- 1,2,3 :- ");

            return invitees;
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

            TakeStartingAndEndingHourOfEvent(eventObj);

            eventObj.IsProposed = false;
        }

        public static void TakeStartingAndEndingHourOfEvent(Event eventObj)
        {
            Console.WriteLine("\nHow would you like to enter the time? : ");
            Console.WriteLine("\n1.Choose 24-hour format (1 to 24 hours) \n2.Choose 12-hour format (1 to 12 hours and AM/PM)");

            int choice = ValidatedInputProvider.GetValidatedInteger("Enter choice : ");

            switch (choice)
            {
                case 1:
                    PrintHandler.PrintInfoMessage("You've selected the 24-hour format.");
                    TakeHourIn24HourFormat(eventObj);
                    break;
                case 2:
                    PrintHandler.PrintInfoMessage("You've selected the 12-hour format.");
                    TakeHourIn12HourFormat(eventObj);
                    break;
                default:
                    TakeStartingAndEndingHourOfEvent(eventObj);
                    break;
            }

        }

        public static void TakeHourIn24HourFormat(Event eventObj)
        {
            PrintHandler.PrintNewLine();

            eventObj.EventStartHour = ValidatedInputProvider.GetValidated24HourFormatTime("Enter Start Hour for the event : ");

            PrintHandler.PrintNewLine();

            eventObj.EventEndHour = ValidatedInputProvider.GetValidated24HourFormatTime("Enter End Hour for the event : ");

            PrintHandler.PrintNewLine();

            if (!ValidationService.IsValidStartAndEndHour(eventObj.EventStartHour, eventObj.EventEndHour))
            {
                PrintHandler.PrintWarningMessage("Invalid input ! Start hour must less than the end hour.");
                TakeHourIn24HourFormat(eventObj);
            }
        }

        public static void TakeHourIn12HourFormat(Event eventObj)
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
                TakeHourIn12HourFormat(eventObj);
            }
        }


        public static void TakeInputToAddEvent()
        {
            try
            {
                Event eventObj = new();

                GetEventDetailsFromUser(eventObj);

                eventObj.UserId = GlobalData.GetUser().Id;

                RecurrenceHandling.AskForRecurrenceChoice(eventObj);

                int eventId = _eventService.InsertEvent(eventObj);

                if (eventId != -1)
                {
                    PrintHandler.PrintSuccessMessage("Data Added Successfully");
                }

            }
            catch
            {
                PrintHandler.PrintErrorMessage("Oops Some error occurred !");
            }
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

        public static void TakeInputToDeleteEvent()
        {
            try
            {
                DisplayEvents();

                if (!IsEventsPresent()) return;

                int serialNumber = TakeIdForUpdateOrDelete(true);

                _eventService.DeleteEvent(serialNumber);

                PrintHandler.PrintSuccessMessage("Data deleted Successfully");
            }
            catch
            {
                PrintHandler.PrintErrorMessage("Oops ! Some error occurred!");
            }

        }

        public static void TakeInputToUpdateEvent()
        {

            try
            {
                DisplayEvents();

                if (!IsEventsPresent()) return;

                int id = TakeIdForUpdateOrDelete(false);

                Event eventObj = _eventService.GetEventsById(_eventService.GetEventIdFromSerialNumber(id));

                if (eventObj == null) return;

                if (eventObj.IsProposed)
                {
                    TakeInputForProposedEvent();
                    return;
                }

                GetEventDetailsFromUser(eventObj);

                eventObj.UserId = GlobalData.GetUser().Id;

                RecurrenceHandling.AskForRecurrenceChoice(eventObj);

                bool isUpdated = _eventService.UpdateEvent(eventObj, id);

                if (isUpdated) PrintHandler.PrintSuccessMessage("Data Updated Successfully");
            }
            catch
            {
                PrintHandler.PrintErrorMessage("Oops ! Some error occurred !");
            }
        }

        public static int TakeIdForUpdateOrDelete(bool isDelete)
        {
            string operation = isDelete ? "delete" : "update";

            int Id = ValidatedInputProvider.GetValidatedIntegerBetweenRange($"From Above events give event no. that you want to {operation} :- ", 1, _eventService.GetTotalEventCount());

            return Id;
        }

    }
}