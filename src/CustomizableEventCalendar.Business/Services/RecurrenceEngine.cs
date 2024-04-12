using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Enums;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Models;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services
{
    internal class RecurrenceEngine
    {
        private readonly ParticipantService eventCollaboratorsService = new();

        private readonly List<DateOnly> occurrences;

        public RecurrenceEngine() => this.occurrences = [];

        public void ScheduleEvents(EventModel eventModel)
        {
            FindOccurrencesOfEvent(eventModel);

            ScheduleEventsFromOccurrences(eventModel);
        }

        public List<DateOnly> FindOccurrencesOfEvent(EventModel eventModel)
        {
            occurrences.Clear();

            if (eventModel.RecurrencePattern.Frequency == Frequency.None)
                FindOccurrenceOfNonRecurrenceEvents(eventModel);
            else
                FindOccurrencesOfEventUsingFrequency(eventModel);

            return new(occurrences);
        }

        private void FindOccurrencesOfEventUsingFrequency(EventModel eventModel)
        {
            Dictionary<Frequency, Action> frequencyActionDictionary = new()
            {
                { Frequency.Daily,()=> FindOccurrenceOfDailyEvents(eventModel)},
                { Frequency.Weekly , ()=> FindOccurrencesOfWeeklyEvents(eventModel) },
                { Frequency.Monthly , () => FindOccurrencesOfMonthlyEvents(eventModel)},
                { Frequency.Yearly , ()=> FindOccurrencesOfYearlyEvents(eventModel)}
            };

            if (eventModel.RecurrencePattern.Frequency == Frequency.None) return;

            if (frequencyActionDictionary.TryGetValue(eventModel.RecurrencePattern.Frequency, out Action? actionMethod))
            {
                actionMethod.Invoke();
            }
        }

        private void ScheduleEventsFromOccurrences(EventModel eventModel)
        {
            foreach (var occurrence in occurrences)
            {
                ScheduleEventsForEachParticipant(eventModel, occurrence);
            }
        }

        private void ScheduleEventsForEachParticipant(EventModel eventModel, DateOnly occurrence)
        {
            foreach (ParticipantModel participant in eventModel.Participants)
            {
                participant.EventDate = occurrence;
                eventCollaboratorsService.InsertParticipant(participant, eventModel.Id);
            }
        }

        private void FindOccurrenceOfNonRecurrenceEvents(EventModel eventModel)
        {
            occurrences.Add(eventModel.RecurrencePattern.StartDate);
        }

        private void FindOccurrenceOfDailyEvents(EventModel eventModel)
        {
            HashSet<int> days = [.. eventModel.RecurrencePattern.ByWeekDay ?? ([])];

            DateOnly currentDate = eventModel.RecurrencePattern.StartDate;

            while (currentDate <= eventModel.RecurrencePattern.EndDate)
            {
                int day = DateTimeManager.GetDayNumberFromWeekDay(currentDate);

                if (IsDailyEventValidForScheduling(eventModel.RecurrencePattern.Interval, days, day))
                {
                    occurrences.Add(currentDate);
                }
                currentDate = currentDate.AddDays(eventModel.RecurrencePattern.Interval == null ? 1 : Convert.ToInt32(eventModel.RecurrencePattern.Interval));
            }
        }

        private static bool IsDailyEventValidForScheduling(int? eventInterval, HashSet<int> days, int day)
        {
            return eventInterval != null || days.Contains(day);
        }

        private void FindOccurrencesOfWeeklyEvents(EventModel eventModel)
        {
            HashSet<int> weekDays = [.. eventModel.RecurrencePattern.ByWeekDay ?? ([])];

            DateOnly startDateOfWeek = DateTimeManager.GetStartDateOfWeek(eventModel.RecurrencePattern.StartDate);
            DateOnly endDateOfWeek = DateTimeManager.GetEndDateOfWeek(eventModel.RecurrencePattern.StartDate);

            DateOnly currentDate = eventModel.RecurrencePattern.StartDate;

            while (currentDate <= eventModel.RecurrencePattern.EndDate)
            {
                int day = DateTimeManager.GetDayNumberFromWeekDay(currentDate);

                if (weekDays.Contains(day) && IsDateInRange(eventModel.RecurrencePattern.StartDate, eventModel.RecurrencePattern.EndDate, currentDate))
                {
                    occurrences.Add(currentDate);
                }

                currentDate = currentDate.AddDays(1);

                if (currentDate > endDateOfWeek)
                {
                    startDateOfWeek = startDateOfWeek.AddDays(7 * (int)eventModel.RecurrencePattern.Interval);
                    currentDate = startDateOfWeek;
                    endDateOfWeek = DateTimeManager.GetEndDateOfWeek(currentDate);
                }
            }
        }

        private static bool IsDateInRange(DateOnly startDate, DateOnly endDate, DateOnly dateToCheck)
        {
            return dateToCheck >= startDate && dateToCheck <= endDate;
        }

        private void FindOccurrencesOfMonthlyEvents(EventModel eventModel)
        {

            if (eventModel.RecurrencePattern.ByMonthDay == null)
            {
                FindOccurrencesOfEventsUsingWeekOrderAndWeekDay(eventModel, true);
            }
            else
            {
                FindOccurrencesOfEventsUsingMonthDay(eventModel, true);
            }

        }

        private void FindOccurrencesOfYearlyEvents(EventModel eventModel)
        {
            if (eventModel.RecurrencePattern.ByMonthDay == null)
            {
                FindOccurrencesOfEventsUsingWeekOrderAndWeekDay(eventModel, false);
            }
            else
            {
                FindOccurrencesOfEventsUsingMonthDay(eventModel, false);
            }
        }

        private void FindOccurrencesOfEventsUsingMonthDay(EventModel eventModel, bool isMonthly)
        {
            int day = (int)eventModel.RecurrencePattern.ByMonthDay;

            DateOnly startDateOfEvent = eventModel.RecurrencePattern.StartDate;

            int month = isMonthly ? startDateOfEvent.Month : (int)eventModel.RecurrencePattern.ByMonth;

            DateOnly currentDate = new(startDateOfEvent.Year, month, GetMinimumDateFromGivenMonthAndDay(day, month, startDateOfEvent.Year));

            while (currentDate <= eventModel.RecurrencePattern.EndDate)
            {
                occurrences.Add(currentDate);

                currentDate = isMonthly ? currentDate.AddMonths((int)eventModel.RecurrencePattern.Interval) : currentDate.AddYears((int)eventModel.RecurrencePattern.Interval);

                currentDate = new DateOnly(currentDate.Year, currentDate.Month, GetMinimumDateFromGivenMonthAndDay(day, currentDate.Month, currentDate.Year));
            }
        }

        private static int GetMinimumDateFromGivenMonthAndDay(int day, int month, int year)
        {
            int daysInMonth = DateTime.DaysInMonth(year, month);

            return Math.Min(day, daysInMonth);
        }

        private void FindOccurrencesOfEventsUsingWeekOrderAndWeekDay(EventModel eventModel, bool isMonthly)
        {
            int weekOrder = (int)eventModel.RecurrencePattern.WeekOrder;

            int weekDay = eventModel.RecurrencePattern.ByWeekDay[0];
            if (weekDay == 7) weekDay = 0;

            DayOfWeek dayOfWeek = (DayOfWeek)weekDay;

            int month = isMonthly ? eventModel.RecurrencePattern.StartDate.Month : (int)eventModel.RecurrencePattern.ByMonth;

            DateOnly currentDate = new(eventModel.RecurrencePattern.StartDate.Year, month, 1);

            while (currentDate <= eventModel.RecurrencePattern.EndDate)
            {
                DateOnly firstDayOfMonth = new(currentDate.Year, currentDate.Month, 1);

                DateOnly nthWeekDay = GetNthWeekDayDate(firstDayOfMonth, dayOfWeek, weekOrder);

                if (nthWeekDay.Month == currentDate.Month && nthWeekDay <= eventModel.RecurrencePattern.EndDate)
                {
                    occurrences.Add(currentDate);
                }

                currentDate = isMonthly ? currentDate.AddMonths((int)eventModel.RecurrencePattern.Interval) : currentDate.AddYears((int)eventModel.RecurrencePattern.Interval);
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