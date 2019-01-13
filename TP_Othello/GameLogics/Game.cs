﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Threading;
using System.Timers;
using System.ComponentModel;

namespace TP_Othello.GameLogics
{
    /// <summary>
    /// This class is the main engine for the Othello game. It synchronizes the board display and the logic behind.
    /// <see cref="Board"/>
    /// <see cref="BoardView"/>
    /// </summary>
    class Game : IPlayable.IPlayable, INotifyPropertyChanged
    {
        BoardView boardView;
        Board logicalBoard;

        private Size BOARD_DIMENSIONS = new System.Drawing.Size(9, 7);

        // Those are the event handlers passed to the cells so the event fired for them is handled here
        private event MouseButtonEventHandler CellClickedEvent;
        private event MouseEventHandler CellHoverEvent;
        public event PropertyChangedEventHandler PropertyChanged;

        private Stopwatch[] playersTimer;
        private Timer refreshTimer;



        // Current player related
        private bool whitePlayerTurn;
        private List<Move> currentPossibleMoves;

        #region BoundData
        private string timeWhite = "00:00:00", timeBlack = "00:00:00";

        private int scoreWhite, scoreBlack;

        public string TimeWhite
        {
            get { return timeWhite; }
            private set {
                timeWhite = value;
                NotifyPropertyChanged("TimeWhite");
            }

        }
        public string TimeBlack
        {
            get { return timeBlack; }
            private set {
                timeBlack = value;
                NotifyPropertyChanged("TimeBlack");
            }
        }

        public int ScoreWhite
        {
            get { return scoreWhite; }
            private set {
                scoreWhite = value;
                NotifyPropertyChanged("ScoreWhite");
            }
        }

        public int ScoreBlack
        {
            get { return scoreBlack; }
            private set {
                scoreBlack = value;
                NotifyPropertyChanged("ScoreBlack");
            }
        }
        #endregion

        public Game(BoardView boardView)
        {
            CellClickedEvent = new MouseButtonEventHandler(CellClickedEventHandler);
            CellHoverEvent = new MouseEventHandler(CellHoverEventHandler);

            this.boardView = boardView;
            this.logicalBoard = new Board(BOARD_DIMENSIONS.Width, BOARD_DIMENSIONS.Height);

            boardView.InitBoardView(BOARD_DIMENSIONS, CellClickedEvent, CellHoverEvent);


            refreshTimer = new Timer(1000);
            refreshTimer.Elapsed += OnTimerEvent;
            refreshTimer.AutoReset = true;

            playersTimer = new Stopwatch[2];
            playersTimer[0] = new Stopwatch();
            playersTimer[1] = new Stopwatch();

            //We get the center
            int centerX = Convert.ToInt32(Math.Floor(BOARD_DIMENSIONS.Width / 2.0) - 1);
            int centerY = Convert.ToInt32(Math.Floor(BOARD_DIMENSIONS.Height / 2.0) - 1);

            //Initialize the center with the pawns
            Move initMove1 = new Move(new Point(centerX, centerY), true);
            List<Point> points1 = new List<Point>()
            {
                new Point(centerX + 1, centerY + 1),
                new Point(0,0)
            };

            initMove1.AddChecksToInvert(points1);

            Move initMove2 = new Move(new Point(centerX + 1, centerY), false);
            List<Point> points2 = new List<Point>()
            {
                new Point(centerX, centerY + 1),
                new Point(1,0),
                new Point(2,0)
            };

            initMove2.AddChecksToInvert(points2);

            PlayMove(initMove1);
            PlayMove(initMove2);

        }

        public void StartGame()
        {
            whitePlayerTurn = true;
            currentPossibleMoves = logicalBoard.GetPossibleMoves(whitePlayerTurn);


            playersTimer[whitePlayerTurn ? 1 : 0].Start();
            refreshTimer.Start();
        }

        /// <summary>
        /// This function changes the variables to change turn
        /// </summary>
        private void ChangeTurn()
        {
            playersTimer[whitePlayerTurn ? 1 : 0].Stop();
            whitePlayerTurn = !whitePlayerTurn;

            var nextPossibleMoves = this.logicalBoard.GetPossibleMoves(whitePlayerTurn);


            if (!nextPossibleMoves.Any())
            {
                // if the possible moves for the previous player and the current one are empty nobody can play anymore, it's the end of the game
                if (!currentPossibleMoves.Any())
                {
                    // end game
                    Debug.Write("Game ended");
                }
                // if only the current ones are empty it's to the other play again
                else
                {
                    currentPossibleMoves = nextPossibleMoves;
                    ChangeTurn();
                }
            }
            else
            {
                playersTimer[whitePlayerTurn ? 1 : 0].Start();
                currentPossibleMoves = nextPossibleMoves;
            }
        }


        /// <summary>
        /// This is the handler function of the click on a cell. It is fired by the BoardCell objects
        /// </summary>
        /// <see cref="BoardCell"/>
        /// <param name="Sender">Sender of the event, should be a BoardCell</param>
        /// <param name="e">Event arguments</param>
        private void CellClickedEventHandler(object Sender, MouseEventArgs e)
        {
            // if the sender object is a BoardCell we cast it and null checks (equivalent to ...  != null)
            if (Sender is BoardCell senderCell)
            {
                Move targetMove = currentPossibleMoves.Where(move => move.position.Equals(senderCell.BoardPosition)).FirstOrDefault();
                if(targetMove != null)
                {
                    PlayMove(targetMove);
                    ChangeTurn();
                }
            }
        }

        private void PlayMove(Move move)
        {
            List<Point> cellsPosInvert = move.GetChecksToInvert();
            cellsPosInvert.Add(move.position);

            foreach(Point point in cellsPosInvert)
            {
                boardView.SetPawnCell(point, move.whitePlayer);
                logicalBoard.ApplyMove(move);
            }
        }

        /// <summary>
        /// This is the handler function of the hover on a cell. It is fired by the BoardCell objects
        /// </summary>
        /// <see cref="BoardCell"/>
        /// <param name="Sender">Sender of the event, should be a BoardCell</param>
        /// <param name="e">Event arguments</param>
        private void CellHoverEventHandler(object Sender, MouseEventArgs e)
        {
            // if the sender object is a BoardCell we cast it and null checks (equivalent to ...  != null)
            if (Sender is BoardCell senderCell)
            {
                if(currentPossibleMoves.Any(move => move.position.Equals(senderCell.BoardPosition)))
                {
                    senderCell.Highlight();
                }
                /*var coords = CoordinatesOf(boardCells, senderCell);
                if (coords.Item1 != -1 && currentPossibleMoves.Any(x => x.position.X == coords.Item1 && x.position.Y == coords.Item2))
                {
                    Debug.Write("Cell is playable");
                    senderCell.Highlight();
                }*/
                //Debug.WriteLine("Cell hovered");
                //Debug.WriteLine(senderCell.CellValue);
            }
        }

        /// <summary>
        /// This function is raised by the players timer
        /// </summary>
        /// <param name="src"></param>
        /// <param name="e"></param>
        private void OnTimerEvent(object sender, ElapsedEventArgs e)
        {
            // TODO - Link text with timer
            if (sender is Timer timer)
            {
                if (whitePlayerTurn)
                {
                    TimeWhite = playersTimer[1].Elapsed.ToString(@"hh\:mm\:ss");
                }
                else
                {
                    TimeBlack = playersTimer[0].Elapsed.ToString(@"hh\:mm\:ss");
                }
            }
        }

        /// <summary>
        /// 
        /// https://www.c-sharpcorner.com/UploadFile/020f8f/data-binding-with-inotifypropertychanged-interface/
        /// </summary>
        /// <param name="propertyName"></param>
        protected void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }


        #region IPlayable
        public string GetName()
        {
            throw new NotImplementedException();
        }

        public bool IsPlayable(int column, int line, bool isWhite)
        {
            throw new NotImplementedException();
        }

        public bool PlayMove(int column, int line, bool isWhite)
        {
            throw new NotImplementedException();
        }

        public Tuple<int, int> GetNextMove(int[,] game, int level, bool whiteTurn)
        {
            throw new NotImplementedException();
        }

        public int[,] GetBoard()
        {
            throw new NotImplementedException();
        }

        public int GetWhiteScore()
        {
            throw new NotImplementedException();
        }

        public int GetBlackScore()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
