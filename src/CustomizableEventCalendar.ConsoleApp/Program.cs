using System.Data.SqlClient;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services;
using CustomizableEventCalendar.src.CustomizableEventCalendar.ConsoleApp;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Data.Repositories;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;
namespace CustomizableEventCalendar.ConsoleApp;
public class Program
{
    public static void Main(string[] args)
    {
        Authentication.LoginOrSignUp();
        //RecurrenceEngine recurrenceEngine = new RecurrenceEngine();
        //GenericRepository repository = new GenericRepository(); 
        //recurrenceEngine.AddScheduler(new Event("abc","def","ghi",27,"1-2",3),repository.Read<RecurrencePattern>(data=>new RecurrencePattern(data),3));
    }
}