using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Enums;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Models;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Mapping
{
    internal class RecurrencePatternMapper
    {
        public RecurrencePatternModel MapEventEntityToRecurrencePatternModel(Event eventObj)
        {
            return new RecurrencePatternModel
            {
                StartDate = eventObj.EventStartDate,
                EndDate = eventObj.EventEndDate,
                Frequency = MapFrequencyToEnum(eventObj.Frequency),
                Interval = eventObj.Interval,
                WeekOrder = eventObj.WeekOrder,
                ByWeekDay = MapWeekDayIntoList(eventObj.ByWeekDay),
                ByMonth = eventObj.ByMonth,
                ByMonthDay = eventObj.ByMonthDay,
            };
        }

        private Frequency MapFrequencyToEnum(string? frequency)
        {
            return frequency switch
            {
                "daily" => Frequency.Daily,
                "weekly" => Frequency.Weekly,
                "Monthly" => Frequency.Monthly,
                "Yearly" => Frequency.Yearly,
                _ => Frequency.None,
            };
        }
        
        private string? MapEnumToFrequency(Frequency frequency)
        {
            return frequency switch
            {
                Frequency.Daily => "daily",
                Frequency.Weekly => "weekly",
                Frequency.Monthly => "Monthly",
                Frequency.Yearly => "Yearly",
                Frequency.None => null,
                _ => null,
            };
        }

        private List<int>? MapWeekDayIntoList(string? weekDay)
        {
            if (weekDay == null) return null;
            return [.. weekDay.Split(",").Select(weekDay => int.Parse(weekDay))];
        }

        public Event MapRecurrencePatternModelToEventEntity(RecurrencePatternModel recurrencePatternModel,Event eventObj)
        {
            eventObj.EventStartDate = recurrencePatternModel.StartDate;
            eventObj.EventEndDate = recurrencePatternModel.EndDate;
            eventObj.ByWeekDay = recurrencePatternModel.ByWeekDay.ToString();
            eventObj.WeekOrder = recurrencePatternModel.WeekOrder;
            eventObj.Interval = recurrencePatternModel.Interval;
            eventObj.Frequency = MapEnumToFrequency(recurrencePatternModel.Frequency);
            eventObj.ByMonth = recurrencePatternModel.ByMonthDay;
            eventObj.ByMonthDay = recurrencePatternModel.ByMonthDay;

            return eventObj;
        }
    }
}
