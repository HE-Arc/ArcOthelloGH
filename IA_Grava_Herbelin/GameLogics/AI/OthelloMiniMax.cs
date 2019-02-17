using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IA_Grava_Herbelin.GameLogics.AI
{
    public class OthelloMiniMax
    {
        /// <summary>
        /// Method called by anyone to calculate the best move to play.
        /// </summary>
        /// <param name="node">The current node to search for</param>
        /// <param name="depth">The max depth to search in</param>
        /// <param name="whitePlayer">Starting with the white player (true) or black(false)</param>
        /// <returns></returns>
        public static Move GetMove(int[,] board, int depth, bool whitePlayer)
        {
            MiniMaxTreeNode rootNode = new MiniMaxTreeNode(board, whitePlayer);

            Tuple<int, Move> moveSelected = Alphabeta2(rootNode, depth, 1, int.MinValue);

            return moveSelected.Item2;
        }

        /// <summary>
        /// The main IA algorithm to get the best move to play. Inspired by the code in the AI course HE-Arc 2018-2019.
        /// </summary>
        /// <param name="node">The current node to search for</param>
        /// <param name="depth">The max depth to search in</param>
        /// <param name="minOrMax">Either -1 or 1 so we use the same code for both</param>
        /// <param name="parentValue">The best value found at the moment</param>
        /// <returns>2 values containing the move chosen and the value that it generates</returns>
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
