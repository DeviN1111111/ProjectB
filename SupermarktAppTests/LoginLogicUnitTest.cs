using Microsoft.Data.Sqlite;
using Dapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace SupermarktAppTests
{
    [TestClass]
    public class LoginLogicTests
    {
        [TestMethod]
        [DataRow("test@gmail.com", "password1")]
        [DataRow("Cheng@gmail.com", "abc123")]
        public void ValidateLogin_CorrectLogin_ReturnsUserModel(string email, string password)
        {
            //Arrange
            DatabaseFiller.RunDatabaseMethods();
            DateTime birthdate = new DateTime(1990, 1, 1);
            UserModel user = new UserModel("Test", "User", email, password, "Test Address", "1234AB", "1234567890", birthdate, "Test City");
            LoginAccess.Register(user);

            //Act
            UserModel actual = LoginLogic.Login(email, password);

            //Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(user.Email, actual.Email);
            Assert.AreEqual(user.Password, actual.Password);
            Assert.AreEqual(user.Name, actual.Name);
        }

        [TestMethod]
        [DataRow("test@gmail.com", "password123")]
        [DataRow("Cheng@gmail.com", "abc12345")]
        [DataRow("Bestaatniet@gmail.com", "bestaatniet")]
        public void ValidateLogin_InCorrectLogin_ReturnsNull(string email, string password)
        {
            //Arrange
            DatabaseFiller.RunDatabaseMethods();

            //Act
            UserModel actual = LoginLogic.Login(email, password);

            //Assert
            Assert.IsNull(actual);
        }
    }
}