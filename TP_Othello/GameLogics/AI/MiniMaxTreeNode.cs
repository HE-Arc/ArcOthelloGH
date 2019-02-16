using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;

namespace TP_Othello.GameLogics.AI
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
    public class MiniMaxTreeNode : TreeNode<int[,]>
    {
        const int CORNER_BONUS = 30;
        bool whitePlayer;

        public MiniMaxTreeNode(int[,] data, bool whitePlayer) : base(data)
        {
            this.whitePlayer = whitePlayer;
        }

        public MiniMaxTreeNode ApplyMove(Move move)
        {
            int[,] dataCopy = (int[,])Data.Clone();
            return new MiniMaxTreeNode(LogicalBoard.ApplyMove(dataCopy, move), !whitePlayer);
        }

        public List<Move> GetMoves()
        {
            return LogicalBoard.GetPossibleMoves(Data, whitePlayer, new Size(Data.GetLength(0), Data.GetLength(1)));
        }

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
                        else
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

