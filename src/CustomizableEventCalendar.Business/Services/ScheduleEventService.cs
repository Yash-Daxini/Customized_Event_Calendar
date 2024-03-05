using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Data.Repositories;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services
{
    internal class ScheduleEventService
    {
        private readonly ScheduleEventRepository scheduleEventRepository = new();

        public List<ScheduleEvent> GetAllScheduleEvents()
        {
            List<ScheduleEvent> scheduleEvents = [];

            try
            {
                scheduleEvents = scheduleEventRepository.GetAll(data => new ScheduleEvent(data));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Some error occurred! " + ex.Message);
            }

            return scheduleEvents;
        }

        public ScheduleEvent? GetScheduleEventById(int scheduleEventId)
        {
            ScheduleEvent? scheduleEvents = null;

            try
            {
                scheduleEvents = scheduleEventRepository.GetById(data => new ScheduleEvent(data), scheduleEventId);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Some error occurred! " + ex.Message);
            }

            return scheduleEvents;
        }

        public void Create(ScheduleEvent scheduleEvent)
        {
            try
            {
                scheduleEventRepository.Insert(scheduleEvent);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Some error occurred! " + ex.Message);
            }
        }

        public EventCollaborators GetEventCollaborator(int eventCollaboratorsId)
        {
            EventCollaboratorsRepository eventCollaboratorsRepository = new();

            List<EventCollaborators> eventCollaborators = eventCollaboratorsRepository.GetAll(data => new
                                                                                EventCollaborators(data));

            EventCollaborators eventCollaborator = eventCollaborators.First(eventCollaboratorObj => eventCollaboratorObj.Id == eventCollaboratorsId);

            return eventCollaborator;
        }

        public int GetEventIdFromEventCollaborators(int eventCollaboratorsId)
        {
            return GetEventCollaborator(eventCollaboratorsId).EventId;
        }

        public int GetUserIdFromEventCollaborators(int eventCollaboratorsId)
        {

            return GetEventCollaborator(eventCollaboratorsId).UserId;
        }

        public void DeleteByEventId(int eventId)
        {
            try
            {
                scheduleEventRepository.DeleteByEventId(eventId);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Some error occurred ! " + ex.Message);
            }
        }

        public List<ScheduleEvent> ReadByUserId()
        {
            return scheduleEventRepository.ReadByUserId();
        }
    }
}