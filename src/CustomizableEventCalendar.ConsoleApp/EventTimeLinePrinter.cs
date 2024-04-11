using System.Data;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Models;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.ConsoleApp
{
    internal static class EventTimeLinePrinter
    {

        private readonly static EventService _eventService = new();

        private static DateOnly GetDate(string inputMessage)
        {
            return ValidatedInputProvider.GetValidDateOnly(inputMessage);
        }

        private static void GetDatesToPrintEventWithTimeline(out DateOnly startDate, out DateOnly endDate)
        {
            startDate = GetDate("Please enter start date from you want to see events : ");
            endDate = GetDate("Please enter end date from you want to see events : ");

            while (!ValidationService.IsValidStartAndEndDate(startDate, endDate))
            {
                PrintHandler.PrintWarningMessage("Invalid input ! Start date must less than or equal to the end date ");
                startDate = GetDate("Please enter start date from you want to see events : ");
                endDate = GetDate("Please enter end date from you want to see events : ");
            }
        }

        private static List<EventByDate> GetDateWiseEventCollaborators(DateOnly startDate, DateOnly endDate, List<Domain.Entities.EventCollaborator> eventCollaborators)
        {
            List<Domain.Entities.EventCollaborator> eventCollaboratorsInGivenDateRange = GetEventCollaboratorsInGivenDateRange(startDate, endDate, eventCollaborators);

            List<EventByDate> eventsByDate = [];

            //List<Event> events = _eventService.GetAllEvents();

            //.............................
            List<Event> events = [];

            foreach (var eventCollaborator in eventCollaboratorsInGivenDateRange)
            {
                Event? eventObj = events.Find(eventObj => eventObj.Id == eventCollaborator.EventId);

                if (eventObj is null) continue;

                eventsByDate.Add(new EventByDate(eventCollaborator.EventDate, eventObj));
            }

            return eventsByDate;
        }

        private static List<Domain.Entities.EventCollaborator> GetEventCollaboratorsInGivenDateRange(DateOnly startDate, DateOnly endDate, List<Domain.Entities.EventCollaborator> eventCollaborators)
        {
            return [..eventCollaborators.FindAll(eventCollaborator => IsDateInRange(startDate, endDate, eventCollaborator.EventDate))
                                        .OrderBy(eventCollaborator=>eventCollaborator.EventDate)
                                        .ThenBy(eventCollaborator => eventCollaborator.ProposedStartHour)];
        }

        private static bool IsDateInRange(DateOnly startDate, DateOnly endDate, DateOnly dateToCheck)
        {
            return dateToCheck >= startDate && dateToCheck <= endDate;
        }

        public static void PrintEventWithTimeLine()
        {
            GetDatesToPrintEventWithTimeline(out DateOnly startDate, out DateOnly endDate);

            List<Domain.Entities.EventCollaborator> eventCollaborators = GetEventCollaborators();

            List<EventByDate> eventsByDate = GetDateWiseEventCollaborators(startDate, endDate, eventCollaborators);

            List<List<string>> tableContentOfEventTimeLine = eventsByDate.InsertInto2DList(["Date", "Day", "Event Name", "Start Time", "End Time"], 
                [
                    eventByDate => eventByDate.Date,
                    eventByDate => DateTimeManager.GetDayFromDateOnly(eventByDate.Date),
                    eventByDate => eventByDate.Event.Title,
                    eventByDate => DateTimeManager.ConvertTo12HourFormat(eventByDate.Event.EventStartHour),
                    eventByDate => DateTimeManager.ConvertTo12HourFormat(eventByDate.Event.EventEndHour),
                ]);

            Console.WriteLine("\n" + PrintService.GenerateTable(tableContentOfEventTimeLine));

        }

        private static List<Domain.Entities.EventCollaborator> GetEventCollaborators()
        {
            EventCollaboratorService eventCollaboratorService = new();

            List<Domain.Entities.EventCollaborator> eventCollaborators = [.. eventCollaboratorService.GetAllParticipants()
                                                                                 .FindAll(eventCollaborator =>
                                                                                            eventCollaborator.UserId == GlobalData.GetUser().Id
                                                                                            && eventCollaborator.ConfirmationStatus != null
                                                                                            && !eventCollaborator.ConfirmationStatus.Equals("reject")
                                                                                         )];
            return eventCollaborators;
        }
    }
}