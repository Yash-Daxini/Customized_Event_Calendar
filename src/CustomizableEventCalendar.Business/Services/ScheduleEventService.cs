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
        ScheduleEventRepository scheduleEventRepository = new ScheduleEventRepository();
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
        public int GetEventIdFromEventCollaborators(int eventCollaboratorsId)
        {
            EventCollaboratorsRepository eventCollaboratorsRepository = new EventCollaboratorsRepository();

            List<EventCollaborators> eventCollaborators = eventCollaboratorsRepository.Read(data => new
                                                                                EventCollaborators(data));

            EventCollaborators eventCollaborator = eventCollaborators.First(eventCollaborator => eventCollaborator.Id == eventCollaboratorsId);
            return eventCollaborator.EventId;
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
