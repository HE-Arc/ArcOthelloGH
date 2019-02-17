using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;

namespace IA_Grava_Herbelin.GameLogics.AI
{
    /* board stored as [column, row]
     *  -----------------> x
     * | -1 -1 -1 -1 -1 -1       
     * | -1 -1 -1 -1 -1 -1
     * | -1 -1  1  0 -1 -1
     * | -1 -1  0  1 -1 -1 
     * | -1 -1 -1 -1 -1 -1
     * | -1 -1 -1 -1 -1 -1
     * v 
     * y
     */

    /// <summary>
    /// This class is a specialized tree for the othello game. Each node represents a board state. It can evaluate itself and create other nodes by applying moves.
    /// </summary>
    public class MiniMaxTreeNode : TreeNode<int[,]>
    {
        const int CORNER_BONUS = 100;
        bool whitePlayer;

        public MiniMaxTreeNode(int[,] data, bool whitePlayer) : base(data)
        {
            this.whitePlayer = whitePlayer;
        }

        /// <summary>
        /// Apply and returns a new board state with the given move.
        /// </summary>
        /// <param name="move">Move object to apply</param>
        /// <returns>A new node with the board calculated</returns>
        public MiniMaxTreeNode ApplyMove(Move move)
        {
            int[,] dataCopy = (int[,])Data.Clone();
            LogicalB.ApplyMove(dataCopy, move);
            return new MiniMaxTreeNode(dataCopy, !whitePlayer);
        }

        /// <summary>
        /// Get the possible moves for this board
        /// </summary>
        /// <returns>A list of move objects returned</returns>
        public List<Move> GetMoves()
        {
            return LogicalB.GetPossibleMoves(Data, whitePlayer, new Size(Data.GetLength(0), Data.GetLength(1)));
        }

        /// <summary>
        /// Evaluate function to calculate the score of the current play. 
        /// The evaluate function is currently pretty basic, as it only gives bonus when you play in a corner
        /// </summary>
        /// <returns>And int number representing a score for this specific game state</returns>
        public int Evaluate()
        {
            int score = 0;
            int playerVal = whitePlayer ? 1 : 0;

            for (int column = 0; column < Data.GetLength(0); column++)
            {
                for (int row = 0; row < Data.GetLength(1); row++)
                {
                    // if we land on a corner
                    if ((column == 0 || column == Data.GetLength(0) - 1) && (row == 0 || row == Data.GetLength(1) - 1))
                    {
                        // maybe do fancy math or boolean stuff to remove the if
                        if (Data[column, row] == playerVal)
                            score += CORNER_BONUS;
                        else if(Data[column, row] != -1)
                            score -= CORNER_BONUS;
                    }

                    if (Data[column, row] == playerVal)
                        score += 1;
                }
            }

            return score;
        }
    }
}

