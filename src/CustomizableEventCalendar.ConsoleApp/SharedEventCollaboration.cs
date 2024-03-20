using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.ConsoleApp
{
    internal static class SharedEventCollaboration
    {
        private readonly static CalendarSharingService _calendarSharingService = new();

        private readonly static List<SharedCalendar> sharedCalendarsList = _calendarSharingService.GetSharedCalendars();
        public static void ShowSharedEvents()
        {
            try
            {
                ViewSharedCalendars();

                if (sharedCalendarsList.Count == 0) return;

                int serialNumberOfSharedCalendar = ValidatedInputProvider.GetValidatedIntegerBetweenRange("Select Sr No. which calendar you want to see :- ", 1, CountSharedCalendars());

                ViewSpecificCalendar(sharedCalendarsList[serialNumberOfSharedCalendar - 1].Id);

                int sharedEventsCount = CalendarSharingService.GetAvailableEventCollaborations().Count;

                if (sharedEventsCount == 0)
                {
                    PrintHandler.PrintWarningMessage("There no events available in this shared calendar. So you can't collaborate.");
                    return;
                }

                int serialNumberOfSharedEvent = ValidatedInputProvider.GetValidatedIntegerBetweenRange("Enter Sr.No of the event which you want to collaborate :- ", 1, sharedEventsCount);

                int eventCollaboratorId = EventCollaboratorIdFromSerialNumber(serialNumberOfSharedEvent);

                CollaborateOnSharedEvent(eventCollaboratorId);
            }
            catch (Exception)
            {
                PrintHandler.PrintErrorMessage("Some error occurred ! Can't collaborate on event");
            }

        }

        private static void CollaborateOnSharedEvent(int eventCollaboratorId)
        {
            SharedEventCollaborationService sharedEventCollaborationService = new();

            EventCollaboratorService eventCollaboratorService = new();

            EventCollaborator? eventCollaborator = eventCollaboratorService.GetEventCollaboratorById(eventCollaboratorId);

            if (eventCollaborator == null) return;

            int eventId = eventCollaborator.EventId;

            EventCollaborator newEventCollaborator = new(eventId, GlobalData.GetUser().Id, "participant", null, null, null,
                                                        eventCollaborator.EventDate);

            if (!sharedEventCollaborationService.IsEligibleToCollaborate(newEventCollaborator)) return;

            sharedEventCollaborationService.AddCollaborator(newEventCollaborator);

            PrintHandler.PrintSuccessMessage($"Successfully collaborated on event");
        }

        private static void ViewSharedCalendars()
        {
            try
            {
                string sharedEvents = _calendarSharingService.DesignSharedCalendarDisplayFormat();

                if (CountSharedCalendars() > 0)
                {
                    Console.WriteLine("Calendars shared to you !");
                    Console.WriteLine(sharedEvents);
                }
                else
                {
                    Console.WriteLine("No calendars available !");
                    return;
                }

            }
            catch
            {
                PrintHandler.PrintErrorMessage("Some error occurred !");
            }
        }

        private static int CountSharedCalendars()
        {
            return _calendarSharingService.GetSharedCalendars().Count;
        }

        private static void ViewSpecificCalendar(int sharedCalendarId)
        {
            string calendar = _calendarSharingService.GetSharedEventsFromSharedCalendar(sharedCalendarId);
            Console.WriteLine(calendar);
        }

        private static int EventCollaboratorIdFromSerialNumber(int serialNumberOfSharedEvent)
        {
            return CalendarSharingService.GetAvailableEventCollaborations()[serialNumberOfSharedEvent - 1].Id;
        }
    }
}