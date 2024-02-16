using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Data.Repositories;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Enums;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.ConsoleApp
{
    internal class EventHandling
    {
        public static EventService eventService = new EventService();
        public static void AskForChoice()
        {
            Console.WriteLine("\n1. Add Event 2. See all events 3. Delete Event 4. Update Event 5. See calendar view  0. Back");
            Console.Write("Select Any Option :- ");

            EventOperationsEnum choice = (EventOperationsEnum)Convert.ToInt32(Console.ReadLine());

            switch (choice)
            {
                case EventOperationsEnum.Add:
                    AddEvent();
                    break;
                case EventOperationsEnum.Display:
                    Display();
                    break;
                case EventOperationsEnum.Delete:
                    Delete();
                    break;
                case EventOperationsEnum.Update:
                    Update();
                    break;
                case EventOperationsEnum.View:
                    CalendarView.ViewSelection();
                    break;
                case EventOperationsEnum.Back:
                    Console.WriteLine("Going Back ...");
                    break;
                default:
                    Console.WriteLine("Oops! Wrong choice");
                    break;
            }

            if (!choice.Equals(EventOperationsEnum.Back)) AskForChoice();
            else Authentication.AskForChoice();
        }
        public static void GetEventDetails(ref Event eventObj)
        {
            Console.WriteLine("\nFill Details Related to Event : ");

            eventObj = new Event();

            PropertyInfo[] properties = eventObj.GetType().GetProperties().Where(property => !Attribute.IsDefined(property, typeof(NotMappedAttribute)) && !property.Name.EndsWith("Id")).ToArray();

            foreach (PropertyInfo property in properties)
            {
                Console.Write($"Enter value for {property.Name}: ");
                string value = Console.ReadLine();
                object typedValue = Convert.ChangeType(value, property.PropertyType);
                property.SetValue(eventObj, typedValue);
            }
        }
        public static void AddEvent()
        {
            Event eventObj = new Event();
            GetEventDetails(ref eventObj);

            eventObj.UserId = GlobalData.user.Id;

            eventObj.RecurrenceId = RecurrenceHandling.AskForRecurrenceChoice(eventObj.RecurrenceId);

            eventService.Create(eventObj);
        }
        public static void Display()
        {
            string eventList = eventService.GenerateEventList();
            Console.WriteLine(eventList);
        }
        public static void Delete()
        {
            Display();

            Console.Write("From Above events give event no. that you want to delete :- ");
            int Id = Convert.ToInt32(Console.ReadLine());

            eventService.Delete(Id);

        }
        public static void Update()
        {
            Display();

            Console.Write("From Above events give event no. that you want to update :- ");
            int Id = Convert.ToInt32(Console.ReadLine());

            Event eventObj = eventService.Read(Id);

            GetEventDetails(ref eventObj);

            eventObj.UserId = GlobalData.user.Id;

            eventObj.RecurrenceId = RecurrenceHandling.AskForRecurrenceChoice(eventObj.RecurrenceId);

            eventService.Update(eventObj, Id);
        }
    }
}