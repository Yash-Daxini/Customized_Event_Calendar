namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Models
{
    internal static class GlobalData
    {
        private static UserModel? user;

        public static UserModel? GetUser() { return user; }

        public static void SetUser(UserModel? newUser)
        {
            user = newUser;
        }
    }
}