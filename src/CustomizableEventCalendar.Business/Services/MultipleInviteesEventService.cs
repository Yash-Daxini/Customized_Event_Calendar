using System.Data;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services
{
    internal class MultipleInviteesEventService
    {
        public void StartSchedulingProcessOfProposedEvent()
        {
            List<Event> events = GetProposedEvents();

            foreach (var eventObj in events)
            {
                int remainingDays = CalculateDayDifferenceBetweenTwoDates(DateTime.Now, DateTime.Parse(eventObj.EventStartDate.ToString()));

                if (remainingDays <= 1)
                {
                    CalculateMutualTime(eventObj);
                    StartProcessOfConvertingProposedEventToScheduleEvent(eventObj);
                    ScheduleProposedEvent(eventObj);
                }
                else if (!IsAnyInviteeWithPendingConfirmationStatus(eventObj))
                {
                    CalculateMutualTime(eventObj);
                    UpdateEventCollaboratorsStatus(eventObj);
                }
            }
        }

        private static bool IsAnyInviteeWithPendingConfirmationStatus(Event eventObj)
        {
            return GetInviteesOfEvent(eventObj.Id)
                   .Exists(eventCollaborator => eventCollaborator.ConfirmationStatus != null
                                               && eventCollaborator.ConfirmationStatus.Equals("pending"));
        }

        private static List<Event> GetProposedEvents()
        {
            EventService eventService = new();

            return [.. eventService.GetAllEventsOfLoggedInUser().Where(eventObj => eventObj.IsProposed)];
        }

        private static int CalculateDayDifferenceBetweenTwoDates(DateTime firstDate, DateTime secondDate)
        {
            return Math.Abs((firstDate - secondDate).Days);
        }

        public static void AddInviteesInProposedEvent(Event eventObj, string invitees)
        {
            HashSet<int> invitedUsers = invitees.Split(",")
                                                .Select(invitedUser => Convert.ToInt32(invitedUser.Trim()))
                                                .ToHashSet();

            EventCollaboratorService eventCollaboratorService = new();

            foreach (int userId in invitedUsers)
            {
                eventCollaboratorService.InsertEventCollaborators(new EventCollaborator(eventObj.Id, userId, "participant",
                                                  "pending", null, null, eventObj.EventStartDate));
            }

        }

        private static void CalculateMutualTime(Event eventObj)
        {
            List<EventCollaborator> eventCollaborators = GetInviteesOfEvent(eventObj.Id);

            int[] proposedHours = new int[23];

            foreach (var eventCollaborator in eventCollaborators.Where(IsNeedToConsiderProposedTime))
            {
                if (eventCollaborator.ProposedStartHour != null && eventCollaborator.ProposedEndHour != null)
                    CountProposeHours((int)eventCollaborator.ProposedStartHour, (int)eventCollaborator.ProposedEndHour, ref proposedHours);
            }

            FindMaximumMutualTimeBlock(proposedHours, eventObj);

        }

        private static void StartProcessOfConvertingProposedEventToScheduleEvent(Event eventObj)
        {
            eventObj.IsProposed = false;

            EventService eventService = new();

            eventService.ConvertProposedEventToScheduleEvent(eventObj.Id);
        }

        private static bool IsNeedToConsiderProposedTime(EventCollaborator eventCollaborator)
        {
            return eventCollaborator.ParticipantRole != null
                   && eventCollaborator.ParticipantRole.Equals("participant")
                   && eventCollaborator.ConfirmationStatus != null
                   && eventCollaborator.ConfirmationStatus.Equals("maybe");
        }

        private static void CountProposeHours(int startHour, int endHour, ref int[] proposedHours)
        {
            while (startHour < endHour)
            {
                proposedHours[startHour]++;
                startHour++;
            }
        }

        private static void FindMaximumMutualTimeBlock(int[] proposedHours, Event eventObj)
        {
            int max = proposedHours.Max();
            max = max > 1 ? max : -1;
            int startHour = -1;
            int endHour = -1;
            int timeBlock = eventObj.EventEndHour - eventObj.EventStartHour;

            foreach (var item in proposedHours)
            {
                Console.WriteLine(item);
            }

            for (int i = 0; i < proposedHours.Length; i++)
            {
                if (proposedHours[i] == max && startHour == -1)
                {
                    startHour = i;
                    endHour = i + 1;
                    int j = i;

                    Console.WriteLine(i);

                    while (j < proposedHours.Length && (endHour - startHour) < timeBlock)
                    {
                        if (proposedHours[j] == max) endHour++;
                        else break;
                        Console.WriteLine(j);
                        j++;
                    }
                }
            }

            if (startHour == -1 && endHour == -1)
            {
                startHour = eventObj.EventStartHour;
                endHour = eventObj.EventEndHour;
            }

            UpdateEventTimeBlockToMutualTimeBlock(eventObj, startHour, endHour);

        }

        private static void ScheduleProposedEvent(Event eventObj)
        {
            List<EventCollaborator> eventCollaborators = [.. GetInviteesOfEvent(eventObj.Id).Where(ConsiderableInvitees)];

            DateOnly date = new(eventObj.EventStartDate.Year, eventObj.EventStartDate.Month, eventObj.EventStartDate.Day);

            EventCollaboratorService eventCollaboratorService = new();

            foreach (var eventCollaborator in eventCollaborators.Where(IsInviteePresentInEvent))
            {
                eventCollaborator.EventDate = date;
                eventCollaboratorService.UpdateEventCollaborators(eventCollaborator, eventCollaborator.Id);
            }
        }

        private static bool IsInviteePresentInEvent(EventCollaborator eventCollaborator)
        {
            return eventCollaborator.ConfirmationStatus != null
                   && (eventCollaborator.ConfirmationStatus.Equals("accept")
                       || eventCollaborator.ConfirmationStatus.Equals("maybe"));
        }

        private static void UpdateEventCollaboratorsStatus(Event eventObj)
        {
            List<EventCollaborator> eventCollaborators = [.. GetInviteesOfEvent(eventObj.Id).Where(ConsiderableInvitees)];

            EventCollaboratorService eventCollaboratorService = new();

            foreach (var eventCollaborator in eventCollaborators)
            {
                if (eventCollaborator.ParticipantRole != null && eventCollaborator.ParticipantRole.Equals("organizer"))
                {
                    eventCollaborator.ProposedStartHour = eventObj.EventStartHour;
                    eventCollaborator.ProposedEndHour = eventObj.EventEndHour;
                }
                else
                {
                    eventCollaborator.ProposedStartHour = null;
                    eventCollaborator.ProposedEndHour = null;
                    eventCollaborator.ConfirmationStatus = "pending";
                }
                eventCollaboratorService.UpdateEventCollaborators(eventCollaborator, eventCollaborator.Id);
            }
        }

        private static void UpdateEventTimeBlockToMutualTimeBlock(Event eventObj, int newStartHour, int newEndHour)
        {
            eventObj.EventStartHour = newStartHour;
            eventObj.EventEndHour = newEndHour;

            EventService eventService = new();
            eventService.UpdateProposedEvent(eventObj, eventObj.Id);
        }

        private static bool ConsiderableInvitees(EventCollaborator eventCollaborator)
        {
            return (eventCollaborator.ParticipantRole != null && eventCollaborator.ParticipantRole.Equals("organizer"))
                   || (eventCollaborator.ConfirmationStatus != null && eventCollaborator.ConfirmationStatus.Equals("accept"))
                   || (
                        eventCollaborator.ConfirmationStatus != null
                        && !eventCollaborator.ConfirmationStatus.Equals("reject")
                      )
                   || (eventCollaborator.ConfirmationStatus != null && eventCollaborator.ConfirmationStatus.Equals("maybe"));
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