using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services
{
    internal class MultipleInviteesEventService
    {
        public void AddInviteesInProposedEvent(int eventId, string invitees)
        {
            HashSet<int> invitedUsers = invitees.Split(",")
                                                .Select(invitedUser => Convert.ToInt32(invitedUser))
                                                .ToHashSet();

            EventCollaboratorsService eventCollaboratorsService = new EventCollaboratorsService();

            foreach (int userId in invitedUsers)
            {
                eventCollaboratorsService.Create(new EventCollaborators(userId, eventId));
            }

        }
    }
}
