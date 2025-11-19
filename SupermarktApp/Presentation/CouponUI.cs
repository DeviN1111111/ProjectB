using Spectre.Console;

public static class CouponUI
{
    public static void DisplayMenu()
    {
        while (true)
        {
            Console.Clear();
            AnsiConsole.Write(
                new FigletText("Coupons")
                    .Centered()
                    .Color(MenuUI.AsciiPrimary));

            var user = SessionManager.CurrentUser;
            if (user == null)
            {
                AnsiConsole.MarkupLine("[red]You must be logged in to view coupons.[/]");
                AnsiConsole.MarkupLine("Press [green]ENTER[/] to continue");
                Console.ReadKey();
                return;
            }

            var coupons = CouponLogic.GetAllCoupons(user.ID)
                .Where(c => c.IsValid && c.Credit > 0)
                .ToList();

            if (coupons.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]You have no valid coupons.[/]");
                AnsiConsole.MarkupLine("Press [green]ENTER[/] to continue");
                Console.ReadKey();
                return;
            }

            var appliedCouponId = Order.SelectedCouponId;

            var orderedCoupons = coupons
                .Select((coupon, index) => new
                {
                    Coupon = coupon,
                    Number = index + 1,
                    RoundedCredit = Math.Round(coupon.Credit, 2),
                    IsApplied = appliedCouponId.HasValue && appliedCouponId.Value == coupon.Id
                })
                .ToList();

            // Table display
            var couponTable = new Table()
                .BorderColor(MenuUI.AsciiPrimary)
                .AddColumn("[white]Coupon[/]")
                .AddColumn("[white]Credit[/]");

            foreach (var entry in orderedCoupons)
            {
                couponTable.AddRow($"#{entry.Number}", $"\u20ac[green]{entry.RoundedCredit}[/]");
            }

            AnsiConsole.Write(couponTable);

            var choices = orderedCoupons
                .Select(entry =>
                {
                    var label = $"Coupon #{entry.Number} - \u20ac[green]{entry.RoundedCredit}[/]";
                    if (entry.IsApplied)
                    {
                        label += " [bold][applied][/]";
                    }

                    return (
                        Coupon: entry.Coupon,
                        Label: label,
                        Number: entry.Number,
                        IsApplied: entry.IsApplied
                    );
                })
                .ToList();

            var selection = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Select a coupon to manage")
                    .PageSize(10)
                    .AddChoices(choices.Select(c => c.Label).Concat(new[] { "Go back" })));

            if (selection == "Go back")
            {
                appliedCouponId = Order.SelectedCouponId;

                if (appliedCouponId.HasValue)
                {
                    var appliedEntry = orderedCoupons
                        .FirstOrDefault(e => e.Coupon.Id == appliedCouponId.Value);

                    if (appliedEntry != null)
                    {
                        var rounded = Math.Round(appliedEntry.Coupon.Credit, 2);
                        AnsiConsole.MarkupLine(
                            $"[green]Coupon #{appliedEntry.Number} applied with [yellow]€{rounded}[/] credit.[/]");
                    }
                    else
                    {
                        AnsiConsole.MarkupLine("[green]A coupon is applied to your cart.[/]");
                    }
                }
                else
                {
                    AnsiConsole.MarkupLine("[yellow]No coupon selected.[/]");
                }

                AnsiConsole.MarkupLine("Press [green]ENTER[/] to continue");
                Console.ReadKey();
                return;
            }

            var selectedEntry = choices.FirstOrDefault(c => c.Label == selection);
            var selectedCoupon = selectedEntry.Coupon;
            if (selectedCoupon == null)
                continue;

            var actionPrompt = new SelectionPrompt<string>()
                .Title(selectedEntry.IsApplied
                    ? $"Coupon #{selectedEntry.Number} is currently applied"
                    : $"Apply coupon #{selectedEntry.Number}?")
                .AddChoices(selectedEntry.IsApplied
                    ? new[] { "Remove coupon", "Back" }
                    : new[] { "Apply coupon", "Back" });

            var action = AnsiConsole.Prompt(actionPrompt);

            if (action == "Back")
                continue;

            else if (action == "Apply coupon")
            {
                CouponLogic.ApplyCouponToCart(selectedCoupon);

                var rounded = Math.Round(selectedCoupon.Credit, 2);
                AnsiConsole.MarkupLine(
                    $"[green]Coupon #{selectedEntry.Number} applied to your cart with [yellow]€{rounded}[/] credit.[/]");
                AnsiConsole.MarkupLine("[grey]Press [green]ENTER[/] to continue.[/]");
                Console.ReadKey();
                continue;
            }

            else if (action == "Remove coupon")
            {
                CouponLogic.ResetCouponSelection();
                AnsiConsole.MarkupLine(
                    $"[yellow]Coupon #{selectedEntry.Number} removed from your cart.[/]");
                AnsiConsole.MarkupLine("[grey]Press [green]ENTER[/] to continue.[/]");
                Console.ReadKey();
            }
        }
    }
}