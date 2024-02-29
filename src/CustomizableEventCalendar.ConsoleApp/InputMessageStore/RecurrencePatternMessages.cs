using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.ConsoleApp.InputMessageStore
{
    public static class RecurrencePatternMessages
    {
        public readonly static string StartDate = "Enter Start Date :-  (Please enter date in dd-mm-yyyy hh:mm:ss) :- ";

        public readonly static string EndDate = "Enter End Date :-  (Please enter date in dd-mm-yyyy hh:mm:ss) :- ";

        public readonly static string Frequency = "How frequent you want to repeat the event:" +
                                                  "\n1. Daily\t2. Weekly\t3. Monthly\t4. Yearly: ";

        public readonly static string Interval = "Enter Interval: (Enter the gap you need between two repetitive events," +                                           "e.g., 1, 2, or 3): ";

        public readonly static string Days = "Please Enter Week days you want to repeat: (Enter the days number from 1 to 7," +
                                             "e.g., Monday = 1, Tuesday = 2, ... etc. Add all the day numbers separated by" +               "commas like 1, 4, 5): ";

        public readonly static string Months = "Please Enter the months you want to repeat: (Enter the month numbers from"+ 
                                               "1 to 12. Add all month numbers separated by commas like 1, 4, 5): ";

        public readonly static string MonthDays = "Please Enter the days of the month you want to repeat:"+ "" +
                                                  "(Enter the day numbers from 1 to the number of days in the month."+ "" +
                                                  "Add all day numbers separated by commas like 1, 4, 5): ";
    }
}
