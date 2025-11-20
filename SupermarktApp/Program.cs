using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NUnit.Framework.Constraints;
using Spectre.Console;
using System.Globalization;

class Program
{
    static async Task Main()
    { 
        //DatabaseFiller.RunDatabaseMethods(2500);
        await MenuUI.ShowMainMenu();
    }
}