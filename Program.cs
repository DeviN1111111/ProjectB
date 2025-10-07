using System;
using System.Collections.Generic;
using System.Linq;
using Spectre.Console;

class Program
{
    static void Main()
    {
        DatabaseFiller.RunDatabaseMethods(1000);
        StatisticsUI.DisplayMenu();
        // DatabaseFiller.RunDatabaseMethods();
        // Console.Clear();
        // StartScreen.Show();
        // LoginUI.Login();

        DatabaseFiller.RunDatabaseMethods();
        ProductLogic.SearchProductByName();
    }
}
