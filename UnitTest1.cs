using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using MixedNumberCalculator;
namespace MixedNumberCalculatorTest
{
    [TestClass]
    public class UnitTest1
    {
     

        [TestMethod]

        public void MixedNumbertoImproperFractionTest()
        {
            string  input = " 1/2 * 3&3/4 + 2&3/8 + 9/8 ";
            string expected = "1/2 * 15/4 + 19/8 + 9/8";
            string result = MixedNumberCalculator.MixedNumberCalculator.MixedNumberstoImproperFractions(input);

            Assert.AreEqual(expected, result);


        }

        [TestMethod]
        public void InfixtoPostFixTest()
        {
            string input = "1/2 * 15/4 + 19/8 + 9/8";
            string expected = "1/2 15/4 * 19/8 + 9/8 + ";
            string result= MixedNumberCalculator.MixedNumberCalculator.InfixtoPostfix(input);

            Assert.AreEqual(expected, result);
        }
        [TestMethod]

        public void PostfixEvaluationTest()
        {

            string input = "1/2 15/4 * 19/8 + 9/8 + ";
            string expected = "43/8";
            string result=MixedNumberCalculator.MixedNumberCalculator.EvaluatePostfix(input);


            Assert.AreEqual(expected, result);





        }
        [TestMethod]
        public void ReduceFractionTest()
        {
           
            MixedNumberCalculator.MixedNumberCalculator.FractionalSubExpressions fraction=new MixedNumberCalculator.MixedNumberCalculator.FractionalSubExpressions();
            fraction.numerator = 4;
            fraction.denominator = 8;
            fraction=MixedNumberCalculator.MixedNumberCalculator.ReduceFraction(fraction);
            string result=fraction.numerator.ToString() + "/" + fraction.denominator.ToString();
            string expected = "1/2";
            Assert.AreEqual(expected, result);


        }

        [TestMethod]



        public void CrossMultiplyFractionTest()
        {

            MixedNumberCalculator.MixedNumberCalculator.FractionalSubExpressions fractionA=new MixedNumberCalculator.MixedNumberCalculator.FractionalSubExpressions();
            MixedNumberCalculator.MixedNumberCalculator.FractionalSubExpressions fractionB = new MixedNumberCalculator.MixedNumberCalculator.FractionalSubExpressions();

            fractionA.numerator = 1;
            fractionA.denominator = 2;
            fractionB.numerator = 1;
            fractionB.denominator = 4;
            MixedNumberCalculator.MixedNumberCalculator.FractionalSubExpressions[] fractions = MixedNumberCalculator.MixedNumberCalculator.CrossMultiplyFractions(fractionA, fractionB);

            fractionA.numerator = fractions[0].numerator;
            fractionA.denominator = fractions[0].denominator;
            fractionB.numerator = fractions[1].numerator;
            fractionB.denominator=fractions[1].denominator;


            Assert.AreEqual(fractions[0].numerator,4);
            Assert.AreEqual(fractions[0].denominator, 8);
            Assert.AreEqual(fractions[1].numerator,2);
            Assert.AreEqual(fractions[1].denominator,8);



        }

        [TestMethod]
        public void ImproperFractiontoMixedNumberTest()
        {
            string input = "18/8";
            string expected = "2&1/4";
            string result= MixedNumberCalculator.MixedNumberCalculator.ImproperFractiontoMixedNumber(input);

            Assert.AreEqual(expected, result);


        }

        [TestMethod]

        public void CalculateMixedNumberTest()
        {
           
            string input = "5 + 3&3/4 * 1/2 % 1/2 - 1/4";
          
            string expected = "5&1/8";
            string result = MixedNumberCalculator.MixedNumberCalculator.CalculateMixedNumbers(input);

            Assert.AreEqual(expected,result);

        }
        [TestMethod]
        public void InfixIsCorrectTest()
        {
           


            string input = " 15/8 + + 2/8 ";

            Assert.IsFalse(MixedNumberCalculator.MixedNumberCalculator.InfixIsCorrect(input));
            
            
            
            string input2 = " 15/8 + 2/8 ";
            Assert.IsTrue(MixedNumberCalculator.MixedNumberCalculator.InfixIsCorrect(input2));





        }
      

    }


    

    


}
