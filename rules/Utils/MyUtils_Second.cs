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
        // --------------------------------------------------------------------- //
        public static bool IsAnOp(this string op)
        {
            return op.Length == 1 && IsAnOp(op[0]);
        }
        public static bool IsBinOp(this string op)
        {
            return op.Length == 1 && IsBinOp(op[0]);
        }
        public static bool IsUnaryOp(this string op)
        {
            return op.Length == 1 && IsUnaryOp(op[0]);
        }
        // --------------------------------------------------------------------- //
        public static bool IsAnOp(this char op)
        {
            return IsBinOp(op) || IsUnaryOp(op);
        }
        public static bool IsBinOp(this char op)
        {
            return BinOps.Contains(op);
        }
        public static bool IsUnaryOp(this char op)
        {
            return UnaryOps.Contains(op);
        }
        // --------------------------------------------------------------------- //
        public static bool IsABracket(this string s)
        {
            return s.Length == 1 && IsABracket(s[0]);
        }
        public static bool IsOpenBracket(this string s)
        {
            return s.Length == 1 && IsOpenBracket(s[0]);
        }
        public static bool IsCloseBracket(this string s)
        {
            return s.Length == 1 && IsCloseBracket(s[0]);
        }
        // --------------------------------------------------------------------- //
        public static bool IsABracket(this char c)
        {
            return IsOpenBracket(c) || IsCloseBracket(c);
        }
        public static bool IsOpenBracket(this char c)
        {
            return OpenBrackets.Contains(c);
        }
        public static bool IsCloseBracket(this char c)
        {
            return CloseBrackets.Contains(c);
        }
        public static char GetCoupleOfBracket(this char open)
        {
            return CloseBrackets[OpenBrackets.IndexOf(open)];
        }
        public static bool AreCoupleOfBrackets(char open, char close)
        {
            return open.GetCoupleOfBracket() == close;
        }
        // --------------------------------------------------------------------- //
        public static bool IsAConstant(this string s)
        {
            return TheConstants.Contains(s);
        }
        public static double GetConstantValue(this string s)
        {
            switch(s)
            {
                case "pi":
                    return Math.PI;

                case "e":
                    return Math.E;

                case "sq2":
                    return Math.Sqrt(2d);

                default:
                    return 0d;
            }
        }

        public static bool IsANumber(this string nb)
        {
            if (nb[0] == ',' || nb[0] == '.')
                return false;

            int i = 0;

            if (nb[0] == '-')
                i = 1;

            bool point = false;

            while(i < nb.Length)
            {
                if (nb[i] == ',' || nb[i] == '.')
                {
                    if (point)
                        return false;

                    point = true;
                }

                else if (!Char.IsDigit(nb[i]))
                    return false;

                ++i;
            }

            return true;
        }
        
        public static bool IsAFunction(this string f)
        {
            return TheFunctions.Contains(f);
        }

        public static bool PerfIsANumber(this string nb)
        {
            return char.IsDigit(nb[0]);
        }
        public static bool PerfIsAFunction(this string nb)
        {
            return !nb[0].IsABracket() && !nb[0].IsAnOp() && !char.IsDigit(nb[0]);
        }

        public static bool IsANumberChar(this string nb)
        {
            return nb.Length == 1 && IsANumberChar(nb[0]);
        }
        public static bool IsANumberChar(this char c)
        {
            return char.IsDigit(c) || c == '.' || c == ',';
        }
    }
}
