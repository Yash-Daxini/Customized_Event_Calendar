using CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Models;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.ConsoleApp
{
    internal static class OverlapHandler
    {

        private readonly static OverlappingEventService _overlappingEventService = new();

        private static bool AskForRescheduleOverlappedEvent()
        {
            Console.WriteLine("\nAre you want to reschedule event ? \n1. Yes \n2. No");

            int choice = ValidatedInputProvider.GetValidIntegerBetweenRange("\nEnter choice : ", 1, 2);

            return choice == 1;
        }

        private static string GetOverlapMessageFromEvents(EventModel eventForVerify, EventModel eventToCheckOverlap, DateOnly matchedDate)
        {
            return $"\"{eventForVerify.Title}\" overlaps with \"{eventToCheckOverlap.Title}\" at {matchedDate} on following duration\n" +
                   $"1. {DateTimeManager.ConvertTo12HourFormat(eventForVerify.Duration.StartHour)} " +
                   $"- {DateTimeManager.ConvertTo12HourFormat(eventForVerify.Duration.EndHour)} \n" +
                   $"overlaps with " +
                   $"\n2. {DateTimeManager.ConvertTo12HourFormat(eventToCheckOverlap.Duration.StartHour)} " +
                   $"- {DateTimeManager.ConvertTo12HourFormat(eventToCheckOverlap.Duration.EndHour)} \n" +
                   $"\nPlease choose another date time !";
        }

        private static void HandleOverlappedEvent(EventModel eventForVerify, OverlappingEventData overlappedEventInformation, bool isInsert, bool isProposed)
        {
            string overlapEventMessage = GetOverlapMessageFromEvents(overlappedEventInformation.EventInformation, overlappedEventInformation.OverlappedEvent, overlappedEventInformation.MatchedDate);

            PrintHandler.PrintWarningMessage(overlapEventMessage);

            if (!AskForRescheduleOverlappedEvent()) return;

            GetInputToRescheduleOverlappedEvent(eventForVerify, isProposed);

            if (isInsert)
                EventHandling.AddEvent(eventForVerify, isProposed);
            else
                EventHandling.UpdateEvent(eventForVerify.Id, eventForVerify, isProposed);
        }

        public static bool IsOverlappingEvent(EventModel eventModel, bool isInsert, bool isProposed)
        {
            OverlappingEventData? overlappedEventInformation = _overlappingEventService.GetOverlappedEventInformation(eventModel, isInsert);

            if (overlappedEventInformation != null)
            {
                HandleOverlappedEvent(eventModel, overlappedEventInformation, isInsert, isProposed);
                return true;
            }
            return false;
        }

        private static void GetInputToRescheduleOverlappedEvent(EventModel eventModel, bool isProposed)
        {
            TimeHandler.GetStartingAndEndingHourOfEvent(eventModel);

            if (isProposed)
                RecurrenceHandling.GetRecurrenceForSingleEvent(eventModel);
            else
                RecurrenceHandling.AskForRecurrenceChoice(eventModel);
        }
    }
}