using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.ConsoleApp
{
    internal static class SharedEventCollaboration
    {
        private readonly static CalendarSharingService _calendarSharingService = new();

        public static void ShowSharedEvents()
        {
            ShareCalendar shareCalendar = new();

            shareCalendar.ViewSharedCalendars();

            int serialNumberOfSharedEvent = ValidatedInputProvider.GetValidatedIntegerBetweenRange("Enter Sr.No of the event which you " +
                                                                             "want to collaborate :- ", 1, _calendarSharingService.GetAvailableEventCollaborations().Count);

            int scheduleEventId = EventCollaboratorIdFromSerialNumber(serialNumberOfSharedEvent);

            SharedEventCollaborationService sharedEventCollaborationService = new();

            sharedEventCollaborationService.AddCollaborator(scheduleEventId);
        }

        public static int EventCollaboratorIdFromSerialNumber(int serialNumberOfSharedEvent)
        {
            return _calendarSharingService.GetAvailableEventCollaborations()[serialNumberOfSharedEvent - 1].Id;
        }
    }
}