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
                AnsiConsole.MarkupLine("Press [green]ENTER[/] to go back");
                Console.ReadKey(true);
                return null;
            }

            var couponTable = new Table()
                .BorderColor(MenuUI.AsciiPrimary)
                .AddColumn("[white]Coupon[/]")
                .AddColumn("[white]Credit[/]");

            foreach (var coupon in coupons)
            {
                couponTable.AddRow($"#{coupon.Id}", $"[green]{Math.Round(coupon.Credit, 2)}[/]");
            }

            AnsiConsole.Write(couponTable);

            var appliedCouponId = Order.SelectedCouponId;
            var choices = coupons
                .Select(c =>
                {
                    var isApplied = appliedCouponId.HasValue && appliedCouponId.Value == c.Id;
                    var label = $"Coupon #{c.Id} - [green]{Math.Round(c.Credit, 2)}[/]";
                    if (isApplied)
                    {
                        label += " [bold][applied][/]";
                    }
                    return (Coupon: c, Label: label, IsApplied: isApplied);
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
                    ? $"Coupon #{selectedCoupon.Id} is currently applied"
                    : $"Apply coupon #{selectedCoupon.Id}?")
                .AddChoices(selectedEntry.IsApplied
                    ? new[] { "Remove coupon", "Back" }
                    : new[] { "Apply coupon", "Back" });

            var action = AnsiConsole.Prompt(actionPrompt);
            if (action == "Back")
                continue;

            if (action == "Apply coupon")
            {
                onCouponSelected?.Invoke(selectedCoupon);
                AnsiConsole.MarkupLine($"[green]Coupon #{selectedCoupon.Id} applied to your cart.[/]");
                return selectedCoupon;
            }

            CouponLogic.ResetCouponSelection();
            AnsiConsole.MarkupLine($"[yellow]Coupon #{selectedCoupon.Id} removed from your cart.[/]");
            return null;
        }
    }
}
