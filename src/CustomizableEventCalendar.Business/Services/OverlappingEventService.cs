using CustomizableEventCalendar.src.CustomizableEventCalendar.ConsoleApp;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services
{
    internal class OverlappingEventService
    {
        public Object? GetOverlappedEventInformation(Event eventForVerify, bool isInsert)
        {
            RecurrenceEngine recurrenceEngine = new();

            List<DateOnly> occurrencesOfEventForVerify = recurrenceEngine.FindOccurrencesOfEvent(eventForVerify);

            EventService eventService = new();

            List<Event> events = eventService.GetAllEventsOfLoggedInUser();

            foreach (var eventToCheckOverlap in events)
            {
                if (!isInsert && eventToCheckOverlap.Id == eventForVerify.Id) continue;

                List<DateOnly> occurrencesOfEventToCheckOverlap = recurrenceEngine.FindOccurrencesOfEvent(eventToCheckOverlap);

                DateOnly matchedDate = occurrencesOfEventToCheckOverlap.Intersect(occurrencesOfEventForVerify).FirstOrDefault();

                if (matchedDate == default) continue;

                if (IsHourOverlaps(eventForVerify.EventStartHour, eventForVerify.EventEndHour, eventToCheckOverlap.EventStartHour, eventToCheckOverlap.EventEndHour))
                {
                    return new { OverlappedEvent = eventToCheckOverlap, MatchedDate = matchedDate };
                }
            }

            return null;
        }

        private static bool IsHourOverlaps(int startHourOfFirstEvent, int endHourOfFirstEvent, int startHourOfSecondEvent, int endHourOfSecondEvent)
        {
            return (startHourOfFirstEvent >= startHourOfSecondEvent && startHourOfFirstEvent < endHourOfSecondEvent)
                || (endHourOfFirstEvent > startHourOfSecondEvent && endHourOfFirstEvent <= endHourOfSecondEvent)
                || (startHourOfSecondEvent >= startHourOfFirstEvent && startHourOfSecondEvent < endHourOfFirstEvent)
                || (endHourOfSecondEvent > startHourOfFirstEvent && endHourOfSecondEvent <= endHourOfFirstEvent);
        }

    }
}