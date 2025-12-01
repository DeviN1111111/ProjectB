using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace SupermarktAppTests
{
    [TestClass]
    public class TwoFALogicUnitTests
    {
        [TestInitialize]
        public void Init()
        {
            // ensure DB seeded for each test
            DatabaseFiller.RunDatabaseMethods();
        }

        [TestMethod]
        public void Generate2FACode_ReturnsSixDigitNumericString()
        {
            string code = TwoFALogic.Generate2FACode();
            Assert.IsNotNull(code);
            Assert.AreEqual(6, code.Length);
            Assert.IsTrue(int.TryParse(code, out _));
        }

        [TestMethod]
        public async Task CreateInsertAndEmailSend2FACode_InsertsCodeAndExpiry()
        {
            int userId = 1;
            int validityMinutes = 5;

            await TwoFALogic.CreateInsertAndEmailSend2FACode(userId, validityMinutes);

            string code = UserAccess.Get2FACode(userId);
            DateTime? expiry = UserAccess.Get2FAExpiry(userId);

            Assert.IsNotNull(code);
            Assert.IsNotNull(expiry);
            Assert.IsTrue(expiry > DateTime.Now);
        }

        [TestMethod]
        public async Task Register2FAEmail_ReturnsSixDigitCode()
        {
            string email = "devinnijhof@gmail.com";
            string code = await TwoFALogic.Register2FAEmail(email);

            Assert.IsNotNull(code);
            Assert.AreEqual(6, code.Length);
            Assert.IsTrue(int.TryParse(code, out _));
        }

        [TestMethod]
        public void Validate2FACode_ReturnsTrueForCorrectCode_FalseForIncorrect()
        {
            int userId = 1;
            string correctCode = TwoFALogic.Generate2FACode();
            DateTime expiry = DateTime.Now.AddMinutes(10);

            UserAccess.Insert2FACode(userId, correctCode, expiry);

            bool valid = TwoFALogic.Validate2FACode(userId, correctCode);
            bool invalid = TwoFALogic.Validate2FACode(userId, "000000");

            Assert.IsTrue(valid);
            Assert.IsFalse(invalid);
        }

        [TestMethod]
        public void EnableAndDisable2FA_TogglesStatus()
        {
            int userId = 1;

            TwoFALogic.Enable2FA(userId);
            Assert.IsTrue(TwoFALogic.Is2FAEnabled(userId));

            TwoFALogic.Disable2FA(userId);
            Assert.IsFalse(TwoFALogic.Is2FAEnabled(userId));
        }
    }
}