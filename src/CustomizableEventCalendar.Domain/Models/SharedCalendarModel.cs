namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Model
{
    internal class SharedCalendarModel
    {
        public int Id { get; set; }

        public UserModel SenderUser { get; set; }

        public UserModel ReceiverUser { get; set; }

        public DateOnly FromDate { get; set; }

        public DateOnly ToDate { get; set; }
    }
}
