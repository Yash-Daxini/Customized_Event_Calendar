using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.ConsoleApp
{
    internal class EventHandling
    {
        public static void AskForChoice()
        {
            Console.WriteLine("1. Add Event 2. See all events 0. Back");
            Console.WriteLine("Select Any Option :- ");
            string? choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    AddEvent();
                    break;
                case "2":
                    Console.WriteLine("See events");
                    break;
                case "0":
                    Console.WriteLine("Bye bye");
                    break;
                default:
                    Console.WriteLine("Oops! Wrong choice"); AskForChoice();
                    break;
            }

        }
        public static void AddEvent()
        {
            Console.WriteLine("\nFill Details Related to Event : ");
            Console.Write("Enter Title :- ");
            string title = Console.ReadLine();
            Console.Write("Enter Location :- ");
            string location = Console.ReadLine();
            Console.Write("Enter Description :- ");
            string description = Console.ReadLine();
            int userId = GlobalData.user.Id;
            int? id = RecurrenceHandling.AskForRecurrenceChoice();
            //Now add the event from here
        }
        public void SeeEvents()
        {

        }
    }
}