using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Data.Repositories;
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
                    Display();
                    break;
                case "0":
                    Console.WriteLine("Back");
                    break;
                default:
                    Console.WriteLine("Oops! Wrong choice"); AskForChoice();
                    break;
            }

        }
        public static void AddEvent()
        {
            Console.WriteLine("\nFill Details Related to Event : ");
            Event eventObj = new Event();
            PropertyInfo[] properties = eventObj.GetType().GetProperties().Where(property => !Attribute.IsDefined(property, typeof(NotMappedAttribute)) && !property.Name.EndsWith("Id")).ToArray();
            foreach (PropertyInfo property in properties)
            {
                Console.Write($"Enter value for {property.Name}: ");
                string value = Console.ReadLine();
                object typedValue = Convert.ChangeType(value, property.PropertyType);
                property.SetValue(eventObj, typedValue);
            }
            eventObj.UserId = GlobalData.user.Id;
            eventObj.RecurrenceId = RecurrenceHandling.AskForRecurrenceChoice();
            GenericRepository genericRepository = new GenericRepository();
            genericRepository.Create<Event>(eventObj);
            AskForChoice();
        }
        public static void Display()
        {
            GenericRepository genericRepository = new GenericRepository();
            List<Event> events = genericRepository.Read(data => new Event(data)).Where(eventObj => eventObj.UserId == GlobalData.user.Id).ToList();
            foreach (var eventObj in events)
            {
                Console.WriteLine(eventObj);
            }
            AskForChoice();
        }
    }
}