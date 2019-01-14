﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Diagnostics;

namespace TP_Othello.GameLogics
{
    class Board
    {
        private int[,] board;
        private Size boardSize;

        public Board(int width, int height)
        {
            board = new int[width, height];
            boardSize = new Size(width, height);

            InitBoard();
        }

        /// <summary>
        /// Init the board with default value and place the starting pawns
        /// </summary>
        public void InitBoard()
        {
            //Fill the board with "emptiness"
            for (int y = 0; y < boardSize.Height; y++)
            {
                for (int x = 0; x < boardSize.Width; x++)
                {
                    board[x, y] = -1;
                }
            }

            /*//We get the center
            int centerX = Convert.ToInt32(Math.Floor(boardSize.Width / 2.0) - 1);
            int centerY = Convert.ToInt32(Math.Floor(boardSize.Height / 2.0) - 1);

            //Initialize the center with the pawns
            board[centerX, centerY] = 1;
            board[centerX + 1, centerY] = 0;
            board[centerX, centerY + 1] = 0;
            board[centerX + 1, centerY + 1] = 1;

            board[0, 0] = 1;
            board[1, 0] = 0;
            board[2, 0] = 0;*/
        }

        /// <summary>
        /// Get the positions where the player can place a pawn
        /// </summary>
        /// <param name="playerId">The id of the current player</param>
        /// <returns>A list of moves</returns>
        public List<Move> GetPossibleMoves(bool playerId)
        {
            //We use a dictonary so we can easily add pawns to an already existing move
            Dictionary<Point, Move> moves = new Dictionary<Point, Move>();
            
            List<List<Point>> horizontalLines = InitializeList(boardSize.Height);
            List<List<Point>> verticalLines = InitializeList(boardSize.Width);
            List<List<Point>> leftDownDiagonalLines = InitializeList(boardSize.Height + boardSize.Width - 1);//-1-2-2: -1 for the middle line and -2 two times for the corners
            List<List<Point>> rightDownDiagonalLines = InitializeList(boardSize.Height + boardSize.Width - 1);//-1-2-2: -1 for the middle line and -2 two times for the corners

            //Populate lists
            for (int y = 0; y < boardSize.Height; y++)
            {
                for (int x = 0; x < boardSize.Width; x++)
                {
                    horizontalLines[y].Add(new Point(x, y));
                    verticalLines[x].Add(new Point(x, y));
                    leftDownDiagonalLines[x + y].Add(new Point(x, y));
                }

                for (int x = boardSize.Width-1; x >= 0; x--)
                {
                    rightDownDiagonalLines[boardSize.Width-1-x + y].Add(new Point(x, y));
                }
            }

            List<List<Point>> mergedLists = new List<List<Point>>();

            mergedLists.AddRange(horizontalLines);
            mergedLists.AddRange(verticalLines);
            mergedLists.AddRange(leftDownDiagonalLines);
            mergedLists.AddRange(rightDownDiagonalLines);

            List<Move> foundMoves;
            //horizontal possibilities
            for (int i = 0; i < mergedLists.Count; i++)
            {
                foundMoves = CheckLineForMove(mergedLists[i], playerId);

                for(int j = 0; j < foundMoves.Count; j++)
                {     
                    //Get the move
                    Move newMove = foundMoves[j];

                    //Test if the move already exists
                    if (!moves.TryGetValue(newMove.position, out Move existingMove))
                    {
                        //If the move doesn't exist, we add it to the dictionary
                        moves.Add(newMove.position, newMove);
                    }
                    else
                    {
                        //Else we create it then add it to the dictionary
                        existingMove.AddChecksToInvert(newMove.GetChecksToInvert());
                    }
                }
            }
            return moves.Select(x => x.Value).ToList();
        }


        public void drawBo()
        {
            Debug.Write("\n-------------------------\n");
            for (int i = 0; i < board.GetLength(1); i++)
            {
                for (int j = 0; j < board.GetLength(0); j++)
                {
                    Debug.Write(board[j, i] + "\t");
                }
                Debug.Write("\n");
            }

            Debug.Write("-------------------------\n");

        }

        /// <summary>
        /// Here we go through a list of point and check if there is a possible move
        /// </summary>
        /// <param name="line">A row/column/diagonal of the board containing the corresponding points</param>
        /// <param name="playerId">The current players id</param>
        /// <returns>A list of the moves found</returns>
        private List<Move> CheckLineForMove(List<Point> line, bool playerId)
        {
            //The result list that will contain all the moves we found
            List<Move> moves = new List<Move>();

            int playerCheck = playerId ? 1 : 0;
            int opponentCheck = playerId ? 0 : 1;

            //We run through the line
            for (int i = 0; i < line.Count; i++)
            {
                int boardCheck = board[line[i].X, line[i].Y];

                //If we are on a pawn of the current player, we can start to check if there's a possible move
                if (boardCheck == playerCheck)
                {
                    //Spree is used to walk through the list without losing the current user's pawn position
                    int spree = i-1;
                    //If we found a move, we will add the opponent's pawns to invert to this list
                    List<Point> pawnsToInvert = new List<Point>();

                    //We go backward to check for a possible move. While we are inbounds and the case contains an opponent's check, we add the case to the list
                    while (spree > -1 && board[line[spree].X, line[spree].Y] == opponentCheck)
                    {
                        pawnsToInvert.Add(line[spree]);
                        spree--;
                    }

                    //If we're still inside the board, and there's at least an opponent's check in between, and we didn't stop on a player's check
                    if (spree > -1 && board[line[spree+1].X, line[spree+1].Y] == opponentCheck && board[line[spree].X, line[spree].Y] == -1)
                    {
                        Move m = new Move(line[spree], playerId);
                        m.AddChecksToInvert(pawnsToInvert);
                        moves.Add(m);
                    }

                    spree = i + 1;
                    pawnsToInvert.Clear();
                    //We go forward to check for a possible move. 
                    while (spree < line.Count && board[line[spree].X, line[spree].Y] == opponentCheck)
                    {
                        pawnsToInvert.Add(line[spree]);
                        spree++;
                    }

                    //If we're still inside the board, and there's at least an opponent's check in between, and we didn't stop on a player's check
                    if (spree != line.Count && board[line[spree-1].X, line[spree-1].Y] == opponentCheck && board[line[spree].X, line[spree].Y] == -1)
                    {
                        Move m = new Move(line[spree], playerId);
                        m.AddChecksToInvert(pawnsToInvert);
                        moves.Add(m);
                    }

                    //We don't have to continue from the players pawn position, because we already evaluated the pawns between it and spree position
                    i = spree;
                }
            }

            return moves;
        }


        /// <summary>
        /// This is a helper function that initialize a list of lists and returns it
        /// </summary>
        /// <param name="size">The number of lists wanted</param>
        /// <returns></returns>
        private List<List<Point>> InitializeList(int size)
        {
            List<List<Point>> list = new List<List<Point>>();

            for (int i = 0; i < size; i++)
            {
                list.Add(new List<Point>());
            }

            return list;
        }


        public List<Point> ApplyMove(Move move)
        {
            Point position = move.position;
            board[position.X, position.Y] = move.whitePlayer ? 1 : 0;

            List<Point> pawnToInvert = move.GetChecksToInvert();

            for(int i = 0; i < pawnToInvert.Count; i++)
            {
                board[pawnToInvert[i].X, pawnToInvert[i].Y] = move.whitePlayer ? 1 : 0;
            }

            return pawnToInvert;
        }
    }
}