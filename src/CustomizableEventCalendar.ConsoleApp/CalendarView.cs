using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Enums;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.ConsoleApp
{
    internal class CalendarView
    {
        static CalendarViewService calendarViewService = new CalendarViewService();
        public static void ViewSelection()
        {
            Console.Write("\nChoose the view you want to see : 1. Daily View 2. Weekly View 3. Monthly View 0. Back :- ");
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
            string dailyView = calendarViewService.GenerateDailyView();
            Console.WriteLine(dailyView);
        }
        public static void WeeklyView()
        {
            string weeklyView = calendarViewService.GenerateWeeklyView();
            Console.WriteLine(weeklyView);
        }
        public static void MonthlyView()
        {
            string monthlyView = calendarViewService.GenerateMonthView();
            Console.WriteLine(monthlyView);
        }
    }
}
