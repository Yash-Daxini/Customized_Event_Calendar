using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.ConsoleApp
{
    internal class EventHandling
    {
        public static void askForChoice()
        {
            Console.WriteLine("1. Add Event 2. See all events 0. Back");
            Console.WriteLine("Select Any Option : ");
            string? choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    Console.WriteLine("Add Event");
                    break;
                case "2":
                    Console.WriteLine("See events");
                    break;
                case "3":
                    Console.WriteLine("Bye bye");
                    break;
                default:
                    Console.WriteLine("Oops! Wrong choice"); askForChoice();
                    break;
            }

        }
    }
}