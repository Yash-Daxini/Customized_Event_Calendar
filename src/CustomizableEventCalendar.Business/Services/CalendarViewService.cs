using System.Text;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;


namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services
{
    internal class CalendarViewService
    {
        private readonly EventService _eventService = new();
        private readonly EventCollaboratorService _eventCollaboratorsService = new();

        public string GenerateDailyView()
        {
            Dictionary<int, Event> hourEventMapping = MapEachHourWithEvent();

            StringBuilder dailyView = new();

            dailyView.AppendLine(PrintService.GetHorizontalLine());

            dailyView.AppendLine("Schedule of date :- " + DateTimeManager.GetDateFromDateTime(DateTime.Today) + "\n");

            List<List<string>> dailyViewTableContent = InsertTodayEventsWithDateIn2DList(hourEventMapping);

            dailyView.AppendLine(PrintService.GenerateTable(dailyViewTableContent));

            return dailyView.ToString();
        }

        private Dictionary<int, Event> MapEachHourWithEvent()
        {
            List<EventCollaborator> eventCollaborators = GetTodayEvents();

            Dictionary<int, Event> hourEventMapping = [];

            foreach (var eventCollaborator in eventCollaborators)
            {
                Event eventObj = _eventService.GetEventById(eventCollaborator.EventId);

                AssignEventToSpecificHour(ref hourEventMapping, eventObj);
            }

            return hourEventMapping;
        }

        private bool IsProposedEvent(int eventId)
        {
            return _eventService.GetProposedEvents().Exists(eventObj => eventObj.Id == eventId);
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

        private List<EventCollaborator> GetTodayEvents()
        {
            return [.. _eventCollaboratorsService.GetAllEventCollaborators()
                   .Where(eventCollaborator => eventCollaborator.EventDate == DateOnly.FromDateTime(DateTime.Today)
                                               && eventCollaborator.UserId == GlobalData.GetUser().Id
                                               && !IsProposedEvent(eventCollaborator.EventId))];
        }

        private static void AssignEventToSpecificHour(ref Dictionary<int, Event> eventRecordByHour, Event eventObj)
        {
            int startHour = eventObj.EventStartHour;
            int endHour = eventObj.EventEndHour;

            for (int i = startHour; i <= endHour; i++)
            {
                eventRecordByHour[i] = eventObj;
            }
        }

        private Dictionary<DateOnly, List<int>> GetGivenWeekEventsWithDate(DateOnly startDateOfWeek, DateOnly endDateOfWeek)
        {
            List<EventCollaborator> eventCollaborators = _eventCollaboratorsService.GetAllEventCollaborators();

            Dictionary<DateOnly, List<int>> currentWeekEvents = GenerateDictionaryOfDateOnlyAndEventIdFromList(GetGivenWeekEvents(eventCollaborators, startDateOfWeek, endDateOfWeek));

            return currentWeekEvents;
        }

        private List<EventCollaborator> GetGivenWeekEvents(List<EventCollaborator> eventCollaborators, DateOnly startDateOfWeek, DateOnly endDateOfWeek)
        {
            return [..eventCollaborators.Where(eventCollaborator => IsEventOccurInGivenWeek(startDateOfWeek,
                                                                       endDateOfWeek, eventCollaborator))];
        }

        private bool IsEventOccurInGivenWeek(DateOnly startDateOfWeek, DateOnly endDateOfWeek, EventCollaborator eventCollaborator)
        {
            return eventCollaborator.EventDate >= startDateOfWeek
                   && eventCollaborator.EventDate <= endDateOfWeek
                   && eventCollaborator.UserId == GlobalData.GetUser().Id
                   && !IsProposedEvent(eventCollaborator.EventId);
        }

        public string GenerateWeeklyView()
        {
            StringBuilder weeklyView = new();

            DateOnly startDateOfWeek = DateTimeManager.GetStartDateOfWeek(DateOnly.FromDateTime(DateTime.Today));
            DateOnly endDateOfWeek = DateTimeManager.GetEndDateOfWeek(DateOnly.FromDateTime(DateTime.Today));

            Dictionary<DateOnly, List<int>> currentWeekEvents = GetGivenWeekEventsWithDate(startDateOfWeek, endDateOfWeek);

            weeklyView.AppendLine(PrintService.GetHorizontalLine());

            weeklyView.AppendLine("Schedule from date :- " + startDateOfWeek + " to date :- " + endDateOfWeek + "\n");

            List<List<string>> weeklyViewTableContent = Generate2DListForWeeklyEvents(startDateOfWeek, endDateOfWeek,
                                                                                              currentWeekEvents);

            weeklyView.AppendLine(PrintService.GenerateTable(weeklyViewTableContent));

            return weeklyView.ToString();
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

        public Dictionary<DateOnly, List<int>> GetGivenMonthEventsWithDate(DateOnly startDateOfMonth, DateOnly endDateOfMonth)
        {
            List<EventCollaborator> eventCollaborators = _eventCollaboratorsService.GetAllEventCollaborators();

            Dictionary<DateOnly, List<int>> currentMonthEvents = GenerateDictionaryOfDateOnlyAndEventIdFromList
                                    (GetGivenMonthEvents(eventCollaborators, startDateOfMonth, endDateOfMonth));

            return currentMonthEvents;
        }

        private bool IsEventOccurInGivenMonth(DateOnly startDateOfMonth, DateOnly endDateOfMonth,
                                             EventCollaborator eventCollaborator)
        {
            return eventCollaborator.EventDate >= startDateOfMonth
                   && eventCollaborator.EventDate <= endDateOfMonth
                   && eventCollaborator.UserId == GlobalData.GetUser().Id
                   && !IsProposedEvent(eventCollaborator.EventId);
        }

        private List<EventCollaborator> GetGivenMonthEvents(List<EventCollaborator> eventCollaborators, DateOnly startDateOfMonth, DateOnly endDateOfMonth)
        {
            return [..eventCollaborators.Where(eventCollaborator => IsEventOccurInGivenMonth
                                                                    (startDateOfMonth, endDateOfMonth, eventCollaborator))];
        }

        private static Dictionary<DateOnly, List<int>> GenerateDictionaryOfDateOnlyAndEventIdFromList(List<EventCollaborator> eventCollaborators)
        {
            return eventCollaborators.GroupBy(eventCollaborator => eventCollaborator.EventDate)
                                           .Select(eventCollaborator => new
                                           {
                                               ScheduleDate = eventCollaborator.Key,
                                               EventCollaboratorIds = eventCollaborator.Select(eventCollaborator =>
                                                                                               eventCollaborator.EventId)
                                                                                       .ToList()
                                           })
                                           .ToDictionary(key => key.ScheduleDate, val => val.EventCollaboratorIds);
        }

        public string GenerateMonthView()
        {
            StringBuilder monthlyView = new();

            DateOnly startDateOfMonth = DateTimeManager.GetStartDateOfMonth(DateTime.Now);
            DateOnly endDateOfMonth = DateTimeManager.GetEndDateOfMonth(DateTime.Now);

            Dictionary<DateOnly, List<int>> currentMonthEvents = GetGivenMonthEventsWithDate(startDateOfMonth, endDateOfMonth);

            monthlyView.AppendLine(PrintService.GetHorizontalLine());

            monthlyView.AppendLine("Schedule from date :- " + startDateOfMonth + " to date :- " + endDateOfMonth + "\n");

            List<List<string>> monthlyViewTableContent = Generate2DListForMonthlyEvents(ref startDateOfMonth, endDateOfMonth, currentMonthEvents);

            monthlyView.Append(PrintService.GenerateTable(monthlyViewTableContent));

            return monthlyView.ToString();
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