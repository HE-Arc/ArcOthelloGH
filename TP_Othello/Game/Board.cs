using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TP_Othello.Game
{
    class Board
    {
        struct Size
        {
            public int width { get; }
            public int height { get; }
       
            public Size(int width, int height)
            {
                this.width = width;
                this.height = height;
            }
        }

        private int[,] board;
        private Size sizes;

        public Board(int width, int height)
        {
            board = new int[width, height];
            sizes = new Size(width, height);

            for(int y = 0; y < sizes.height; y++)
            {
                for(int x = 0; x < sizes.width; x++)
                {
                    board[x, y] = -1;
                }
            }

            int centerX = Convert.ToInt32(Math.Ceiling(width / 2.0) - 1);
            int centerY = Convert.ToInt32(Math.Ceiling(height / 2.0) - 1);

            board[centerX, centerY] = 1;
            board[centerX+1, centerY] = 0;
            board[centerX, centerY+1] = 0;
            board[centerX+1, centerY+1] = 1;
        }

        public void GetPossibleMoves(int playerId)
        {
            List<List<int>> horizontalLines = InitializeList(sizes.height);
            List<List<int>> verticalLines = InitializeList(sizes.width);
            List<List<int>> leftDownDiagonalLines = InitializeList(sizes.height + sizes.width - 5);//-1-2-2: -1 for the middle line and -2 two times for the corners
            List<List<int>> rightDownDiagonalLines = InitializeList(sizes.height + sizes.width - 5);//-1-2-2: -1 for the middle line and -2 two times for the corners

            //Populate horizontal and vertical lists
            for (int y = 0; y < sizes.height; y++)
            {
                for (int x = 0; x < sizes.width; x++)
                {
                    horizontalLines[y].Add(board[x, y]);
                    verticalLines[x].Add(board[x, y]);
                    leftDownDiagonalLines[x + y].Add(board[x, y]);
                }

                for (int x = sizes.width-1; x >= 0; x--)
                {
                    leftDownDiagonalLines[sizes.width-1-x + y].Add(board[x, y]);
                }
            }

            for (int y = 0; y < sizes.height; y++)
            {
                for (int x = 0; x < sizes.width; x++)
                {
                    
                }
            }
        }

        private void CheckLineForMove(List<int> line, bool playerId)
        {
            List<int> movesPosition = new List<int>();

            int playerCheck = playerId ? 1 : 0;
            int opponentCheck = playerId ? 0 : 1;

            //We run through the line
            for (int i = 0; i < line.Count; i++)
            {
                int boardCheck = line[i];

                //If there's a player's check
                if (boardCheck == playerCheck)
                {
                    int spree = i;

                    //We continue forward to check for a possible move
                    do
                    {
                        spree++;
                    }
                    while (spree < line.Count && line[spree] == opponentCheck);

                    //If we're still inside the board, and there's at least an opponent's check in between, and we didn't stop on a player's check
                    if (spree != line.Count && line[spree--] == opponentCheck && line[spree] == -1)
                    {
                        movesPosition.Add(spree);
                    }

                    //We go backward to check for a possible move
                    spree = i;

                    do
                    {
                        spree--;
                    }
                    while (spree == -1 || line[spree] == opponentCheck);

                    //If we're still inside the board, and there's at least an opponent's check in between, and we didn't stop on a player's check
                    if (spree != -1 && line[spree++] == opponentCheck)
                    {
                        movesPosition.Add(spree);
                    }

                }
            }
        }

        private List<List<int>> InitializeList(int size)
        {
            List<List<int>> list = new List<List<int>>();

            for (int i = 0; i < size; i++)
            {
                list.Add(new List<int>());
            }

            return list;
        }

        //private void GetMoveByLine
    }
}
