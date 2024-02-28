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
        private readonly ScheduleEventRepository scheduleEventRepository = new ScheduleEventRepository();
        public List<ScheduleEvent> Read()
        {
            List<ScheduleEvent> schedulers = new List<ScheduleEvent>();

            try
            {
                schedulers = scheduleEventRepository.Read(data => new ScheduleEvent(data));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Some error occurred! " + ex.Message);
            }

            return schedulers;
        }
        public ScheduleEvent? Read(int Id)
        {
            ScheduleEvent? schedulers = null;

            try
            {
                schedulers = scheduleEventRepository.Read(data => new ScheduleEvent(data), Id);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Some error occurred! " + ex.Message);
            }

            return schedulers;
        }
        public void Create(ScheduleEvent scheduler)
        {
            try
            {
                scheduleEventRepository.Create(scheduler);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Some error occurred! " + ex.Message);
            }
        }
        public EventCollaborators GetEventCollaborator(int eventCollaboratorsId)
        {
            EventCollaboratorsRepository eventCollaboratorsRepository = new EventCollaboratorsRepository();

            List<EventCollaborators> eventCollaborators = eventCollaboratorsRepository.Read(data => new
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
        public void DeleteByEventId(int eventId, int userId)
        {
            try
            {
                scheduleEventRepository.DeleteByEventId(eventId, userId);
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