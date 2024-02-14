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
        public int Create(Event eventObj)
        {
            GenericRepository genericRepository = new GenericRepository();
            int Id = genericRepository.Create<Event>(eventObj);
            return Id;
        }
        public List<Event> Read()
        {
            GenericRepository genericRepository = new GenericRepository();
            List<Event> listOfEvents = genericRepository.Read<Event>(data => new Event(data));
            return listOfEvents;
        }
        public Event Read(int Id)
        {
            GenericRepository genericRepository = new GenericRepository();
            Event listOfEvents = genericRepository.Read<Event>(data => new Event(data), Id);
            return listOfEvents;
        }
        public void Delete(int Id)
        {
            GenericRepository genericRepository = new GenericRepository();
            genericRepository.Delete<Event>(Id);
        }
        public void Update(Event eventObj, int Id)
        {
            GenericRepository genericRepository = new GenericRepository();
            genericRepository.Update(eventObj, Id);
        }
    }
}
