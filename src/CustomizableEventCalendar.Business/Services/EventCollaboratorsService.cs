//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using CustomizableEventCalendar.src.CustomizableEventCalendar.Data.Repositories;
//using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;

//namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services
//{
//    internal class EventCollaboratorsService
//    {
//        private readonly EventCollaboratorsRepository _eventCollaboratorsRepository = new();

//        public List<EventCollaborators> GetAllEventCollaborators()
//        {
//            List<EventCollaborators> eventCollaborators = [];

//            try
//            {
//                eventCollaborators = _eventCollaboratorsRepository.GetAll(data => new EventCollaborators(data));
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine("Some error occurred ! " + ex.Message);
//            }

//            return eventCollaborators;
//        }

//        public EventCollaborators? GetEventCollaboratorsById(int eventCollaboratorId)
//        {
//            EventCollaborators? eventCollaborators = null;

//            try
//            {
//                eventCollaborators = _eventCollaboratorsRepository.GetById(data => new EventCollaborators(data), eventCollaboratorId);
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine("Some error occurred ! " + ex.Message);
//            }

//            return eventCollaborators;
//        }

//        public int InsertEventCollaborators(EventCollaborators eventCollaborators)
//        {
//            int eventCollaboratorsId = 0;

//            try
//            {
//                eventCollaboratorsId = _eventCollaboratorsRepository.Insert(eventCollaborators);
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine("Some error occurred ! " + ex.Message);
//            }

//            return eventCollaboratorsId;
//        }

//        public void DeletEventCollaboratorsByEventId(int eventId)
//        {
//            try
//            {
//                _eventCollaboratorsRepository.DeleteByEventId(eventId);
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine("Some error occurred ! " + ex.Message);
//            }
//        }

//        public EventCollaborators GetEventCollaboratorsByEventId(int eventId)
//        {
//            EventCollaborators eventCollaborator = null;

//            try
//            {
//                eventCollaborator = _eventCollaboratorsRepository.ReadByEventId(eventId);
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine("Some error occurred ! " + ex.Message);
//            }

//            return eventCollaborator;
//        }

//        public int GetUserIdFromEventCollaborationId(int eventCollaborationId)
//        {
//            EventCollaborators? eventCollaborator = GetEventCollaboratorsById(eventCollaborationId);

//            if (eventCollaborator == null) return 0;

//            return eventCollaborator.UserId;
//        }

//        public int? GetEventCollaboratorIdFromEventId(int eventId)
//        {
//            EventCollaborators? eventCollaborators = GetAllEventCollaborators().FirstOrDefault(eventCollaborator =>
//                                                                    eventCollaborator.UserId == GlobalData.user.Id
//                                                                    && eventCollaborator.EventId == eventId);

//            return eventCollaborators == null ? null : eventCollaborators.Id;
//        }
//    }
//}