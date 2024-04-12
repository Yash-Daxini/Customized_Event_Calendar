using System.Data;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Enums;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Models;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.ConsoleApp
{
    internal static class EventTimeLinePrinter
    {

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

        private static List<EventModel> GetDateWiseEventCollaborators(DateOnly startDate, DateOnly endDate, List<EventModel> eventModels)
        {
            return GetEventCollaboratorsInGivenDateRange(startDate, endDate, eventModels);
        }

        private static List<EventModel> GetEventCollaboratorsInGivenDateRange(DateOnly startDate, DateOnly endDate, List<EventModel> eventModels)
        {
            return [..eventModels.Where(eventModel => IsDateInRange(startDate, endDate, eventModel.EventDate))
                                        .OrderBy(eventModel=>eventModel.EventDate)
                                        .ThenBy(eventModel=> eventModel.Duration.StartHour)];
        }

        private static bool IsDateInRange(DateOnly startDate, DateOnly endDate, DateOnly dateToCheck)
        {
            return dateToCheck >= startDate && dateToCheck <= endDate;
        }

        public static void PrintEventWithTimeLine()
        {
            GetDatesToPrintEventWithTimeline(out DateOnly startDate, out DateOnly endDate);

            List<EventModel> eventModels = GetUserParticipationEvents();

            List<EventModel> eventsByDate = GetDateWiseEventCollaborators(startDate, endDate, eventModels);

            List<List<string>> tableContentOfEventTimeLine = eventsByDate.InsertInto2DList(["Date", "Day", "Event Name", "Start Time", "End Time"],
                [
                    eventModel => eventModel.EventDate,
                    eventModel => DateTimeManager.GetDayFromDateOnly(eventModel.EventDate),
                    eventModel => eventModel.Title,
                    eventModel => DateTimeManager.ConvertTo12HourFormat(eventModel.Duration.StartHour),
                    eventModel => DateTimeManager.ConvertTo12HourFormat(eventModel.Duration.EndHour),
                ]);

            Console.WriteLine("\n" + PrintService.GenerateTable(tableContentOfEventTimeLine));

        }

        private static List<EventModel> GetUserParticipationEvents()
        {
            return [..new EventService().GetAllEventsOfLoggedInUser()
                                        .Select(eventModel => 
                                        {
                                            eventModel.Participants = eventModel.Participants.Where(participant => participant.ConfirmationStatus == ConfirmationStatus.Reject).ToList();
                                            return eventModel;
                                        })];
        }
    }
}