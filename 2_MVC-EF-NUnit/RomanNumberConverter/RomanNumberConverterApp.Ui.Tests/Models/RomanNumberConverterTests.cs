using RomanNumberConverterApp.Ui.Models;

namespace RomanNumberConverterApp.Ui.Tests.Models
{
    [TestFixture]
    public class RomanNumberConverterTests
    {
        [TestCase(-100)]
        [TestCase(0)]
        [TestCase(4000)]
        [TestCase(10000)]
        public void Convert_ValueIsNotBetweenOneAnd3999_ShouldThrowArgumentException(int value)
        {
            // ARANGE
            IRomanNumberConverter converter = new RomanNumberConverter();

            // ACT & ASSERT
            Assert.That(()=> converter.Convert(value), Throws.ArgumentException.And.Message.EqualTo("Out of Roman range (1-3999)"));
        }


        [TestCase(1, "I")]
        [TestCase(5, "V")]
        [TestCase(9, "IX")]
        [TestCase(50, "L")]
        [TestCase(699, "DCXCIX")]
        [TestCase(1499, "MCDXCIX")]
        public void Convert_ValidValue_ShouldReturnRomanNumberEquivalent(int number, string romanNumber)
        {
            //ARRANGE
            //IDictionary<Int32, String> decimalLatinDictionary = new Dictionary<Int32, String>();
            //decimalLatinDictionary.Add(1, "I");
            //decimalLatinDictionary.Add(2, "II");
            //decimalLatinDictionary.Add(3, "III");
            //decimalLatinDictionary.Add(4, "IV");
            //decimalLatinDictionary.Add(5, "V");
            //decimalLatinDictionary.Add(6, "VI");
            //decimalLatinDictionary.Add(7, "VII");
            //decimalLatinDictionary.Add(8, "VIII");
            //decimalLatinDictionary.Add(9, "IX");
            //decimalLatinDictionary.Add(10, "X");
            //decimalLatinDictionary.Add(13, "XIII");
            //decimalLatinDictionary.Add(14, "XIV");
            //decimalLatinDictionary.Add(15, "XV");
            //decimalLatinDictionary.Add(19, "XIX");
            //decimalLatinDictionary.Add(29, "XXIX");
            //decimalLatinDictionary.Add(35, "XXXV");


            IRomanNumberConverter converter = new RomanNumberConverter();

            // ACT & ASSERT
            Assert.That(romanNumber, Is.EqualTo(converter.Convert(number)));
            
            
            //foreach (var keyValue in decimalLatinDictionary)
            //{
            //    Assert.That(converter.Convert(keyValue.Key) == keyValue.Value);
            //}
            
            //foreach (int number in goodNumber)
            //for (int i = 1; i < 3999; i++)
            //    {
            //    Assert.DoesNotThrow(() => converter.Convert(i));
            //}
        }
    }
}