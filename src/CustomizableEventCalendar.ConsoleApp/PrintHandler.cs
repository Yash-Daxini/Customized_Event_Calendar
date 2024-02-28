using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.ConsoleApp
{
    internal class PrintHandler
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
            Console.WriteLine("Fetching your data");
            for (int i = 0; i <= 100; i++)
            {
                Console.SetCursorPosition(0, Console.CursorTop);
                Console.Write(new string(' ', Console.WindowWidth));

                Console.SetCursorPosition(0, Console.CursorTop);

                Console.Write("Progress: [{0}{1}] {2}%", new string('=', i), new string(' ', 100 - i), i);

                Thread.Sleep(10);
            }
            Console.Clear();
        }
        public static string PrintTable(List<List<string>> data)
        {
            StringBuilder table = new StringBuilder();

            var columnWidths = new List<int>();
            for (int col = 0; col < data[0].Count; col++)
            {
                int maxWidth = 0;
                foreach (var row in data)
                {
                    maxWidth = Math.Max(maxWidth, row[col].Length);
                }
                columnWidths.Add(maxWidth);
            }

            for (int col = 0; col < data[0].Count; col++)
            {
                for (int row = 0; row < data.Count; row++)
                {
                    columnWidths[col] = Math.Max(columnWidths[col], data[row][col].Length);
                }
            }

            for (int row = 0; row < data.Count; row++)
            {
                if (row == 0)
                {
                    table.Append("┌");
                    for (int col = 0; col < data[row].Count; col++)
                    {
                        table.Append(new string('─', columnWidths[col] + 2) + "┬");
                    }
                    table.AppendLine();
                }

                table.Append("│");
                for (int col = 0; col < data[row].Count; col++)
                {
                    table.Append(data[row][col].PadRight(columnWidths[col] + 2));
                    table.Append("│");
                }
                table.AppendLine();

                if (row < data.Count - 1)
                {
                    table.Append("├");
                    for (int col = 0; col < data[row].Count; col++)
                    {
                        table.Append(new string('─', columnWidths[col] + 2) + "┼");
                    }
                    table.AppendLine();
                }
                else
                {
                    table.Append("└");
                    for (int col = 0; col < data[row].Count; col++)
                    {
                        table.Append(new string('─', columnWidths[col] + 2) + "┴");
                    }
                    table.AppendLine();
                }
            }

            return table.ToString();
        }
    }
}
