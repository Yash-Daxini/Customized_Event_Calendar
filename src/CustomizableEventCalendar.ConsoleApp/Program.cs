﻿using System.Data.SqlClient;
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
    public static void Main(string[] args)
    {
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