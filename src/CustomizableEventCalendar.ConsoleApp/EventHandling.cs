using System.Data;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Enums;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Models;

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

        public static void GetEventDetailsFromUser(EventModel eventModel)
        {
            Console.WriteLine("\nFill Details Related to Event : ");

            eventModel.Title = ValidatedInputProvider.GetValidString("Enter title : ");

            PrintHandler.PrintNewLine();

            eventModel.Description = ValidatedInputProvider.GetValidString("Enter description : ");

            PrintHandler.PrintNewLine();

            eventModel.Location = ValidatedInputProvider.GetValidString("Enter Location : ");
        }

        private static void GetInputToAddEvent()
        {
            EventModel eventModel = new()
            {
                Duration = new DurationModel(),
                RecurrencePattern = new RecurrencePatternModel(),
                Participants = [(new ParticipantModel(ParticipantRole.Organizer, ConfirmationStatus.Accept, null, null, new DateOnly(), GlobalData.GetUser()))],
            };

            GetEventDetailsFromUser(eventModel);

            TimeHandler.GetStartingAndEndingHourOfEvent(eventModel);

            RecurrenceHandling.AskForRecurrenceChoice(eventModel);

            AddEvent(eventModel, false);
        }

        public static void AddEvent(EventModel eventModel, bool isProposed)
        {
            try
            {
                if (OverlapHandler.IsOverlappingEvent(eventModel, true, isProposed)) return;

                _eventService.InsertEvent(eventModel);

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
            List<EventModel> events = _eventService.GetSingleInstanceOfAllEvents();

            List<List<string>> outputRows = events.InsertInto2DList(["Event NO.", "Title", "Description", "Location", "Event Repetition", "Start Date", "End Date", "Duration"],
                [
                    eventModel => events.IndexOf(eventModel) + 1,
                    eventModel => eventModel.Title,
                    eventModel => eventModel.Description,
                    eventModel => eventModel.Location,
                    eventModel => RecurrencePatternMessageGenerator.GenerateRecurrenceMessage(eventModel),
                    eventModel => eventModel.RecurrencePattern.StartDate.ToString(),
                    eventModel => eventModel.RecurrencePattern.EndDate.ToString(),
                    eventModel => DateTimeManager.ConvertTo12HourFormat(eventModel.Duration.StartHour) + " - " +
                                DateTimeManager.ConvertTo12HourFormat(eventModel.Duration.EndHour)
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

                if (serialNumber == 0) return;

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

            if (serialNumber == 0) return;

            List<EventModel>? eventModels = _eventService.GetEventById(_eventService.GetEventIdFromSerialNumber(serialNumber));

            EventModel? eventModel = eventModels == null ? null : eventModels.FirstOrDefault();

            if (eventModel == null) return;

            if (_eventService.GetProposedEvents().Exists(proposedEvent => proposedEvent.Id == eventModel.Id))
            {
                ProposedEventHandler.GetInputForProposedEvent(eventModel);
                return;
            }

            AskForItemsToUpdate(eventModel);

            UpdateEvent(eventModel.Id, eventModel, false);
        }

        public static void UpdateEvent(int id, EventModel eventModel, bool isProposed)
        {
            try
            {
                if (OverlapHandler.IsOverlappingEvent(eventModel, false, isProposed)) return;

                _eventService.UpdateEvent(eventModel, id);

                PrintHandler.PrintSuccessMessage("Event Updated Successfully");
            }
            catch
            {
                PrintHandler.PrintErrorMessage("Oops ! Some error occurred !");
            }
        }

        private static void AskForItemsToUpdate(EventModel eventModel)
        {
            string inputMessage = "\nWhat items you want to update ? \n1. Event Details (Event Title , Event Description , Event Location) \n2. Event repetition details (Event dates, Event Duration, Event frequency etc ...)";

            Console.WriteLine(inputMessage);

            int choice = ValidatedInputProvider.GetValidIntegerBetweenRange("\nEnter choice : ", 1, 2);

            switch (choice)
            {
                case 1:
                    GetEventDetailsFromUser(eventModel);
                    break;
                case 2:
                    TimeHandler.GetStartingAndEndingHourOfEvent(eventModel);
                    RecurrenceHandling.AskForRecurrenceChoice(eventModel);
                    break;
            }
        }

        private static int GetSerialNumberForUpdateOrDelete(bool isDelete)
        {
            string operation = isDelete ? "delete" : "update";

            int serialNumber = ValidatedInputProvider.GetValidIntegerBetweenRange($"From Above events give event no. that you want to {operation}. (Press 0 to go back)  :- ", 0, _eventService.GetTotalEventCount());

            return serialNumber;
        }

    }
}