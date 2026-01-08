using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace SupermarktAppTests
{
    [TestClass]
    public class ChristmasBoxLogicUnitTests
    {
        [TestMethod]
        public void CreateBox_for2Persons_SetConfigurePrice()
        {
            // arrange 
            DatabaseFiller.RunDatabaseMethods();

            var baseBoxProduct = new ProductModel
            {
                ID = 910001,
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

            ProductAccess.AddProductUnitTest(new ProductModel { ID = 910101, Name = "Eligible A", Price = 9, Category = "Fruit", Visible = 1, Quantity = 50, NutritionDetails="x", Description="x", Location=1, IsChristmasBoxItem = true });
            ProductAccess.AddProductUnitTest(new ProductModel { ID = 910102, Name = "Eligible B", Price = 8, Category = "Fruit", Visible = 1, Quantity = 50, NutritionDetails="x", Description="x", Location=1, IsChristmasBoxItem = true });
            ProductAccess.AddProductUnitTest(new ProductModel { ID = 910103, Name = "Eligible C", Price = 4, Category = "Fruit", Visible = 1, Quantity = 50, NutritionDetails="x", Description="x", Location=1, IsChristmasBoxItem = true });
            ProductAccess.AddProductUnitTest(new ProductModel { ID = 910104, Name = "Eligible D", Price = 4, Category = "Fruit", Visible = 1, Quantity = 50, NutritionDetails="x", Description="x", Location=1, IsChristmasBoxItem = true });


            // act 
            var box = ChristmasBoxLogic.CreateBox(baseBoxProduct);

            // assert
            Assert.IsNotNull(box);
            Assert.AreEqual(25.0, box.Price, 0.0001);
        }

        
    }
}
