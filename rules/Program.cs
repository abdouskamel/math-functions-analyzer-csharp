using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rules
{
    using Utils;

    class Program
    {
        static void Main(string[] args)
        {
            string exp = Console.ReadLine();

            while(exp != "")
            {
                try
                {
                    string[] tokens = Tokenizer.TokenizeExp(exp);

                    ExpTree tree = TheParser.CreateExpTree(tokens),
                            tmp = tree;

                    Console.WriteLine(tree);

                    var save = ExpTree.GetCommonFactor(tree.LC, tree.RC);

                    for(int i = 0; i < 5; ++i)
                    {
                        tmp = MyCalculator.GetDerivative(tmp);
                        Console.WriteLine(tmp);
                    }

                    string x = Console.ReadLine();
                    while (x != "")
                    {
                        Console.WriteLine(MyCalculator.CalculateOutput(tree, Double.Parse(x)));
                        x = Console.ReadLine();
                    } 
                }

                catch (TokensException e)
                {
                    if (e.ErrorPos.Begin >= 0)
                    {
                        Console.Write(exp.Substring(0, e.ErrorPos.Begin));
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write(exp.Substring(e.ErrorPos.Begin, e.ErrorPos.End - e.ErrorPos.Begin));
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine(exp.Substring(e.ErrorPos.End));
                    }

                    Console.WriteLine(e.Message);
                }

                exp = Console.ReadLine();
            }

            Console.Read();
        }
    }
}
