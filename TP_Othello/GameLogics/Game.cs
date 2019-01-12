using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Threading;

namespace TP_Othello.GameLogics
{
    /// <summary>
    /// This class is the main engine for the Othello game. It synchronizes the board display and the logic behind.
    /// <see cref="Board"/>
    /// <see cref="BoardView"/>
    /// </summary>
    class Game : IPlayable.IPlayable
    {
        BoardView boardView;
        Board logicalBoard;

        private Size BOARD_DIMENSIONS = new System.Drawing.Size(9, 7);

        // Those are the event handlers passed to the cells so the event fired for them is handled here
        private event MouseButtonEventHandler CellClickedEvent;
        private event MouseEventHandler CellHoverEvent;


        private Stopwatch[] playersTimer;
        private DispatcherTimer refreshTimer;

        // Current player related
        private bool whitePlayerTurn;
        private List<Move> currentPossibleMoves;

        #region BoundData
        private string p1Time = "00:00:00", p2Time = "00:00:00";

        private int scoreP1, scoreP2;

        public string P1Time
        {
            get { return p1Time; }
        }

        public string P2Time
        {
            get { return p2Time; }
        }

        public int ScoreP1
        {
            get { return scoreP1; }
        }

        public int ScoreP2
        {
            get { return scoreP2; }
        }
        #endregion

        public Game(BoardView boardView)
        {
            CellClickedEvent = new MouseButtonEventHandler(CellClickedEventHandler);
            CellHoverEvent = new MouseEventHandler(CellHoverEventHandler);

            this.boardView = boardView;
            this.logicalBoard = new Board(BOARD_DIMENSIONS.Width, BOARD_DIMENSIONS.Height);

            boardView.InitBoardView(BOARD_DIMENSIONS, CellClickedEvent, CellHoverEvent);


            refreshTimer = new DispatcherTimer();
            refreshTimer.Interval = new TimeSpan(0, 0, 0, 1);
            refreshTimer.Tick += new EventHandler(OnTimerEvent);

            playersTimer = new Stopwatch[2];
            playersTimer[0] = new Stopwatch();
            playersTimer[1] = new Stopwatch();
        }

        public void StartGame()
        {
            whitePlayerTurn = true;
            currentPossibleMoves = logicalBoard.GetPossibleMoves(whitePlayerTurn);
            // start the timers and stuff
        }


        /// <summary>
        /// This function changes the variables to change turn
        /// </summary>
        private void ChangeTurn()
        {
            playersTimer[whitePlayerTurn ? 1 : 0].Stop();
            whitePlayerTurn = !whitePlayerTurn;

            var nextPossibleMoves = this.logicalBoard.GetPossibleMoves(whitePlayerTurn);

            // if the possible moves for the previous player and the current one are empty nobody can play anymore, it's the end of the game
            if (!nextPossibleMoves.Any() && !currentPossibleMoves.Any())
            {
                // end game
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
        private void OnTimerEvent(object sender, EventArgs e)
        {
            // TODO - Link text with timer
            if (sender is DispatcherTimer timer)
            {
                if (whitePlayerTurn)
                {
                    string a = playersTimer[1].Elapsed.ToString("HH:mm:ss");

                }
                else
                {
                    string a = playersTimer[0].Elapsed.ToString("HH:mm:ss");
                }
            }
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
