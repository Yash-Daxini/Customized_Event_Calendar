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
    internal class EventHandling
    {
        public static EventService eventService = new EventService();

        public static ShareCalendar shareCalendar = new ShareCalendar();

        static ValidationService validationService = new ValidationService();
        public static void PrintColorMessage(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ResetColor();
        }
        public static void ShowAllChoices()
        {
            PrintColorMessage("1. Add Event", ConsoleColor.Green);
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
                    AddEvent();
                    break;
                case EventOperationsEnum.Display:
                    Display();
                    break;
                case EventOperationsEnum.Delete:
                    Delete();
                    break;
                case EventOperationsEnum.Update:
                    Update();
                    break;
                case EventOperationsEnum.View:
                    CalendarView.ViewSelection();
                    break;
                case EventOperationsEnum.ShareCalendar:
                    shareCalendar.GetDetailsToShareCalendar();
                    break;
                case EventOperationsEnum.ViewSharedCalendar:
                    shareCalendar.ViewSharedCalendars();
                    break;
                case EventOperationsEnum.SharedEventCollaboration:
                    SharedEventCollaboration sharedEventCollaboration = new SharedEventCollaboration();
                    sharedEventCollaboration.ShowSharedEvents();
                    break;
                case EventOperationsEnum.EventWithMultipleInvitees:
                    HandleProposedEvent();
                    break;
                case EventOperationsEnum.Back:
                    Console.WriteLine("Going Back ...");
                    break;
                default:
                    Console.WriteLine("Oops! Wrong choice");
                    break;
            }

            if (option.Equals(EventOperationsEnum.Back)) Authentication.AskForChoice();
            else AskForChoice();
        }
        public static int GetValidatedChoice()
        {
            ShowAllChoices();

            string inputFromConsole = Console.ReadLine();
            int choice;

            while (!validationService.ValidateInput(inputFromConsole, out choice, int.TryParse))
            {
                ShowAllChoices();

                inputFromConsole = Console.ReadLine();
            }
            return choice;
        }
        public static void HandleProposedEvent()
        {
            Event eventObj = new Event();
            eventObj.IsProposed = true;

            GetEventDetails(ref eventObj);

            RecurrencePatternCustom recurrencePattern = new RecurrencePatternCustom();

            DateTime propedDate = ValidatedInputProvider.GetValidatedDateTime("Enter date for the propose event (Enter " +
                                                                              "date in dd-MM-yyyy) :- ");

            recurrencePattern.DTSTART = propedDate;
            recurrencePattern.UNTILL = propedDate;

            int eventId = eventService.Create(eventObj, recurrencePattern);

            string invitees = GetInvitees();

            MultipleInviteesEventService multipleInviteesEventService = new MultipleInviteesEventService();
            multipleInviteesEventService.AddInviteesInProposedEvent(eventId, invitees);
        }
        public static void ShowAllUser()
        {
            UserService userService = new UserService();
            string users = userService.GetInsensitiveInformationOfUser();

            Console.WriteLine(users);
        }
        public static string GetInvitees()
        {
            ShowAllUser();

            string invitees = ValidatedInputProvider.GetValidatedCommaSeparatedInput("Enter users you want to Invite. " +
                                                                        "Enter users Sr No. comma separated Ex:- 1,2,3");

            return invitees;
        }
        public static void GetEventDetails(ref Event eventObj)
        {
            Console.WriteLine("\nFill Details Related to Event : ");

            eventObj = new Event();

            PropertyInfo[] properties = eventObj.GetType().GetProperties().Where(property => !Attribute.IsDefined(property, typeof(NotMappedAttribute)) && !property.Name.EndsWith("Id")).ToArray();

            foreach (PropertyInfo property in properties)
            {
                Console.Write($"Enter value for {property.Name}: ");
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
        public static void AddEvent()
        {
            Event eventObj = new Event();
            GetEventDetails(ref eventObj);

            eventObj.UserId = GlobalData.user.Id;

            RecurrencePatternCustom recurrencePattern = RecurrenceHandling.AskForRecurrenceChoice(null);

            eventService.Create(eventObj, recurrencePattern);
        }
        public static void Display()
        {
            string eventList = eventService.GenerateEventTable();
            Console.WriteLine(eventList);
        }
        public static void Delete()
        {
            Display();

            int Id = ValidatedInputProvider.GetValidatedInteger("From Above events give event no. that you want to delete :- ");

            eventService.Delete(Id);

        }
        public static void Update()
        {
            Display();

            int Id = ValidatedInputProvider.GetValidatedInteger("From Above events give event no. that you want to update :- ");

            Event eventObj = eventService.Read(Id);

            int recurrenceId = eventObj.RecurrenceId;

            GetEventDetails(ref eventObj);

            eventObj.UserId = GlobalData.user.Id;

            eventObj.RecurrenceId = recurrenceId;

            RecurrencePatternCustom recurrencePattern = RecurrenceHandling.AskForRecurrenceChoice(recurrenceId);

            eventService.Update(eventObj, recurrencePattern, Id, eventObj.RecurrenceId);
        }
    }
}