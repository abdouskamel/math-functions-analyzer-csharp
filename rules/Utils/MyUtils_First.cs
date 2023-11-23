using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace Rules.Utils
{
    public static partial class MyUtils
    {
        public delegate double RealFunction(double x); // Le type délégué des fonctions réels à variable réelle

        public const string BinOps = "+-*/^", UnaryOps = "-",
                            OpenBrackets = "([", CloseBrackets = ")]";

        public static readonly string[] TheConstants = { "pi", "e", "sq2" };

        public static readonly string[] TheFunctions = { "sin", "cos", "tan", "arcsin", "arccos", "arctan",
                                                         "ch", "sh", "th","argch", "argsh", "argth",
                                                         "ln", "exp", "sqrt", "sqrt3", "log", "log2", "log3"};

        public static readonly string[][] FuncsDerivative;

        public static readonly RealFunction[] FuncsDelegates = { new RealFunction(Math.Sin), new RealFunction(Math.Cos),
                                                                new RealFunction(Math.Tan), new RealFunction(Math.Asin),
                                                                new RealFunction(Math.Acos), new RealFunction(Math.Atan),
                                                                new RealFunction(Math.Cosh), new RealFunction(Math.Sinh),
                                                                new RealFunction(Math.Tanh), new RealFunction(Argch),
                                                                new RealFunction(Argsh), new RealFunction(Argth),
                                                                new RealFunction(Math.Log), new RealFunction(Math.Exp),
                                                                new RealFunction(Math.Sqrt), new RealFunction(Sqrt3),
                                                                new RealFunction(Math.Log10), new RealFunction(Log2), new RealFunction(Log3)};

        static MyUtils()
        {
            FuncsDerivative = new string[TheFunctions.Length][];

            FuncsDerivative[0] = new string[]{ "cos", "(", "x", ")" };
            FuncsDerivative[1] = new string[]{ "-", "sin", "(", "x", ")" };
            FuncsDerivative[2] = new string[]{ "1", "-", "tan", "(", "x", ")", "^", "2" };
            FuncsDerivative[3] = new string[]{ "1", "/", "(", "1", "-", "x", "^", "2", ")" };
            FuncsDerivative[4] = new string[]{ "-", "1", "/", "(", "1", "-", "x", "^", "2", ")" };
            FuncsDerivative[5] = new string[]{ "1", "/", "(", "1", "+", "x", "^", "2", ")" };
            FuncsDerivative[6] = new string[]{ "sh", "(", "x", ")" };
            FuncsDerivative[7] = new string[]{ "sh", "(", "x", ")" };
            FuncsDerivative[8] = new string[]{ "1", "-", "th", "(", "x", ")", "^", "2" };
            FuncsDerivative[9] = new string[]{ "1", "/", "sqrt", "(", "x", "^", "2", "-", "1", ")" };
            FuncsDerivative[10] = new string[]{ "1", "/", "sqrt", "(", "x", "^", "2", "+", "1", ")" };
            FuncsDerivative[11] = new string[]{ "1", "/", "(", "1", "-", "x", "^", "2", ")" };
            FuncsDerivative[12] = new string[]{ "1", "/", "x" };
            FuncsDerivative[13] = new string[]{ "exp", "(", "x", ")" };
            FuncsDerivative[14] = new string[]{ "1", "/", "(", "2", "*", "sqrt", "(", "x", ")", ")" };
            FuncsDerivative[15] = new string[]{ "1", "/", "(", "3", "*", "x", "^", "(", "2", "/", "3", ")", ")" };
            FuncsDerivative[16] = new string[]{ "1", "/", "(", "ln", "(", "10", ")", "*", "x", ")" };
            FuncsDerivative[17] = new string[]{ "1", "/", "(", "ln", "(", "2", ")", "*", "x", ")" };
            FuncsDerivative[18] = new string[]{ "1", "/", "(", "ln", "(", "2", ")", "*", "x", ")" };
        }

        public static RealFunction GetFuncDelegate(string func)
        {
            return FuncsDelegates[Array.IndexOf(TheFunctions, func)];
        }

        public static string GetFuncString(RealFunction f)
        {
            return TheFunctions[Array.IndexOf(FuncsDelegates, f)];
        }

        public static ExpTree GetFuncDerivative(RealFunction s, ExpTree X = null)
        {
            return TheParser.CreateExpTree(FuncsDerivative[Array.IndexOf(FuncsDelegates, s)], X);
        }

        public static double Argch(double x)
        {
            return Math.Log(x + Math.Sqrt(x * x - 1));
        }

        public static double Argsh(double x)
        {
            return Math.Log(x + Math.Sqrt(x * x + 1));
        }

        public static double Argth(double x)
        {
            return 1 / 2 * Math.Log((1 + x) / (1 - x));
        }

        public static double Sqrt3(double x)
        {
            return Math.Exp(1 / 3 * Math.Log(x));
        }

        public static double Log2(double x)
        {
            return Math.Log(x) / Math.Log(2);
        }

        public static double Log3(double x)
        {
            return Math.Log(x) / Math.Log(3);
        }

        public static double MyParseDouble(string s)
        {
            double res;
            bool ok = Double.TryParse(s, NumberStyles.Number, new CultureInfo("fr-FR"), out res);

            if (!ok)
                res = Double.Parse(s, new CultureInfo("en-US"));

            return res;
        }

        public static double DoOperation(char op, double a, double b)
        {
            switch (op)
            {
                case '+':
                    return a + b;

                case '-':
                    return a - b;

                case '*':
                    return a * b;

                case '/':
                    return a / b;

                case '^':
                    return Math.Pow(a, b);

                default:
                    return 0d;
            }
        }
    }
}
