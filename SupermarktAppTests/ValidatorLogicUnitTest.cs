using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace SupermarktAppTests
{
    [TestClass]
    public class ValidaterLogicTests
    {
        [TestMethod]
        [DataRow("test@gmail.com")]
        [DataRow("Cheng@gmail.com")]
        public void ValidateEmail_Valid_IsTrue(string email)
        {
            //Act
            bool actual = ValidaterLogic.ValidateEmail(email);

            //Assert
            Assert.IsTrue(actual);
        }

        [TestMethod]
        [DataRow("NIETGELDIG")]        //Geen @ en geen .
        [DataRow("OOKNIETGELDIG")]     //Geen @ en geen .
        [DataRow("test@gmailcom")]     //Geen . 
        [DataRow("testgmail.com")]     //Geen @
        public void ValidateEmail_InValid_IsFalse(string email)
        {
            //Act
            bool actual = ValidaterLogic.ValidateEmail(email);

            //Assert
            Assert.IsFalse(actual);
        }
        //-------------------------------------------------------------------
        [TestMethod]
        [DataRow("123456")]
        [DataRow("abc123")]
        [DataRow("password1")]
        public void ValidatePassword_Valid_IsTrue(string password)
        {
            //Act
            bool actual = ValidaterLogic.ValidatePassword(password);

            //Assert
            Assert.IsTrue(actual);
        }

        [TestMethod]
        [DataRow("abcde")]//Te kort
        [DataRow("abcdef")]//Geen cijfer
        public void ValidatePassword_Invalid_IsFalse(string password)
        {
            //Act
            bool actual = ValidaterLogic.ValidatePassword(password);

            //Assert
            Assert.IsFalse(actual);
        }
        //-------------------------------------------------------------------
        [TestMethod]
        [DataRow("1234567890")]
        [DataRow("0987654321")]
        [DataRow("0616123456")]
        public void ValidatePhoneNumber_Valid_IsTrue(string phoneNumber)
        {
            //Act
            bool actual = ValidaterLogic.ValidatePhoneNumber(phoneNumber);

            //Assert
            Assert.IsTrue(actual);
        }

        [TestMethod]
        [DataRow("12345678")]      // 8 cijfers – false    
        [DataRow("123456789")]     // 9 cijfers – false
        [DataRow("01234567891")]   // 11 cijfers – false
        [DataRow("012345678912")]  // 12 cijfers – false
        public void ValidatePhoneNumber_Invalid_IsFalse(string phoneNumber)
        {
            //Act
            bool actual = ValidaterLogic.ValidatePhoneNumber(phoneNumber);

            //Assert
            Assert.IsFalse(actual);
        }
        //-------------------------------------------------------------------
        [TestMethod]
        [DataRow("2222AA")]
        [DataRow("2222  AA")]
        [DataRow("   2222  AA")]
        public void ValidateZipcode_Valid_IsTrue(string zipcode)
        {
            //Act
            bool actual = ValidaterLogic.ValidateZipcode(zipcode);

            //Assert
            Assert.IsTrue(actual);
        }
        [TestMethod]
        [DataRow("123AB")]         // te kort
        [DataRow("12345AB")]       // te lang
        [DataRow("12AB34")]        // verkeerde volgorde
        [DataRow("123456")]        // alleen cijfers
        [DataRow("ABCDE1")]        // geen juiste structuur
        public void ValidateZipcode_Invalid_IsFalse(string zipcode)
        {
            //Act
            bool actual = ValidaterLogic.ValidateZipcode(zipcode);

            //Assert
            Assert.IsFalse(actual);
        }
        //-------------------------------------------------------------------
        [TestMethod]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(43)]
        [DataRow(42)]
        public void ValidateLocationProduct_Valid_IsTrue(int location)
        {
            //Act
            bool actual = ValidaterLogic.ValidateLocationProduct(location);

            //Assert
            Assert.IsTrue(actual);
        }

        [TestMethod]
        [DataRow(0)]    // te laag
        [DataRow(-1)]   // te laag
        [DataRow(44)]   // te hoog
        [DataRow(45)]   // te hoog
        public void ValidateLocationProduct_Invalid_IsFalse(int location)
        {
            //Act
            bool actual = ValidaterLogic.ValidateLocationProduct(location);

            //Assert
            Assert.IsFalse(actual);
        }
        //-------------------------------------------------------------------
        [TestMethod]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(10)]
        public void ValidateQuantityProduct_Valid_IsTrue(int quantity)
        {
            //Act
            bool actual = ValidaterLogic.ValidateQuantityProduct(quantity);

            //Assert
            Assert.IsTrue(actual);
        }
        [TestMethod]
        [DataRow(-1)]        // negatief – ongeldig
        [DataRow(-2)]        // negatief – ongeldig
        [DataRow(-100)]      // negatief – ongeldig
        public void ValidateQuantityProduct_Invalid_IsFalse(int quantity)
        {
            //Act
            bool actual = ValidaterLogic.ValidateQuantityProduct(quantity);

            //Assert
            Assert.IsFalse(actual);
        }
    }
}
