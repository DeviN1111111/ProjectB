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
        IDatabaseFactory dbFactory = new SqliteDatabaseFactory("Data Source=database.db");

        Console.OutputEncoding = System.Text.Encoding.UTF8;
        DatabaseFiller.RunDatabaseMethods(100);
        DiscountsLogic.AddExpiryDateDiscounts(daysBeforeExpiry: 3, discountPercentage: 50); // Give products discounts that are about to expire in 3 days (remove this method and add to admin)
        await MenuUI.ShowMainMenu();
    }
}