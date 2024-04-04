using CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.ConsoleApp
{
    internal static class OverlapHandler
    {

        private readonly static OverlappingEventService _overlappingEventService = new();

        private static bool AskForRescheduleOverlappedEvent()
        {
            Console.WriteLine("\nAre you want to reschedule event ? \n1. Yes \n2. No");

            int choice = ValidatedInputProvider.GetValidatedIntegerBetweenRange("\nEnter choice : ", 1, 2);

            return choice == 1;
        }

        private static string GetOverlapMessageFromEvents(Event eventForVerify, Event eventToCheckOverlap, DateOnly matchedDate)
        {
            return $"\"{eventForVerify.Title}\" overlaps with \"{eventToCheckOverlap.Title}\" at {matchedDate} on following duration\n" +
                   $"1. {DateTimeManager.ConvertTo12HourFormat(eventForVerify.EventStartHour)} " +
                   $"- {DateTimeManager.ConvertTo12HourFormat(eventForVerify.EventEndHour)} \n" +
                   $"overlaps with " +
                   $"\n2. {DateTimeManager.ConvertTo12HourFormat(eventToCheckOverlap.EventStartHour)} " +
                   $"- {DateTimeManager.ConvertTo12HourFormat(eventToCheckOverlap.EventEndHour)} \n" +
                   $"\nPlease choose another date time !";
        }

        private static void HandleOverlappedEvent(Event eventForVerify, OverlappingEventData overlappedEventInformation, bool isInsert)
        {
            string overlapEventMessage = GetOverlapMessageFromEvents(eventForVerify, overlappedEventInformation.EventInformation, overlappedEventInformation.MatchedDate);

            PrintHandler.PrintWarningMessage(overlapEventMessage);

            if (!AskForRescheduleOverlappedEvent()) return;

            GetInputToRescheduleOverlappedEvent(eventForVerify);

            if (isInsert)
                EventHandling.AddEvent(eventForVerify);
            else
                EventHandling.UpdateEvent(eventForVerify.Id, eventForVerify);
        }

        public static bool IsOverlappingEvent(Event eventObj, bool isInsert)
        {
            OverlappingEventData? overlappedEventInformation = _overlappingEventService.GetOverlappedEventInformation(eventObj, isInsert);

            if (overlappedEventInformation != null)
            {
                HandleOverlappedEvent(eventObj, overlappedEventInformation, isInsert);
                return true;
            }
            return false;
        }

        private static void GetInputToRescheduleOverlappedEvent(Event eventObj)
        {
            TimeHandler.GetStartingAndEndingHourOfEvent(eventObj);

            if (eventObj.IsProposed)
                RecurrenceHandling.GetRecurrenceForSingleEvent(eventObj);
            else
                RecurrenceHandling.AskForRecurrenceChoice(eventObj);
        }
    }
}