using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;
using Ical.Net;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using Ical.Net.Serialization;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services
{
    internal class OverlappingEventService
    {
        public RecurrencePattern GenerateRecurrencePattern(RecurrencePatternCustom recurrencePatternCustom)
        {
            RecurrencePattern recurrencePattern = new RecurrencePattern();
            AddFrequencyInPattern(recurrencePatternCustom, recurrencePattern);
            return recurrencePattern;
        }

        public void AddFrequencyInPattern(RecurrencePatternCustom recurrencePatternCustom, RecurrencePattern recurrencePattern)
        {

            AddInterval(recurrencePatternCustom.INTERVAL, recurrencePattern);

            switch (recurrencePatternCustom.FREQ)
            {
                case null:
                    recurrencePattern.Frequency = FrequencyType.Daily;
                    break;
                case "daily":
                    recurrencePattern.Frequency = FrequencyType.Daily;
                    AddByDay(recurrencePatternCustom.BYDAY, recurrencePattern);
                    break;
                case "weekly":
                    recurrencePattern.Frequency = FrequencyType.Weekly;
                    AddByDay(recurrencePatternCustom.BYDAY, recurrencePattern);
                    break;
                case "monthly":
                    recurrencePattern.Frequency = FrequencyType.Monthly;
                    AddByMonthDay(recurrencePatternCustom.BYMONTHDAY, recurrencePattern);
                    break;
                case "yearly":
                    recurrencePattern.Frequency = FrequencyType.Yearly;
                    AddByMonthDay(recurrencePatternCustom.BYMONTHDAY, recurrencePattern);
                    AddByMonth(recurrencePatternCustom.BYMONTH, recurrencePattern);
                    break;
            }
        }

        public void AddInterval(string interval, RecurrencePattern recurrencePattern)
        {
            if (interval == null || interval.Equals("0")) return;
            recurrencePattern.Interval = Convert.ToInt32(interval);
        }
        public void AddByDay(string ByDay, RecurrencePattern recurrencePattern)
        {
            List<int> days = ByDay.Split(',').Select(day => Convert.ToInt32(day)).ToList();

            List<WeekDay> weekdays = [];

            foreach (int day in days)
            {
                switch (day)
                {
                    case 1:
                        weekdays.Add(new WeekDay(DayOfWeek.Monday));
                        break;
                    case 2:
                        weekdays.Add(new WeekDay(DayOfWeek.Tuesday));
                        break;
                    case 3:
                        weekdays.Add(new WeekDay(DayOfWeek.Wednesday));
                        break;
                    case 4:
                        weekdays.Add(new WeekDay(DayOfWeek.Thursday));
                        break;
                    case 5:
                        weekdays.Add(new WeekDay(DayOfWeek.Friday));
                        break;
                    case 6:
                        weekdays.Add(new WeekDay(DayOfWeek.Saturday));
                        break;
                    case 7:
                        weekdays.Add(new WeekDay(DayOfWeek.Sunday));
                        break;
                }
            }

            recurrencePattern.ByDay = weekdays;
        }

        public void AddByMonthDay(string ByMonthDay, RecurrencePattern recurrencePattern)
        {
            List<int> monthDays = ByMonthDay.Split(',').Select(day => Convert.ToInt32(day)).ToList();
            recurrencePattern.ByMonthDay = monthDays;
        }

        public void AddByMonth(string ByMonth, RecurrencePattern recurrencePattern)
        {
            List<int> months = ByMonth.Split(',').Select(day => Convert.ToInt32(day)).ToList();
            recurrencePattern.ByMonth = months;
        }

        public bool IsOverlappingEvent(RecurrencePatternCustom recurrencePatternCustom)
        {

            RecurrencePattern rrule = GenerateRecurrencePattern(recurrencePatternCustom);

            var event1 = new CalendarEvent
            {
                Start = new CalDateTime(recurrencePatternCustom.DTSTART),
                End = new CalDateTime(recurrencePatternCustom.UNTILL),
                RecurrenceRules = new List<RecurrencePattern>
                {
                    rrule
                }
            };

            var serializer = new CalendarSerializer();

            var event1Serialized = serializer.SerializeToString(new Calendar { Events = { event1 } });

            var calendar1 = Calendar.Load(event1Serialized);

            EventService eventService = new EventService();

            List<Event> events = eventService.Read()
                                             .Where(eventObj => eventObj.UserId == GlobalData.user.Id)
                                             .ToList();

            RecurrenceService recurrenceService = new RecurrenceService();

            List<RecurrencePatternCustom> recurrencePatternCustoms = events.Select(eventObj => recurrenceService
                                                                                    .Read(eventObj.RecurrenceId))
                                                                           .ToList();

            foreach (var recurrencePattern in recurrencePatternCustoms)
            {
                RecurrencePattern rrule1 = GenerateRecurrencePattern(recurrencePattern);

                var event2 = new CalendarEvent
                {
                    Start = new CalDateTime(recurrencePattern.DTSTART),
                    End = new CalDateTime(recurrencePattern.UNTILL),
                    RecurrenceRules = new List<RecurrencePattern>
                    {
                        rrule1
                    }
                };

                var event2Serialized = serializer.SerializeToString(new Calendar { Events = { event2 } });

                var calendar2 = Calendar.Load(event2Serialized);

                var occurrences1 = calendar1.GetOccurrences(event1.Start, event1.End).ToList();
                var occurrences2 = calendar2.GetOccurrences(event2.Start, event2.End).ToList();

                foreach (var occurrence1 in occurrences1)
                {
                    foreach (var occurrence2 in occurrences2)
                    {
                        if (occurrence1.Period.CollidesWith(occurrence2.Period))
                        {
                            return true;
                        }
                    }
                }

            }

            return false;
        }
    }
}
