using System;
using System.Collections.Generic;
using System.Linq;
using Spectre.Console;

class Program
{
    static void Main()
    {
        DatabaseFiller.RunDatabaseMethods(1000);
        // StartScreen.ShowMainMenu();
        StatisticsUI.DisplayMenu();
    }
}
