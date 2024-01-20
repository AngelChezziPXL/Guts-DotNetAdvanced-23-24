using System.Text;

namespace RomanNumberConverterApp.Ui.Models;

public class RomanNumberConverter : IRomanNumberConverter
{
    public string Convert(int number)
    {
        if(number < 1 || number > 3999)
        {
            throw new ArgumentException("Out of Roman range (1-3999)");
        }

        StringBuilder romanNumber = new StringBuilder();
        while (number >= 1000)
        {
            romanNumber.Append("M");
            number -= 1000;
        } 
        while (number >= 900)
        {
            romanNumber.Append("CM");
            number -= 900;
        }
        while (number >= 500)
        {
            romanNumber.Append("D");
            number -= 500;
        } 
        while (number >= 400)
        {
            romanNumber.Append("CD");
            number -= 400;
        }
        while (number >= 100)
        {
            romanNumber.Append("C");
            number -= 100;
        } 
        while (number >= 90)
        {
            romanNumber.Append("XC");
            number -= 90;
        }
        while (number >= 50)
        {
            romanNumber.Append("L");
            number -= 50;
        } 
        while (number >= 40)
        {
            romanNumber.Append("XL");
            number -= 40;
        }
        while (number >= 10)
        {
            romanNumber.Append("X");
            number -= 10;
        } 
        while (number >= 9)
        {
            romanNumber.Append("IX");
            number -= 9;
        } 
        while (number >= 5)
        {
            romanNumber.Append("V");
            number -= 5;
        }
        while (number >= 4)
        {
            romanNumber.Append("IV");
            number -= 4;
        } 
        while (number >= 1)
        {
            romanNumber.Append("I");
            number -= 1;
        }



        return romanNumber.ToString();
        //throw new NotImplementedException();
    }
}