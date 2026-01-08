using Spectre.Console;
using System.Linq;
using System;
using System.Collections.Generic;
using SQLitePCL;

public static class AdminChristmasBoxUI
{
    public static void Show()
    {
        while (true)
        {
            Console.Clear();
            Utils.PrintTitle("Christmas Box Admin");

            var allProducts = ChristmasBoxLogic.GetAllProductsForChristmasBoxAdmin()
                .OrderBy(p => p.Category)
                .ThenBy(p => p.Name)
                .ToList();

            var categories = allProducts.Select(p => p.Category)
                .Distinct()
                .OrderBy(c => c)
                .ToList();

            categories.Add("Exit");

            var chosenCategory = Utils.CreateSelectionPrompt(
                categories, "[white]Pick a category to manage (or Exit)[/]");

            if (chosenCategory == "Exit")
                return;
            
            var productsInCategory = allProducts
                .Where (p => p.Category == chosenCategory)
                .OrderBy(p => p.Name)
                .ToList();

            Console.Clear();
            Utils.PrintTitle($"Category: {chosenCategory}");

            var action = Utils.CreateSelectionPrompt(
                new List<string>
                {
                    "Toggle SOME items",
                    "Mark ALL as eligible",
                    "Remove ALL eligibility",
                    "Back"
                },
                "[white]What do you want to do?[/]"
            );

            switch (action)
            {
                case "Back":
                    continue;
                
                case "Mark ALL as eligible":
                    ChristmasBoxLogic.SetChristmasBoxEligibility(productsInCategory.Select(p => p.ID), true);
                    AnsiConsole.MarkupLine($"[green]Marked {productsInCategory.Count} item(s) as eligible.[/]");
                    Pause();
                    continue;

                case "Remove ALL eligibility":
                ChristmasBoxLogic.SetChristmasBoxEligibility(
                    productsInCategory.Select(p => p.ID),
                    false
                );
                AnsiConsole.MarkupLine(
                    $"[green]Removed eligibility from {productsInCategory.Count} item(s).[/]"
                );
                Pause();
                continue;

                case "Toggle SOME items":
                var selected = Utils.CreateMultiSelectionPrompt(
                    productsInCategory,
                    $"[white]Select items to TOGGLE in[/] [yellow]{chosenCategory}[/]",
                    p =>
                    {
                        var status = p.IsChristmasBoxItem ? "[green]✓[/]" : "[grey]·[/]";
                        return $"{status} {p.ID} - {p.Name}";
                    }
                );

                if (selected.Count == 0)
                {
                    AnsiConsole.MarkupLine("[grey]No changes made.[/]");
                    Pause();
                    continue;
                }

                ChristmasBoxLogic.ToggleChristmasBoxEligibility(
                    selected.Select(p => p.ID)
                );
                AnsiConsole.MarkupLine(
                    $"[green]Toggled {selected.Count} item(s).[/]"
                );
                Pause();
                continue;
            }
        }
    }
    private static void Pause()
    {
        AnsiConsole.MarkupLine("\nPress [green]ENTER[/] to continue");
        Console.ReadLine();
    }
}
