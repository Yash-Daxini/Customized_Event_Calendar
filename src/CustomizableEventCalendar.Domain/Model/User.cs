namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Model
{
    internal class User
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        private string Password { get; set; }

        public bool IsPasswordMatch(string password)
        {
            return this.Password == password;
        }

    }
}
