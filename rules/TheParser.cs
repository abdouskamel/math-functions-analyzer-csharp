using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rules
{
    using Utils;

    static class TheParser
    {
        public static ExpTree CreateExpTree(string[] tokens, ExpTree X = null)
        {
            ExpTree tree = CreateExpTree_bis(tokens, 0, tokens.Length - 1, X);
            tree.SimplifyYou();

            return tree;
        }

        public static ExpTree CreateExpTree_bis(string[] tokens, int beg, int end, ExpTree X = null)
        {
            if (beg > end)
                return null;

            ExpTree tree = null;
            int index = GetWeakOperationIndex(tokens, beg, end);

            if (index != -1)
            {
                tree = new ExpTree();
                if(tokens[index] == "x")
                {
                    if (X == null)
                        tree.Value = 'x';

                    else
                        tree = (ExpTree)X.Clone();
                }

                else
                {
                    // Les opérateurs sont considérés comme des caractères
                    if (tokens[index].IsAnOp())
                        tree.Value = tokens[index].First();

                    // Les nombres comme des double
                    else if (tokens[index].PerfIsANumber())
                        tree.Value = MyUtils.MyParseDouble(tokens[index]);

                    // Les constantes aussi
                    else if (tokens[index].IsAConstant())
                        tree.Value = MyUtils.GetConstantValue(tokens[index]);

                    // Les fonctions comme des délégués de type double F(double)
                    else
                        tree.Value = MyUtils.GetFuncDelegate(tokens[index]);

                    tree.LC = CreateExpTree_bis(tokens, beg, index - 1, X);
                    tree.RC = CreateExpTree_bis(tokens, index + 1, end, X);
                }
            }

            return tree;
        }
         
        // Retourne l'index de l'opération la moins prioritaire. Retourne -1 s'il n'y a plus d'opération
        public static int GetWeakOperationIndex(string[] tokens, int beg, int end)
        {
            int op_i = -1, op_level = 0,
                brack_cpt = 0, rel_brack_cpt = 0;

            for(int i = beg; i <= end; ++i)
            {
                if (tokens[i].IsOpenBracket())
                {
                    brack_cpt++;
                    if (op_i != -1)
                        rel_brack_cpt++;
                }

                else if (tokens[i].IsCloseBracket())
                {
                    brack_cpt--;
                    if (rel_brack_cpt != 0)
                        rel_brack_cpt--;
                }

                else if (op_i == -1 || brack_cpt < op_level || (rel_brack_cpt == 0 && (tokens[i] == "+" || tokens[i] == "-" ||
                        ((tokens[i] == "*" || tokens[i] == "/") && tokens[op_i] != "-" && tokens[op_i] != "+") ||
                        ((tokens[i] == "^" || tokens[i].IsAFunction()) && !tokens[op_i].IsAnOp()))))
                {
                    op_i = i;
                    op_level = brack_cpt;
                }
            }

            return op_i;
        }
    }
}























