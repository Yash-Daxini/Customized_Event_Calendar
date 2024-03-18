using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.ConsoleApp
{
    internal static class PrintHandler
    {

        public static string CenterText()
        {
            int padding = Console.WindowWidth / 2;

            return $"{new string(' ', padding)}";
        }

        public static string PrintHorizontalLine()
        {
            return new string('-', Console.WindowWidth);
        }

        public static void PrintInvalidMessage(string message)
        {
            Console.WriteLine(message);
        }

        public static void ShowLoadingAnimation()
        {
            string message = "Fetching your data";

            Console.SetCursorPosition((Console.WindowWidth - message.Length) / 2, Console.CursorTop);
            Console.WriteLine(message);

            for (int i = 0; i <= 100; i++)
            {
                Console.SetCursorPosition(0, Console.CursorTop);
                Console.Write(new string(' ', Console.WindowWidth));

                Console.SetCursorPosition((Console.WindowWidth - 100) / 2, Console.CursorTop);

                Console.Write("Progress: [{0}{1}] {2}%", new string('=', i), new string(' ', 100 - i), i);

                Thread.Sleep(10);
            }

            Console.Clear();
        }

        public static void PrintEndingLine(int left)
        {
            Console.SetCursorPosition(left + 1, Console.CursorTop - 1);

            Console.WriteLine("|");
        }

        public static string GiveTable(List<List<string>> data)
        {

            string table = PrintService.GenerateTable(data);

            return table.ToString();
        }

        public static string GiveTableForNotification(List<List<string>> data)
        {

            string table = PrintService.GenerateTableForNotification(data);

            return table.ToString();
        }

        public static void SetCursorToMiddle()
        {
            Console.SetCursorPosition((Console.WindowWidth - 30) / 2, Console.CursorTop);
        }
        public static void SetCursorToMiddle(int nameLength)
        {
            Console.SetCursorPosition((Console.WindowWidth / 2) - nameLength, Console.CursorTop);
        }

        public static void PrintUserName(string userName)
        {
            int length = userName.Length + 20;

            SetCursorToMiddle(userName.Length);
            Console.WriteLine("╔" + new string('═', length) + "╗");

            SetCursorToMiddle(userName.Length);
            Console.WriteLine("║" + new string(' ', length) + "║");

            SetCursorToMiddle(userName.Length);
            Console.WriteLine($"{new string(' ', length / 2)}{userName}{new string(' ', length)}");

            SetCursorToMiddle(userName.Length);
            Console.WriteLine("║" + new string(' ', length) + "║");

            SetCursorToMiddle(userName.Length);
            Console.WriteLine("╚" + new string('═', length) + "╝");
        }

        public static void PrintWithColor(string message, ConsoleColor consoleColor)
        {
            Console.ForegroundColor = consoleColor;
            Console.WriteLine("\n" + message + "\n");
            Console.ResetColor();
        }

        public static void PrintErrorMessage(string message)
        {
            PrintWithColor(message, ConsoleColor.Red);
        }

        public static void PrintSuccessMessage(string message)
        {
            PrintWithColor(message, ConsoleColor.Green);
        }

        public static void PrintNewLine()
        {
            Console.WriteLine("\n");
        }

        public static void PrintInfoMessage(string message)
        {
            PrintWithColor(message, ConsoleColor.Cyan);
        }

        public static void PrintWarningMessage(string message)
        {
            PrintWithColor(message, ConsoleColor.Yellow);
        }

        public static void PrintNotification(string message)
        {
            PrintWithColor(message, ConsoleColor.Cyan);
        }
    }
}