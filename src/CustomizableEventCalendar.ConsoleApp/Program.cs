using System.Data.SqlClient;
using System.Linq.Expressions;
using System.Net.Http.Headers;
using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services;
using CustomizableEventCalendar.src.CustomizableEventCalendar.ConsoleApp;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Data.Repositories;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;
namespace CustomizableEventCalendar.ConsoleApp;
public class Program
{
    [DllImport("kernel32.dll", SetLastError = true)]

    static extern IntPtr GetConsoleWindow();


    [DllImport("user32.dll")]

    static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);


    const int SWP_SHOWWINDOW = 0x0040;

    static readonly IntPtr HWND_TOP = new(0);

    public static void Main(string[] args)
    {
        IntPtr handle = GetConsoleWindow();

        SetWindowPos(handle, HWND_TOP, 0, 0, 0, 0, SWP_SHOWWINDOW);

        Console.SetWindowSize(Console.LargestWindowWidth, Console.LargestWindowHeight);

        try
        {
            //Authentication.AuthenticationChoice();

            List<List<string>> l = [
                                      ["Solution No.","Frequency","Interval", "ByDaysOfWeek", "ByMonthDay", "ByDay", "ByMonth"],
                                      ["1","Daily","2","-","-","-","-"],
                                      ["2","Daily","-", "2,3,5,7", "-","-","-"],
                                      ["3","Weekly","3","1,2,5","-","-","-"],
                                      ["4","Monthly","3","-","25","-","-"],
                                      ["5","Monthly", "4","-","-","1TH","-"],
                                      ["6","Yearly","4","-","31","-","May"],
                                      ["7","Yearly","1","-","-","-1FR","March"]
                                   ];

            PrintService printService = new PrintService();
            Console.WriteLine(printService.GenerateTable(l));

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

    }
}