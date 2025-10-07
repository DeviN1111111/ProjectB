using System;
using System.Collections.Generic;
using System.Linq;
using Spectre.Console;

class Program
{
    static void Main()
    {
        DatabaseFiller.RunDatabaseMethods();
        Console.Clear();
        StartScreen.Show();
        // LoginUI.Login();
    }
}
