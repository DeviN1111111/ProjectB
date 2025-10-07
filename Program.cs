using System;
using System.Collections.Generic;
using System.Linq;
using Spectre.Console;

class Program
{
    static void Main()
    {
        DatabaseFiller.RunDatabaseMethods(1000);
        // StatisticsUI.DisplayMenu();
        // LoginUI.Login();
        StartScreen.ShowMainMenu();
    }
}
