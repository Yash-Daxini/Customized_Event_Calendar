using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Data.Repositories;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services
{
    internal class RecurrenceEngine
    {
        public void AddEventToScheduler(Event eventObj)
        {
            GenericRepository genericRepository = new GenericRepository();
            int recurrenceId = eventObj.RecurrenceId == null ? 0 : eventObj.RecurrenceId.Value;
            if (eventObj.RecurrenceId == null) AddScheduler(eventObj);
            else
            {
                RecurrencePattern recurrencePattern = genericRepository.Read<RecurrencePattern>(data => new RecurrencePattern(data), recurrenceId);
                AddScheduler(eventObj, recurrencePattern);
            }
        }
        public void AddScheduler(Event eventObj)
        {

        }
        public void AddScheduler(Event eventObj, RecurrencePattern recurrencePattern)
        {
            if (recurrencePattern.FREQ.Equals("daily"))
            {
                DateTime startDate = recurrencePattern.DTSTART;
                HashSet<string> days = recurrencePattern.BYDAY.Split(",").ToHashSet<string>();
                while (startDate < recurrencePattern.UNTILL)
                {
                    startDate = startDate.AddDays(Convert.ToInt32(recurrencePattern.INTERVAL + 1));
                    string day = startDate.DayOfWeek.ToString("d");
                    Console.WriteLine(day);
                }
            }
        }
    }
}
