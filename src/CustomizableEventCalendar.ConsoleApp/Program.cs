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
        //recurrenceEngine.ScheduleMonthlyEvents(repository2.Read<Event>(data => new Event(data), 5), repository.Read<RecurrencePattern>(data => new RecurrencePattern(data), 3), new DateTime(2024,2,1));
        recurrenceEngine.ScheduleYearlyEvents(repository2.Read<Event>(data => new Event(data), 9), repository.Read<RecurrencePattern>(data => new RecurrencePattern(data), 5),Convert.ToDateTime("14-02-2024"));
        //string q = QueryBuilder.Create<RecurrencePattern>("RecurrencePattern",new RecurrencePattern(Convert.ToDateTime("14-02-2024"),Convert.ToDateTime("14-02-2030"),"yearly",null,"2",null,null,"2,3",null,"14,15"));
        //Console.WriteLine(q);

    }
}