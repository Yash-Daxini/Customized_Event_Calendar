using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.ConsoleApp.InputMessageStore
{
    public static class RecurrencePatternMessages
    {
        public readonly static string StartDate = "Enter Start Date :-  (Please enter date in dd-mm-yyyy) :- ";

        public readonly static string EndDate = "Enter End Date :-  (Please enter date in dd-mm-yyyy) :- ";

        public readonly static string Frequency = "How frequent you want to repeat the event:" +
                                                  "\n1. Daily\t2. Weekly\t3. Monthly\t4. Yearly: ";
    }
}
