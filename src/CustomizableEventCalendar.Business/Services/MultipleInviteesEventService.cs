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
        public void CalculateMutualTime(Event eventObj) //WIP
        {
            EventCollaboratorsService eventCollaboratorsService = new EventCollaboratorsService();

            HashSet<int> eventInviteesIds = GetInviteesOfEvent(eventObj.Id);

            HashSet<int> eventInviteesUserIds = eventInviteesIds.Select(eventCollaboratorsService
                                                                        .GetUserIdFromEventCollaborationId)
                                                                .ToHashSet();

            foreach (int userId in eventInviteesIds)
            {
                 
            }

            CalendarSharingService calendarSharingService = new CalendarSharingService();

            List<SharedCalendar> sharedCalendars = calendarSharingService.GetSharedEvents();

            //HashSet<int> notAvailableUsers = sharedCalendars.Where(sharedCalendar=>sharedCalendar.Event);

            //HashSet<int> notAgreedInvitees =
        }
        public void ScheduleProposedEvent(Event eventObj)
        {
            MakeConfirmEventFromProposedEvent(ref eventObj);

            RecurrenceEngine recurrenceEngine = new RecurrenceEngine();

            HashSet<int> eventCollaboratorsIds = GetInviteesOfEvent(eventObj.Id);

            foreach (var eventCollaboratorsId in eventCollaboratorsIds)
            {
                recurrenceEngine.AddEventToScheduler(eventObj, eventCollaboratorsId);
            }

        }
        public HashSet<int> GetInviteesOfEvent(int eventId)
        {
            EventCollaboratorsService eventCollaboratorsService = new EventCollaboratorsService();

            HashSet<int> eventCollaboratorsId = eventCollaboratorsService.Read()
                                                                      .Where(eventCollaborator =>
                                                                             eventCollaborator.EventId == eventId)
                                                                      .Select(eventCollaborator =>
                                                                             eventCollaborator.Id)
                                                                      .ToHashSet();
            return eventCollaboratorsId;
        }
        public void MakeConfirmEventFromProposedEvent(ref Event eventObj)
        {
            eventObj.IsProposed = false;
        }
    }
}
