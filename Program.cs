using System;
using System.Collections.Generic;
using System.Linq;
using Spectre.Console;

class Program
{
    static void Main()
    {
        DatabaseFiller.RunDatabaseMethods(200);
        // StartScreen.ShowMainMenu();
        // StatisticsUI.DisplayMenu();
        MenuUI.ShowMainMenu();

    }
}

// Als out of stock laten zien op search menu dat die out of search is.
// Product details gelijk laten zien als je product selecteert.
//