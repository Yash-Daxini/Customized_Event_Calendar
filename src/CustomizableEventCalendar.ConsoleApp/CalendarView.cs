using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.ConsoleApp
{
    internal class CalendarView
    {
        public static void ViewSelection()
        {
            Console.WriteLine();
            Console.Write("Choose the view you want to see : 1. Daily View 2. Weekly View 3. Monthly View :- ");
            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    DailyView();
                    break;
                case "2":
                    WeeklyView();
                    break;
                case "3":
                    MonthlyView();
                    break;
                default:
                    Console.WriteLine("Oops! Wrong choice");
                    ViewSelection();
                    break;
            }
        }
        public static void DailyView()
        {
            DateTime todayDate = DateTime.Today;
            Console.WriteLine("\n\t\t\t\t\t\t" + todayDate.Date);
            Console.WriteLine("This is Daily View");
        }
        public static void WeeklyView()
        {
            Console.WriteLine("This is Weekly View");
        }
        public static void MonthlyView()
        {
            Console.WriteLine("This is Monthly View");
        }
    }
}
