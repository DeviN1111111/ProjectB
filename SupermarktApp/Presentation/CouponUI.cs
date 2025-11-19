using System;
using System.Linq;
using Spectre.Console;
public static class CouponUI
{
    public static Coupon? DisplayMenu(Action<Coupon>? onCouponSelected = null)
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
                Console.ReadKey(true);
                return null;
            }

            var coupons = CouponLogic.GetAllCoupons(user.ID)
                .Where(c => c.IsValid && c.Credit > 0)
                .ToList();

            if (coupons.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]You have no valid coupons.[/]");
                return null;
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
                    return (Coupon: entry.Coupon, Label: label, Number: entry.Number, IsApplied: entry.IsApplied);
                })
                .ToList();

            var selection = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Select a coupon to manage")
                    .PageSize(10)
                    .AddChoices(choices.Select(c => c.Label).Concat(new[] { "Go back" })));

            if (selection == "Go back")
                return null;

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

            if (action == "Apply coupon")
            {
                onCouponSelected?.Invoke(selectedCoupon);
                AnsiConsole.MarkupLine($"[green]Coupon #{selectedEntry.Number} applied to your cart.[/]");
                return selectedCoupon;
            }

            CouponLogic.ResetCouponSelection();
            AnsiConsole.MarkupLine($"[yellow]Coupon #{selectedEntry.Number} removed from your cart.[/]");
            return null;
        }
    }
}
