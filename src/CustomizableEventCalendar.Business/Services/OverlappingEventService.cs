using CustomizableEventCalendar.src.CustomizableEventCalendar.ConsoleApp;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services
{
    internal class OverlappingEventService
    {
        public Object? GetOverlappedEventInformation(Event eventForVerify, bool isInsert)
        {

            List<DateTime> occurrencesOfEventForVerify = [];

            FindOccurrencesForSpecificHour(eventForVerify, ref occurrencesOfEventForVerify);

            EventService eventService = new();

            List<Event> events = eventService.GetAllEventsOfLoggedInUser();

            foreach (var eventToCheckOverlap in events)
            {
                if (!isInsert && eventToCheckOverlap.Id == eventForVerify.Id) continue;

                List<DateTime> occurrencesOfEventToCheckOverlap = [];

                FindOccurrencesForSpecificHour(eventToCheckOverlap, ref occurrencesOfEventToCheckOverlap);

                foreach (var occurrence in occurrencesOfEventForVerify)
                {
                    DateTime matchedDate = occurrencesOfEventToCheckOverlap.Find(singlOccurrence => singlOccurrence == occurrence);
                    if (matchedDate != new DateTime())
                    {
                        return new { OverlappedEvent = eventToCheckOverlap, MatchedDate = DateOnly.FromDateTime(matchedDate) };
                    }
                }

            }

            return null;
        }

        private static void FindOccurrencesForSpecificHour(Event eventObj, ref List<DateTime> occurrences)
        {
            RecurrenceEngine recurrenceEngine = new();

            List<DateOnly> occurrencesInDateOnly = recurrenceEngine.FindOccurrencesOfEvent(eventObj);

            foreach (var occurrenceInDateOnly in occurrencesInDateOnly)
            {

                DateOnly date = occurrenceInDateOnly;

                int startHour = eventObj.EventStartHour;
                int endHour = eventObj.EventEndHour;

                while (startHour < endHour)
                {
                    occurrences.Add(new DateTime(date.Year, date.Month, date.Day, startHour, 0, 0));

                    startHour++;
                }
            }

        }
    }
}