using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Enums;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.ConsoleApp
{
    internal static class CalendarView
    {
        private static readonly CalendarViewService _calendarViewService = new();

        public static void ViewSelection()
        {
            int choice = ValidatedInputProvider.GetValidatedInteger("\nChoose the view you want to see : 1. Daily View " +
                                                                    "2. Weekly View 3. Monthly View 0. Back :- ");

            Domain.Enums.CalendarView option = (Domain.Enums.CalendarView)choice;

            switch (option)
            {
                case Domain.Enums.CalendarView.Daily:
                    DailyView();
                    ViewSelection();
                    break;
                case Domain.Enums.CalendarView.Weekly:
                    WeeklyView();
                    ViewSelection();
                    break;
                case Domain.Enums.CalendarView.Monthly:
                    MonthlyView();
                    ViewSelection();
                    break;
                case Domain.Enums.CalendarView.Back:
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
            string dailyView = _calendarViewService.GenerateDailyView();

            Console.WriteLine(dailyView);
        }

        public static void WeeklyView()
        {
            string weeklyView = _calendarViewService.GenerateWeeklyView();

            Console.WriteLine(weeklyView);
        }

        public static void MonthlyView()
        {
            string monthlyView = _calendarViewService.GenerateMonthView();

            Console.WriteLine(monthlyView);
        }
    }
}