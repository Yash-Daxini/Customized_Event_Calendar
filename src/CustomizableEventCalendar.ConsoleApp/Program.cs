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

    static readonly IntPtr HWND_TOP = new IntPtr(0);

    public static void Main(string[] args)
    {
        IntPtr handle = GetConsoleWindow();

        SetWindowPos(handle, HWND_TOP, 0, 0, 0, 0, SWP_SHOWWINDOW);

        Console.SetWindowSize(Console.LargestWindowWidth, Console.LargestWindowHeight);

        try
        {
            Authentication.AskForChoice();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

    }
}