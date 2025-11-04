using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NUnit.Framework.Constraints;
using Spectre.Console;

class Program
{
    static void Main()
    {
        //DatabaseFiller.RunDatabaseMethods(5000);
        // StartScreen.ShowMainMenu();
        // StatisticsUI.DisplayMenu();
        MenuUI.ShowMainMenu();
    }
}

// Als out of stock laten zien op search menu dat die out of search is.
// Product details gelijk laten zien als je product selecteert.
//