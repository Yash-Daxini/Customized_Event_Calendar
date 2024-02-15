using CustomizableEventCalendar.src.CustomizableEventCalendar.Data.Repositories;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services
{
    internal class RecurrenceService
    {
        RecurrencePatternRepository recurrencePatternRepository = new RecurrencePatternRepository();
        public List<RecurrencePattern> Read()
        {
            List<RecurrencePattern> recurrencePatterns = recurrencePatternRepository.Read<RecurrencePattern>(data => new RecurrencePattern(data));
            return recurrencePatterns;
        }
        public RecurrencePattern Read(int Id)
        {
            RecurrencePattern recurrencePatterns = recurrencePatternRepository.Read<RecurrencePattern>(data => new RecurrencePattern(data), Id);
            return recurrencePatterns;
        }
        public int Create(RecurrencePattern recurrencePattern)
        {
            int Id = recurrencePatternRepository.Create(recurrencePattern);
            return Id;
        }
        public void Delete(int Id)
        {
            recurrencePatternRepository.Delete<RecurrencePattern>(Id);
        }
        public void Update(RecurrencePattern recurrencePattern, int Id)
        {
            recurrencePatternRepository.Update(recurrencePattern, Id);
        }
    }
}
