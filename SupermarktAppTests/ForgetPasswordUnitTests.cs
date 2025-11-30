using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace SupermarktAppTests
{
    [TestClass]
    public class ForgetPasswordUnitTests
    {
        [TestMethod]
        [DataRow("test@gmail.com", "password1")]
        public async Task ForgotPassword_ValidEmail_SendsResetCode(string email, string password)
        {
            // Arrange - Create new test user for database
            DatabaseFiller.RunDatabaseMethods();

            DateTime birthdate = new DateTime(1990, 1, 1);
            UserModel user = new UserModel("Test", "User", email, password,
                                           "Test Address", "1234AB", "1234567890",
                                           birthdate, "Test City");

            LoginAccess.Register(user);

            // Act
            var result = LoginLogic.GetUserByEmail(email);

            // Assert - user exists
            Assert.IsNotNull(result);

            // Call forget password method with test user
            string code = await TwoFALogic.ForgetPassword2FAEmail(result.ID, result.Email);

            // Assert code exists
            Assert.IsFalse(string.IsNullOrWhiteSpace(code));
            Assert.AreEqual(6, code.Length);

            // Assert code stored in Database
            string storedCode = UserAccess.Get2FACode(result.ID);
            Assert.AreEqual(code, storedCode);
        }
    }
}
