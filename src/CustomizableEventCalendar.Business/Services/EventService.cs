using CustomizableEventCalendar.src.CustomizableEventCalendar.Data.Repositories;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services
{
    internal class EventService
    {
        EventRepository eventRepository = new EventRepository();
        public int Create(Event eventObj)
        {
            int Id = eventRepository.Create<Event>(eventObj);
            return Id;
        }
        public List<Event> Read()
        {
            List<Event> listOfEvents = eventRepository.Read<Event>(data => new Event(data));
            return listOfEvents;
        }
        public Event Read(int Id)
        {
            Event listOfEvents = eventRepository.Read<Event>(data => new Event(data), Id);
            return listOfEvents;
        }
        public void Delete(int Id)
        {
            eventRepository.Delete<Event>(Id);
        }
        public void Update(Event eventObj, int Id)
        {
            eventRepository.Update(eventObj, Id);
        }
    }
}
