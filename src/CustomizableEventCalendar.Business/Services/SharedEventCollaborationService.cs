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

        public void AddCollaborator(int eventCollaboratorId)
        {
            EventCollaboratorsService eventCollaboratorsService = new();

            EventCollaborators? eventCollaborator = eventCollaboratorsService.GetEventCollaboratorsById(eventCollaboratorId);

            if (eventCollaborator == null) return;

            int eventId = eventCollaborator.EventId;

            EventCollaborators eventCollaborators = new(eventId, GlobalData.user.Id, "participant", "", null, null,
                                                        eventCollaborator.EventDate);

            eventCollaboratorsService.InsertEventCollaborators(eventCollaborators);

        }
    }
}