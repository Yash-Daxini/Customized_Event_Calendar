using System.Text;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Enums;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.ConsoleApp
{
    internal static class CalendarView
    {
        private static readonly CalendarViewService _calendarViewService = new();

        public static void ChooseView()
        {
            Console.WriteLine("\nChoose the view you want to see : \n1. Daily View \n2. Weekly View \n3. Monthly View \n0. Back :- ");

            int choice = ValidatedInputProvider.GetValidatedIntegerBetweenRange("Enter choice : ", 0, 3);

            Dictionary<CalendarViewChoice, Action> operationDictionary = new()
            {{ CalendarViewChoice.Daily, PrintDailyView},
            { CalendarViewChoice.Weekly, PrintWeeklyView },
            { CalendarViewChoice.Monthly, PrintMonthlyView }};

            CalendarViewChoice option = (CalendarViewChoice)choice;

            if (operationDictionary.TryGetValue(option, out Action? actionMethod))
            {
                actionMethod.Invoke();
            }

            if (option != CalendarViewChoice.Back) ChooseView();
        }

        public static void PrintDailyView()
        {
            Dictionary<int, Event> hourEventMapping = _calendarViewService.GetHourAndEventsForDailyView();

            StringBuilder dailyView = new();

            dailyView.AppendLine(PrintService.GetHorizontalLine());

            dailyView.AppendLine("\nSchedule of " + DateTimeManager.GetDateFromDateTime(DateTime.Today) + "\n");

            List<List<string>> dailyViewTableContent = InsertTodayEventsWithDateIn2DList(hourEventMapping);

            dailyView.AppendLine(PrintService.GenerateTable(dailyViewTableContent));

            Console.WriteLine(dailyView);
        }

        public static void PrintWeeklyView()
        {
            DateOnly startDateOfWeek = DateTimeManager.GetStartDateOfWeek(DateOnly.FromDateTime(DateTime.Today));
            DateOnly endDateOfWeek = DateTimeManager.GetEndDateOfWeek(DateOnly.FromDateTime(DateTime.Today));

            Dictionary<DateOnly, List<Event>> currentWeekEvents = _calendarViewService.GetDateAndEventsForWeeklyView();

            StringBuilder weeklyView = new();

            weeklyView.AppendLine(PrintService.GetHorizontalLine());

            weeklyView.AppendLine("\nSchedule from " + startDateOfWeek + " to " + endDateOfWeek + "\n");

            List<List<string>> weeklyViewTableContent = Generate2DListForEvents(startDateOfWeek, endDateOfWeek,
                                                                                              currentWeekEvents);

            weeklyView.AppendLine(PrintService.GenerateTable(weeklyViewTableContent));

            Console.WriteLine(weeklyView);
        }

        public static void PrintMonthlyView()
        {
            StringBuilder monthlyView = new();

            DateOnly startDateOfMonth = DateTimeManager.GetStartDateOfMonth(DateTime.Now);
            DateOnly endDateOfMonth = DateTimeManager.GetEndDateOfMonth(DateTime.Now);

            Dictionary<DateOnly, List<Event>> currentMonthEvents = _calendarViewService.GetGivenMonthEventsWithDate(startDateOfMonth, endDateOfMonth);

            monthlyView.AppendLine(PrintService.GetHorizontalLine());

            monthlyView.AppendLine("\nSchedule from " + startDateOfMonth + " to " + endDateOfMonth + "\n");

            List<List<string>> monthlyViewTableContent = Generate2DListForEvents(startDateOfMonth, endDateOfMonth, currentMonthEvents);

            monthlyView.Append(PrintService.GenerateTable(monthlyViewTableContent));


            Console.WriteLine(monthlyView);
        }

        private static List<List<string>> InsertTodayEventsWithDateIn2DList(Dictionary<int, Event> hourEventMapping)
        {
            List<List<string>> dailyViewTableContent = [["Hour", "Event Title"]];

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

        private static List<List<string>> Generate2DListForEvents(DateOnly startDate, DateOnly endDate, Dictionary<DateOnly, List<Event>> dateEventDictionary)
        {
            List<List<string>> tableContent = [["Date", "Day", "Event Title", "Start Time", "End Time"]];

            DateOnly currentDate = startDate;

            while (currentDate <= endDate)
            {
                InsertGivenEventsWithDateIn2DList(currentDate, dateEventDictionary, tableContent);

                currentDate = currentDate.AddDays(1);
            }

            return tableContent;
        }

        private static void InsertGivenEventsWithDateIn2DList(DateOnly currentDate, Dictionary<DateOnly, List<Event>> dateEventDictionary, List<List<string>> tableContent)
        {
            if (dateEventDictionary.TryGetValue(currentDate, out List<Event>? events))
            {
                foreach (var eventObj in events)
                {
                    tableContent.Add([currentDate.ToString(),
                                                DateTimeManager.GetDayFromDateTime(currentDate),
                                                eventObj.Title,
                                                DateTimeManager.ConvertTo12HourFormat(eventObj.EventStartHour),
                                                DateTimeManager.ConvertTo12HourFormat(eventObj.EventEndHour)]);
                }
            }
            else
            {
                tableContent.Add([currentDate.ToString(),
                                            DateTimeManager.GetDayFromDateTime(currentDate),
                                            "-","-","-"]);
            }
        }
    }
}