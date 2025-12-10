using Spectre.Console;

public static class CouponUI
{
    public static void DisplayMenu()
    {
        Console.Clear();
        Utils.PrintTitle("Coupons");

        // --- Fetch and filter valid coupons ---
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
        // coupons has now only valid coupons

        if (coupons.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]You have no valid coupons.[/]");
            AnsiConsole.MarkupLine("Press [green]ENTER[/] to continue");
            Console.ReadKey();
            return;
        }

        if (HandleAlreadyAppliedCoupon(coupons))
            return;

        ShowCouponSelection(coupons);
    }

    private static bool HandleAlreadyAppliedCoupon(List<Coupon> coupons)
    {
        // check if user has already applied a coupon for the current order
        var appliedCouponId = Order.SelectedCouponId; // # maybe change OrderModel to hold applied coupon id #

        if (!appliedCouponId.HasValue)
            return false;

        // find the applied coupon in the coupons list
        Coupon appliedCoupon = null
        ;

        for (int i = 0; i < coupons.Count; i++)
        {
            if (coupons[i].Id == appliedCouponId.Value)
            {
                appliedCoupon = coupons[i];
                break;
            }
        }
        // appliedCoupon is now either the applied coupon or null if not found

        if (appliedCoupon != null)
        {
            var credit = Utils.ChangePriceFormat(appliedCoupon.Credit);
            AnsiConsole.MarkupLine(
                "[green]You already have a coupon applied with [yellow]" +
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

        return true;
    }

    private static void ShowCouponSelection(List<Coupon> coupons)
    {
        var table = new Table()
            .BorderColor(MenuUI.AsciiPrimary)
            .AddColumn("Coupon")
            .AddColumn("Credit");

        List<string> labels = new List<string>();

        for (int i = 0; i < coupons.Count; i++)
        {
            int number = i + 1;
            string credit = Utils.ChangePriceFormat(coupons[i].Credit, "green");

            table.AddRow($"#{number}", $"{credit}");

            string label = $"Coupon #{number} - {credit}";
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

        // find selected coupon index
        int selectedIndex = -1;

        for (int i = 0; i < labels.Count; i++)
        {
            if (labels[i] == selectedLabel)
            {
                selectedIndex = i;
                break;
            }
        }

        // selected coupon is now the one at selectedIndex
        Coupon selectedCoupon = coupons[selectedIndex];
        string selectedRounded = Utils.ChangePriceFormat(selectedCoupon.Credit, "yellow");

        CouponLogic.ApplyCouponToCart(selectedCoupon);

        AnsiConsole.MarkupLine("[green]Coupon applied with " + selectedRounded + " credit.[/]");
        AnsiConsole.MarkupLine("Press ENTER to continue");
    }
}
