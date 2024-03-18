using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomizableEventCalendar.src.CustomizableEventCalendar.ConsoleApp;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Data.Repositories;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services
{
    internal class EventCollaboratorService
    {
        private readonly EventCollaboratorRepository _eventCollaboratorsRepository = new();

        public List<EventCollaborator> GetAllEventCollaborators()
        {
            List<EventCollaborator> eventCollaborators = [];

            try
            {
                eventCollaborators = _eventCollaboratorsRepository.GetAll(data => new EventCollaborator(data));
            }
            catch
            {
                PrintHandler.PrintErrorMessage("Some error occurred ! ");
            }

            return eventCollaborators;
        }

        public EventCollaborator? GetEventCollaboratorsById(int eventCollaboratorId)
        {
            EventCollaborator? eventCollaborators = null;

            try
            {
                eventCollaborators = _eventCollaboratorsRepository.GetById(data => new EventCollaborator(data), eventCollaboratorId);
            }
            catch (Exception)
            {
                PrintHandler.PrintErrorMessage("Some error occurred ! ");
            }

            return eventCollaborators;
        }

        public int InsertEventCollaborators(EventCollaborator eventCollaborators)
        {
            int eventCollaboratorsId = 0;

            try
            {
                eventCollaboratorsId = _eventCollaboratorsRepository.Insert(eventCollaborators);
            }
            catch (Exception)
            {
                PrintHandler.PrintErrorMessage("Some error occurred ! ");
            }

            return eventCollaboratorsId;
        }

        public void UpdateEventCollaborators(EventCollaborator eventCollaborator, int eventCollaboratorId)
        {
            try
            {
                _eventCollaboratorsRepository.Update(eventCollaborator, eventCollaboratorId);
            }
            catch
            {
                PrintHandler.PrintErrorMessage("Some error occurred ! ");
            }
        }

        public void DeleteEventCollaboratorsByEventId(int eventId)
        {
            try
            {
                _eventCollaboratorsRepository.DeleteByEventId(eventId);
            }
            catch (Exception)
            {
                PrintHandler.PrintErrorMessage("Some error occurred ! ");
            }
        }

        public EventCollaborator? GetEventCollaboratorFromEventIdAndUserId(int eventId)
        {
            EventCollaborator? eventCollaborator = GetAllEventCollaborators()
                                                   .FirstOrDefault(eventCollaborator =>
                                                                   eventCollaborator.UserId == GlobalData.user.Id
                                                                   && eventCollaborator.EventId == eventId);

            return eventCollaborator;
        }
    }
}