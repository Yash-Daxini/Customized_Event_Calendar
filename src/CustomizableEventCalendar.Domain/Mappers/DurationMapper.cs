﻿using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Models;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Mapping
{
    internal class DurationMapper
    {
        public DurationModel MapDurationModel(int startHour,int endHour)
        {
            return new DurationModel
            {
                StartHour = startHour,
                EndHour = endHour
            };
        }
    }
}
