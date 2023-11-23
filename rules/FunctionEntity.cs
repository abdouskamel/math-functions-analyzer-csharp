using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rules
{
    using Utils;

    public class FunctionEntity
    {
        public string[] Tokens { get; private set; }
        public ExpTree FuncTree { get; private set; }

        string TexFormula;

        public FunctionEntity(string exp)
        {
            Tokens = Tokenizer.TokenizeExp(exp);
            FuncTree = TheParser.CreateExpTree(Tokens);
            TexFormula = FuncTree.ToString();   
        }

        public FunctionEntity(ExpTree tree)
        {
            FuncTree = tree;
            TexFormula = FuncTree.ToString();
        }

        public double GetOutput(double x)
        {
            return MyCalculator.CalculateOutput(FuncTree, x);
        }

        public FunctionEntity GetDerivative()
        {
            return new FunctionEntity(MyCalculator.GetDerivative(FuncTree));
        }

        public string GetTexFormula()
        {
            return TexFormula;
        }
    }
}
