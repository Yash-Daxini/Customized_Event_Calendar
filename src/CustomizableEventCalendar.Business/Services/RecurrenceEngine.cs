using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services
{
    internal class RecurrenceEngine
    {
        private readonly EventCollaboratorService eventCollaboratorsService = new();

        public void ScheduleEvents(Event eventObj)
        {
            if (eventObj.Frequency == null)
            {
                ScheduleNonRecurrenceEvents(eventObj);
                return;
            }

            ScheduleEventsUsingFrequency(eventObj);
        }

        private void ScheduleEventsUsingFrequency(Event eventObj)
        {
            Dictionary<string, Action> frequencyActionDictionary = new()
            {
                { "daily",()=> ScheduleDailyEvents(eventObj)},
                { "weekly" , ()=> ScheduleWeeklyEvents(eventObj) },
                { "monthly" , () => ScheduleMonthlyEvents(eventObj)},
                { "yearly" , ()=> ScheduleYearlyEvents(eventObj)}
            };

            if (eventObj.Frequency == null) return;

            if (frequencyActionDictionary.TryGetValue(eventObj.Frequency, out Action? actionMethod))
            {
                actionMethod.Invoke();
            }
        }

        private void ScheduleEvent(EventCollaborator eventCollaborators)
        {
            eventCollaboratorsService.InsertEventCollaborators(eventCollaborators);
        }

        private void ScheduleNonRecurrenceEvents(Event eventObj)
        {
            EventCollaborator eventCollaborator = new(eventObj.Id, GlobalData.GetUser().Id, "organizer", "accept", eventObj.EventStartHour, eventObj.EventEndHour, eventObj.EventStartDate);

            ScheduleEvent(eventCollaborator);
        }

        private void ScheduleDailyEvents(Event eventObj)
        {
            HashSet<int> days = eventObj.ByWeekDay == null ? [] : [.. eventObj.ByWeekDay.Split(",").Select(day => Convert.ToInt32(day))];

            DateOnly currentDate = eventObj.EventStartDate;

            while (currentDate <= eventObj.EventEndDate)
            {
                int day = DateTimeManager.GetDayNumberFromWeekDay(currentDate);

                if (IsDailyEventValidForScheduling(eventObj.Interval, days, day))
                {
                    ScheduleEvent(ConstructScheduleEventObject(eventObj, currentDate));
                }
                currentDate = currentDate.AddDays(eventObj.Interval == null ? 1 : Convert.ToInt32(eventObj.Interval));
            }
        }

        private EventCollaborator ConstructScheduleEventObject(Event eventObj, DateOnly eventDate)
        {
            return new(eventObj.Id, GlobalData.GetUser().Id, "organizer", "accept", eventObj.EventStartHour, eventObj.EventEndHour, eventDate);
        }

        private static bool IsDailyEventValidForScheduling(int? eventInterval, HashSet<int> days, int day)
        {
            return eventInterval != null || days.Contains(day);
        }

        private void ScheduleWeeklyEvents(Event eventObj)
        {
            HashSet<int> weekDays = eventObj.ByWeekDay == null ? [] : [.. eventObj.ByWeekDay.Split(",").Select(weekDay => Convert.ToInt32(weekDay))];

            DateOnly startDateOfWeek = DateTimeManager.GetStartDateOfWeek(eventObj.EventStartDate);
            DateOnly endDateOfWeek = DateTimeManager.GetEndDateOfWeek(eventObj.EventStartDate);

            DateOnly currentDate = eventObj.EventStartDate;

            while (currentDate <= eventObj.EventEndDate)
            {
                int day = DateTimeManager.GetDayNumberFromWeekDay(currentDate);

                if (weekDays.Contains(day) && IsDateInRange(eventObj.EventStartDate, eventObj.EventEndDate, currentDate))
                {
                    ScheduleEvent(ConstructScheduleEventObject(eventObj, currentDate));
                }

                currentDate = currentDate.AddDays(1);

                if (currentDate > endDateOfWeek)
                {
                    startDateOfWeek = startDateOfWeek.AddDays(7 * (int)eventObj.Interval);
                    endDateOfWeek = DateTimeManager.GetEndDateOfWeek(currentDate);
                    currentDate = startDateOfWeek;
                }
            }
        }

        private static bool IsDateInRange(DateOnly startDate, DateOnly endDate, DateOnly dateToCheck)
        {
            return dateToCheck >= startDate && dateToCheck <= endDate;
        }

        private void ScheduleMonthlyEvents(Event eventObj)
        {

            if (eventObj.ByMonthDay == null)
            {
                ScheduleEventsUsingWeekOrderAndWeekDay(eventObj, true);
            }
            else
            {
                ScheduleEventsUsingMonthDay(eventObj, true);
            }

        }

        private void ScheduleYearlyEvents(Event eventObj)
        {
            if (eventObj.ByMonthDay == null)
            {
                ScheduleEventsUsingWeekOrderAndWeekDay(eventObj, false);
            }
            else
            {
                ScheduleEventsUsingMonthDay(eventObj, false);
            }
        }

        private void ScheduleEventsUsingMonthDay(Event eventObj, bool isMonthly)
        {
            int day = (int)eventObj.ByMonthDay;

            DateOnly startDateOfEvent = eventObj.EventStartDate;

            int month = isMonthly ? startDateOfEvent.Month : (int)eventObj.ByMonth;

            DateOnly currentDate = new(startDateOfEvent.Year, month, GetMinimumDateFromGivenMonthAndDay(day, startDateOfEvent));

            while (currentDate <= eventObj.EventEndDate)
            {
                ScheduleEvent(ConstructScheduleEventObject(eventObj, currentDate));

                currentDate = isMonthly ? currentDate.AddMonths((int)eventObj.Interval) : currentDate.AddYears((int)eventObj.Interval);

                currentDate = new DateOnly(currentDate.Year, currentDate.Month, GetMinimumDateFromGivenMonthAndDay(day, currentDate));
            }
        }

        private static int GetMinimumDateFromGivenMonthAndDay(int day, DateOnly date)
        {
            int daysInMonth = DateTime.DaysInMonth(date.Year, date.Month);

            return Math.Min(day, daysInMonth);
        }

        private void ScheduleEventsUsingWeekOrderAndWeekDay(Event eventObj, bool isMonthly)
        {
            int weekOrder = (int)eventObj.WeekOrder;

            int weekDay = Convert.ToInt32(eventObj.ByWeekDay.Split(",")[0]);
            if (weekDay == 7) weekDay = 0;

            DayOfWeek dayOfWeek = (DayOfWeek)weekDay;

            int month = isMonthly ? eventObj.EventStartDate.Month : (int)eventObj.ByMonth;

            DateOnly currentDate = new(eventObj.EventStartDate.Year, month, 1);

            while (currentDate <= eventObj.EventEndDate)
            {
                DateOnly firstDayOfMonth = new(currentDate.Year, currentDate.Month, 1);

                DateOnly nthWeekDay = GetNthWeekDayDate(firstDayOfMonth, dayOfWeek, weekOrder);

                if (nthWeekDay.Month == currentDate.Month && nthWeekDay <= eventObj.EventEndDate)
                {
                    ScheduleEvent(ConstructScheduleEventObject(eventObj, nthWeekDay));
                }

                currentDate = isMonthly ? currentDate.AddMonths((int)eventObj.Interval) : currentDate.AddYears((int)eventObj.Interval);
            }
        }

        private static DateOnly GetNthWeekDayDate(DateOnly firstDayOfMonth, DayOfWeek dayOfWeek, int weekOrder)
        {
            if (weekOrder == 5) return GetLastWeekDayDate(firstDayOfMonth.Year, firstDayOfMonth.Month, dayOfWeek);

            DateOnly firstWeekDayDate = firstDayOfMonth;

            while (firstWeekDayDate.DayOfWeek != dayOfWeek)
            {
                firstWeekDayDate = firstWeekDayDate.AddDays(1);
            }

            DateOnly nthWeekDay = firstWeekDayDate.AddDays(7 * (weekOrder - 1));

            return nthWeekDay;
        }

        private static DateOnly GetLastWeekDayDate(int year, int month, DayOfWeek dayOfWeek)
        {
            int totalDaysInGivenMonth = DateTime.DaysInMonth(year, month);

            DateOnly lastDayOfMonth = new(year, month, totalDaysInGivenMonth);

            while (lastDayOfMonth.DayOfWeek != dayOfWeek && lastDayOfMonth.Day >= 1)
            {
                lastDayOfMonth = lastDayOfMonth.AddDays(-1);
            }

            return lastDayOfMonth;
        }
    }
}