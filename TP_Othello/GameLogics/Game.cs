using System;
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
using System.Runtime.Serialization;

namespace TP_Othello.GameLogics
{
    [Serializable]
    /// <summary>
    /// This class is the main engine for the Othello game. It synchronizes the board display and the logic behind.
    /// <see cref="Board"/>
    /// <see cref="TP_Othello.BoardView"/>
    /// </summary>
    class Game : IPlayable.IPlayable, INotifyPropertyChanged, ISerializable
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

        

        private List<Move> playedMovesStack;


        // Current player related
        private bool whitePlayerTurn;
        private List<Move> currentPossibleMoves;

        #region BoundData
        private string timeWhite = "00:00:00", timeBlack = "00:00:00";

        private int scoreWhite = 0, scoreBlack = 0;

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

        /*public string P1Score
        {
            get { return ScoreWhite.ToString(); }
        }

        public string P2Score
        {
            get { return ScoreBlack.ToString(); }
        }*/

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

        public BoardView BoardView { get { return boardView; } set { boardView = value; RefreshBoardView(); } }

        private void RefreshBoardView()
        {
            int[,] boardArray = logicalBoard.BoardArray;
            for(int i = 0; i < boardArray.GetLength(0); i++)
            {
                for(int j = 0; j < boardArray.GetLength(1); j++)
                {
                    if (boardArray[i, j] == -1)
                        boardView.UnsetPawnCell(new Point(i, j));
                    else
                        boardView.SetPawnCell(new Point(i, j), boardArray[i,j] == 1 ? true : false);
                }
            }
        }
        #endregion

        /// <summary>
        /// This constructor is the base to init different values called when the program is first started and when it loads a saved game
        /// </summary>
        private Game()
        {
            this.logicalBoard = new Board(BOARD_DIMENSIONS.Width, BOARD_DIMENSIONS.Height);
            playedMovesStack = new List<Move>();

            refreshTimer = new Timer(1000);
            refreshTimer.Elapsed += OnTimerEvent;
            refreshTimer.AutoReset = true;

            playersTimer = new Stopwatch[2];
            playersTimer[0] = new Stopwatch();
            playersTimer[1] = new Stopwatch();
        }

        public Game(BoardView boardView) : this()
        {
            CellClickedEvent = new MouseButtonEventHandler(CellClickedEventHandler);
            CellHoverEvent = new MouseEventHandler(CellHoverEventHandler);

            this.boardView = boardView;

            boardView.InitBoardView(BOARD_DIMENSIONS, CellClickedEvent, CellHoverEvent);
            InitBoard();
        }

        /// <summary>
        /// This constructor is called when the board is unserialized
        /// </summary>
        /// <param name="info">The list of parameters containing the serialized values</param>
        /// <param name="context">The stream where the data come from</param>
        private Game(SerializationInfo info, StreamingContext context) : this()
        {
            TimeSpan timeSpanWhite = new TimeSpan().Add((TimeSpan)info.GetValue("WhiteTime", typeof(TimeSpan)));
            Stopwatch whiteStopwatch = GetPlayerStopwatch(true);
            whiteStopwatch.Reset();
            whiteStopwatch.Elapsed.Add(timeSpanWhite);

            TimeSpan timeSpanBlack = new TimeSpan().Add((TimeSpan)info.GetValue("BlackTime", typeof(TimeSpan)));
            Stopwatch blackStopwatch = GetPlayerStopwatch(false);
            blackStopwatch.Reset();
            blackStopwatch.Elapsed.Add(timeSpanBlack);

            whitePlayerTurn = info.GetBoolean("Turn");
            this.logicalBoard = (Board)info.GetValue("Board", typeof(Board));
        }

        public void StartGame()
        {
            currentPossibleMoves = logicalBoard.GetPossibleMoves(whitePlayerTurn);

            GetPlayerStopwatch(whitePlayerTurn).Start();
            refreshTimer.Start();
        }

        private void InitBoard()
        {
            //We get the center
            int centerX = Convert.ToInt32(Math.Floor(BOARD_DIMENSIONS.Width / 2.0) - 1);
            int centerY = Convert.ToInt32(Math.Floor(BOARD_DIMENSIONS.Height / 2.0) - 1);

            //Initialize the center with the pawns
            List<Move> initMoves = new List<Move>()
            {
                //new Move(new Point(0, 0), true),
                //new Move(new Point(1, 0), true),
                //new Move(new Point(2, 0), false),
                new Move(new Point(centerX, centerY), true),
                new Move(new Point(centerX + 1, centerY + 1), true),
                new Move(new Point(centerX + 1, centerY), false),
                new Move(new Point(centerX, centerY + 1), false)
            };

            foreach (Move m in initMoves)
            {
                PlayMove(m);
            }
        }

        /// <summary>
        /// This function changes the variables to change turn
        /// </summary>
        private void ChangeTurn()
        {
            GetPlayerStopwatch(whitePlayerTurn).Stop();
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
                GetPlayerStopwatch(whitePlayerTurn).Start();
                currentPossibleMoves = nextPossibleMoves;
            }
        }

        private Stopwatch GetPlayerStopwatch(bool whitePlayer)
        {
            return playersTimer[whitePlayer ? 0 : 1];
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
                    playedMovesStack.Add(targetMove);
                    PlayMove(targetMove);
                    ChangeTurn();
                }
            }
        }

        private void PlayMove(Move move)
        {
            int score = 0;
            List<Point> cellsPosInvert = move.GetChecksToInvert();
            logicalBoard.ApplyMove(move);

            boardView.SetPawnCell(move.position, move.whitePlayer);
            score++;

            foreach(Point point in cellsPosInvert)
            {
                boardView.SetPawnCell(point, move.whitePlayer);
                score++;
            }

            if(move.whitePlayer)
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
                    senderCell.Highlight(whitePlayerTurn);
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
                    TimeWhite = GetPlayerStopwatch(whitePlayerTurn).Elapsed.ToString(@"hh\:mm\:ss");
                }
                else
                {
                    TimeBlack = GetPlayerStopwatch(whitePlayerTurn).Elapsed.ToString(@"hh\:mm\:ss");
                }
            }
        }

        public void UndoLastMove()
        {
            if (playedMovesStack.Count == 0)
                return;

            int score = 0;
            Move lastMove = playedMovesStack[playedMovesStack.Count-1];
            playedMovesStack.RemoveAt(playedMovesStack.Count-1);

            List<Point> cellsPosInvert = logicalBoard.UndoMove(lastMove);
            boardView.UnsetPawnCell(lastMove.position);
            score++;

            foreach (Point point in cellsPosInvert)
            {
                boardView.SetPawnCell(point, !lastMove.whitePlayer);
                score++;
            }

            if (lastMove.whitePlayer)
            {
                ScoreWhite -= score;
                ScoreBlack += score - 1;
            }
            else
            {
                ScoreBlack -= score;
                ScoreWhite += score - 1;
            }

            ChangeTurn();
        }

        /// <summary>
        /// 
        /// https://www.c-sharpcorner.com/UploadFile/020f8f/data-binding-with-inotifypropertychanged-interface/
        /// </summary>
        /// <param name="propertyName"></param>
        protected void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Turn", whitePlayerTurn);
            info.AddValue("WhiteTime", GetPlayerStopwatch(true).Elapsed);
            info.AddValue("BlackTime", GetPlayerStopwatch(false).Elapsed);
            info.AddValue("Board", this.logicalBoard, typeof(Board));
        }

    }
}
