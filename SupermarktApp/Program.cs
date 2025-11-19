using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NUnit.Framework.Constraints;
using Spectre.Console;

class Program
{
    static async Task Main()
    { 
        // DatabaseFiller.RunDatabaseMethods(10);
        await MenuUI.ShowMainMenu();
    }
}