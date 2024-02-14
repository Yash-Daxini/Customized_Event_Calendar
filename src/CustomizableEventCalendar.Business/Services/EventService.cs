using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Data.Repositories;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services
{
    internal class EventService
    {
        GenericRepository genericRepository = new GenericRepository();
        public int Create(Event eventObj)
        {
            int Id = genericRepository.Create<Event>(eventObj);
            return Id;
        }
        public List<Event> Read()
        {
            List<Event> listOfEvents = genericRepository.Read<Event>(data => new Event(data));
            return listOfEvents;
        }
        public Event Read(int Id)
        {
            Event eventObj = genericRepository.Read<Event>(data => new Event(data), Id);
            return eventObj;
        }
        public void Delete(int Id)
        {
            genericRepository.Delete<Event>(Id);
        }
        public void Update(Event eventObj, int Id)
        {
            genericRepository.Update(eventObj, Id);
        }
    }
}
