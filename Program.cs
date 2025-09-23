public class Program
{
    public static void Main()
    {
        Console.WriteLine("Please enter the section you want to locate (fruits vegetables dairy bakery):");
        string userInput = Console.ReadLine();
        System.Console.WriteLine($"Loading map for {userInput}");
        MapUI.DisplayMap(userInput);
        System.Console.WriteLine("The section of your preference is highlighted in green.");
    }
}