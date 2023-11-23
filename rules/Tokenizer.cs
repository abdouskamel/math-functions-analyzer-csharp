using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rules
{
    using Utils;

    static class Tokenizer
    {
        public static string[] TokenizeExp(string exp)
        {
            List<string> tokens = new List<string>();
            StringBuilder tmp = new StringBuilder();

            // Extrait les tokens.
            foreach(char c in exp)
            {
                if (char.IsWhiteSpace(c) || c.IsAnOp() || c.IsABracket())
                {
                    if (tmp.Length != 0)
                    {
                        tokens.Add(tmp.ToString());
                        tmp.Clear();
                    }

                    if (!char.IsWhiteSpace(c))
                        tokens.Add(c.ToString());
                }

                else
                {
                    if(tmp.Length != 0 && !c.IsANumberChar() && char.IsDigit(tmp[0]))
                    {
                        tokens.Add(tmp.ToString());
                        tmp.Clear();
                    }

                    tmp.Append(char.ToLower(c));
                }
            }

            if (tmp.Length != 0)
                tokens.Add(tmp.ToString());

            // Vérifie les tokens.
            CheckTokens(tokens, exp);

            // Rajoute les multiplications implicites.
            for(int i = 0; i < tokens.Count; ++i)
            {
                if(i + 1 != tokens.Count && (tokens[i].IsCloseBracket() || tokens[i].PerfIsANumber() || tokens[i].IsAConstant()) && 
                  (tokens[i + 1].IsOpenBracket() || tokens[i + 1] == "x" || tokens[i + 1].IsAConstant() || tokens[i + 1].IsAFunction()))
                {
                    tokens.Insert(i + 1, "*");
                    i += 1;
                }
            }

            return tokens.ToArray();
        }

        public static void CheckTokens(List<string> tokens, string exp)
        {
            Stack<char> brack_lifo = new Stack<char>();
            TokensException e = null;
            TokErrorPos err_pos = new TokErrorPos { Begin = 0, End = 0 };
            int i = 0, j;
            char tmp;
            bool funcisgood;

            while(i < tokens.Count && e == null)
            {
                err_pos.End += tokens[i].Length;
                // On empile la parenthèse ouvrante trouvée.
                if (tokens[i].IsOpenBracket())
                {
                    brack_lifo.Push(tokens[i].First());
                }
                // On vérifie que la parenthèse fermante trouvée possède une parenthèse ouvrante associée.
                else if(tokens[i].IsCloseBracket())
                {
                    if(brack_lifo.Count < 1)
                        e = new TokensException("Parenthèse fermante indépendante.", err_pos);
                    
                    else
                    {
                        tmp = brack_lifo.Pop();
                        if (!MyUtils.AreCoupleOfBrackets(tmp, tokens[i].First()))
                            e = new TokensException(string.Format("Parenthèse fermante incorrecte, attendue : {0}.", tmp.GetCoupleOfBracket()), err_pos);
                    }
                }
                // On vérifie qu'il n'y a pas deux opérateurs qui se suivent, ou bien qu'il n'y a pas
                // d'opérateurs en début ou en fin d'expression.
                else if (tokens[i].IsAnOp())
                {
                    if (i + 1 == tokens.Count)
                        e = new TokensException("Opérateur en fin d'expression.", err_pos);

                    else if (i == 0 && !tokens[i].IsUnaryOp())
                        e = new TokensException("Opérateur binaire en début d'expression.", err_pos);

                    else if (tokens[i + 1].IsAnOp())
                    {
                        err_pos.End += tokens[i + 1].Length;
                        e = new TokensException("Deux opérateurs qui se suivent sans opérandes.", err_pos);
                    }

                    else if (tokens[i + 1].IsCloseBracket())
                    {
                        err_pos.End += tokens[i + 1].Length;
                        e = new TokensException("Opérateur suivi d'une parenthèse fermante.", err_pos);
                    }
                }
                // On vérifie qu'i n'y a pas deux nombres qui se suivent sans opérateur entre eux.
                else if (tokens[i].IsANumber())
                {
                    if(i + 1 != tokens.Count && tokens[i + 1].IsANumber())
                    {
                        err_pos.End += tokens[i + 1].Length;
                        e = new TokensException("Deux nombres qui se suivent sans opérateur.", err_pos);
                    }
                }
                // On vérifie que l'appel de fonction est correcte : f(exp).
                else if(tokens[i].IsAFunction())
                {
                    if (i + 1 == tokens.Count)
                        e = new TokensException("Appel de fonction sans paramètre en fin d'expression.", err_pos);

                    else if(!tokens[i + 1].IsOpenBracket())
                    {
                        err_pos.End += tokens[i + 1].Length;
                        e = new TokensException("Un appel de fonction doit être suivie d'une parenthèse ouvrante.", err_pos);
                    }

                    funcisgood = false;
                    j = i + 1;
                    while(j < tokens.Count && !tokens[j].IsCloseBracket())
                    {
                        if (!tokens[j].IsABracket())
                        {
                            funcisgood = true;
                            break;
                        }

                        j++;
                    }

                    if(!funcisgood)
                        e = new TokensException("Appel de fonction erroné, aucun paramètre spécifié.", err_pos);
                }

                else if(tokens[i].IsAConstant())
                {
                    if(i + 1 != tokens.Count && (tokens[i + 1].IsAConstant() || tokens[i + 1].IsANumber()))
                    {
                        err_pos.End += tokens[i + 1].Length;
                        e = new TokensException("Constante suivie d'une autre constante ou d'un nombre sans opérateur.", err_pos);
                    }
                }
                // On vérifie que x n'est pas suivi d'un nombre ou d'un autre x.
                else if(tokens[i] == "x")
                {
                    if(i + 1 != tokens.Count)
                    {
                        if(tokens[i + 1].IsANumber())
                        {
                            err_pos.End += tokens[i + 1].Length;
                            e = new TokensException("Variable x suivie d'un nombre, veuillez mettre un opérateur.", err_pos);
                        }

                        else if(tokens[i + 1] == "x")
                        {
                            err_pos.End += tokens[i + 1].Length;
                            e = new TokensException("Variable x suivie d'une variable x", err_pos);
                        }
                    }
                }
                // Ici on a affaire à une expression inconnue.
                else
                {
                    e = new TokensException("Expression inconnue.", err_pos);
                }

                err_pos.Begin += tokens[i].Length;
                i++;
            }


            if (e != null)
            {
                // Calcul de la véritable position de l'erreur.
                // Les espaces posent problème et faussent la position calculée en haut
                err_pos = e.ErrorPos;
                i = j = 0;
                // On fait avancer la position de début et de fin de l'erreur
                while(j <= e.ErrorPos.Begin)
                {
                    if (char.IsWhiteSpace(exp[i]))
                    {
                        err_pos.Begin++;
                        err_pos.End++;
                    }
                        
                    else
                        j++;

                    i++;
                }

                // On fait encore avancer celle de fin
                while(j < e.ErrorPos.End)
                {
                    if (char.IsWhiteSpace(exp[i]))
                        err_pos.End++;
                    else
                        j++;

                    i++;
                }

                e.ErrorPos = err_pos;
                throw e;
            }

            // Si la pile n'est pas vide, alors il reste des parenthèses ouvrantes non fermées.
            if (brack_lifo.Count > 0)
                throw new TokensException(string.Format("{0} parenthèses non fermées.", brack_lifo.Count), new TokErrorPos { Begin = -1, End = -1 });
        }
    }
}
