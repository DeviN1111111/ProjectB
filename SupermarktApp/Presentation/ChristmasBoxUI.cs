using Spectre.Console;
using System;
using System.Linq;

public static class ChristmasBoxUI
{
    public static void Show()
    {
        while(true)
        {
            Console.Clear();
            Utils.PrintTitle("Christmas Boxes");

            var boxes = ChristmasBoxLogic.GetAvailableBoxes();

            if (boxes.Count == 0)
            {
                AnsiConsole.MarkupLine("[red]No Christmas boxes available at the moment.[/]");
                AnsiConsole.MarkupLine("\n[grey]Press [green]ENTER[/] to go back[/]");
                Console.ReadKey(true);
                return;
            }

            var table = Utils.CreateTable(new[]
            {
                "[white]#[/]",
                "[white]Box[/]",
                "[white]Price[/]",
                "[white]Items[/]"
            });

            for (int i = 0; i < boxes.Count; i++)
            {
                var box = boxes[i];

                table.AddRow(
                    (i + 1).ToString(),
                    box.Name,
                    $"€{box.Price}",
                    box.Products.Count.ToString()
                );
            }

            AnsiConsole.Write(table);

            var backOption = new ChristmasBoxModel
            {
                ID = -1,
                Name = "Go back",
                Price = 0
            };

            var selectableBoxes = boxes.Append(backOption).ToList();

            var selectedBox = Utils.CreateSelectionPrompt(
                selectableBoxes,
                title: "[white]Choose a Christmas box[/]",
                format: box =>
                    box.ID == -1
                        ? "[grey]Go back[/]"
                        : $"{box.Name} — €{box.Price}"
            );

            // escape
            if (selectedBox.ID == -1)
            {
                return;
            }

            Console.Clear();
            Utils.PrintTitle(selectedBox.Name);

            if (selectedBox.Products.Any())
            {
                var list = new List<string>(selectedBox.Products.Select(p => p.Name));

                AnsiConsole.Write(
                    new Panel(new Markup("[white]" + string.Join("\n", list.Select(n => $"• {Markup.Escape(n)}")) + "[/]"))
                        .Header("[yellow]Contents[/]")
                        .Border(BoxBorder.Double)
                );
            }
            else
            {
                AnsiConsole.Write(
                    new Panel("[grey]No items yet[/]")
                        .Header("[yellow]Contents[/]")
                        .Border(BoxBorder.Double)
                );
            }

            // ask user to confirm
            bool confirm = Utils.CreateYesNoSelectionPrompt(
                "[yellow]Add this box to your cart?[/]"
            );
            
            if (!confirm)
            { 
                continue;
            }
            // addtocart
            bool added = ChristmasBoxLogic.TryAddChristmasBoxToCart(selectedBox);

            if (added)
            {
                AnsiConsole.MarkupLine("\n[green]Added to cart![/]");
            }   
            else if(selectedBox.Products == null || selectedBox.Products.Count == 0)
            {
                AnsiConsole.MarkupLine("\n[red]This Christmas box is not available yet.[/]");
            }
            else if (OrderItemAccess.HasUserPurchasedProduct(SessionManager.CurrentUser!.ID, selectedBox.ID))
            {
                AnsiConsole.MarkupLine("\n[yellow]You already purchased this Christmas box size before.[/]");
            }
            else
            {
                AnsiConsole.MarkupLine("\n[red]You can only buy one Christmas box per size.[/]");
            }
                
            AnsiConsole.MarkupLine("\n[grey]Press [green]ENTER[/] to go back[/]");
            Console.ReadKey();
        }   
    }
}