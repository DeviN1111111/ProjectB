using Microsoft.Data.Sqlite;
using Dapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using System;


namespace SupermarktAppTests
{
    [TestClass]
    public class TwoFALogicTests
    {
        [TestMethod]
        public void Generate2FACode_ShouldReturnSixDigitString()
        {
            //Act
            string code = TwoFALogic.Generate2FACode();

            //Assert
            Assert.AreEqual(6, code.Length);
            Assert.IsTrue(int.TryParse(code, out _));
        }

        [TestMethod]
        [DataRow(1, 10)]
        [DataRow(2, 5)]
        public async Task CreateInsertAndEmailSend2FACode_ShouldInsertCodeAndSendEmail(int userId, int validityMinutes)
        {
            //Arrange
            DatabaseFiller.RunDatabaseMethods();

            //Act
            await TwoFALogic.CreateInsertAndEmailSend2FACode(userId, validityMinutes);

            //Assert
            string code = UserAccess.Get2FACode(userId);
            DateTime? expiry = UserAccess.Get2FAExpiry(userId);

            Assert.IsNotNull(code);
            Assert.IsNotNull(expiry);
            Assert.IsTrue(expiry > DateTime.Now);
        }

        [TestMethod]
        [DataRow("test@example.com")]
        [DataRow("user@gmail.com")]
        public async Task Register2FAEmail_ShouldSendEmailAndReturnCode(string email)
        {
            //Act
            string code = await TwoFALogic.Register2FAEmail(email);

            //Assert
            Assert.IsNotNull(code);
            Assert.AreEqual(6, code.Length);
            Assert.IsTrue(int.TryParse(code, out _));
        }

        [TestMethod]
        [DataRow(1, true)]
        [DataRow(2, false)]
        public void Validate2FACode_ShouldReturnExpectedResult(int userId, bool expected)
        {
            //Arrange
            DatabaseFiller.RunDatabaseMethods();
            string code = TwoFALogic.Generate2FACode();
            DateTime expiry = DateTime.Now.AddMinutes(10);

            UserAccess.Insert2FACode(userId, expected ? code : "999999", expiry);

            //Act
            bool result = TwoFALogic.Validate2FACode(userId, code);

            //Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        [DataRow(1)]
        public void Enable2FA_ShouldSet2FAEnabled(int userId)
        {
            //Arrange
            DatabaseFiller.RunDatabaseMethods();

            //Act
            TwoFALogic.Enable2FA(userId);

            //Assert
            Assert.IsTrue(TwoFALogic.Is2FAEnabled(userId));
        }

        [TestMethod]
        [DataRow(1)]
        public void Disable2FA_ShouldSet2FADisabled(int userId)
        {
            //Arrange
            DatabaseFiller.RunDatabaseMethods();
            TwoFALogic.Enable2FA(userId);

            //Act
            TwoFALogic.Disable2FA(userId);

            //Assert
            Assert.IsFalse(TwoFALogic.Is2FAEnabled(userId));
        }
    }
}
