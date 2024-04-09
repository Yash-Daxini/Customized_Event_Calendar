using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Enums;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Model
{
    internal class RecurrencePattern
    {
        public DateOnly StartDate { get; set; }

        public DateOnly EndDate { get; set;}

        public Frequency Frequency { get; set;}

        public int Interval { get; set;}

        public List<int> ByWeekDay { get; set; }
        
        public int WeekOrder { get; set;}

        public int ByMonthDay { get; set;}

        public int ByMonth { get; set;}
    }
}
