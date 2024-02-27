﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.ConsoleApp
{
    internal class SharedEventCollaboration
    {
        static ValidationService validationService = new ValidationService();
        public void ShowSharedEvents()
        {
            ShareCalendar shareCalendar = new ShareCalendar();
            shareCalendar.ViewSharedCalendars();

            int scheduleEventId = ValidatedInputProvider.GetValidatedInteger("Enter Sr.No of the event which you " +
                                                                             "want to collaborate :- ");

            SharedEventCollaborationService sharedEventCollaborationService = new SharedEventCollaborationService();
            sharedEventCollaborationService.AddCollaborator(scheduleEventId);
        }
    }
}