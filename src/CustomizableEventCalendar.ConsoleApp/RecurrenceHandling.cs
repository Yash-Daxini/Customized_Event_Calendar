using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Data.Repositories;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.ConsoleApp
{
    internal class RecurrenceHandling
    {
        public static int? AskForRecurrenceChoice()
        {
            Console.WriteLine("Are you want repeat this event ? \n 1. Yes 2. No ");
            string isRepeative = Console.ReadLine();
            switch (isRepeative)
            {
                case "1":
                    int id = AddRecurrence();
                    return id;
                    break;
                case "2":
                    return null;
                    break;
                default:
                    Console.WriteLine("Please enter correct value : ");
                    AskForRecurrenceChoice();
                    break;

            }
            return null;
        }
        public static int AddRecurrence()
        {
            RecurrencePattern recurrencePattern = new RecurrencePattern();
            Console.WriteLine("Fill details to make event repetive :- ");
            Console.Write("Enter Start Date :-  (Please enter date in dd-mm-yyyy hh:mm:ss)");
            string DTSTAR = Console.ReadLine();
            recurrencePattern.DTSTART = Convert.ToDateTime(DTSTAR);
            Console.Write("Enter End Date :-  (Please enter date in dd-mm-yyyy hh:mm:ss)");
            string UNTILL = Console.ReadLine();
            recurrencePattern.UNTILL = Convert.ToDateTime(UNTILL);
            Console.WriteLine("How frequent you want to repet event :- \n 1. Daily\t2. Weekly\t3. Monthly\t4. Yearly ");
            string choiceForFreq = Console.ReadLine();
            switch (choiceForFreq)
            {
                case "1":
                    recurrencePattern.FREQ = "daily";
                    recurrencePattern.BYDAY = DailyRecurrence();
                    break;
                case "2":
                    recurrencePattern.FREQ = "weekly";
                    recurrencePattern.BYDAY = WeeklyRecurrence();
                    break;
                case "3":
                    recurrencePattern.FREQ = "monthly";
                    recurrencePattern.BYWEEK = MonthlyRecurrence();
                    break;
                case "4":
                    recurrencePattern.FREQ = "yearly";
                    recurrencePattern.BYMONTH = YearlyRecurrence().Split("-")[0];
                    recurrencePattern.BYMONTHDAY = YearlyRecurrence().Split("-")[1];
                    break;
                default:
                    Console.WriteLine("Please Enter correct option !");
                    AddRecurrence();
                    break;
            }
            Console.Write("Enter Interval : (how much gap you need between two repetive event) Ex:- 1 or 2 or 3 ");
            string INTERVAL = Console.ReadLine();
            recurrencePattern.INTERVAL = INTERVAL;
            GenericRepository genericRepository = new GenericRepository();
            int id = genericRepository.Create<RecurrencePattern>(new RecurrencePatternQuerySupplier(), recurrencePattern.generateDictionary());
            Console.WriteLine("Event Added Successfull !");
            return id
        }
        public static string DailyRecurrence()
        {
            Console.WriteLine("Please Enter Week days you want to repeat :- (Enter days number from 1 to 7 \t Sunday = 1 like this. Add all day number comma separated like 1,4,5 )");
            string days = Console.ReadLine();
            return days;
        }
        public static string WeeklyRecurrence()
        {
            Console.WriteLine("Please Enter Week days you want to repeat :- (Enter days number from 1 to 7 \t Sunday = 1 like this. Add all day number comma separated like 1,4,5 )");
            string days = Console.ReadLine();
            return days;
        }
        public static string MonthlyRecurrence()
        {
            Console.WriteLine("Please Enter month days you want to repeat :- (Enter days number from 1 to number of days in month \t 1 day or 2 day like this. Add all day number comma separated like 1,4,5 )");
            string days = Console.ReadLine();
            return days;
        }
        public static string YearlyRecurrence()
        {
            Console.WriteLine("Please Enter month you want to repeat :- (Enter month number from 1 to 12 \t 1 day or 2 day like this. Add all day number comma separated like 1,4,5 )");
            string days = Console.ReadLine();
            Console.WriteLine("Please Enter month days you want to repeat :- (Enter days number from 1 to number of days in month \t 1 day or 2 day like this. Add all day number comma separated like 1,4,5 )");
            string month = Console.ReadLine();
            return days + "-" + month;
        }
    }
}
