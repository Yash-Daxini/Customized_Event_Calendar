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
        //Authentication.LoginOrSignUp();
        RecurrenceEngine recurrenceEngine = new RecurrenceEngine();
        GenericRepository repository = new GenericRepository();
        GenericRepository repository2 = new GenericRepository();
        //recurrenceEngine.ScheduleEvents(repository2.Read<Event>(data=>new Event(data),5), repository.Read<RecurrencePattern>(data => new RecurrencePattern(data), 3));
        recurrenceEngine.ScheduleMonthlyEvents(repository2.Read<Event>(data => new Event(data), 5), repository.Read<RecurrencePattern>(data => new RecurrencePattern(data), 3), new DateTime(2024,2,1));

    }
}