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
        public static double CalculateOutput(ExpTree tree, double x)
        {
            if (tree == null)
                return 0d;

            else if(!tree.IsALeaf())
            {
                char? op = tree.Value as char?;
                double result;
                // Les opérandes sont le fils gauche et le fils droit
                if (op != null)
                {
                    if (op == '/')
                    {
                        double tmp = CalculateOutput(tree.RC, x);
                        if(tmp == 0d)
                            throw new OutputCalculException("Cette valeur de X ne fait pas partie de l'ensemble de définition de la fonction.", CalculErrorType.INVALID_VALUE);

                        result = MyUtils.DoOperation('/', CalculateOutput(tree.LC, x), tmp);
                    }

                    else
                        result = MyUtils.DoOperation(op.Value, CalculateOutput(tree.LC, x), CalculateOutput(tree.RC, x));
                }

                // Une opérande : le fils droit
                else
                    result = ((MyUtils.RealFunction)tree.Value)(CalculateOutput(tree.RC, x));

                if (double.IsPositiveInfinity(result))
                    throw new OutputCalculException("L'image de X est trop grande pour être affichée.", CalculErrorType.POSITIVE_OVERFLOW);

                else if(double.IsNegativeInfinity(result))
                    throw new OutputCalculException("L'image de X est trop petite pour être affichée.", CalculErrorType.NEGATIVE_OVERFLOW);

                else if (double.IsNaN(result))
                    throw new OutputCalculException("Cette valeur de X ne fait pas partie de l'ensemble de définition de la fonction.", CalculErrorType.INVALID_VALUE);

                return result;
            }

            // Si on trouve une feuille, on renvoit X ou bien le nombre contenu dans la feuille
            else if (tree.Value is char)
                return x;

            else
                return (double)tree.Value;
        }
    }
}
