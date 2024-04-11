using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Mapping;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Models;
namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Data.Repositories
{

    internal class EventRepository : GenericRepository<Event>
    {

        private static readonly EventMapper _eventMapper = new();
        private static readonly EventCollaboratorRepository _eventCollaboratorRepository = new();

        public List<EventModel> GetAll()
        {
            List<Event> events = GetAll(data => new Event(data));
            List<EventModel> eventModels = [];

            foreach (List<EventModel> eventModelList in GetEventModelsFromEvents(events))
            {
                eventModels = [.. eventModels.Concat(eventModelList)];
            }

            return eventModels;
        }

        public List<EventModel>? GetById(int eventId)
        {
            Event? eventObj = GetById(data => new Event(data), eventId);
            if (eventObj == null) return null;
            return _eventMapper.MapEventEntityToModel(eventObj, _eventCollaboratorRepository.GetByEventId(eventId));
        }

        public int Insert(EventModel eventModel)
        {
            return Insert(_eventMapper.MapEventModelToEntity(eventModel));
        }

        public void Update(EventModel eventModel)
        {
            Event eventObj = _eventMapper.MapEventModelToEntity(eventModel);
            Update(eventObj, eventObj.Id);
        }

        private static List<List<EventModel>> GetEventModelsFromEvents(List<Event> events)
        {
            return [.. events.Select(eventObj => _eventMapper.MapEventEntityToModel(eventObj, _eventCollaboratorRepository.GetByEventId(eventObj.Id))).Select(eventModels => eventModels)];
        }

        public void ConvertProposedEventToScheduleEvent(int eventId)
        {
            string query = @$"Update [dbo].[Event]
                              set IsProposed = 0
                              where Id = {eventId}";

            Connect();

            ExecuteNonQuery(query);

            Disconnect();
        }
    }
}