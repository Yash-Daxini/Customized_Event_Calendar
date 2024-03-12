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

        //private readonly static ShareCalendar _shareCalendar = new();

        public static void PrintColorMessage(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        public static void ShowAllChoices()
        {
            PrintColorMessage("\n1. Add Event", ConsoleColor.Green);
            PrintColorMessage("2. See all Events", ConsoleColor.Blue);
            PrintColorMessage("3. Delete Event", ConsoleColor.Red);
            PrintColorMessage("4. Update Event", ConsoleColor.White);
            PrintColorMessage("5. See calendar view", ConsoleColor.Yellow);
            PrintColorMessage("6. Share calendar", ConsoleColor.Cyan);
            PrintColorMessage("7. View Shared calendar", ConsoleColor.Magenta);
            PrintColorMessage("8. Collaborate from shared calendar", ConsoleColor.DarkGreen);
            PrintColorMessage("9. Add event with multiple invitees", ConsoleColor.DarkGray);
            PrintColorMessage("0. Back", ConsoleColor.Gray);

            Console.Write("\nSelect Any Option :- ");
        }

        public static void AskForChoice()
        {
            int choice = GetValidatedChoice();

            EventOperations option = (EventOperations)choice;

            switch (option)
            {
                case EventOperations.Add:
                    TakeInputToAddEvent();
                    break;
                case EventOperations.Display:
                    DisplayEvents();
                    break;
                case EventOperations.Delete:
                    TakeInputToDeleteEvent();
                    break;
                case EventOperations.Update:
                    TakeInputToUpdateEvent();
                    break;
                case EventOperations.View:
                    //CalendarView.ViewSelection();
                    break;
                case EventOperations.ShareCalendar:
                    //_shareCalendar.GetDetailsToShareCalendar();
                    break;
                case EventOperations.ViewSharedCalendar:
                    //_shareCalendar.ViewSharedCalendars();
                    break;
                case EventOperations.SharedEventCollaboration:
                    SharedEventCollaboration sharedEventCollaboration = new SharedEventCollaboration();
                    sharedEventCollaboration.ShowSharedEvents();
                    break;
                case EventOperations.EventWithMultipleInvitees:
                    TakeInputForProposedEvent();
                    break;
                case EventOperations.Back:
                    Console.WriteLine("Going Back ...");
                    break;
                default:
                    Console.WriteLine("Oops! Wrong choice");
                    break;
            }

            if (option.Equals(EventOperations.Back)) Authentication.AuthenticationChoice();
            else AskForChoice(); // Remove recursion
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
            Event eventObj = new();

            GetEventDetailsFromUser(eventObj);

            DateTime propedDate = ValidatedInputProvider.GetValidatedDateTime("Enter date for the propose event (Enter " +
                                                                              "date in dd-MM-yyyy) :- ");

            eventObj.EventStartDate = DateOnly.FromDateTime(propedDate);
            eventObj.EventEndDate = DateOnly.FromDateTime(propedDate);

            eventObj.UserId = GlobalData.user.Id;

            eventObj.IsProposed = true;

            int eventId = _eventService.InsertEvent(eventObj);

            string invitees = GetInviteesFromUser();

            if (invitees.Length == 0) return;

            //MultipleInviteesEventService multipleInviteesEventService = new();
            //multipleInviteesEventService.AddInviteesInProposedEvent(eventId, invitees);
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

                userInformation.AppendLine(PrintHandler.GiveTable(userTableContent));

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

            Console.WriteLine("Enter title : ");
            eventObj.Title = Console.ReadLine();

            PrintHandler.PrintNewLine();

            Console.WriteLine("Enter description : ");
            eventObj.Description = Console.ReadLine();

            PrintHandler.PrintNewLine();

            Console.WriteLine("Enter Location : ");
            eventObj.Location = Console.ReadLine();

            PrintHandler.PrintNewLine();

            eventObj.EventStartHour = ValidatedInputProvider.GetValidatedInteger("Enter Start Hour for the event : ");

            PrintHandler.PrintNewLine();

            eventObj.EventEndHour = ValidatedInputProvider.GetValidatedInteger("Enter End Hour for the event : ");

            PrintHandler.PrintNewLine();

            eventObj.IsProposed = false;
        }

        public static void TakeStartingAndEndingHourOfEvent(Event eventObj)
        {
            Console.WriteLine("\nHow would you like to enter the time? : ");
            Console.WriteLine("\n1.Choose 24-hour format (1 to 24 hours) \n2. Choose 12-hour format (1 to 12 hours and AM/PM)");

            int choice = ValidatedInputProvider.GetValidatedInteger("Enter choice : ");

            switch (choice)
            {
                case 1:
                    TakeHourIn24HourFormat(eventObj);
                    PrintHandler.PrintInfoMessage("You've selected the 24-hour format.");
                    break;
                case 2:
                    TakeHourIn12HourFormat(eventObj);
                    PrintHandler.PrintInfoMessage("You've selected the 12-hour format.");
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

            if (!ValidationService.IsValidStartAndEndHour(eventObj.EventStartHour, eventObj.EventEndHour)) TakeHourIn24HourFormat(eventObj);
        }

        public static void TakeHourIn12HourFormat(Event eventObj)
        {
            PrintHandler.PrintNewLine();

            eventObj.EventStartHour = ValidatedInputProvider.GetValidated12HourFormatTime("Enter Start Hour for the event (From 1 to 12) : ");

            string startHourAbbreviation = ValidatedInputProvider.GetValidatedAbbreviations();

            PrintHandler.PrintNewLine();

            eventObj.EventEndHour = ValidatedInputProvider.GetValidated12HourFormatTime("Enter End Hour for the event (From 1 to 12) : ");

            string endHourAbbreviation = ValidatedInputProvider.GetValidatedAbbreviations();

            eventObj.EventStartHour += startHourAbbreviation.Equals("PM") ? 12 : 0;

            eventObj.EventEndHour += endHourAbbreviation.Equals("PM") ? 12 : 0;

            PrintHandler.PrintNewLine();

            if (!ValidationService.IsValidStartAndEndHour(eventObj.EventStartHour, eventObj.EventEndHour)) TakeHourIn12HourFormat(eventObj);
        }


        public static void TakeInputToAddEvent()
        {
            try
            {
                Event eventObj = new();

                GetEventDetailsFromUser(eventObj);

                eventObj.UserId = GlobalData.user.Id;

                RecurrenceHandling.AskForRecurrenceChoice(eventObj);

                _eventService.InsertEvent(eventObj);
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

                int Id = TakeIdForUpdateOrDelete(true);

                _eventService.DeleteEvent(Id);
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

                int Id = TakeIdForUpdateOrDelete(false);

                Event eventObj = _eventService.GetEventsById(Id);

                if (eventObj == null) return;

                if (eventObj.IsProposed)
                {
                    TakeInputForProposedEvent();
                    return;
                }

                GetEventDetailsFromUser(eventObj);

                eventObj.UserId = GlobalData.user.Id;

                RecurrenceHandling.AskForRecurrenceChoice(eventObj);

                _eventService.UpdateEvent(eventObj, Id);
            }
            catch
            {
                PrintHandler.PrintErrorMessage("Oops ! Some error occurred !");
            }
        }

        public static int TakeIdForUpdateOrDelete(bool isDelete)
        {
            string operation = isDelete ? "delete" : "update";

            int Id = ValidatedInputProvider.GetValidatedInteger($"From Above events give event no. that you want to {operation} :- ");

            return Id;
        }

    }
}