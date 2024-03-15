using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomizableEventCalendar.src.CustomizableEventCalendar.ConsoleApp;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services
{
    internal class MultipleInviteesEventService
    {
        public void StartSchedulingProcessOfProposedEvent()
        {

            EventService eventService = new();

            List<Event> events = eventService.GetAllEvents()
                                             .Where(eventObj => eventObj.IsProposed
                                              && eventObj.UserId == GlobalData.user.Id)
                                             .ToList();

            foreach (var eventObj in events)
            {
                int remainingDays = (DateTime.Now - DateTime.Parse(eventObj.EventStartDate.ToString())).Days;

                if (remainingDays <= 1)
                {
                    CalculateMutualTime(eventObj);
                    ScheduleProposedEvent(eventObj);
                }
            }
        }

        public static void AddInviteesInProposedEvent(Event eventObj, string invitees)
        {
            HashSet<int> invitedUsers = invitees.Split(",")
                                                .Select(invitedUser => Convert.ToInt32(invitedUser))
                                                .ToHashSet();

            EventCollaboratorService eventCollaboratorService = new();

            foreach (int userId in invitedUsers)
            {
                eventCollaboratorService.InsertEventCollaborators(new EventCollaborator(eventObj.Id, userId, "participant", "pending",
                                                                  null, null,
                                                                  DateTime.Parse(eventObj.EventStartDate.ToString())));
            }

        }

        public static DateOnly GetDateOnlyFromDateTime(DateTime date)
        {
            return DateOnly.FromDateTime(date);
        }

        public void CalculateMutualTime(Event eventObj)
        {
            List<EventCollaborator> eventCollaborators = GetInviteesOfEvent(eventObj.Id);

            int[] proposedHours = new int[23];

            foreach (var eventCollaborator in eventCollaborators)
            {
                if (IsNeedToConsiderProposedTime(eventCollaborator))
                {
                    CountProposeHours((int)eventCollaborator.ProposedStartHour, (int)eventCollaborator.ProposedEndHour, ref proposedHours);
                }

            }

            FindMaximumMutualTimeBlock(proposedHours, eventObj);

            eventObj.IsProposed = false;

            EventService eventService = new();

            eventService.ConvertProposedEventToScheduleEvent(eventObj.Id);

        }

        public static bool IsNeedToConsiderProposedTime(EventCollaborator eventCollaborator)
        {
            return eventCollaborator.ParticipantRole.Equals("participant") &&
                   eventCollaborator.ConfirmationStatus.Equals("reject") &&
                   eventCollaborator.ProposedStartHour != null &&
                   eventCollaborator.ProposedEndHour != null;
        }

        public static void CountProposeHours(int startHour, int endHour, ref int[] proposedHours)
        {
            while (startHour < endHour)
            {
                proposedHours[startHour]++;
                startHour++;
            }

        }

        public static void FindMaximumMutualTimeBlock(int[] proposedHours, Event eventObj)
        {

            int max = 0;

            int maxHour = 0;

            for (int i = 0; i < proposedHours.Length; i++)
            {
                if (proposedHours[i] > max)
                {
                    max = proposedHours[i];
                    maxHour = i;
                }
            }

            eventObj.EventStartHour = maxHour;
            eventObj.EventEndHour = maxHour + 1;

        }

        public static void ScheduleProposedEvent(Event eventObj)
        {
            List<EventCollaborator> eventCollaborators = [..GetInviteesOfEvent(eventObj.Id)
                                                           .Where(eventCollaborator=>ConsiderableInvitees(eventCollaborator))];

            DateTime date = new DateTime(eventObj.EventStartDate.Year, eventObj.EventStartDate.Month, eventObj.EventStartDate.Day,
                            (int)eventObj.EventStartHour, 0, 0);

            EventCollaboratorService eventCollaboratorService = new();

            foreach (var eventCollaborator in eventCollaborators)
            {
                eventCollaborator.EventDate = date;
                eventCollaboratorService.UpdateEventCollaborators(eventCollaborator, eventCollaborator.Id);
            }

        }

        public static bool ConsiderableInvitees(EventCollaborator eventCollaborator)
        {
            if (eventCollaborator.ParticipantRole.Equals("organizer")) return true;
            return eventCollaborator.ConfirmationStatus.Equals("accept") || (eventCollaborator.ConfirmationStatus.Equals("reject")
                   && eventCollaborator.ProposedStartHour != null && eventCollaborator.ProposedEndHour != null)
                   || eventCollaborator.ConfirmationStatus.Equals("maybe");
        }

        public static List<EventCollaborator> GetInviteesOfEvent(int eventId)
        {
            EventCollaboratorService eventCollaboratorService = new();

            List<EventCollaborator> eventCollaborators = [..eventCollaboratorService.GetAllEventCollaborators()
                                                                       .Where(eventCollaborator => eventCollaborator.EventId == eventId)];
            return eventCollaborators;
        }
    }
}