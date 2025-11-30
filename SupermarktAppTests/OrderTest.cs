using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Data.Sqlite;
using Dapper;
using System.Collections.Generic;

namespace SupermarktAppTests
{
    [TestClass]
    public class OrderLogicTests
    {

        [TestMethod]
        
        public void AddToCart_NewProduct_AddsToCart(){}

        [TestMethod]
        public void AddToCart_ExistingProduct_IncreasesQuantityUpToLimit(){}
    }
}
