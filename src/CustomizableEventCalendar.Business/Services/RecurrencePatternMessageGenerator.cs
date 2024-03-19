using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services
{
    internal static class RecurrencePatternMessageGenerator
    {
        public static string GenerateRecurrenceMessage(Event eventObj)
        {
            StringBuilder recurrenceMessage = new();

            if (eventObj.Frequency == null) return GenerateRecurrenceMessageForNonRecurringEvent(eventObj);

            recurrenceMessage.Append(GetCombinationOfIntervalAndFrequency(eventObj));

            if (eventObj.ByWeekDay != null && eventObj.WeekOrder == null)
                recurrenceMessage.Append($"{GetWeekDays(eventObj.ByWeekDay)} ");

            if (eventObj.ByMonthDay != null) recurrenceMessage.Append($"{GetMonthDay((int)
                                                                      eventObj.ByMonthDay)} day ");

            recurrenceMessage.Append(GetCombinationOfWeekOrderAndWeekDay(eventObj));

            if (eventObj.ByMonth != null) recurrenceMessage.Append($"in {GetMonth((int)
                                                                   eventObj.ByMonth)} ");

            return recurrenceMessage.ToString();
        }

        private static string GenerateRecurrenceMessageForNonRecurringEvent(Event eventObj)
        {
            return $"On {eventObj.EventStartDate} at {DateTimeManager.ConvertTo12HourFormat(eventObj.EventStartHour)} " +
                   $" to {DateTimeManager.ConvertTo12HourFormat(eventObj.EventEndHour)}";
        }

        private static string GetCombinationOfIntervalAndFrequency(Event eventObj)
        {
            if (eventObj.Frequency.Equals("daily") && eventObj.Interval == null) return $"Every ";
            return $"Every {GetInterval(eventObj.Interval)} {GetFrequency(eventObj.Frequency)} on ";
        }

        private static string GetCombinationOfWeekOrderAndWeekDay(Event eventObj)
        {
            if (eventObj.WeekOrder != null && eventObj.ByWeekDay != null)
                return $"{GetWeekOrder((int)eventObj.WeekOrder)} {GetWeekDays(eventObj.ByWeekDay)} of week ";
            return "";
        }

        private static string GetInterval(int? interval)
        {
            return interval == null || interval <= 1 ? "" : interval + "";
        }

        private static string GetFrequency(string frequency)
        {
            if (frequency.Equals("daily")) return "day";
            return frequency[..^2];
        }

        private static string GetWeekDays(string days)
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
            return GetMonthDay(weekOrder);
        }
    }
}
