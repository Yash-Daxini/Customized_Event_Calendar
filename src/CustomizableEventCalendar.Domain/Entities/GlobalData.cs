using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities
{
    internal static class GlobalData
    {
        private static User? user;

        public static User? GetUser() { return user; }

        public static void SetUser(User? newUser)
        {
            user = newUser;
        }
    }
}