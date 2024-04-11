using System.Text;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Enums;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Models;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services
{
    internal static class RecurrencePatternMessageGenerator
    {
        public static string GenerateRecurrenceMessage(EventModel eventModel)
        {
            StringBuilder recurrenceMessage = new();

            if (eventModel.RecurrencePattern.Frequency == null) return GenerateRecurrenceMessageForNonRecurringEvent(eventModel);

            recurrenceMessage.Append(GetCombinationOfIntervalAndFrequency(eventModel));

            if (eventModel.RecurrencePattern.ByWeekDay != null && eventModel.RecurrencePattern.WeekOrder == null) recurrenceMessage.Append($"{GetWeekDays(eventModel.RecurrencePattern.ByWeekDay)} ");

            if (eventModel.RecurrencePattern.ByMonthDay != null) recurrenceMessage.Append($"{GetMonthDay((int)eventModel.RecurrencePattern.ByMonthDay)} day ");

            recurrenceMessage.Append(GetCombinationOfWeekOrderAndWeekDay(eventModel));

            if (eventModel.RecurrencePattern.ByMonth != null) recurrenceMessage.Append($"in {GetMonth((int)eventModel.RecurrencePattern.ByMonth)} ");

            return recurrenceMessage.ToString();
        }

        private static string GenerateRecurrenceMessageForNonRecurringEvent(EventModel eventModel)
        {
            return $"On {eventModel.RecurrencePattern.StartDate} at {DateTimeManager.ConvertTo12HourFormat(eventModel.Duration.StartHour)} " +
                   $" to {DateTimeManager.ConvertTo12HourFormat(eventModel.Duration.EndHour)}";
        }

        private static string GetCombinationOfIntervalAndFrequency(EventModel eventModel)
        {
            if (eventModel.RecurrencePattern.Frequency != null && eventModel.RecurrencePattern.Frequency == Frequency.Daily && eventModel.RecurrencePattern.Interval == null) return $"Every ";
            return $"Every {GetInterval(eventModel.RecurrencePattern.Interval)} {GetFrequency(eventModel.RecurrencePattern.Frequency)} ";
        }

        private static string GetCombinationOfWeekOrderAndWeekDay(EventModel eventModel)
        {
            if (eventModel.RecurrencePattern.WeekOrder != null && eventModel.RecurrencePattern.ByWeekDay != null)
                return $"{GetWeekOrder((int)eventModel.RecurrencePattern.WeekOrder)} {GetWeekDays(eventModel.RecurrencePattern.ByWeekDay)} of week ";
            return "";
        }

        private static string GetInterval(int? interval)
        {
            return interval == null || interval <= 1 ? "" : interval + "";
        }

        private static string GetFrequency(Frequency frequency)
        {
            if (frequency == Frequency.Daily) return "day";
            return frequency.ToString()[..^2] + " on";
        }

        private static string GetWeekDays(List<int> days)
        {
            return DateTimeManager.GetWeekDaysFromWeekDayNumbers(days);
        }

        private static string GetMonthDay(int monthDay)
        {
            if (monthDay == 1) return "1st";
            else if (monthDay == 2) return "2nd";
            else if (monthDay == 3) return "3rd";
            return monthDay + "th";
        }

        private static string GetMonth(int month)
        {
            return DateTimeManager.GetMonthFromMonthNumber(month);
        }

        private static string GetWeekOrder(int weekOrder)
        {
            if (weekOrder == 5) return "last";
            return GetMonthDay(weekOrder);
        }
    }
}
