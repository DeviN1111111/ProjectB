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
        DatabaseFiller.RunDatabaseMethods(1000);
        MenuUI.ShowMainMenu();
    }
}