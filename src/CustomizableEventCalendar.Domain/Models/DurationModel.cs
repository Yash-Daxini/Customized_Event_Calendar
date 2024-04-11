namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Models
{
    internal class DurationModel
    {
        public int StartHour { get; set; }

        public int EndHour { get; set; }

        public override string ToString()
        {
            return $"StartHour : {StartHour}\tEndHour : {EndHour}";
        }
    }
}
