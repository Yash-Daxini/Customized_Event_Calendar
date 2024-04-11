using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Models;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services
{
    internal class OverlappingEventService
    {
        public OverlappingEventData? GetOverlappedEventInformation(EventModel eventForVerify, bool isInsert)
        {
            RecurrenceEngine recurrenceEngine = new();

            List<DateOnly> occurrencesOfEventForVerify = recurrenceEngine.FindOccurrencesOfEvent(eventForVerify);

            EventService eventService = new();

            List<EventModel> events = eventService.GetAllEventsOfLoggedInUser();

            foreach (var existingEvent in events)
            {
                if (!isInsert && existingEvent.Id == eventForVerify.Id) continue;

                List<DateOnly> occurrencesOfEventToCheckOverlap = recurrenceEngine.FindOccurrencesOfEvent(existingEvent);

                DateOnly matchedDate = occurrencesOfEventToCheckOverlap.Intersect(occurrencesOfEventForVerify).FirstOrDefault();

                if (matchedDate == default) continue;

                if (IsHourOverlaps(eventForVerify.Duration.StartHour, eventForVerify.Duration.EndHour, existingEvent.Duration.StartHour, existingEvent.Duration.EndHour))
                {
                    return new OverlappingEventData(eventForVerify,existingEvent, matchedDate);
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