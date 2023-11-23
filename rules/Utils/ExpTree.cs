using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rules.Utils
{
    public class ExpTree : ICloneable
    {
        public object Value { get; set; }
        public ExpTree LC { get; set; } // Left Child
        public ExpTree RC { get; set; } // Right Child

        public ExpTree()
        {
            LC = RC = null;
        }

        public ExpTree(object Value, ExpTree LC = null, ExpTree RC = null)
        {
            this.Value = Value;
            this.LC = LC;
            this.RC = RC;
        }

        public bool IsALeaf()
        {
            return LC == null && RC == null;
        }

        public object Clone()
        {
            ExpTree tree = new ExpTree(Value);

            if (LC != null)
                tree.LC = (ExpTree)LC.Clone();

            if (RC != null)
                tree.RC = (ExpTree)RC.Clone();

            return tree;
        }

        // Retourne la formule mathématique de la fonction en langage Tex
        public override string ToString()
        {
            StringBuilder exp = new StringBuilder();

            if (Value is double)
                exp.Append((double)Value);

            else if(Value is char)
            {
                char tmp = (char)Value;
                if (tmp == 'x')
                    exp.Append('x');

                else if (tmp == '/')
                    exp.Append(string.Format("\\frac{{{0}}}{{{1}}}", LC.ToString(), RC.ToString()));

                else if (tmp == '-' && LC == null)
                    exp.Append(string.Format("-{0}", RC.ToString()));

                else if(tmp == '^')
                {
                    char? lc_value = LC.Value as char?,
                          rc_value = RC.Value as char?;

                    if(lc_value != null && lc_value != 'x' && lc_value != '^')
                        exp.Append(string.Format("({0})", LC.ToString()));

                    else
                        exp.Append(LC.ToString());

                    exp.Append(string.Format("^{{{0}}}", RC.ToString()));
                }

                else
                {
                    if(tmp == '*')
                    {
                        if (LC.Value is double || RC.Value is double)
                        {
                            string s1 = LC.ToString(), s2 = RC.ToString();

                            exp.Append(s1);
                            if (char.IsDigit(s1.Last()) && char.IsDigit(s2.First()))
                                exp.Append("\\times");

                            exp.Append(s2);
                        }

                        else
                        {
                            char? lc_value = LC.Value as char?,
                                  rc_value = RC.Value as char?;

                            if (lc_value == '+' || lc_value == '-')
                                exp.Append(string.Format("({0}).", LC.ToString()));

                            else
                                exp.Append(LC.ToString());

                            if (rc_value == '+' || rc_value == '-')
                            {
                                if (exp[exp.Length - 1] != '.')
                                    exp.Append('.');

                                exp.Append(string.Format("({0})", RC.ToString()));
                            }

                            else
                                exp.Append(RC.ToString());
                        }
                    }

                    else
                        exp.Append(string.Format("{0} {1} {2}", LC.ToString(), tmp, RC.ToString()));
                }
            }

            else
            {
                string func_str = MyUtils.GetFuncString((MyUtils.RealFunction)Value);

                switch(func_str)
                {
                    case "sqrt":
                        exp.Append(string.Format("\\sqrt{{{0}}}", RC.ToString()));
                        break;

                    case "sqrt3":
                        exp.Append(string.Format("\\sqrt[3]{{{0}}}", RC.ToString()));
                        break;

                    case "log2":
                        exp.Append(string.Format("\\log[2]{{{0}}}", RC.ToString()));
                        break;

                    case "log3":
                        exp.Append(string.Format("\\log[3]{{{0}}}", RC.ToString()));
                        break;

                    default:
                        exp.Append(string.Format("\\{0}({1})", func_str, RC.ToString()));
                        break;
                }
            }

            return exp.ToString();
        }

        // ------------------------- SIMPLIFICATION DE L'ARBRE ----------------------------------------- //
        public void SimplifyYou()
        {
            if (IsALeaf())
                return;

            if (LC != null)
                LC.SimplifyYou();

            if (RC != null)
                RC.SimplifyYou();
            // ----------------------------------------------------------------------------- //
            char? op = Value as char?;
            if(op != null)
            {
                // - unaire
                if(LC == null)
                { 
                    if(RC.Value is char && (char)RC.Value == '-' && RC.LC == null)
                    {
                        Value = RC.RC.Value;
                        LC = RC.RC.LC;
                        RC = RC.RC.RC;
                    }
                }
                // Opérateur binaire
                else
                {
                    double? oper1 = LC.Value as double?,
                            oper2 = RC.Value as double?;

                    // Expression constante
                    if(oper1 != null && oper2 != null)
                    {
                        Value = MyUtils.DoOperation(op.Value, oper1.Value, oper2.Value);
                        RC = LC = null;
                    }

                    else
                    {
                        switch (op)
                        {
                            case '+':
                                SimplifyPlusExp(this);
                                break;

                            case '-':
                                SimplifyMinusExp(this);
                                break;

                            case '*':
                                SimplifyMulExp(this);
                                break;

                            case '/':
                                SimplifyDivExp(this);
                                break;

                            case '^':
                                SimplifyPowExp(this);
                                break;
                        }
                    }
                }
            }
        }

        private static void SimplifyPlusExp(ExpTree tree)
        {
            double? oper1 = tree.LC.Value as double?,
                    oper2 = tree.RC.Value as double?;

            char? tmp = tree.RC.Value as char?;
            // + suivi d'un - unaire
            if (tmp == '-' && tree.RC.LC == null)
            {
                tree.Value = '-';
                tree.RC = tree.RC.RC;
            }
            // 0 +
            else if (oper1 == 0d)
            {
                tree.Value = tree.RC.Value;
                tree.LC = tree.RC.LC;
                tree.RC = tree.RC.RC;
            }
            // + 0
            else if (oper2 == 0d)
            {
                tree.Value = tree.LC.Value;
                tree.RC = tree.LC.RC;
                tree.LC = tree.LC.LC;
            }

            // On vérifie s'il n'y a d'expression additionnable
            else
            {
                MyUtils.RealFunction func1 = tree.LC.Value as MyUtils.RealFunction,
                                     func2 = tree.RC.Value as MyUtils.RealFunction;

                if(func1 != null && func2 != null && func1 == func2)
                {
                    tree.Value = '*';
                    tree.LC.Value = 2d;
                }

                else
                {
                    char? oplc = tree.LC.Value as char?,
                          oprc = tree.RC.Value as char?;

                    if (oplc == '*' && oprc == '*')
                    {
                        
                    }
                }
            }
        }

        private static void SimplifyMinusExp(ExpTree tree)
        {
            double? oper1 = tree.LC.Value as double?,
                    oper2 = tree.RC.Value as double?;

            // 0 -
            if (oper1 == 0)
                tree.LC = null;

            // - 0
            else if (oper2 == 0)
            {
                tree.Value = tree.LC.Value;
                tree.RC = tree.LC.RC;
                tree.LC = tree.LC.LC;
            }
        }

        private static void SimplifyMulExp(ExpTree tree)
        {
            double? oper1 = tree.LC.Value as double?,
                    oper2 = tree.RC.Value as double?;

            // 0 * ou * 0
            if (oper1 == 0d || oper2 == 0d)
            {
                tree.Value = 0d;
                tree.LC = tree.RC = null;
            }
            // 1 *
            else if (oper1 == 1d)
            {
                tree.Value = tree.RC.Value;
                tree.LC = tree.RC.LC;
                tree.RC = tree.RC.RC;
            }
            // * 1
            else if (oper2 == 1d)
            {
                tree.Value = tree.LC.Value;
                tree.RC = tree.LC.RC;
                tree.LC = tree.LC.LC;
            }
        }

        private static void SimplifyDivExp(ExpTree tree)
        {
            double? oper1 = tree.LC.Value as double?,
                    oper2 = tree.RC.Value as double?;

            // Ici on cherche si on peut factoriser la fraction
            if(oper1 == null && oper2 == null)
            {
                Stack<Tuple<ExpTree, ExpTree>> factors = GetCommonFactor(tree.LC, tree.RC);
            }

            // 0 /
            else if (oper1 == 0d)
            {
                tree.Value = 0d;
                tree.LC = tree.RC = null;
            }
            // / 1
            else if (oper2 == 1d)
            {
                tree.Value = tree.LC.Value;
                tree.RC = tree.LC.RC;
                tree.LC = tree.LC.LC;
            }
        }

        private static void SimplifyPowExp(ExpTree tree)
        {
            double? oper1 = tree.LC.Value as double?,
                    oper2 = tree.RC.Value as double?;

            char? tmp = tree.LC.Value as char?;
            // Deux puissances qui se suivent
            if (tmp == '^')
            {
                ExpTree foo = new ExpTree('*', tree.RC, tree.LC.RC);
                foo.SimplifyYou();

                tree.LC = tree.LC.LC;
                tree.RC = foo;
            }
            // Puissance 1 ou puissance 0
            else if (oper1 == 1d || oper1 == 0d || oper2 == 0d)
            {
                tree.Value = 1d;
                tree.LC = tree.RC = null;
            }
            // 1 puissance x
            else if (oper2 == 1d)
            {
                tree.Value = tree.LC.Value;
                tree.RC = tree.LC.RC;
                tree.LC = tree.LC.LC;
            }
        }

        // Renvoie les facteurs en commun de tree1 et tree2.
        // Chaque tuple contient la position du facteur dans tree1 et dans tree2
        public static Stack<Tuple<ExpTree, ExpTree>> GetCommonFactor(ExpTree tree1, ExpTree tree2)
        {
            Stack<Tuple<ExpTree, ExpTree>> factors = new Stack<Tuple<ExpTree, ExpTree>>();
            GetCommonFactor_bis(tree1, tree2, factors);

            return factors;
        }

        private static void GetCommonFactor_bis(ExpTree tree1, ExpTree tree2, Stack<Tuple<ExpTree, ExpTree>> factors)
        {
            if (tree1 == null || tree2 == null)
                return;

            else if (tree1.Value.Equals(tree2.Value))
            {
                factors.Push(Tuple.Create(tree1, tree2));
                GetCommonFactor_bis(tree1.LC, tree2.LC, factors);
                GetCommonFactor_bis(tree1.RC, tree2.RC, factors);
            }

            else if(factors.Count == 0 || !(factors.Peek().Item1.Value is MyUtils.RealFunction))
            {
                char? op1 = tree1.Value as char?,
                      op2 = tree2.Value as char?;

                if (op1 != '*' && op1 != '^' && op2 != '*' && op2 != '^')
                {
                    if(factors.Count != 0)
                    {
                        char? tmp = factors.Peek().Item1.Value as char?;
                        if (tmp == '^' || tmp == '-' || tmp == '/')
                            factors.Pop();
                    }

                    return;
                }

                else if (op1 == '*')
                {
                    factors.Push(Tuple.Create(tree1, tree2));
                    GetCommonFactor_bis(tree1.LC, tree2, factors);
                    GetCommonFactor_bis(tree1.RC, tree2, factors);
                }

                else if (op2 == '*')
                {
                    factors.Push(Tuple.Create(tree1, tree2));
                    GetCommonFactor_bis(tree1, tree2.LC, factors);
                    GetCommonFactor_bis(tree1, tree2.RC, factors);
                }

                else if (op1 == '^')
                {
                    factors.Push(Tuple.Create(tree1, tree2));

                    int save = factors.Count;
                    GetCommonFactor_bis(tree1.LC, tree2, factors);

                    if (factors.Count <= save)
                        factors.Pop();
                }

                else
                {
                    factors.Push(Tuple.Create(tree1, tree2));
                    GetCommonFactor_bis(tree1, tree2.LC, factors);
                }
            }

            else
            {
                factors.Pop();
            }
        }
    }
}
