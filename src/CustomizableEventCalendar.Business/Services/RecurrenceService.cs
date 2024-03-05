using CustomizableEventCalendar.src.CustomizableEventCalendar.Data.Repositories;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services
{
    internal class RecurrenceService
    {
        private readonly RecurrencePatternRepository recurrencePatternRepository = new();

        public List<RecurrencePatternCustom> GetAllRecurrencePatterns()
        {
            List<RecurrencePatternCustom> recurrencePatterns = [];

            try
            {
                recurrencePatterns = recurrencePatternRepository.GetAll(data => new RecurrencePatternCustom(data));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Some error occurred! " + ex.Message);
            }

            return recurrencePatterns;
        }

        public RecurrencePatternCustom GetRecurrencePatternById(int recurrencePatternId)
        {
            RecurrencePatternCustom recurrencePatterns = new();

            try
            {
                recurrencePatterns = recurrencePatternRepository.GetById<RecurrencePatternCustom>(data => new RecurrencePatternCustom(data), recurrencePatternId);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Some error occurred! " + ex.Message);
            }

            return recurrencePatterns;
        }

        public int InsertRecurrencePattern(RecurrencePatternCustom recurrencePattern)
        {
            int Id = 0;
            try
            {
                Id = recurrencePatternRepository.Insert(recurrencePattern);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Some error occurred! " + ex.Message);
            }
            return Id;
        }

        public void DeleteRecurrencePattern(int recurrencePatternId)
        {
            try
            {
                recurrencePatternRepository.Delete<RecurrencePatternCustom>(recurrencePatternId);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Some error occurred! " + ex.Message);
            }
        }

        public void UpdateRecurrencePattern(RecurrencePatternCustom recurrencePattern, int recurrencePatternId)
        {
            try
            {
                recurrencePatternRepository.Update(recurrencePattern, recurrencePatternId);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Some error occurred! " + ex.Message);
            }
        }
    }
}