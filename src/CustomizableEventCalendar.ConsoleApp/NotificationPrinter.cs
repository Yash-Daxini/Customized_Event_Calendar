using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Models;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.ConsoleApp
{
    internal static class NotificationPrinter
    {

        public static void ShowNotification()
        {
            NotificationHandler notificationHandler = new();

            Console.ForegroundColor = ConsoleColor.Cyan;

            notificationHandler.PrintNotifications();

            Console.ResetColor();
        }

        public static void DisplayUserInfo()
        {
            Console.Clear();

            //PrintHandler.ShowLoadingAnimation();

            //Console.SetCursorPosition(0, Console.CursorTop);

            //PrintHandler.PrintSuccessMessage($"{PrintHandler.CenterText()}Welcome {GlobalData.GetUser().Name}");

            //Thread.Sleep(1000);

            //Console.Clear();

            PrintHandler.PrintUserName(GlobalData.GetUser().Name);

            Console.WriteLine();

            ShowNotification();
        }
    }
}