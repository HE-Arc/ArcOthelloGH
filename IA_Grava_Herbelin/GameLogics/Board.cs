using IA_Grava_Herbelin.GameLogics;
using IA_Grava_Herbelin.GameLogics.AI;
using IPlayable;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace IA_Grava_Herbelin
{
    /// <summary>
    /// This class is the main engine for the Othello game. It synchronizes the board display and the logic behind.
    /// <see cref="LogicalB"/>
    /// <see cref="TP_Othello.BoardView"/>
    /// </summary>
    class Board : IPlayable.IPlayable
    {
        private Size BOARD_DIMENSIONS = new System.Drawing.Size(9, 7);

        // Current player related
        private LogicalB logicalBoard;
        private List<Move> currentPossibleMoves;

        #region BoundData
        private string timeWhite = "00:00:00", timeBlack = "00:00:00";

        private int scoreWhite = 0, scoreBlack = 0;

        /// <summary>
        /// This is a helper to return the white player's play time. Bound to the UI
        /// </summary>
        public string TimeWhite
        {
            get { return timeWhite; }
            private set
            {
                timeWhite = value;
            }

        }

        /// <summary>
        /// This is a helper to return the black player's play time. Bound to the UI
        /// </summary>
        public string TimeBlack
        {
            get { return timeBlack; }
            private set
            {
                timeBlack = value;
            }
        }

        /// <summary>
        /// This is the white player's score bound to the UI
        /// </summary>
        public int ScoreWhite
        {
            get { return scoreWhite; }
            private set
            {
                scoreWhite = value;
            }
        }

        /// <summary>
        /// This is the black player's score bound to the UI
        /// </summary>
        public int ScoreBlack
        {
            get { return scoreBlack; }
            private set
            {
                scoreBlack = value;
            }
        }
        #endregion


        /// <summary>
        /// This constructor is the base to init different values called when the program is first started and when it loads a saved game
        /// It should be private since it's only used as a helper but the tournament needs a default constructor
        /// </summary>
        public Board()
        {
            currentPossibleMoves = new List<Move>();
            logicalBoard = new LogicalB(BOARD_DIMENSIONS.Width, BOARD_DIMENSIONS.Height);
        }


        /// <summary>
        /// This method applies a move on the logical part of the board and applies its consequences to the UI
        /// </summary>
        /// <see cref="Move"/>
        /// <param name="move">The move object containing the position played at and the pawns to modify</param>
        private void PlayMove(Move move)
        {
            int score = 0;
            List<Point> cellsPosInvert = move.GetChecksToInvert();
            logicalBoard.ApplyMove(move);

            score++;

            foreach (Point point in cellsPosInvert)
            {
                score++;
            }

            if (move.whitePlayer)
            {
                ScoreWhite += score;
                ScoreBlack -= score - 1;
            }
            else
            {
                ScoreBlack += score;
                ScoreWhite -= score - 1;
            }
        }

        #region IPlayable
        public string GetName()
        {
            return "Grava_Herbelin";
        }

        public bool IsPlayable(int column, int line, bool isWhite)
        {
            Point movePos = new Point(column, line);
            currentPossibleMoves = LogicalB.GetPossibleMoves(logicalBoard.BoardArray, isWhite, BOARD_DIMENSIONS);
            return currentPossibleMoves.Any(move => move.position.Equals(movePos));
        }

        public bool PlayMove(int column, int line, bool isWhite)
        {
            Point movePos = new Point(column, line);
            currentPossibleMoves = LogicalB.GetPossibleMoves(logicalBoard.BoardArray, isWhite, BOARD_DIMENSIONS);
            Move targetMove = currentPossibleMoves.Where(move => move.position.Equals(movePos)).FirstOrDefault();
            if (targetMove != null)
            {
                PlayMove(targetMove);
                return true;
            }

            return false;
        }

        public Tuple<int, int> GetNextMove(int[,] game, int level, bool whiteTurn)
        {
            logicalBoard.BoardArray = game;
            try
            {
                currentPossibleMoves = LogicalB.GetPossibleMoves(game, whiteTurn, BOARD_DIMENSIONS);
                Move move = OthelloMiniMax.GetMove(game, level, whiteTurn);
                if (move == null)
                    move = new Move(new Point(-1, -1), whiteTurn);
                return new Tuple<int, int>(move.position.X, move.position.Y);
            }
            catch
            {
                return new Tuple<int, int>(-1, -1);
            }
        }

        public int[,] GetBoard()
        {
            return logicalBoard.BoardArray;
        }

        public int GetWhiteScore()
        {
            return ScoreWhite;
        }

        public int GetBlackScore()
        {
            return ScoreBlack;
        }
        #endregion


    }
}
