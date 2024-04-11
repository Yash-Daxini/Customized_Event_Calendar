using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Mapping;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Models;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Data.Repositories
{
    internal class SharedCalendarRepository : GenericRepository<SharedCalendar>
    {
        private readonly SharedCalendarMapper _sharedCalendarMapper = new();

        public List<SharedCalendarModel> GetAll()
        {
            List<SharedCalendar> eventCollaborators = GetAll(data => new SharedCalendar(data));

            return [.. eventCollaborators.Select(_sharedCalendarMapper.MapSharedCalendarEntityToModel)];
        }

        public SharedCalendarModel? GetById(int sharedCalendarId)
        {
            SharedCalendar? sharedCalendar = GetById(data => new SharedCalendar(data), sharedCalendarId);

            if (sharedCalendar == null) return null;
            return _sharedCalendarMapper.MapSharedCalendarEntityToModel(sharedCalendar);
        }

        public int Insert(SharedCalendarModel sharedCalendarModel)
        {
            SharedCalendar sharedCalendar = _sharedCalendarMapper.MapSharedCalendarModelToEntity(sharedCalendarModel);
            return Insert(sharedCalendar);
        }

        public void Update(SharedCalendarModel sharedCalendarModel)
        {
            SharedCalendar sharedCalendar = _sharedCalendarMapper.MapSharedCalendarModelToEntity(sharedCalendarModel);

            Update(sharedCalendar, sharedCalendar.Id);
        }
    }
}
