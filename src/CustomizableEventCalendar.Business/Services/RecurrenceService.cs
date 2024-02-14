using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Data.Repositories;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services
{
    internal class RecurrenceService
    {
        GenericRepository genericRepository = new GenericRepository();
        public List<RecurrencePattern> Read()
        {
            List<RecurrencePattern> recurrencePatterns = genericRepository.Read<RecurrencePattern>(data => new RecurrencePattern(data));
            return recurrencePatterns;
        }
        public RecurrencePattern Read(int Id)
        {
            RecurrencePattern recurrencePatterns = genericRepository.Read<RecurrencePattern>(data => new RecurrencePattern(data), Id);
            return recurrencePatterns;
        }
        public int Create(RecurrencePattern recurrencePattern)
        {
            int Id = genericRepository.Create(recurrencePattern);
            return Id;
        }
        public void Delete(int Id)
        {
            genericRepository.Delete<RecurrencePattern>(Id);
        }
        public void Update(RecurrencePattern recurrencePattern, int Id)
        {
            genericRepository.Update(recurrencePattern, Id);
        }
    }
}
