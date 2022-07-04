using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace MixedNumberCalculator
{
    public class MixedNumberCalculator
    {//data structure for storing either fractional operands or operators during evaluation and infix to postfix conversion. 
       
        //can be used to store both operators and operands
        public struct FractionalSubExpressions 
        {
            public int numerator;
            public int denominator;
            public string foperator;
        }

        //stack for converting infix to postfix and evaluating postfix expression
        
        static Stack<FractionalSubExpressions> OperatorsandOperands = new Stack<FractionalSubExpressions>();

        static void Main(string[] args)
        {
            string input;


            do {
               
                Console.WriteLine("Enter an expression to evaluate:");
                input = Console.ReadLine();
                
                if(input!="exit")
                Console.WriteLine(CalculateMixedNumbers(input));




            } while (input!="exit");



        }


        public static string CalculateMixedNumbers(string input)
        {
            string answer;
            //1)convert mixed numbers to improper fraction parse and infix expression
            //add white at the beginning and end  to distinguish a wholenumber from a fraction or mixed number
            //in case it comes at the beginning or end of expression
            string expression = MixedNumberstoImproperFractions(" " + input + " ");
            //2)convert expression from infix to postfix
            expression=InfixtoPostfix(expression);
             //3)evaluate postfix expression to improper fraction
             answer=EvaluatePostfix(expression);
            //4)convert improper fraction to mixed number and reduce fractional part of mixed number
              answer=ImproperFractiontoMixedNumber(answer);
              return answer;

        }

        public static Boolean InfixIsCorrect(string input)
        {   //the expression is correct if the opcount = 1 less than the operand count

            bool correct;

            //count improperfractions
           
            
            Regex improperfraction = new Regex(@"\s\d+/\d+\s");
            MatchCollection improperfractionpn = improperfraction.Matches(input);

            int operandcount = improperfractionpn.Count;

            int opcount = 0;
            string [] parts=input.Split(' ');

            //count operators
            foreach(string part in parts)
                if(part=="/"||part=="+"||part=="%"||part=="-"||part=="*")
                    opcount++;

           

            if (opcount==operandcount-1)
                correct=true;
            else
                correct=false;



            return correct;
        }
        public static string MixedNumberstoImproperFractions(string input)
        {

            //regex pattern for capturing mixed numbers
            Regex regex = new Regex(@"\d+&\d+/\d+");
            MatchCollection mixednumbers = regex.Matches(input);
            char[] delimiterChars = { '&', '/' };

            foreach (Match m in mixednumbers)
            {
                string[] numbers = m.ToString().Split(delimiterChars);
                //numbers[0]=whole number part
                //numbers[1]= numerator part
                //numbers[2]= denominator part

                /*
                 * (numbers[0] * numbers[2] + numbers[1])/numbers[2]
                 */

                int den = Int32.Parse(numbers[2]);
                int num = Int32.Parse(numbers[0]) * den + Int32.Parse(numbers[1]);

                input = input.Replace(m.ToString(), num.ToString() + "/" + den.ToString());
            }

            //convert whole number to improper fractions

            //regex for capturing wholenumbers
            Regex wholenumber = new Regex(@"^\d+$");

            string[] Subexpressions = input.Split(new char[] { ' ' });

            foreach (String exp in Subexpressions)
                if (wholenumber.IsMatch(exp)) //whole numbers need to be padded with whitespace on both left and right so they aren't mistaken for denominors or numerators
                    input = input.Replace( " "+exp + " ", " " + exp + "/1" + " ");
            //check if it is in the correct format
            if (!InfixIsCorrect(input))
                throw new InvalidOperationException(@"The postfix conversion failed. invalid user input!");
            //trim out whitespace at the end and beginning for postfix conversion
            input =  input.Remove(0, 1);   
            input = input.Remove(input.Length - 1, 1);
            return input;

        }
        public static string InfixtoPostfix(string input)
        {
            string[] Subexpressions = input.Split(new char[] { ' ' });
            string postfix = "";
            Regex regex = new Regex(@"\d+/\d+");
            foreach (string sub in Subexpressions)
            {


                if (regex.IsMatch(sub)) //its an operand
                {
                    //concatinate to postfix expression
                    postfix += sub + " ";

                }

                else if (OpPriority(sub) > 0) //its an operator
                {   //if stack is not empty
                    //if there is operator with <= priority pop it from the stack and concatinate it to the string
                    while (OperatorsandOperands.Count > 0 && OpPriority(sub) <= OpPriority(OperatorsandOperands.Peek().foperator))
                        postfix += OperatorsandOperands.Pop().foperator + " ";


                    //store and push the operator on to the stack

                    FractionalSubExpressions fractionOperator = new FractionalSubExpressions();

                    fractionOperator.foperator = sub;

                    OperatorsandOperands.Push(fractionOperator);
                }

                else
                    throw new InvalidOperationException(@"The postfix conversion failed. invalid user input!");


            }
          

            //pop reamaining operands and concatinate to string
            while (OperatorsandOperands.Count > 0)
            {
                postfix += OperatorsandOperands.Pop().foperator + " ";
            }

            return postfix;


        }
        static int OpPriority(string op) //determines proirity of operation
        {
            switch (op)
            {
                case "+":
                case "-":
                    return 1;

                case "*":
                case "/":
                case "%":
                    return 2;


            }
            return -1;
        }

        public static string EvaluatePostfix(string input)
        {
            string[] Subexpressions = input.Split(new char[] { ' ' });
            string improperfraction;
            Regex regex = new Regex(@"\d+/\d+");

            FractionalSubExpressions answer;

            foreach (string sub in Subexpressions)
            {
                if (regex.IsMatch(sub)) //its an operand
                {

                    //use splitter parse the numerator and the denominator

                    //store operand and push it on to the  stack


                    string[] parts = sub.Split(new char[] { '/' });
                    FractionalSubExpressions fraction = new FractionalSubExpressions();
                    fraction.numerator = Int32.Parse(parts[0]);
                    fraction.denominator = Int32.Parse(parts[1]);

                    OperatorsandOperands.Push(fraction);






                }

                else if (sub == "+" || sub == "-")
                {
                    FractionalSubExpressions fractionA = OperatorsandOperands.Pop();
                    FractionalSubExpressions fractionB = OperatorsandOperands.Pop();
                    int denominator;
                    answer = new FractionalSubExpressions();
                    //cross multiply to obtain a common denomionator

                    if (fractionA.denominator != fractionB.denominator)
                    {
                        //cross multiply to get common denominator

                        FractionalSubExpressions[] fractions = CrossMultiplyFractions(fractionA, fractionB);
                        fractionA = fractions[0];
                        fractionB = fractions[1];


                       
                    }

                        denominator = fractionA.denominator;

                    if (sub == "+")
                        answer.numerator = fractionA.numerator + fractionB.numerator;
                    
                    //subtraction needs to be done in reverse order or the order in which they both popped need to be
                    //reversed
                 
                    else
                        answer.numerator = fractionB.numerator - fractionA.numerator;



                    answer.denominator = denominator;

                    //push answer on to stack

                    OperatorsandOperands.Push(answer);


                }

                else if (sub == "*" || sub == "/")
                {
                    FractionalSubExpressions fractionA = OperatorsandOperands.Pop();
                    FractionalSubExpressions fractionB = OperatorsandOperands.Pop();
                    answer = new FractionalSubExpressions();

                    //get the reciprocal of fractionA (this is the right operand when viewed in infix)
                    //note if the stack were popped and assigned in reverse order the reciprocal of fractionB would be needed instead
                    if (sub == "/") 
                        fractionA = GetReciprocal(fractionA);

                    answer.numerator = fractionA.numerator * fractionB.numerator;

                    answer.denominator = fractionA.denominator * fractionB.denominator;
                    OperatorsandOperands.Push(answer);


                }


                else if (sub == "%")
                {
                    FractionalSubExpressions fractionA = OperatorsandOperands.Pop();
                    FractionalSubExpressions fractionB = OperatorsandOperands.Pop();

                    //compute the decimal value of both of the fractions so they can be compared
                    double valueA = Convert.ToDouble(fractionA.numerator) / Convert.ToDouble(fractionA.denominator);
                    double valueB = Convert.ToDouble(fractionB.numerator) / Convert.ToDouble(fractionB.denominator);

                    //if the left hand operand is < the right then the % is that operand itself on the left
                   
                    //the 2nd value popped from the stack is the operand on the left side of the operator
                    //when viewed in infix format
                    
                    if (valueB >= valueA)

                    {
                        //cross multiplication is needed if denominators differ 
                        if (fractionA.denominator != fractionB.denominator)
                        {
                            //cross multiply to get common denominator

                            FractionalSubExpressions[] fractions = CrossMultiplyFractions(fractionA, fractionB);
                            fractionA = fractions[0];
                            fractionB = fractions[1];


                        }

                        //get the modulo of numerator
                        fractionB.numerator= fractionB.numerator % fractionA.numerator;
                       

                    }



                    answer = fractionB;

                    OperatorsandOperands.Push(answer);



                }




            }
            answer = OperatorsandOperands.Pop();
            improperfraction = answer.numerator.ToString() + "/" + answer.denominator.ToString();
            return improperfraction;

        }

        public static string ImproperFractiontoMixedNumber(string input)
        {

            string mixednumber= input;

            string [] parts=mixednumber.Split('/');



            int numerator = Int32.Parse(parts[0]);
            int denominator=Int32.Parse(parts[1]);

            int wholenumber=numerator / denominator;

            numerator=numerator % denominator;

            //reduce fraction
            FractionalSubExpressions fraction=new FractionalSubExpressions();
            fraction.numerator = numerator;
            fraction.denominator = denominator;
            fraction=ReduceFraction(fraction);

            //if the denominator is negative convert to positive value
            //and convert denominator to positve value
            if (fraction.denominator < 0 && fraction.numerator>0)
            { 
                fraction.numerator = fraction.numerator * -1;
                fraction.denominator= fraction.denominator * -1;



            };

            if (numerator != 0 && wholenumber!=0)
                mixednumber = wholenumber.ToString() + "&" + fraction.numerator.ToString() + "/" + fraction.denominator.ToString();
            else if(wholenumber==0)
                mixednumber = fraction.numerator.ToString() + "/" + fraction.denominator.ToString();
            
            else
                mixednumber = wholenumber.ToString();



            return mixednumber;

        }
        static public FractionalSubExpressions ReduceFraction(FractionalSubExpressions fr)
        {
            int gcd = GetGcd(fr.numerator, fr.denominator);

            fr.numerator = fr.numerator / gcd;
            fr.denominator = fr.denominator / gcd;

           return fr;
        }
        static int GetGcd(int a, int b)
        {
            //use Euclid's alogorthmn to get gcd
            int c;
            while (b != 0)
            {
                c = a % b;
                a = b;
                b = c;

            }
            return a;
        }


       public static FractionalSubExpressions[] CrossMultiplyFractions(FractionalSubExpressions fractionA, FractionalSubExpressions fractionB)
        {

            FractionalSubExpressions[] fractions = new FractionalSubExpressions[2];
            
            //multiply denominators to get a common denominator
            int cd = fractionA.denominator * fractionB.denominator;
            fractionA.numerator = fractionA.numerator * fractionB.denominator;
            fractionB.numerator = fractionB.numerator * fractionA.denominator;

            fractionA.denominator = fractionB.denominator =cd;
            fractions[0] = fractionA;
            fractions[1] = fractionB;


            return fractions;


        }

       public static FractionalSubExpressions GetReciprocal(FractionalSubExpressions fraction)
        {
            //flip denominator and numerator values
            int denominator = fraction.numerator;
            int numerator = fraction.denominator;

            fraction.denominator = denominator;
            fraction.numerator = numerator;
            return fraction;

        }
      
    
    
    
    
    
    }
}
