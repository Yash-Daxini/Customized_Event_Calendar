using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Enums;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Models
{
    internal class RecurrencePatternModel   
    {
        public DateOnly StartDate { get; set; }

        public DateOnly EndDate { get; set;}

        public Frequency Frequency { get; set;}

        public int? Interval { get; set;}

        public List<int>? ByWeekDay { get; set; }
        
        public int? WeekOrder { get; set;}

        public int? ByMonthDay { get; set;}

        public int? ByMonth { get; set;}

        public override string ToString()
        {
            return $"Start Date : {StartDate}\tEnd Date : {EndDate}\tFrequency : {Frequency}\tInterval : {Interval}\tWeek Days : {string.Format(" ",ByWeekDay)}\tWeek Order : {WeekOrder}\tMonth Day : {ByMonthDay}\tBy Month : {ByMonth}";
        }
    }
}
