using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Models;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services
{
    internal class SharedEventCollaborationService
    {
        private readonly ParticipantService _eventCollaboratorService = new();

        public void AddCollaborator(ParticipantModel participantModel, int eventId)
        {
            _eventCollaboratorService.InsertParticipant(participantModel, eventId);
        }

        public bool IsEventAlreadyCollaborated(EventModel eventModelToCheckOverlap)
        {
            return new EventService().GetEventById(eventModelToCheckOverlap.Id)
                                         .Where(eventModel => eventModel.EventDate == eventModelToCheckOverlap.EventDate
                                                && eventModel.Duration.StartHour == eventModelToCheckOverlap.Duration.StartHour
                                                && eventModel.Duration.EndHour == eventModelToCheckOverlap.Duration.EndHour
                                                && eventModel.Participants.Exists(participant => participant.User.Id == GlobalData.GetUser().Id)
                                                ).Count() > 0;
        }

        public EventModel? GetCollaborationOverlap(EventModel eventToCollaborate)
        {
            List<EventModel> events = new EventService().GetAllEventsOfLoggedInUser();

            return events.FirstOrDefault(eventModel => eventModel.EventDate == eventToCollaborate.EventDate
                                && eventModel.Participants.Exists(participant => participant.User.Id == GlobalData.GetUser().Id)
                                && IsHourOvelapps(eventModel.Duration.StartHour, eventModel.Duration.EndHour, eventToCollaborate.Duration.StartHour, eventToCollaborate.Duration.EndHour));
        }

        private static bool IsHourOvelapps(int startHourOfFirstEvent, int endHourOfFirstEvent, int startHourOfSecondEvent, int endHourOfSecondEvent)
        {
            return (startHourOfFirstEvent >= startHourOfSecondEvent
                    && startHourOfFirstEvent < endHourOfSecondEvent)
                || (endHourOfFirstEvent > startHourOfSecondEvent
                    && endHourOfFirstEvent <= endHourOfSecondEvent)
                || (startHourOfSecondEvent >= startHourOfFirstEvent
                    && startHourOfSecondEvent < endHourOfFirstEvent)
                || (endHourOfSecondEvent > startHourOfFirstEvent
                    && endHourOfSecondEvent <= endHourOfFirstEvent);
        }
    }
}