using System.Data;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Enums;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Models;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services
{
    internal class MultipleInviteesEventService
    {
        public void StartSchedulingProcessOfProposedEvent()
        {
            List<EventModel> events = GetProposedEvents();

            foreach (var eventModel in events)
            {
                int remainingDays = CalculateDayDifferenceBetweenTwoDates(DateTime.Now, DateTime.Parse(eventModel.RecurrencePattern.StartDate.ToString()));

                if (remainingDays <= 1)
                {
                    CalculateMutualTime(eventModel);
                    StartProcessOfConvertingProposedEventToScheduleEvent(eventModel);
                    ScheduleProposedEvent(eventModel);
                }
                else if (!IsAnyInviteeWithPendingConfirmationStatus(eventModel))
                {
                    CalculateMutualTime(eventModel);
                    UpdateEventCollaboratorsStatus(eventModel);
                }
            }
        }

        private static bool IsAnyInviteeWithPendingConfirmationStatus(EventModel eventModel)
        {
            return eventModel.Participants.Exists(participant => participant.ConfirmationStatus == ConfirmationStatus.Pending);
        }

        private static List<EventModel> GetProposedEvents()
        {
            EventService eventService = new();

            return eventService.GetProposedEvents();
        }

        private static int CalculateDayDifferenceBetweenTwoDates(DateTime firstDate, DateTime secondDate)
        {
            return Math.Abs((firstDate - secondDate).Days);
        }

        public static void AddInviteesInProposedEvent(EventModel eventModel)
        {
            ParticipantService eventCollaboratorService = new();

            foreach (ParticipantModel participant in eventModel.Participants)
            {
                eventCollaboratorService.InsertParticipant(new ParticipantModel(ParticipantRole.Participant, ConfirmationStatus.Pending, null, null, eventModel.RecurrencePattern.StartDate, participant.User), eventModel.Id);
            }

        }

        private static void CalculateMutualTime(EventModel eventModel)
        {
            int[] proposedHours = new int[23];

            foreach (var participant in eventModel.Participants.Where(IsNeedToConsiderProposedTime))
            {
                if (participant.ProposedStartHour != null && participant.ProposedEndHour != null)
                    CountProposeHours((int)participant.ProposedStartHour, (int)participant.ProposedEndHour, ref proposedHours);
            }

            FindMaximumMutualTimeBlock(proposedHours, eventModel);

        }

        private static void StartProcessOfConvertingProposedEventToScheduleEvent(EventModel eventModel)
        {
            EventService eventService = new();

            eventService.ConvertProposedEventToScheduleEvent(eventModel.Id);
        }

        private static bool IsNeedToConsiderProposedTime(ParticipantModel participant)
        {
            return participant.ParticipantRole == ParticipantRole.Participant
                   && participant.ConfirmationStatus == ConfirmationStatus.Proposed;
        }

        private static void CountProposeHours(int startHour, int endHour, ref int[] proposedHours)
        {
            while (startHour < endHour)
            {
                proposedHours[startHour]++;
                startHour++;
            }
        }

        private static void FindMaximumMutualTimeBlock(int[] proposedHours, EventModel eventModel)
        {
            int max = proposedHours.Max();
            max = max > 1 ? max : -1;
            int startHour = -1;
            int endHour = -1;
            int timeBlock = eventModel.Duration.EndHour - eventModel.Duration.StartHour;

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
                startHour = eventModel.Duration.StartHour;
                endHour = eventModel.Duration.EndHour;
            }

            UpdateEventTimeBlockToMutualTimeBlock(eventModel, startHour, endHour);

        }

        private static void ScheduleProposedEvent(EventModel eventModel)
        {
            DateOnly eventDate = eventModel.RecurrencePattern.StartDate;

            ParticipantService eventCollaboratorService = new();

            foreach (var eventCollaborator in eventModel.Participants.Where(IsInviteePresentInEvent))
            {
                eventCollaborator.EventDate = eventDate;
                eventCollaboratorService.UpdateParticipant(eventCollaborator, eventModel.Id);
            }
        }

        private static bool IsInviteePresentInEvent(ParticipantModel participantModel)
        {
            return participantModel.ConfirmationStatus == ConfirmationStatus.Accept
                   || participantModel.ConfirmationStatus == ConfirmationStatus.Maybe;
        }

        private static void UpdateEventCollaboratorsStatus(EventModel eventModel)
        {
            foreach (var participants in eventModel.Participants)
            {
                if (participants.ParticipantRole == ParticipantRole.Organizer)
                {
                    participants.ProposedStartHour = eventModel.Duration.StartHour;
                    participants.ProposedEndHour = eventModel.Duration.EndHour;
                }
                else
                {
                    participants.ProposedStartHour = null;
                    participants.ProposedEndHour = null;
                    participants.ConfirmationStatus = ConfirmationStatus.Pending;
                }
                new ParticipantService().UpdateParticipant(participants, eventModel.Id);
            }
        }

        private static void UpdateEventTimeBlockToMutualTimeBlock(EventModel eventModel, int newStartHour, int newEndHour)
        {
            eventModel.Duration.StartHour = newStartHour;
            eventModel.Duration.EndHour = newEndHour;

            EventService eventService = new();
            eventService.UpdateProposedEvent(eventModel);
        }
    }
}