using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rules
{
    using Utils;

    static partial class MyCalculator
    {
        public static ExpTree GetDerivative(ExpTree tree)
        {
            ExpTree der = GetDerivative_bis(tree);
            der.SimplifyYou();

            return der;
        }

        public static ExpTree GetDerivative_bis(ExpTree tree)
        {
            if (tree == null)
                return null;

            // Dérivée d'un nombre : 0
            else if (tree.Value is double)
                return new ExpTree(0d);

            // Une fonction
            else if(tree.Value is MyUtils.RealFunction)
            {
                // Si la fonction est constante, 0
                if (tree.RC.Value is double)
                    return new ExpTree(0d);

                char? variable = tree.RC.Value as char?;
                // Si la variable de la fonction est x, on renvoie f'
                if (variable == 'x')
                    return MyUtils.GetFuncDerivative((MyUtils.RealFunction)tree.Value);

                // Sinon fog
                else
                {
                    ExpTree tmp = GetDerivative_bis(tree.RC);
                    double? tmp_val = tmp.Value as double?;
                    // La dérivée de g est 1, on renvoit f'(g)
                    if (tmp_val == 1d)
                        return MyUtils.GetFuncDerivative((MyUtils.RealFunction)tree.Value, (ExpTree)tree.RC.Clone());

                    // g'.f'(g)
                    else
                        return new ExpTree('*',
                                           GetDerivative_bis(tree.RC),
                                           MyUtils.GetFuncDerivative((MyUtils.RealFunction)tree.Value, (ExpTree)tree.RC.Clone()));
                }
            }

            char? op = tree.Value as char?;
            if (op == 'x')
                return new ExpTree(1d);

            // Ici on a affaire à un opérateur

            // Expression constante
            if ((tree.LC == null || tree.LC.Value is double) && tree.RC.Value is double)
                return new ExpTree(0d);

            switch(op)
            {
                // f + g
                case '+':
                    // g'
                    if (tree.LC.Value is double)
                        return GetDerivative_bis(tree.RC);

                    // f'
                    else if (tree.RC.Value is double)
                        return GetDerivative_bis(tree.LC);

                    // f' + g'
                    else
                        return new ExpTree('+',
                                           GetDerivative_bis(tree.LC),
                                           GetDerivative_bis(tree.RC));
                // f - g
                case '-':
                    // -g'
                    if (tree.LC == null || tree.LC.Value is double)
                        return new ExpTree('-', null, GetDerivative_bis(tree.RC));

                    // f'
                    else if (tree.RC.Value is double)
                        return GetDerivative_bis(tree.LC);
                    
                    // f' - g'
                    else
                        return new ExpTree('-',
                                           GetDerivative_bis(tree.LC),
                                           GetDerivative_bis(tree.RC));
                
                // f * g
                case '*':
                    // ag'
                    if (tree.LC.Value is double)
                        return new ExpTree('*', new ExpTree(tree.LC.Value), GetDerivative_bis(tree.RC));

                    // af'
                    else if(tree.RC.Value is double)
                        return new ExpTree('*', new ExpTree(tree.RC.Value), GetDerivative_bis(tree.LC));

                    // f'g + fg'
                    else
                        return new ExpTree('+',
                                           new ExpTree('*',
                                                       GetDerivative_bis(tree.LC),
                                                       (ExpTree)tree.RC.Clone()),
                                           new ExpTree('*',
                                                       tree.LC,
                                                       GetDerivative_bis(tree.RC)));
                // f / g
                case '/':
                    // -(a*g')/g²
                    if (tree.LC.Value is double)
                        return new ExpTree('-', null,
                                           new ExpTree('/', 
                                                       new ExpTree('*',
                                                                   new ExpTree(tree.LC.Value),
                                                                   GetDerivative_bis(tree.RC)),

                                                       new ExpTree('^',
                                                                   (ExpTree)tree.RC.Clone(),
                                                                   new ExpTree(2d))));
                    // (1/a)*f'
                    else if (tree.RC.Value is double)
                        return new ExpTree('*', new ExpTree(1d / (double)tree.RC.Value), GetDerivative_bis(tree.LC));

                    // (f'.g - f.g') / g²
                    else
                        return new ExpTree('/',
                                           new ExpTree('-',
                                                       new ExpTree('*',
                                                                   GetDerivative_bis(tree.LC),
                                                                   (ExpTree)tree.RC.Clone()),
                                                       new ExpTree('*',
                                                                   (ExpTree)tree.LC.Clone(),
                                                                   GetDerivative_bis(tree.RC))),
                                           new ExpTree('^',
                                                       (ExpTree)tree.RC.Clone(),
                                                       new ExpTree(2d)));

                case '^':
                    // a^f
                    if (tree.LC.Value is double)
                        return new ExpTree('*',
                                           new ExpTree('*',
                                                       new ExpTree(MyUtils.GetFuncDelegate("ln"),
                                                                   null,
                                                                   new ExpTree(tree.LC.Value)),
                                                       GetDerivative_bis(tree.RC)),
                                           new ExpTree('^', new ExpTree(tree.LC.Value), (ExpTree)tree.RC.Clone()));

                    // f^a
                    else if (tree.RC.Value is double)
                        return new ExpTree('*',
                                           new ExpTree('*',
                                                       (ExpTree)tree.RC.Clone(),
                                                       GetDerivative_bis(tree.LC)),
                                           new ExpTree('^',
                                                       (ExpTree)tree.LC.Clone(),
                                                       new ExpTree((double)tree.RC.Value - 1d)));
                    // f^g
                    else
                        return new ExpTree('*',
                                           new ExpTree('+',
                                                       new ExpTree('*',
                                                                   GetDerivative_bis(tree.RC),
                                                                   new ExpTree(MyUtils.GetFuncDelegate("ln"),
                                                                               null,
                                                                               (ExpTree)tree.LC.Clone())),
                                                       new ExpTree('/',
                                                                   new ExpTree('*',
                                                                               GetDerivative_bis(tree.LC),
                                                                               (ExpTree)tree.RC.Clone()),
                                                                   (ExpTree)tree.LC.Clone())),
                                           new ExpTree(MyUtils.GetFuncDelegate("exp"),
                                                       null,
                                                       new ExpTree('*',
                                                                   (ExpTree)tree.RC.Clone(),
                                                                   new ExpTree(MyUtils.GetFuncDelegate("ln"),
                                                                               null,
                                                                               (ExpTree)tree.LC.Clone()))));

                default:
                    return null;
            }
        }
    }
}
