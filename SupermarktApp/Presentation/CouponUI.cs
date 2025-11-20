using Spectre.Console;

public static class CouponUI
{
    public static void DisplayMenu()
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
            return;
        }

        var allCoupons = CouponLogic.GetAllCoupons(user.ID);
        List<Coupon> coupons = new List<Coupon>();

        for (int i = 0; i < allCoupons.Count; i++)
        {
            var c = allCoupons[i];
            if (c.IsValid && c.Credit > 0)
                coupons.Add(c);
        }

        if (coupons.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]You have no valid coupons.[/]");
            AnsiConsole.MarkupLine("Press [green]ENTER[/] to continue");
            return;
        }

        var appliedCouponId = Order.SelectedCouponId;

        if (appliedCouponId.HasValue)
        {
            Coupon appliedCoupon = null;

            for (int i = 0; i < coupons.Count; i++)
            {
                if (coupons[i].Id == appliedCouponId.Value)
                {
                    appliedCoupon = coupons[i];
                    break;
                }
            }

            if (appliedCoupon != null)
            {
                var credit = Math.Round(appliedCoupon.Credit, 2);
                AnsiConsole.MarkupLine(
                    "[green]You already have a coupon applied with [yellow]€" +
                    credit + "[/] credit.[/]");
            }
            else
            {
                AnsiConsole.MarkupLine("[green]You already have a coupon applied to your cart.[/]");
            }

            var action = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("What do you want to do?")
                    .AddChoices("Remove coupon", "Back"));

            if (action == "Remove coupon")
            {
                CouponLogic.ResetCouponSelection();
                AnsiConsole.MarkupLine("[yellow]Coupon removed[/]");
                AnsiConsole.MarkupLine("Press ENTER to continue");
            }

            return;
        }

        var table = new Table()
            .BorderColor(MenuUI.AsciiPrimary)
            .AddColumn("Coupon")
            .AddColumn("Credit");

        List<string> labels = new List<string>();

        for (int i = 0; i < coupons.Count; i++)
        {
            int number = i + 1;
            double credit = Math.Round(coupons[i].Credit, 2);

            table.AddRow($"#{number} €[green]{credit}[/]");

            string label = $"Coupon #{number} - €[green]{credit}[/]";
            labels.Add(label);
        }

        labels.Add("Go back");

        AnsiConsole.Write(table);

        var selectedLabel = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Select a coupon to apply")
                .AddChoices(labels));

        if (selectedLabel == "Go back")
            return;

        int selectedIndex = -1;

        for (int i = 0; i < labels.Count; i++)
        {
            if (labels[i] == selectedLabel)
            {
                selectedIndex = i;
                break;
            }
        }

        Coupon selectedCoupon = coupons[selectedIndex];
        double selectedRounded = Math.Round(selectedCoupon.Credit, 2);

        CouponLogic.ApplyCouponToCart(selectedCoupon);

        AnsiConsole.MarkupLine("[green]Coupon applied with [yellow]€" + selectedRounded + "[/] credit.[/]");
        AnsiConsole.MarkupLine("Press ENTER to continue");
    }
}