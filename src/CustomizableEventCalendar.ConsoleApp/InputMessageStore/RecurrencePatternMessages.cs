using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.ConsoleApp.InputMessageStore
{
    public static class RecurrencePatternMessages
    {
        public static string StartDate = "Enter Start Date :-  (Please enter date in dd-mm-yyyy hh:mm:ss) :- ";

        public static string EndDate = "Enter Start Date :-  (Please enter date in dd-mm-yyyy hh:mm:ss) :- ";

        public static string Frequency = "How frequent you want to repeat event :- \n 1. Daily\t2. Weekly\t3. Monthly\t4. Yearly :-  ";

        public static string Interval = "Enter Interval : (how much gap you need between two repetitive event Ex:- 1 or 2 or 3) :-  ";

        public static string Days = @"Please Enter Week days you want to repeat :- (Enter days number from 1 to 7 
                                    , Monday = 1 like this. Add all day number comma separated like 1,4,5 )  :-  ";

        public static string Months = @"Please Enter month you want to repeat :- (Enter month number from 1 to 12 , 
                                        Add all day number comma separated like 1,4,5 ) :-  ";

        public static string MonthDays = @"Please Enter month days you want to repeat :- (Enter days number from 1 to number of
                              days in month , 1 day or 2 day like this. Add all day number comma separated like 1,4,5 )  :-  ";
    }
}
