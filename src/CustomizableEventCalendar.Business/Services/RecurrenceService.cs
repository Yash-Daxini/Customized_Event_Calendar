using CustomizableEventCalendar.src.CustomizableEventCalendar.Data.Repositories;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services
{
    internal class RecurrenceService
    {
        RecurrencePatternRepository recurrencePatternRepository = new RecurrencePatternRepository();
        public List<RecurrencePattern> Read()
        {
            List<RecurrencePattern> recurrencePatterns = new List<RecurrencePattern>();

            try
            {
                recurrencePatterns = recurrencePatternRepository.Read<RecurrencePattern>(data => new RecurrencePattern(data));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Some error occurred! " + ex.Message);
            }

            return recurrencePatterns;
        }
        public RecurrencePattern Read(int Id)
        {
            RecurrencePattern recurrencePatterns = new RecurrencePattern();

            try
            {
                recurrencePatterns = recurrencePatternRepository.Read<RecurrencePattern>(data => new RecurrencePattern(data), Id);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Some error occurred! " + ex.Message);
            }

            return recurrencePatterns;
        }
        public int Create(RecurrencePattern recurrencePattern)
        {
            int Id = 0;
            try
            {
                Id = recurrencePatternRepository.Create(recurrencePattern);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Some error occurred! " + ex.Message);
            }
            return Id;
        }
        public void Delete(int Id)
        {
            try
            {
                recurrencePatternRepository.Delete<RecurrencePattern>(Id);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Some error occurred! " + ex.Message);
            }
        }
        public void Update(RecurrencePattern recurrencePattern, int Id)
        {
            try
            {
                recurrencePatternRepository.Update(recurrencePattern, Id);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Some error occurred! " + ex.Message);
            }
        }
    }
}
