using System.Data;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Enums;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.ConsoleApp
{
    internal static class EventHandling
    {
        private readonly static EventService _eventService = new();

        private readonly static ShareCalendar _shareCalendar = new();

        private static readonly Dictionary<EventOperation, Action> operationDictionary = new()
        {
            { EventOperation.Add, GetInputToAddEvent },
            { EventOperation.Display, DisplayEvents },
            { EventOperation.Delete, DeleteEvent },
            { EventOperation.Update, GetInputToUpdateEvent },
            { EventOperation.View, CalendarView.ChooseView },
            { EventOperation.ShareCalendar, _shareCalendar.GetDetailsToShareCalendar },
            { EventOperation.ViewSharedCalendar, _shareCalendar.ShowSharedCalendars },
            { EventOperation.EventWithMultipleInvitees, () => ProposedEventHandler.GetInputForProposedEvent(null) },
            { EventOperation.GiveResponseToProposedEvent, ProposedEventResponseHandler.ShowProposedEvents},
            { EventOperation.EventsTimeline , EventTimeLinePrinter.PrintEventWithTimeLine}
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

                int choice = ValidatedInputProvider.GetValidIntegerBetweenRange("\nEnter choice :- ", 0, 10);

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

        public static void GetEventDetailsFromUser(Event eventObj)
        {
            Console.WriteLine("\nFill Details Related to Event : ");

            eventObj.Title = ValidatedInputProvider.GetValidString("Enter title : ");

            PrintHandler.PrintNewLine();

            eventObj.Description = ValidatedInputProvider.GetValidString("Enter description : ");

            PrintHandler.PrintNewLine();

            eventObj.Location = ValidatedInputProvider.GetValidString("Enter Location : ");

            eventObj.IsProposed = false;
        }

        private static void GetInputToAddEvent()
        {
            Event eventObj = new();

            GetEventDetailsFromUser(eventObj);

            TimeHandler.GetStartingAndEndingHourOfEvent(eventObj);

            eventObj.UserId = GlobalData.GetUser().Id;

            RecurrenceHandling.AskForRecurrenceChoice(eventObj);

            AddEvent(eventObj);
        }

        public static void AddEvent(Event eventObj)
        {
            try
            {
                if (OverlapHandler.IsOverlappingEvent(eventObj, true)) return;

                eventObj.Id = _eventService.InsertEvent(eventObj);

                PrintHandler.PrintSuccessMessage("Event Added Successfully");

            }
            catch
            {
                PrintHandler.PrintErrorMessage("Oops Some error occurred !");
            }
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
                ProposedEventHandler.GetInputForProposedEvent(eventObj);
                return;
            }

            AskForItemsToUpdate(eventObj);

            eventObj.UserId = GlobalData.GetUser().Id;

            UpdateEvent(eventObj.Id, eventObj);
        }

        public static void UpdateEvent(int id, Event eventObj)
        {
            try
            {
                if (OverlapHandler.IsOverlappingEvent(eventObj, false)) return;

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
            string inputMessage = "\nWhat items you want to update ? \n1. Event Details (Event Title , Event Description , Event Location) \n2. Event repetition details (Event dates, Event Duration, Event frequency etc ...)";

            Console.WriteLine(inputMessage);

            int choice = ValidatedInputProvider.GetValidIntegerBetweenRange("\nEnter choice : ", 1, 2);

            switch (choice)
            {
                case 1:
                    GetEventDetailsFromUser(eventObj);
                    break;
                case 2:
                    TimeHandler.GetStartingAndEndingHourOfEvent(eventObj);
                    RecurrenceHandling.AskForRecurrenceChoice(eventObj);
                    break;
            }
        }

        private static int GetSerialNumberForUpdateOrDelete(bool isDelete)
        {
            string operation = isDelete ? "delete" : "update";

            int serialNumber = ValidatedInputProvider.GetValidIntegerBetweenRange($"From Above events give event no. that you want to {operation} :- ", 1, _eventService.GetTotalEventCount());

            return serialNumber;
        }

    }
}