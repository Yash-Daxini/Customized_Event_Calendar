using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services
{
    internal class PrintService
    {
        public static string GenerateTable(List<List<string>> data)
        {
            StringBuilder table = new();

            List<int> columnWidths = FindTheMaxWidthOfColumns(data);

            for (int row = 0; row < data.Count; row++)
            {
                if (row == 0)
                {
                    table.Append('┌');

                    for (int col = 0; col < data[row].Count; col++)
                    {
                        table.Append(new string('─', columnWidths[col] + 2) + "┬");
                    }

                    table.AppendLine();
                }

                table.Append('│');

                for (int col = 0; col < data[row].Count; col++)
                {
                    table.Append(data[row][col].PadRight(columnWidths[col] + 2));
                    table.Append('│');
                }

                table.AppendLine();

                if (row < data.Count - 1)
                {
                    table.Append('├');

                    for (int col = 0; col < data[row].Count; col++)
                    {
                        table.Append(new string('─', columnWidths[col] + 2) + "┼");
                    }

                    table.AppendLine();
                }
                else
                {
                    table.Append('└');

                    for (int col = 0; col < data[row].Count; col++)
                    {
                        table.Append(new string('─', columnWidths[col] + 2) + "┴");
                    }

                    table.AppendLine();
                }
            }

            return table.ToString();
        }
        public static string GenerateTableForNotification(List<List<string>> data)
        {
            StringBuilder table = new();

            List<int> columnWidths = FindTheMaxWidthOfColumns(data);

            for (int row = 0; row < data.Count; row++)
            {

                if (row == 0)
                {
                    table.Append('╔');

                    for (int col = 0; col < data[row].Count; col++)
                    {
                        table.Append(new string('═', columnWidths[col] + 2) + "╦");
                    }

                    table.AppendLine();
                }

                table.Append('║');

                for (int col = 0; col < data[row].Count; col++)
                {
                    table.Append(data[row][col].PadRight(columnWidths[col] + 2));
                    table.Append('║');
                }

                table.AppendLine();

                if (row < data.Count - 1)
                {
                    table.Append('╠');
                    for (int col = 0; col < data[row].Count; col++)
                    {
                        table.Append(new string('═', columnWidths[col] + 2) + "╬");
                    }
                    table.AppendLine();

                }
                else
                {
                    table.Append('╚');

                    for (int col = 0; col < data[row].Count; col++)
                    {
                        table.Append(new string('═', columnWidths[col] + 2) + "╩");
                    }

                    table.AppendLine();
                }
            }

            return table.ToString();
        }

        private static List<int> FindTheMaxWidthOfColumns(List<List<string>> data)
        {
            List<int> columnWidths = [];

            for (int col = 0; col < data[0].Count; col++)
            {
                int maxWidth = 0;

                foreach (var row in data)
                {
                    maxWidth = Math.Max(maxWidth, row[col].Length);
                }

                columnWidths.Add(maxWidth);
            }

            return columnWidths;

        }
    }
}
