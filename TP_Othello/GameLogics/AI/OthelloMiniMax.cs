using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TP_Othello.GameLogics.AI
{
    public class OthelloMiniMax
    {
        public static Move GetMove(int[,] board, int depth, bool whitePlayer)
        {
            MiniMaxTreeNode rootNode = new MiniMaxTreeNode(board, whitePlayer);

            Tuple<int, Move> moveSelected = Alphabeta2(rootNode, depth, 1, int.MinValue);

            return moveSelected.Item2;
        }

        private static Tuple<int, Move> Alphabeta2(MiniMaxTreeNode node, int depth, int minOrMax, int parentValue)
        {
            if(depth == 0 || node.IsLeaf())
            {
                return new Tuple<int, Move>(node.Evaluate(), null);
            }

            int optVal = minOrMax == 1 ? int.MinValue : int.MaxValue;
            Move optMove = null;

            foreach(Move move in node.GetMoves())
            {
                MiniMaxTreeNode newNode = node.ApplyMove(move);

                Tuple<int, Move> valMove = Alphabeta2(newNode, depth - 1, -minOrMax, optVal);
                
                if(valMove.Item1 * minOrMax > optVal * minOrMax)
                {
                    optVal = valMove.Item1;
                    optMove = move;

                    if (optVal * minOrMax > parentValue * minOrMax)
                        break;
                }
            }

            return new Tuple<int, Move>(optVal, optMove);
        }

    }
}
