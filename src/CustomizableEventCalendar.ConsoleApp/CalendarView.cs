using System.Text;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Enums;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.ConsoleApp
{
    internal class CalendarView
    {
        private static readonly CalendarViewService _calendarViewService = new();

        private readonly EventService _eventService = new();

        public void ViewSelection()
        {
            int choice = ValidatedInputProvider.GetValidatedInteger("\nChoose the view you want to see : 1. Daily View " +
                                                                    "2. Weekly View 3. Monthly View 0. Back :- ");

            CalendarViewChoice option = (CalendarViewChoice)choice;

            switch (option)
            {
                case CalendarViewChoice.Daily:
                    PrintDailyView();
                    ViewSelection();
                    break;
                case CalendarViewChoice.Weekly:
                    PrintWeeklyView();
                    ViewSelection();
                    break;
                case CalendarViewChoice.Monthly:
                    PrintMonthlyView();
                    ViewSelection();
                    break;
                case CalendarViewChoice.Back:
                    Console.WriteLine("Going back");
                    break;
                default:
                    Console.WriteLine("Oops! Wrong choice");
                    ViewSelection();
                    break;
            }
        }

        public static void PrintDailyView()
        {
            Dictionary<int, Event> hourEventMapping = _calendarViewService.GetHourAndEventsForDailyView();

            StringBuilder dailyView = new();

            dailyView.AppendLine(PrintService.GetHorizontalLine());

            dailyView.AppendLine("Schedule of date :- " + DateTimeManager.GetDateFromDateTime(DateTime.Today) + "\n");

            List<List<string>> dailyViewTableContent = InsertTodayEventsWithDateIn2DList(hourEventMapping);

            dailyView.AppendLine(PrintService.GenerateTable(dailyViewTableContent));

            Console.WriteLine(dailyView);
        }

        public void PrintWeeklyView()
        {
            DateOnly startDateOfWeek = DateTimeManager.GetStartDateOfWeek(DateOnly.FromDateTime(DateTime.Today));
            DateOnly endDateOfWeek = DateTimeManager.GetEndDateOfWeek(DateOnly.FromDateTime(DateTime.Today));

            Dictionary<DateOnly, List<int>> currentWeekEvents = _calendarViewService.GetDateAndEventsForWeeklyView();

            StringBuilder weeklyView = new();

            weeklyView.AppendLine(PrintService.GetHorizontalLine());

            weeklyView.AppendLine("Schedule from date :- " + startDateOfWeek + " to date :- " + endDateOfWeek + "\n");

            List<List<string>> weeklyViewTableContent = Generate2DListForWeeklyEvents(startDateOfWeek, endDateOfWeek,
                                                                                              currentWeekEvents);

            weeklyView.AppendLine(PrintService.GenerateTable(weeklyViewTableContent));

            Console.WriteLine(weeklyView);
        }

        public void PrintMonthlyView()
        {
            StringBuilder monthlyView = new();

            DateOnly startDateOfMonth = DateTimeManager.GetStartDateOfMonth(DateTime.Now);
            DateOnly endDateOfMonth = DateTimeManager.GetEndDateOfMonth(DateTime.Now);

            Dictionary<DateOnly, List<int>> currentMonthEvents = _calendarViewService.GetGivenMonthEventsWithDate(startDateOfMonth, endDateOfMonth);

            monthlyView.AppendLine(PrintService.GetHorizontalLine());

            monthlyView.AppendLine("Schedule from date :- " + startDateOfMonth + " to date :- " + endDateOfMonth + "\n");

            List<List<string>> monthlyViewTableContent = Generate2DListForMonthlyEvents(ref startDateOfMonth, endDateOfMonth, currentMonthEvents);

            monthlyView.Append(PrintService.GenerateTable(monthlyViewTableContent));


            Console.WriteLine(monthlyView);
        }

        private static List<List<string>> InsertTodayEventsWithDateIn2DList(Dictionary<int, Event> hourEventMapping)
        {
            List<List<string>> dailyViewTableContent = [["Date", "Event Title"]];

            DateTime today = DateTime.Today;

            while (today.Date <= DateTime.Today.Date)
            {
                int curHour = today.Hour;

                hourEventMapping.TryGetValue(curHour, out Event? eventObj);

                dailyViewTableContent.Add([DateTimeManager.GetDateWithAbbreviationFromDateTime(today), eventObj == null ? "-" : eventObj.Title]);

                today = today.AddHours(1);
            }

            return dailyViewTableContent;
        }

        private List<List<string>> Generate2DListForWeeklyEvents(DateOnly startDateOfWeek, DateOnly endDateOfWeek, Dictionary<DateOnly, List<int>> currentWeekEvents)
        {
            List<List<string>> weeklyViewTableContent = [["Date", "Day", "Event Title", "Start Time", "End Time"]];

            while (startDateOfWeek < endDateOfWeek)
            {
                InsertGivenWeekEventsWithDateIn2DList(startDateOfWeek, currentWeekEvents, weeklyViewTableContent);

                startDateOfWeek = startDateOfWeek.AddDays(1);
            }

            return weeklyViewTableContent;
        }

        private void InsertGivenWeekEventsWithDateIn2DList(DateOnly startDateOfWeek, Dictionary<DateOnly, List<int>> currentWeekEvents, List<List<string>> weeklyViewTableContent)
        {
            if (currentWeekEvents.TryGetValue(startDateOfWeek, out List<int>? eventIds))
            {
                foreach (var eventId in eventIds)
                {

                    Event eventObj = _eventService.GetEventById(eventId);

                    weeklyViewTableContent.Add([startDateOfWeek.ToString(),
                                                DateTimeManager.GetDayFromDateTime(startDateOfWeek),
                                                eventObj.Title,
                                                DateTimeManager.ConvertTo12HourFormat(eventObj.EventStartHour),
                                                DateTimeManager.ConvertTo12HourFormat(eventObj.EventEndHour)]);
                }
            }
            else
            {
                weeklyViewTableContent.Add([startDateOfWeek.ToString(),
                                            DateTimeManager.GetDayFromDateTime(startDateOfWeek),
                                            "-","-","-"]);
            }
        }

        private List<List<string>> Generate2DListForMonthlyEvents(ref DateOnly startDateOfMonth, DateOnly endDateOfMonth, Dictionary<DateOnly, List<int>> currentMonthEvents)
        {
            List<List<string>> monthlyViewTableContent = [["Date", "Day", "Event Title", "Start Time", "End Time"]];

            while (startDateOfMonth <= endDateOfMonth)
            {
                InsertGivenMonthEventsWithDateIn2DList(startDateOfMonth, currentMonthEvents, monthlyViewTableContent);

                startDateOfMonth = startDateOfMonth.AddDays(1);
            }

            return monthlyViewTableContent;
        }

        private void InsertGivenMonthEventsWithDateIn2DList(DateOnly startDateOfMonth, Dictionary<DateOnly, List<int>> currentMonthEvents, List<List<string>> monthlyViewTableContent)
        {
            if (currentMonthEvents.TryGetValue(startDateOfMonth, out List<int>? eventIds))
            {
                foreach (var eventId in eventIds)
                {

                    Event eventObj = _eventService.GetEventById(eventId);
                    monthlyViewTableContent.Add([startDateOfMonth.ToString(),
                                                 startDateOfMonth.DayOfWeek.ToString(),
                                                 eventObj.Title,
                                                 DateTimeManager.ConvertTo12HourFormat(eventObj.EventStartHour),
                                                 DateTimeManager.ConvertTo12HourFormat(eventObj.EventEndHour)]);
                }
            }
            else
            {
                monthlyViewTableContent.Add([startDateOfMonth.ToString(),
                                             startDateOfMonth.DayOfWeek.ToString(),
                                             "-","-","-"]);
            }
        }
    }
}