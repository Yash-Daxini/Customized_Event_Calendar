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
        ScheduleRepository scheduleRepository = new ScheduleRepository();
        public List<Scheduler> Read()
        {
            List<Scheduler> schedulers = scheduleRepository.Read(data => new Scheduler(data));
            return schedulers;
        }
        public Scheduler? Read(int Id)
        {
            Scheduler? schedulers = scheduleRepository.Read(data => new Scheduler(data), Id);
            return schedulers;
        }
        public void Create(Scheduler scheduler)
        {
            scheduleRepository.Create(scheduler);
        }
    }
}
