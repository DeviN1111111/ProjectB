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

        // --- Fetch and filter valid coupons ---
        var user = SessionManager.CurrentUser;
        var allCoupons = CouponLogic.GetAllCoupons(user!.ID);
        List<Coupon> coupons = new List<Coupon>();

        for (int i = 0; i < allCoupons.Count; i++)
        {
            var coupon = allCoupons[i];
            if (coupon.IsValid && coupon.Credit > 0)
                coupons.Add(coupon);
        }
        // coupons has now only valid coupons
            
        if (coupons.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]You have no valid coupons.[/]");
            AnsiConsole.MarkupLine("Press [green]ENTER[/] to continue");
            Console.ReadKey();
            return;
        }

        CheckCurrentCoupon(coupons);
        AddCoupon(coupons);
    }

    public static void CheckCurrentCoupon(List<Coupon> coupons)
    {
        // check if user has already applied a coupon for the current order
        var appliedCouponId = Order.SelectedCouponId; // # maybe change OrderModel to hold applied coupon id #

        // find the applied coupon in the coupons list
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
            // appliedCoupon is now either the applied coupon or null if not found

            if (appliedCoupon != null)
            {
                var credit = Math.Round(appliedCoupon.Credit, 2);
                AnsiConsole.MarkupLine(
                    $"[green]You already have a coupon applied with [yellow]€{credit} credit.[/]"); 
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
                Console.ReadKey();
            }
        }
        return;
    }

    public static void AddCoupon(List<Coupon> coupons)
    {
        var table = new Table()
            .BorderColor(MenuUI.AsciiPrimary)
            .AddColumn("Coupon")
            .AddColumn("Credit");

        List<string> labels = new List<string>();

        for (int i = 0; i < coupons.Count; i++)
        {
            int number = i + 1;
            double credit = Math.Round(coupons[i].Credit, 2);

            table.AddRow($"#{number}", $"€[green]{credit}[/]");

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
        double selectedRounded = Math.Round(selectedCoupon.Credit, 2);

        CouponLogic.ApplyCouponToCart(selectedCoupon);

        AnsiConsole.MarkupLine($"[green]Coupon applied with [yellow]€{selectedRounded} credit.[/]");
        AnsiConsole.MarkupLine("Press ENTER to continue");    
        Console.ReadKey();
        return;    
    }
}