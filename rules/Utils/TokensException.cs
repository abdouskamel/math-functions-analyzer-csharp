using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rules.Utils
{
    public struct TokErrorPos
    {
        public int Begin { get; set; }
        public int End { get; set; }
    }

    public class TokensException : Exception
    {
        public TokErrorPos ErrorPos { get; set; }

        public TokensException(string msg) : base(msg)
        {

        }
        public TokensException(string msg, TokErrorPos ErrorPos) : this(msg)
        {
            this.ErrorPos = ErrorPos;
        }
    }
}
