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
        public static void ShowSharedEvents()
        {
            try
            {
                ShareCalendar shareCalendar = new();

                shareCalendar.ViewSharedCalendars();

                string inputMessage = "Enter Sr.No of the event which you want to collaborate :- ";

                int serialNumberOfSharedEvent = ValidatedInputProvider.GetValidatedIntegerBetweenRange(inputMessage, 1,
                                                CalendarSharingService.GetAvailableEventCollaborations().Count);

                int scheduleEventId = EventCollaboratorIdFromSerialNumber(serialNumberOfSharedEvent);

                SharedEventCollaborationService sharedEventCollaborationService = new();

                sharedEventCollaborationService.AddCollaborator(scheduleEventId);

                PrintHandler.PrintSuccessMessage($"Successfully collaborated on event");
            }
            catch (Exception)
            {
                PrintHandler.PrintErrorMessage("Some error occurred ! Can't collaborate on event");
            }

        }

        private static int EventCollaboratorIdFromSerialNumber(int serialNumberOfSharedEvent)
        {
            return CalendarSharingService.GetAvailableEventCollaborations()[serialNumberOfSharedEvent - 1].Id;
        }
    }
}