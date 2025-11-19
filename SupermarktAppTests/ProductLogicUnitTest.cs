using Microsoft.Data.Sqlite;
using Dapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics.CodeAnalysis;

namespace SupermarktAppTests
{
    [TestClass]
    public class ProductLogicTests
    {
        [TestMethod]
        [DataRow(10000, "TestProduct")]
        [DataRow(100000, "TestProduct")]
        public void ValidateSearchProductByName_CorrectName_ReturnsProductModel(int ProductID, string Name)
        {
            //Arrange
            DatabaseFiller.RunDatabaseMethods();
            string productName = "TestProduct";
            ProductModel product = new ProductModel(ProductID, productName, 10.0, "TestNutrition", "TestDescription", "TestCategory", 1, 0, 1);
            ProductAccess.AddProductUnitTest(product);

            //Act
            ProductModel actual = ProductAccess.GetProductByName(productName);

            //Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(Name, actual.Name);
            Assert.AreEqual(ProductID, actual.ID);
        }

        [TestMethod]
        [DataRow(10000, "TestProduct")]
        [DataRow(100000, "TestProduct")]
        public void ValidateSearchProductByID_CorrectID_ReturnsProductModel(int ProductID, string Name)
        {
            //Arrange
            DatabaseFiller.RunDatabaseMethods();
            string productName = "TestProduct";
            ProductModel product = new ProductModel(ProductID, productName, 10.0, "TestNutrition", "TestDescription", "TestCategory", 1, 0, 1);
            ProductAccess.AddProductUnitTest(product);

            //Act
            ProductModel actual = ProductAccess.GetProductByID(ProductID);

            //Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(Name, actual.Name);
            Assert.AreEqual(ProductID, actual.ID);
        }

        [TestMethod]
        [DataRow(10000, "TestProduct", true)]
        [DataRow(100000, "TestProduct", true)]
        public void ValidateAddProduct_CorrectAddProduct_ReturnsTrue(int ProductID, string Name, bool Expected)
        {
            //Arrange
            DatabaseFiller.RunDatabaseMethods();
            string productName = "TestProduct";

            //Act
            bool expected = ProductLogic.AddProduct(productName, 10.0, "TestNutrition", "TestDescription", "TestCategory", 1, 0, 1);

            //Assert
            Assert.IsTrue(expected);
            Assert.AreEqual(Expected, expected);
        }

        [TestMethod]
        [DataRow(10000)]
        [DataRow(100000)]
        public void ValidateDeleteProductByID_CorrectDeleteProductByID_ReturnsVoid(int ProductID)
        {
            //Arrange
            DatabaseFiller.RunDatabaseMethods();
            string productName = "TestProduct";
            ProductModel product = new ProductModel(ProductID, productName, 10.0, "TestNutrition", "TestDescription", "TestCategory", 1, 0, 1);
            ProductAccess.AddProductUnitTest(product);

            //Act
            ProductLogic.DeleteProductByID(ProductID); //Set visible word 0 het product wordt niet echt verwijdert!
            ProductModel expected = ProductAccess.GetProductByID(ProductID);

            //Assert
            Assert.IsNotNull(expected);
            Assert.AreEqual(0, expected.Visible);
        }


        [TestMethod]
        [DataRow(10000)]
        [DataRow(100000)]
        public void ValidateChangeProductDetails_CorrectDetails_ReturnsVoid(int ProductID)
        {
            //Arrange
            DatabaseFiller.RunDatabaseMethods();
            string productName = "TestProduct";
            ProductModel product = new ProductModel(ProductID, productName, 10.0, "TestNutrition", "TestDescription", "TestCategory", 1, 0, 1);
            ProductAccess.AddProductUnitTest(product);

            //Act
            ProductLogic.ChangeProductDetails(ProductID, "ChangedProduct", 20.0, "ChangedNutrition", "ChangedDescription", "ChangedCategory", 2, 5, 0);
            ProductModel expected = ProductAccess.GetProductByID(ProductID);

            //Assert
            Assert.IsNotNull(expected);
            Assert.AreEqual("ChangedProduct", expected.Name);
            Assert.AreEqual(20.0, expected.Price);
            Assert.AreEqual("ChangedNutrition", expected.NutritionDetails);
            Assert.AreEqual("ChangedDescription", expected.Description);
            Assert.AreEqual("ChangedCategory", expected.Category);
            Assert.AreEqual(2, expected.Location);
            Assert.AreEqual(5, expected.Quantity);
            Assert.AreEqual(0, expected.Visible);
        }
        //ChangeProductDetails(int id, string name, double price, string nutritionDetails, string description, string category, int location, int quantity, int visible)
    
        //-------------------------------------------------------------------------------------------------------------------------------------------- Nu start incorrect tests

        [TestMethod]
        [DataRow(10000, "TestProduct", false)]
        [DataRow(100000, "TestProduct", false)]
        public void ValidateAddProduct_IncorrectAddProduct_ReturnsFalse(int ProductID, string Name, bool Expected)
        {
            //Arrange
            DatabaseFiller.RunDatabaseMethods();
            string productName = "TestProduct";
            ProductModel product = new ProductModel(ProductID, productName, 10.0, "TestNutrition", "TestDescription", "TestCategory", 1, 0, 1);
            ProductAccess.AddProductUnitTest(product); // Ik voeg hier een product toe zodat de addproduct false returned

            //Act
            bool expected = ProductLogic.AddProduct(productName, 10.0, "TestNutrition", "TestDescription", "TestCategory", 1, 0, 1); // Hier bestaat het product al dus zou false moeten returnen

            //Assert
            Assert.IsFalse(expected);
            Assert.AreEqual(Expected, expected);
        }

        [TestMethod]
        [DataRow(10000, "TestProduct")]
        [DataRow(100000, "TestProduct")]
        public void ValidateSearchProductByName_IncorrectName_ReturnsNull(int ProductID, string Name)
        {
            //Arrange
            DatabaseFiller.RunDatabaseMethods();
            string productName = "TestProduct";
            ProductModel product = new ProductModel(ProductID, productName, 10.0, "TestNutrition", "TestDescription", "TestCategory", 1, 0, 1);
            ProductAccess.AddProductUnitTest(product);

            //Act
            ProductModel actual = ProductAccess.GetProductByName("WrongName");

            //Assert
            Assert.IsNull(actual);
        }


        [TestMethod]
        [DataRow(10000, "TestProduct")]
        [DataRow(100000, "TestProduct")]
        public void ValidateSearchProductByID_IncorrectID_ReturnsNull(int ProductID, string Name)
        {
            //Arrange
            DatabaseFiller.RunDatabaseMethods();
            string productName = "TestProduct";
            ProductModel product = new ProductModel(ProductID, productName, 10.0, "TestNutrition", "TestDescription", "TestCategory", 1, 0, 1);
            ProductAccess.AddProductUnitTest(product);

            //Act
            ProductModel actual = ProductAccess.GetProductByID(31238248);

            //Assert
            Assert.IsNull(actual);
        }
    }
}