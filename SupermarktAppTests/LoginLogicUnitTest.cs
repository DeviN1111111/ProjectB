// using Microsoft.Data.Sqlite;
// using Dapper;
// using Microsoft.VisualStudio.TestTools.UnitTesting;
// using System;
// using System.Collections.Generic;

// namespace SupermarktAppTests
// {
//     [TestClass]
//     public class LoginLogicTests
//     {
//         [TestMethod]
//         [DataRow("test@gmail.com", "password1")]
//         [DataRow("Cheng@gmail.com", "abc123")]
//         public void ValidateLogin_CorrectLogin_ReturnsUserModel(string email, string password)
//         {
//             //Arrange
//             DatabaseFiller.RunDatabaseMethods();
//             DateTime birthdate = new DateTime(1990, 1, 1);
//             UserModel user = new UserModel("Test", "User", email, password, "Test Address", "1234AB", "1234567890", birthdate, "Test City");
//             LoginAccess.Register(user);

//             //Act
//             UserModel actual = LoginLogic.Login(email, password);

//             //Assert
//             Assert.IsNotNull(actual);
//             Assert.AreEqual(user.Email, actual.Email);
//             Assert.AreEqual(user.Password, actual.Password);
//             Assert.AreEqual(user.Name, actual.Name);
//         }

//         [TestMethod]
//         [DataRow("test@gmail.com", "password123")]
//         [DataRow("Cheng@gmail.com", "abc12345")]
//         [DataRow("Bestaatniet@gmail.com", "bestaatniet")]
//         public void ValidateLogin_InCorrectLogin_ReturnsNull(string email, string password)
//         {
//             //Arrange
//             DatabaseFiller.RunDatabaseMethods();

//             //Act
//             UserModel actual = LoginLogic.Login(email, password);

//             //Assert
//             Assert.IsNull(actual);
//         }

//         [TestMethod]
//         [DataRow("FirstNameTest", "LastNameTest", "test@gmail.com", "123456", "Test Address", "1234AB", "1234567890", "1990-01-01", "Test City", false, "User")]
//         [DataRow("FirstNameTest2", "LastNameTest", "test2@gmail.com", "123456", "Test Address", "1234AB", "1234567890", "1990-01-01", "Test City", false, "User")]
//         [DataRow("FirstNameTest3", "LastNameTest", "test3@gmail.com", "123456", "Test Address", "1234AB", "1234567890", "1990-01-01", "Test City", false, "User")]
//         public void ValidateRegister_CorrectRegister_ReturnsListErrors(string name, string lastName, string email, string password, string address, string zipcode, string phoneNumber, string birthdateString, string city, bool is2FAEnabled, string AccountStatus)
//         {
//             //Arrange
//             DatabaseFiller.RunDatabaseMethods();
//             DateTime birthdate = DateTime.Parse(birthdateString);

//             //Act
//             List<string> actual = LoginLogic.Register(name, lastName, email, password, address, zipcode, phoneNumber, birthdate, city, is2FAEnabled, AccountStatus);

//             //Assert
//             Assert.IsNotNull(actual);
//             Assert.AreEqual(0, actual.Count);
//         }
//     }
// }