using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services
{
    internal class SharedEventCollaborationService
    {

        public void AddCollaborator(int scheduleEventId)
        {
            ScheduleEventService scheduleEventService = new();

            ScheduleEvent? scheduleEvent = scheduleEventService.GetScheduleEventById(scheduleEventId);

            if (scheduleEvent == null) return;

            int eventId = scheduleEventService.GetEventIdFromEventCollaborators(scheduleEvent.EventCollaboratorsId);

            EventCollaboratorsService eventCollaboratorsService = new();

            int? eventCollaboratorId = eventCollaboratorsService.GetEventCollaboratorIdFromEventId(eventId);

            if (eventCollaboratorId == null)
            {
                EventCollaborators eventCollaborators = new(GlobalData.user.Id, eventId);
                eventCollaboratorId = eventCollaboratorsService.InsertEventCollaborators(eventCollaborators);
            }

            scheduleEvent.EventCollaboratorsId = Convert.ToInt32(eventCollaboratorId);
            scheduleEventService.Create(scheduleEvent);
        }
    }
}