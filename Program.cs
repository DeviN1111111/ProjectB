using System;
using System.Collections.Generic;
using System.Linq;
using Spectre.Console;

class Program
{
    static void Main()
    {
        LoginAccess.CreateTable();
        Console.Clear();
        LoginUI.Register();
        // LoginUI.Login();
        
    //     ProductAccess.CreateTable();
        //     var random = new Random();

        //     var products = new List<ProductModel>
        //     {
        //         // Fruits
        //         new ProductModel { Name = "Apple", Price = 0.5, NutritionDetails = "52 kcal per 100g", Description = "Fresh red apple", Category = "Fruits", Quantity = random.Next(1, 100) },
        //         new ProductModel { Name = "Banana", Price = 0.3, NutritionDetails = "89 kcal per 100g", Description = "Yellow ripe banana", Category = "Fruits", Quantity = random.Next(1, 100) },
        //         new ProductModel { Name = "Orange", Price = 0.6, NutritionDetails = "47 kcal per 100g", Description = "Juicy orange", Category = "Fruits", Quantity = random.Next(1, 100) },

        //         // Vegetables
        //         new ProductModel { Name = "Carrot", Price = 0.2, NutritionDetails = "41 kcal per 100g", Description = "Organic carrot", Category = "Vegetables", Quantity = random.Next(1, 100) },
        //         new ProductModel { Name = "Broccoli", Price = 1.5, NutritionDetails = "55 kcal per 100g", Description = "Fresh broccoli", Category = "Vegetables", Quantity = random.Next(1, 100) },

        //         // Dairy
        //         new ProductModel { Name = "Milk", Price = 1.2, NutritionDetails = "42 kcal per 100ml", Description = "Full cream milk", Category = "Dairy", Quantity = random.Next(1, 100) },
        //         new ProductModel { Name = "Cheese", Price = 2.5, NutritionDetails = "402 kcal per 100g", Description = "Cheddar cheese", Category = "Dairy", Quantity = random.Next(1, 100) },

        //         // Bakery
        //         new ProductModel { Name = "Bread", Price = 1.0, NutritionDetails = "265 kcal per 100g", Description = "Whole wheat bread", Category = "Bakery", Quantity = random.Next(1, 100) },
        //         new ProductModel { Name = "Muffin", Price = 2.0, NutritionDetails = "377 kcal per 100g", Description = "Blueberry muffin", Category = "Bakery", Quantity = random.Next(1, 100) },

        //         // Meat
        //         new ProductModel { Name = "Chicken Breast", Price = 5.0, NutritionDetails = "165 kcal per 100g", Description = "Boneless chicken breast", Category = "Meat", Quantity = random.Next(1, 100) },
        //         new ProductModel { Name = "Beef Steak", Price = 7.0, NutritionDetails = "250 kcal per 100g", Description = "Fresh beef steak", Category = "Meat", Quantity = random.Next(1, 100) },

        //         // Beverages
        //         new ProductModel { Name = "Orange Juice", Price = 2.0, NutritionDetails = "45 kcal per 100ml", Description = "Fresh orange juice", Category = "Beverages", Quantity = random.Next(1, 100) },
        //         new ProductModel { Name = "Coffee", Price = 3.0, NutritionDetails = "2 kcal per 100ml", Description = "Ground coffee", Category = "Beverages", Quantity = random.Next(1, 100) }
        //     };

        //     ProductAccess.InsertProducts(products);

        //     var allProducts = ProductAccess.GetAllProducts().ToList();


        //     var chart = new BarChart()
        //         .Width(60)
        //         .Label("[green bold underline]Number of quantity per product[/]")
        //         .CenterLabel();


        //     var colors = new[] { Color.Yellow, Color.Green, Color.Blue, Color.Red, Color.Purple, Color.Orange1, Color.Aqua, Color.Teal, Color.Fuchsia, Color.Gold1 };
        //     int colorIndex = 0;

        //     foreach (var product in allProducts)
        //     {
        //         var color = colors[colorIndex % colors.Length];
        //         colorIndex++;
        //         chart.AddItem($"{product.Id} - {product.Name}", product.Quantity, color);
        //     }

        //     AnsiConsole.Write(chart);

        //     Console.WriteLine("\nSelect a product to view the location on the map (enter Id):");
        //     string userInput = Console.ReadLine();

        //     if (int.TryParse(userInput, out int productId))
        //     {
        //         var selected = allProducts.FirstOrDefault(p => p.Id == productId);
        //         if (selected != null)
        //         {
        //             Console.WriteLine($"You selected {selected.Name}. Opening map...");
        //             MapUI.DisplayMap(selected.Category);
        //         }
        //         else
        //         {
        //             Console.WriteLine("Product Id not found.");
        //         }
        //     }
        //     else
        //     {
        //         Console.WriteLine("Invalid input. Please enter a valid product Id.");
        //     }
    }
}
