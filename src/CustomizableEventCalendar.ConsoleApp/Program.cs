using System.Runtime.InteropServices;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Business.Services;
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

        SetWindowPos(handle, HWND_TOP, -5, -5, 0, 0, SWP_SHOWWINDOW);

        Console.SetWindowSize(Console.LargestWindowWidth, Console.LargestWindowHeight - 1);

        try
        {
            Authentication.AuthenticationChoice();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

    }
}