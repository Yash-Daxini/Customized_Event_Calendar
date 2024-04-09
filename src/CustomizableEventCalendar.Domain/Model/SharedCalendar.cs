namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Model
{
    internal class SharedCalendar
    {
        public User SenderUser { get; set; }

        public User ReceiverUser { get; set; }

        public DateOnly FromDate { get; set; }

        public DateOnly ToDate { get; set; }
    }
}
