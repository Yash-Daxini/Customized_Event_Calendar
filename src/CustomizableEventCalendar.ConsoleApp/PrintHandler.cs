using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
