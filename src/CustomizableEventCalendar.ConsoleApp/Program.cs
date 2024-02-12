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
        Authentication authentication = new Authentication();
        authentication.LoginOrSignUp();
        //Console.WriteLine(QueryBuilder.Read<User>("User"));
        //Console.WriteLine(QueryBuilder.Read<User>("User", 1));
        //Console.WriteLine(QueryBuilder.Insert<User>("User", new User("abc", "def", "gjhi")));
        //Console.WriteLine(QueryBuilder.Insert<Scheduler>("Scheduler", new Scheduler(1, "2PM - 4PM", DateTime.Now)));
        //Console.WriteLine(QueryBuilder.Update<User>("User", new User("abc", "def", "gjhi"), 1));
        //Console.WriteLine(QueryBuilder.Delete<User>("User", 1));
        //GenericRepository genericRepository = new GenericRepository();
        //List<User> users = genericRepository.Read<User>(data=>new User(data));
        //users.ForEach(u => Console.WriteLine(u));
        //User user = genericRepository.Read<User>(data=>new User(data),1);
    }
}