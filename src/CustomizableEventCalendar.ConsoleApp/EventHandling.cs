using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Data.Repositories;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Enums;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.ConsoleApp
{
    internal static class EventHandling
    {
        private readonly static EventService _eventService = new();

        private readonly static ShareCalendar _shareCalendar = new();

        private readonly static ValidationService _validationService = new();

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

            EventOperationsEnum option = (EventOperationsEnum)choice;

            switch (option)
            {
                case EventOperationsEnum.Add:
                    TakeInputToAddEvent();
                    break;
                case EventOperationsEnum.Display:
                    DisplayEvents();
                    break;
                case EventOperationsEnum.Delete:
                    TakeInputToDeleteEvent();
                    break;
                case EventOperationsEnum.Update:
                    TakeInputToUpdateEvent();
                    break;
                case EventOperationsEnum.View:
                    CalendarView.ViewSelection();
                    break;
                case EventOperationsEnum.ShareCalendar:
                    _shareCalendar.GetDetailsToShareCalendar();
                    break;
                case EventOperationsEnum.ViewSharedCalendar:
                    _shareCalendar.ViewSharedCalendars();
                    break;
                case EventOperationsEnum.SharedEventCollaboration:
                    SharedEventCollaboration sharedEventCollaboration = new SharedEventCollaboration();
                    sharedEventCollaboration.ShowSharedEvents();
                    break;
                case EventOperationsEnum.EventWithMultipleInvitees:
                    TakeInputForProposedEvent();
                    break;
                case EventOperationsEnum.Back:
                    Console.WriteLine("Going Back ...");
                    break;
                default:
                    Console.WriteLine("Oops! Wrong choice");
                    break;
            }

            if (option.Equals(EventOperationsEnum.Back)) Authentication.AuthenticationChoice();
            else AskForChoice(); // Remove recursion
        }

        public static int GetValidatedChoice()
        {
            ShowAllChoices();

            string inputFromConsole = Console.ReadLine();
            int choice;

            while (!_validationService.ValidateInput(inputFromConsole, out choice, int.TryParse))
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

            RecurrencePatternCustom recurrencePattern = new();

            DateTime propedDate = ValidatedInputProvider.GetValidatedDateTime("Enter date for the propose event (Enter " +
                                                                              "date in dd-MM-yyyy) :- ");

            recurrencePattern.DTSTART = propedDate;
            recurrencePattern.UNTILL = propedDate;

            eventObj.UserId = GlobalData.user.Id;

            eventObj.IsProposed = true;

            int eventId = _eventService.InsertEventWithRecurrencePattern(eventObj, recurrencePattern);

            string invitees = GetInviteesFromUser();

            if (invitees.Length == 0) return;

            MultipleInviteesEventService multipleInviteesEventService = new();
            multipleInviteesEventService.AddInviteesInProposedEvent(eventId, invitees);
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

            PropertyInfo[] properties = eventObj.GetType().GetProperties().Where(property =>
                                                                                 !Attribute.IsDefined(property,
                                                                                    typeof(NotMappedAttribute))
                                                                                 && !property.Name.EndsWith("Id"))
                                                                          .ToArray();

            foreach (PropertyInfo property in properties)
            {
                if (property.Name.Equals("IsProposed")) continue;

                if (property.Name.Equals("TimeBlock"))
                {
                    Console.Write($"Enter {property.Name} (Enter like this 2PM-9PM or 10AM-5PM) :- ");
                }
                else
                {
                    Console.Write($"Enter {property.Name}: ");
                }

                string value = Console.ReadLine();

                if (property.Name.Equals("TimeBlock"))
                {
                    value = ValidatedInputProvider.GetValidatedTimeBlock(value);
                }

                object typedValue = Convert.ChangeType(value, property.PropertyType);
                property.SetValue(eventObj, typedValue);
            }
            eventObj.IsProposed = false;
        }

        public static void TakeInputToAddEvent()
        {
            Event eventObj = new();

            GetEventDetailsFromUser(eventObj);

            eventObj.UserId = GlobalData.user.Id;

            RecurrencePatternCustom recurrencePattern = RecurrenceHandling.AskForRecurrenceChoice(null);

            _eventService.InsertEventWithRecurrencePattern(eventObj, recurrencePattern);
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
            DisplayEvents();

            if (!IsEventsPresent()) return;
            int Id = ValidatedInputProvider.GetValidatedInteger("From Above events give event no. that you want to delete :- ");

            _eventService.DeleteEventWithRecurrencePattern(Id);

        }

        public static void TakeInputToUpdateEvent()
        {
            DisplayEvents();

            if (!IsEventsPresent()) return;

            int Id = ValidatedInputProvider.GetValidatedInteger("From Above events give event no. that you want to update :- ");

            Event eventObj = _eventService.GetEventsById(Id);

            if (eventObj == null) return;

            if (eventObj.IsProposed)
            {
                TakeInputForProposedEvent();
                return;
            }

            int recurrenceId = eventObj.RecurrenceId;

            GetEventDetailsFromUser(eventObj);

            eventObj.UserId = GlobalData.user.Id;

            eventObj.RecurrenceId = recurrenceId;

            RecurrencePatternCustom recurrencePattern = RecurrenceHandling.AskForRecurrenceChoice(recurrenceId);

            _eventService.UpdateEventWithRecurrencePattern(eventObj, recurrencePattern, Id, eventObj.RecurrenceId);
        }
    }
}