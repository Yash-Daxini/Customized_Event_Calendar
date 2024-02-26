using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.ConsoleApp
{
    internal class SharedEventCollaboration
    {
        public void ShowSharedEvents()
        {
            ShareCalendar shareCalendar = new ShareCalendar();
            shareCalendar.ViewSharedCalendars();

            Console.Write("Enter Sr.No of the event which you want to collaborate :- ");
            int scheduleEventId = Convert.ToInt32(Console.ReadLine());

            SharedEventCollaborationService sharedEventCollaborationService = new SharedEventCollaborationService();
            sharedEventCollaborationService.AddCollaborator(scheduleEventId);
        }
    }
}