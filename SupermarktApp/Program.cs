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
        DatabaseFiller.RunDatabaseMethods(500);
        //DatabaseFiller.RunDatabaseMethods(10000);
        MenuUI.ShowMainMenu();
    }
}