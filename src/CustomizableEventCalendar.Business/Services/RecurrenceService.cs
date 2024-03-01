using CustomizableEventCalendar.src.CustomizableEventCalendar.Data.Repositories;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services
{
    internal class RecurrenceService
    {
        private readonly RecurrencePatternRepository recurrencePatternRepository = new RecurrencePatternRepository();

        public List<RecurrencePatternCustom> Read()
        {
            List<RecurrencePatternCustom> recurrencePatterns = new List<RecurrencePatternCustom>();

            try
            {
                recurrencePatterns = recurrencePatternRepository.GetAll<RecurrencePatternCustom>(data => new RecurrencePatternCustom(data));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Some error occurred! " + ex.Message);
            }

            return recurrencePatterns;
        }

        public RecurrencePatternCustom Read(int Id)
        {
            RecurrencePatternCustom recurrencePatterns = new RecurrencePatternCustom();

            try
            {
                recurrencePatterns = recurrencePatternRepository.GetById<RecurrencePatternCustom>(data => new RecurrencePatternCustom(data), Id);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Some error occurred! " + ex.Message);
            }

            return recurrencePatterns;
        }

        public int Create(RecurrencePatternCustom recurrencePattern)
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

        public void Delete(int Id)
        {
            try
            {
                recurrencePatternRepository.Delete<RecurrencePatternCustom>(Id);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Some error occurred! " + ex.Message);
            }
        }

        public void Update(RecurrencePatternCustom recurrencePattern, int Id)
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