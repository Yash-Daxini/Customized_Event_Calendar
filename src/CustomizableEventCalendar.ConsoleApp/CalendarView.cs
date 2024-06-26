﻿using System.Text;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Enums;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Models;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.ConsoleApp
{
    internal static class CalendarView
    {
        private static readonly CalendarViewService _calendarViewService = new();

        public static void ChooseView()
        {
            Console.WriteLine("\nChoose the view you want to see : \n1. Daily View \n2. Weekly View \n3. Monthly View \n0. Back :- ");

            int choice = ValidatedInputProvider.GetValidIntegerBetweenRange("Enter choice : ", 0, 3);

            Dictionary<CalendarViewChoice, Action> operationDictionary = new()
            {{ CalendarViewChoice.Daily, PrintDailyView},
            { CalendarViewChoice.Weekly, PrintWeeklyView },
            { CalendarViewChoice.Monthly, PrintMonthlyView }};

            CalendarViewChoice option = (CalendarViewChoice)choice;

            if (operationDictionary.TryGetValue(option, out Action? actionMethod))
            {
                actionMethod.Invoke();
            }

            if (option != CalendarViewChoice.Back) ChooseView();
        }

        public static void PrintDailyView()
        {
            List<EventModel> currentDayEvents = _calendarViewService.GetEventsFroDailyView();

            if (IsMessagePrintedOnEventUnavailability(currentDayEvents)) return;

            StringBuilder dailyView = new();

            dailyView.AppendLine(PrintService.GetHorizontalLine());

            dailyView.AppendLine("\nSchedule of " + DateTimeManager.GetDateFromDateTime(DateTime.Today) + "\n");

            List<List<string>> dailyViewTableContent = currentDayEvents.InsertInto2DList(["Time Block", "Event Title"],
                [
                    eventModel => DateTimeManager.ConvertTo12HourFormat(eventModel.Duration.StartHour) + " - " +                                 DateTimeManager.ConvertTo12HourFormat(eventModel.Duration.EndHour),
                    eventModel => eventModel.Title,
                ]);

            dailyView.AppendLine(PrintService.GenerateTable(dailyViewTableContent));

            Console.WriteLine(dailyView);
        }

        private static bool IsMessagePrintedOnEventUnavailability(List<EventModel> currentDayEvents)
        {
            if (currentDayEvents.Count == 0)
            {
                PrintHandler.PrintWarningMessage("No events available !");
                return true;
            }

            return false;
        }

        public static void PrintWeeklyView()
        {
            DateOnly startDateOfWeek = DateTimeManager.GetStartDateOfWeek(DateOnly.FromDateTime(DateTime.Today));
            DateOnly endDateOfWeek = DateTimeManager.GetEndDateOfWeek(DateOnly.FromDateTime(DateTime.Today));

            List<EventModel> currentWeekEvents = _calendarViewService.GetDateAndEventsForWeeklyView();

            if (IsMessagePrintedOnEventUnavailability(currentWeekEvents)) return;

            StringBuilder weeklyView = new();

            weeklyView.AppendLine(PrintService.GetHorizontalLine());

            weeklyView.AppendLine("\nSchedule from " + startDateOfWeek + " to " + endDateOfWeek + "\n");

            weeklyView.AppendLine(PrintService.GenerateTable(Get2DListForWeeklyAndMonthlyView(currentWeekEvents)));

            Console.WriteLine(weeklyView);
        }


        public static void PrintMonthlyView()
        {
            StringBuilder monthlyView = new();

            DateOnly startDateOfMonth = DateTimeManager.GetStartDateOfMonth(DateTime.Now);
            DateOnly endDateOfMonth = DateTimeManager.GetEndDateOfMonth(DateTime.Now);

            List<EventModel> currentMonthEvents = _calendarViewService.GetGivenMonthEventsWithDate(startDateOfMonth, endDateOfMonth);

            if (IsMessagePrintedOnEventUnavailability(currentMonthEvents)) return;

            monthlyView.AppendLine(PrintService.GetHorizontalLine());

            monthlyView.AppendLine("\nSchedule from " + startDateOfMonth + " to " + endDateOfMonth + "\n");

            monthlyView.Append(PrintService.GenerateTable(Get2DListForWeeklyAndMonthlyView(currentMonthEvents)));

            Console.WriteLine(monthlyView);
        }

        private static List<List<string>> Get2DListForWeeklyAndMonthlyView(List<EventModel> currentWeekEvents)
        {
            return currentWeekEvents.InsertInto2DList(["Date", "Day", "Event Title", "Start Time", "End Time"],
                [
                    eventModel => eventModel.EventDate,
                    eventModel => DateTimeManager.GetDayFromDateTime(eventModel.EventDate),
                    eventModel => eventModel.Title,
                    eventModel => DateTimeManager.ConvertTo12HourFormat(eventModel.Duration.StartHour),
                    eventModel => DateTimeManager.ConvertTo12HourFormat(eventModel.Duration.EndHour)
                ]);
        }

    }
}