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

        public bool IsEventAlreadyCollaborated(EventModel eventModel)
        {
            return eventModel.Participants.Exists(participant => participant.User.Id == GlobalData.GetUser().Id);
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