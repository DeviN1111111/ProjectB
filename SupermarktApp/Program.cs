using Spectre.Console;

class Program
{
    static void Main()
    {
        DatabaseFiller.RunDatabaseMethods(200);
        // StartScreen.ShowMainMenu();
        // StatisticsUI.DisplayMenu();
        MenuUI.ShowMainMenu();

    }
}

