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
            List<Scheduler> schedulers = new List<Scheduler>();

            try
            {
                schedulers = scheduleRepository.Read(data => new Scheduler(data));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Some error occurred! " + ex.Message);
            }

            return schedulers;
        }
        public Scheduler? Read(int Id)
        {
            Scheduler? schedulers = null;

            try
            {
                schedulers = scheduleRepository.Read(data => new Scheduler(data), Id);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Some error occurred! " + ex.Message);
            }

            return schedulers;
        }
        public void Create(Scheduler scheduler)
        {
            try
            {
                scheduleRepository.Create(scheduler);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Some error occurred! " + ex.Message);
            }
        }
    }
}
