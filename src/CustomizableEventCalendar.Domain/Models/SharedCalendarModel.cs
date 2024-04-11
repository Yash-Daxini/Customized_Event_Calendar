namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Models
{
    internal class SharedCalendarModel
    {
        public int Id { get; set; }

        public UserModel SenderUser { get; set; }

        public UserModel ReceiverUser { get; set; }

        public DateOnly FromDate { get; set; }

        public DateOnly ToDate { get; set; }

        public override string ToString()
        {
            return $"Sender : {SenderUser}\tReceiver : {ReceiverUser}\tFrom Date : {FromDate}\tTo Date : {ToDate}";
        }
    }
}
