using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Data.Repositories;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services
{
    internal class SchedulerService
    {
        public void Create(Scheduler scheduler)
        {
            GenericRepository genericRepository = new GenericRepository();
            genericRepository.Create(scheduler);
        }
    }
}
