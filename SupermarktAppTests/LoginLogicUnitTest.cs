using Microsoft.Data.Sqlite;
using Dapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SupermarktAppTests
{
    [TestClass]
    public class LoginLogicTests
    {
        [TestMethod]
        [DataRow("test@gmail.com", "password1", true)]
        [DataRow("Cheng@gmail.com", "abc123", true)]
        public void ValidateLogin_CorrectLogin_ReturnsUserModel(string email, string password, bool expected)
        {
            //Arrange
            DatabaseFiller.RunDatabaseMethods();
            UserModel user = new UserModel("Test", "User", email, password, "Test Address", "1234AB", "1234567890", "Test City");
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
        [DataRow("test@gmail.com", "password123", false)]
        [DataRow("Cheng@gmail.com", "abc12345", false)]
        public void ValidateLogin_InCorrectLogin_ReturnsNull(string email, string password, bool expected)
        {
            //Act
            UserModel actual = LoginLogic.Login(email, password);

            //Assert
            Assert.IsNull(actual);
        }
    }
}