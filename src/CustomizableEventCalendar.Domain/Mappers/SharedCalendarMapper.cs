using CustomizableEventCalendar.src.CustomizableEventCalendar.Data.Repositories;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Models;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Mapping
{
    internal class SharedCalendarMapper
    {
        public SharedCalendarModel MapSharedCalendarEntityToModel(SharedCalendar sharedCalendar)
        {
            return new SharedCalendarModel
            (
                sharedCalendar.Id,
                MapUserIdToUserModel(sharedCalendar.SenderUserId),
                MapUserIdToUserModel(sharedCalendar.ReceiverUserId),
                sharedCalendar.FromDate,
                sharedCalendar.ToDate
            );
        }

        private UserModel MapUserIdToUserModel(int userId)
        {
            return new UserMapper().MapUserEntityToModel(new UserRepository().GetById(data => new User(data), userId));
        }

        public SharedCalendar MapSharedCalendarModelToEntity(SharedCalendarModel sharedCalendarModel)
        {
            return new SharedCalendar
            {
                Id = sharedCalendarModel.Id,
                ReceiverUserId = sharedCalendarModel.ReceiverUser.Id,
                SenderUserId = sharedCalendarModel.SenderUser.Id,
                FromDate = sharedCalendarModel.FromDate,
                ToDate = sharedCalendarModel.ToDate,
            };
        }
    }
}
