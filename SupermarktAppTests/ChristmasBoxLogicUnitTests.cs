using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;


namespace SupermarktAppTests
{
    [TestClass]
    public class ChristmasBoxLogicUnitTests
    {
        [TestMethod]
        public void CreateBox_for2Persons_SetConfigurePrice25()
        {
            // create xmas box for 2 pp 
            // set final price at 25m not base price

            // arrange 
            DatabaseFiller.RunDatabaseMethods();

            var baseBoxProduct = new ProductModel
            {
                ID = 910001,
                Name = "Christmas Box (2 persons)",  // neemds the numer of the name
                Price = 15, // base price
                Category = "ChristmasBox",
                Visible = 1,
                Quantity = 999,
                NutritionDetails = "Varies",
                Description = "Base box product",
                Location = 0,
                IsChristmasBoxItem = false // base box if not a item that can be put in a box.
            };
            ProductAccess.AddProductUnitTest(baseBoxProduct);

            // add products that re allowed in the box
            ProductAccess.AddProductUnitTest(new ProductModel { ID = 910101, Name = "Eligible A", Price = 9, Category = "Fruit", Visible = 1, Quantity = 50, NutritionDetails="x", Description="x", Location=1, IsChristmasBoxItem = true });
            ProductAccess.AddProductUnitTest(new ProductModel { ID = 910102, Name = "Eligible B", Price = 8, Category = "Fruit", Visible = 1, Quantity = 50, NutritionDetails="x", Description="x", Location=1, IsChristmasBoxItem = true });
            ProductAccess.AddProductUnitTest(new ProductModel { ID = 910103, Name = "Eligible C", Price = 4, Category = "Fruit", Visible = 1, Quantity = 50, NutritionDetails="x", Description="x", Location=1, IsChristmasBoxItem = true });
            ProductAccess.AddProductUnitTest(new ProductModel { ID = 910104, Name = "Eligible D", Price = 4, Category = "Fruit", Visible = 1, Quantity = 50, NutritionDetails="x", Description="x", Location=1, IsChristmasBoxItem = true });


            // act 
            var box = ChristmasBoxLogic.CreateBox(baseBoxProduct);

            // assert
            Assert.IsNotNull(box);
            Assert.AreEqual(25.0, box.Price);
        }

        [TestMethod]
        public void CreateBox_OnlyContainsEligibleChristmasBoxItems()
        {
            // check if only eligible items are added to the box

            // arrange 
            DatabaseFiller.RunDatabaseMethods();
            
            var baseBoxProduct = new ProductModel
            {
                ID = 920001,
                Name = "Christmas Box (2 persons)",
                Price = 15,
                Category = "ChristmasBox",
                Visible = 1,
                Quantity = 999,
                NutritionDetails = "Varies",
                Description = "Base box product",
                Location = 0,
                IsChristmasBoxItem = false
            };
            ProductAccess.AddProductUnitTest(baseBoxProduct);

            // Add eligible items
            ProductAccess.AddProductUnitTest(new ProductModel { ID = 920101, Name = "Eligible A", Price = 9, Category = "Fruit", Visible = 1, Quantity = 50, NutritionDetails="x", Description="x", Location=1, IsChristmasBoxItem = true });
            ProductAccess.AddProductUnitTest(new ProductModel { ID = 920102, Name = "Eligible B", Price = 8, Category = "Fruit", Visible = 1, Quantity = 50, NutritionDetails="x", Description="x", Location=1, IsChristmasBoxItem = true });
            ProductAccess.AddProductUnitTest(new ProductModel { ID = 920103, Name = "Eligible C", Price = 4, Category = "Fruit", Visible = 1, Quantity = 50, NutritionDetails="x", Description="x", Location=1, IsChristmasBoxItem = true });
            ProductAccess.AddProductUnitTest(new ProductModel { ID = 920104, Name = "Eligible D", Price = 4, Category = "Fruit", Visible = 1, Quantity = 50, NutritionDetails="x", Description="x", Location=1, IsChristmasBoxItem = true });

            // act 
            var box = ChristmasBoxLogic.CreateBox(baseBoxProduct);

            // assert

            //all the items should be eligible not one is not test fails
            Assert.IsTrue(box.Products.All(p => p.IsChristmasBoxItem));
        }

        [TestMethod]
        public void CreateBox_FillBetween90And100Percent()
        {
            // total value of box should be at least 90% en niet meer dan 100% van de box prijs.

            // aarange
            DatabaseFiller.RunDatabaseMethods();

            var baseBoxProduct = new ProductModel
            {
                ID = 930001,
                Name = "Christmas Box (2 persons)",
                Price = 15,
                Category = "ChristmasBox",
                Visible = 1,
                Quantity = 999,
                NutritionDetails = "Varies",
                Description = "Base box product",
                Location = 0,
                IsChristmasBoxItem = false
            };
            ProductAccess.AddProductUnitTest(baseBoxProduct);

            ProductAccess.AddProductUnitTest(new ProductModel { ID = 930101, Name = "Eligible A", Price = 9, Category = "Fruit", Visible = 1, Quantity = 50, NutritionDetails="x", Description="x", Location=1, IsChristmasBoxItem = true });
            ProductAccess.AddProductUnitTest(new ProductModel { ID = 930102, Name = "Eligible B", Price = 8, Category = "Fruit", Visible = 1, Quantity = 50, NutritionDetails="x", Description="x", Location=1, IsChristmasBoxItem = true });
            ProductAccess.AddProductUnitTest(new ProductModel { ID = 930103, Name = "Eligible C", Price = 4, Category = "Fruit", Visible = 1, Quantity = 50, NutritionDetails="x", Description="x", Location=1, IsChristmasBoxItem = true });
            ProductAccess.AddProductUnitTest(new ProductModel { ID = 930104, Name = "Eligible D", Price = 4, Category = "Fruit", Visible = 1, Quantity = 50, NutritionDetails="x", Description="x", Location=1, IsChristmasBoxItem = true });

            // act 
            var box = ChristmasBoxLogic.CreateBox(baseBoxProduct);

            // assert
            
            // total price of all products inside the created box
            double total = box.Products.Sum(p => p.Price);

            // total price > 90%
            Assert.IsTrue(total >= box.Price * 0.90);
            // Assert.IsGreaterThanOrEqualTo ???

            // total not above box price
            Assert.IsTrue(total <= box.Price);
        }
    }
}
