namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Models
{
    internal class UserModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public override string ToString()
        {
            return $"Name : {Name}\tEmail : {Email}";
        }

    }
}
