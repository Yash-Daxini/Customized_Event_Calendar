using System.Data;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.ConsoleApp
{
    internal static class EventTimeLinePrinter
    {

        private readonly static EventService _eventService = new();

        private static DateOnly GetDate(string inputMessage)
        {
            return ValidatedInputProvider.GetValidatedDateOnly(inputMessage);
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

        private static Dictionary<DateOnly, List<EventCollaborator>> GetDateWiseEventCollaborators(DateOnly startDate, DateOnly endDate, List<EventCollaborator> eventCollaborators)
        {
            Dictionary<DateOnly, List<EventCollaborator>> dateWiseEventCollaborators = [];

            DateOnly currentDate = startDate;

            while (currentDate <= endDate)
            {
                List<EventCollaborator> eventCollaboratorBetweenGivenRange = eventCollaborators.FindAll(eventCollaborator =>
                                                                             IsDateInRange(startDate, endDate, eventCollaborator.EventDate) &&
                                                                             currentDate == eventCollaborator.EventDate);

                dateWiseEventCollaborators[currentDate] = new(eventCollaboratorBetweenGivenRange);

                currentDate = currentDate.AddDays(1);
            }

            return dateWiseEventCollaborators;
        }

        private static bool IsDateInRange(DateOnly startDate, DateOnly endDate, DateOnly dateToCheck)
        {
            return dateToCheck >= startDate && dateToCheck <= endDate;
        }

        public static void PrintEventWithTimeLine()
        {

            GetDatesToPrintEventWithTimeline(out DateOnly startDate, out DateOnly endDate);

            EventCollaboratorService eventCollaboratorService = new();
            List<EventCollaborator> eventCollaborators = [..eventCollaboratorService.GetAllEventCollaborators()
                                                                                 .FindAll(eventCollaborator =>
                                                                                            eventCollaborator.UserId == GlobalData.GetUser().Id
                                                                                            && eventCollaborator.ConfirmationStatus != null
                                                                                            && !eventCollaborator.ConfirmationStatus.Equals("reject")
                                                                                         )];

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
                                                     eventObj.Title,
                                                     DateTimeManager.ConvertTo12HourFormat(eventObj.EventStartHour),
                                                     DateTimeManager.ConvertTo12HourFormat(eventObj.EventEndHour)]);
                }
                if (dateWiseEventCollaborators[date].Count == 0) tableContentOfEventTimeLine.Add([date.ToString(),
                                                                                                  DateTimeManager.GetDayFromDateOnly(date),
                                                                                                  "-", "-", "-"]);
            }

            Console.WriteLine("\n" + PrintService.GenerateTable(tableContentOfEventTimeLine));

        }
    }
}