using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Enums;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.ConsoleApp
{
    internal class CalendarView
    {
        public static void ViewSelection()
        {
            Console.WriteLine();
            Console.Write("Choose the view you want to see : 1. Daily View 2. Weekly View 3. Monthly View 0. Back :- ");
            int choice = Convert.ToInt32(Console.ReadLine());
            switch ((CalendarViewEnum)choice)
            {
                case CalendarViewEnum.Daily:
                    DailyView();
                    ViewSelection();
                    break;
                case CalendarViewEnum.Weekly:
                    WeeklyView();
                    ViewSelection();
                    break;
                case CalendarViewEnum.Monthly:
                    MonthlyView();
                    ViewSelection();
                    break;
                case CalendarViewEnum.Back:
                    Console.WriteLine("Going back");
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
            Console.WriteLine("\n\t\t\t\t\tSchedule of date :- " + todayDate.ToString("dd-MM-yyyy"));
            while (todayDate.Date <= DateTime.Today.Date)
            {
                Console.WriteLine("\t\t\t\t\t\t" + todayDate.ToString("hh:mm:ss tt"));
                todayDate = todayDate.AddHours(1);
            }
        }
        public static void WeeklyView()
        {
            DayOfWeek dayOfWeek = DateTime.Now.DayOfWeek;
            int dayTillCurrentDay = dayOfWeek - DayOfWeek.Monday;
            DateTime startDateOfWeek = DateTime.Now.AddDays(-dayTillCurrentDay);
            DateTime endDateOfWeek = startDateOfWeek.AddDays(7);
            Console.WriteLine("\n\t\t\tSchedule from date :- " + startDateOfWeek.ToString("dd-MM-yyyy") + " to date :- " + endDateOfWeek.ToString("dd-MM-yyyy"));
            while (startDateOfWeek < endDateOfWeek)
            {
                Console.WriteLine("\t\t\t\t\t\t" + startDateOfWeek.ToString("dd-MM-yyyy") + " " + startDateOfWeek.ToString("dddd"));
                startDateOfWeek = startDateOfWeek.AddDays(1);
            }
        }
        public static void MonthlyView()
        {
            DateTime todayDate = DateTime.Now;
            DateTime startDateOfMonth = new DateTime(todayDate.Year, todayDate.Month, 1);
            DateTime endDateOfMonth = new DateTime(todayDate.Year, todayDate.Month, DateTime.DaysInMonth(todayDate.Year, todayDate.Month));
            Console.WriteLine("\n\t\t\tSchedule from date :- " + startDateOfMonth.ToString("dd-MM-yyyy") + " to date :- " + endDateOfMonth.ToString("dd-MM-yyyy"));
            while (startDateOfMonth.Date <= endDateOfMonth.Date)
            {
                Console.WriteLine("\t\t\t\t\t\t" + startDateOfMonth.ToString("dd-MM-yyyy") + " " + startDateOfMonth.DayOfWeek);
                startDateOfMonth = startDateOfMonth.AddDays(1);
            }
        }
    }
}
