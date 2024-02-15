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

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.ConsoleApp
{
    internal class EventHandling //WIP
    {
        public static EventService eventService = new EventService();
        public static void AskForChoice()
        {
            Console.WriteLine("\n1. Add Event 2. See all events 3. Delete Event 4. Update Event 5. See calendar view  0. Back");
            Console.Write("Select Any Option :- ");
            string? choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    AddEvent();
                    AskForChoice();
                    break;
                case "2":
                    Display();
                    AskForChoice();
                    break;
                case "3":
                    Delete();
                    AskForChoice();
                    break;
                case "4":
                    Update();
                    AskForChoice();
                    break;
                case "5":
                    CalendarView.ViewSelection();
                    AskForChoice();
                    break;
                case "0":
                    Console.WriteLine("Going Back ...");
                    Authentication.AskForChoice();
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
            eventObj.RecurrenceId = RecurrenceHandling.AskForRecurrenceChoice(eventObj.RecurrenceId);
            eventService.Create(eventObj);
        }
        public static void Display()
        {
            List<Event> events = eventService.Read().Where(eventObj => eventObj.UserId == GlobalData.user.Id).ToList();
            StringBuilder eventDetails = new StringBuilder();
            eventDetails.AppendLine("Event No. ,\tTitle,\tDescription,\tLocation,\tTimeBlock");
            foreach (var eventObj in events)
            {
                eventDetails.AppendLine(eventObj.ToString());
            }
            Console.WriteLine(eventDetails);
        }
        public static void Delete()
        {
            Display();
            Console.Write("From Above events give event no. that you want to delete :- ");
            int Id = Convert.ToInt32(Console.ReadLine());
            Event eventObj = eventService.Read(Id);
            int recurrenceId = eventObj == null ? 0 : Convert.ToInt32(eventObj.RecurrenceId);
            eventService.Delete(Id);
            RecurrenceService recurrenceService = new RecurrenceService();
            recurrenceService.Delete(recurrenceId);
        }
        public static void Update()
        {
            Display();
            Console.Write("From Above events give event no. that you want to update :- ");
            int Id = Convert.ToInt32(Console.ReadLine());
            Event eventObj = eventService.Read(Id);
            PropertyInfo[] properties = eventObj.GetType().GetProperties().Where(property => !Attribute.IsDefined(property, typeof(NotMappedAttribute)) && !property.Name.EndsWith("Id")).ToArray();
            foreach (PropertyInfo property in properties)
            {
                Console.Write($"Enter value for {property.Name}: ");
                string value = Console.ReadLine();
                object typedValue = Convert.ChangeType(value, property.PropertyType);
                property.SetValue(eventObj, typedValue);
            }
            eventObj.UserId = GlobalData.user.Id;
            eventObj.RecurrenceId = RecurrenceHandling.AskForRecurrenceChoice(eventObj.RecurrenceId);
            eventService.Update(eventObj, Id);
        }
    }
}