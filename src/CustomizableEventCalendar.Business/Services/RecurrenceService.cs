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
        public List<RecurrencePattern> Read()
        {
            GenericRepository genericRepository = new GenericRepository();
            List<RecurrencePattern> recurrencePatterns = genericRepository.Read<RecurrencePattern>(data => new RecurrencePattern(data));
            return recurrencePatterns;
        }
        public RecurrencePattern Read(int Id)
        {
            GenericRepository genericRepository = new GenericRepository();
            RecurrencePattern recurrencePatterns = genericRepository.Read<RecurrencePattern>(data => new RecurrencePattern(data),Id);
            return recurrencePatterns;
        }
        public int Create(RecurrencePattern recurrencePattern)
        {
            GenericRepository genericRepository = new GenericRepository();
            int Id = genericRepository.Create(recurrencePattern);
            return Id;
        }
        public void Delete(int Id)
        {
            GenericRepository genericRepository = new GenericRepository();
            genericRepository.Delete<RecurrencePattern>(Id);
        }
        public void Update(RecurrencePattern recurrencePattern,int Id)
        {
            GenericRepository genericRepository = new GenericRepository();
            genericRepository.Update(recurrencePattern,Id);
        }
    }
}
