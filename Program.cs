using System;
using System.Collections.Generic;
using System.Linq;
using Spectre.Console;

class Program
{
    static void Main()
    {
        DatabaseFiller.RunDatabaseMethods(5000);
        // StartScreen.ShowMainMenu();
        // StatisticsUI.DisplayMenu();
        MenuUI.ShowMainMenu();
    }
}
