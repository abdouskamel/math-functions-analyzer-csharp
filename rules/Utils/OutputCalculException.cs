using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rules.Utils
{
    public enum CalculErrorType
    {
        POSITIVE_OVERFLOW,
        NEGATIVE_OVERFLOW,
        INVALID_VALUE
    }

    public class OutputCalculException : Exception
    {
        public CalculErrorType ErrorType { get; protected set; }

        public OutputCalculException(string msg, CalculErrorType ErrorType) : base(msg)
        {
            this.ErrorType = ErrorType;
        }
    }
}
