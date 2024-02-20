using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Data.Repositories;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.ConsoleApp
{
    internal class ShareCalendar
    {
        CalendarSharingService calendarSharingService = new CalendarSharingService();
        public void GetDetailsToShareCalendar()
        {
            ShowAllUser();

            Console.Write("Enter User No. whom you want to share calendar :- ");
            int UserId = Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("Enter Time Range for which you want to share events");

            Console.Write("Enter start date in dd-MM-yyyy format :-  ");
            DateOnly startDate = DateOnly.Parse(Console.ReadLine());

            Console.Write("Enter end date in dd-MM-yyyy format :-  ");
            DateOnly endDate = DateOnly.Parse(Console.ReadLine());

            UserRepository userRepository = new UserRepository();
            User user = userRepository.Read<User>(data => new User(data), UserId);

            SharedEvents sharedEvents = new SharedEvents(UserId, GlobalData.user == null ? 0 : GlobalData.user.Id, startDate, endDate);

            calendarSharingService.AddSharedCalendar(sharedEvents);

            Console.WriteLine($"Your Calendar shared with {user.Name} from {startDate} to {endDate}");
        }
        public void ShowAllUser()
        {
            UserService userService = new UserService();
            string userInformations = userService.GetInsensitiveInformationOfUser();
            Console.WriteLine(userInformations);
        }
        public void ViewSharedCalendars()
        
        {
            Console.WriteLine("This users shared you a calendar");

            string sharedEvents = calendarSharingService.GenerateDisplayFormatForSharedEvents();

            Console.WriteLine(sharedEvents);

            Console.Write("Select Sr No. which calendar you want to see :- ");
            int sharedCalendarId = Convert.ToInt32(Console.ReadLine());

            ViewSpecificCalendar(sharedCalendarId);
        }
        public void ViewSpecificCalendar(int sharedCalendarId)
        {
            string calendar = calendarSharingService.GenerateSharedCalendar(sharedCalendarId);
            Console.WriteLine(calendar);
        }
    }
}
