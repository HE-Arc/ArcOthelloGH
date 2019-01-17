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
    class GameWithBoardInItsName : IPlayable.IPlayable, INotifyPropertyChanged, ISerializable
    {
        BoardView boardView;
        Board logicalBoard;

        private Size BOARD_DIMENSIONS = new System.Drawing.Size(9, 7);

        // Those are the event handlers passed to the cells so the event fired for them is handled here
        private event MouseButtonEventHandler CellClickedEvent;
        private event MouseEventHandler CellHoverEvent;
        public event PropertyChangedEventHandler PropertyChanged;

        private Stopwatch[] playersTimer;
        private DispatcherTimer refreshTimer;

        private List<Move> playedMovesStack;


        // Current player related
        private bool whitePlayerTurn;
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
            private set {
                timeWhite = value;
                NotifyPropertyChanged("TimeWhite");
            }

        }

        /// <summary>
        /// This is a helper to return the black player's play time. Bound to the UI
        /// </summary>
        public string TimeBlack
        {
            get { return timeBlack; }
            private set {
                timeBlack = value;
                NotifyPropertyChanged("TimeBlack");
            }
        }

        /// <summary>
        /// This is the white player's score bound to the UI
        /// </summary>
        public int ScoreWhite
        {
            get { return scoreWhite; }
            private set {
                scoreWhite = value;
                NotifyPropertyChanged("ScoreWhite");
            }
        }

        /// <summary>
        /// This is the black player's score bound to the UI
        /// </summary>
        public int ScoreBlack
        {
            get { return scoreBlack; }
            private set {
                scoreBlack = value;
                NotifyPropertyChanged("ScoreBlack");
            }
        }

   
        /// <summary>
        /// This method is called to refresh the board's UI and propagate the new Event handlers
        /// Called when board is unserialized and we need to reflect its state on the UI
        /// </summary>
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

            boardView.SetHandlers(this.CellClickedEventHandler, this.CellHoverEventHandler);
        }

        /// <summary>
        /// This method is called when a game is loaded to set the game's board and update some informations on it.
        /// Will call <see cref="RefreshBoardView"/>
        /// </summary>
        /// <param name="boardView"></param>
        public void SetBoardView(BoardView boardView)
        {
            this.boardView = boardView;
            RefreshBoardView();
        }
        #endregion

        /// <summary>
        /// This constructor is the base to init different values called when the program is first started and when it loads a saved game
        /// It should be private since it's only used as a helper but the tournament needs a default constructor
        /// </summary>
        public GameWithBoardInItsName()
        {
            this.logicalBoard = new Board(BOARD_DIMENSIONS.Width, BOARD_DIMENSIONS.Height);
            playedMovesStack = new List<Move>();

            refreshTimer = new DispatcherTimer();
            refreshTimer.Tick += OnTimerEvent;
            refreshTimer.Interval = TimeSpan.FromSeconds(1);

            playersTimer = new Stopwatch[2];
            playersTimer[0] = new Stopwatch();
            playersTimer[1] = new Stopwatch();
        }

        /// <summary>
        /// This is the constructor that should be used in most cases when not using the AI for the tournament
        /// </summary>
        /// <param name="boardView">The BoardView object that the object will update as the game is played</param>
        public GameWithBoardInItsName(BoardView boardView) : this()
        {
            CellClickedEvent = new MouseButtonEventHandler(CellClickedEventHandler);
            CellHoverEvent = new MouseEventHandler(CellHoverEventHandler);

            this.boardView = boardView;

            boardView.InitBoardView(BOARD_DIMENSIONS, CellClickedEvent, CellHoverEvent);
            InitBoard();

            Random random = new Random();

            // starts with a random player
            whitePlayerTurn = random.Next(0, 2) == 1;
        }

        /// <summary>
        /// This constructor is called when the board is unserialized
        /// </summary>
        /// <param name="info">The list of parameters containing the serialized values</param>
        /// <param name="context">The stream where the data come from</param>
        private GameWithBoardInItsName(SerializationInfo info, StreamingContext context) : this()
        {
            // Add the saved play time of each player 
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


            ScoreWhite = info.GetInt32("ScoreWhite");
            ScoreBlack = info.GetInt32("ScoreBlack");
        }

        /// <summary>
        /// This method inits some data for the first game turn.
        /// It looks for the possible moves and starts the timer.
        /// </summary>
        public void StartGame()
        {
            currentPossibleMoves = logicalBoard.GetPossibleMoves(whitePlayerTurn);
            foreach (Move m in currentPossibleMoves)
            {
                boardView.SetMoveHint(m.position);
            }

            GetPlayerStopwatch(whitePlayerTurn).Start();
            refreshTimer.Start();
        }

        /// <summary>
        /// This method sets the center pawns 
        /// </summary>
        private void InitBoard()
        {
            //We get the center
            int centerX = Convert.ToInt32(Math.Floor(BOARD_DIMENSIONS.Width / 2.0) - 1);
            int centerY = Convert.ToInt32(Math.Floor(BOARD_DIMENSIONS.Height / 2.0) - 1);

            //Initialize the center with the pawns
            List<Move> initMoves = new List<Move>()
            {
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
        /// This function changes the variables to change turn, calculates the possible moves for the current player and checks for game over.
        /// </summary>
        private void ChangeTurn()
        {
            GetPlayerStopwatch(whitePlayerTurn).Stop();
            whitePlayerTurn = !whitePlayerTurn;

            var nextPossibleMoves = this.logicalBoard.GetPossibleMoves(whitePlayerTurn);

            foreach(Move m in nextPossibleMoves)
            {
                boardView.SetMoveHint(m.position);
            }

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

        /// <summary>
        /// Helper fuction to return the player's stopwatch
        /// </summary>
        /// <param name="whitePlayer">the player's stopwatch to get</param>
        /// <returns></returns>
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

                    foreach (Move m in currentPossibleMoves)
                    {
                        boardView.ResetHint(m.position);
                    }

                    playedMovesStack.Add(targetMove);
                    PlayMove(targetMove);

                    ChangeTurn();
                }
            }
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
                    //senderCell.Highlight(whitePlayerTurn);
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
        /// <param name="sender">Event sender object</param>
        /// <param name="e">Args</param>
        private void OnTimerEvent(object sender, EventArgs e)
        {
            if (sender is DispatcherTimer timer)
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

        /// <summary>
        /// This method checks the moves stack and returns to the previous state of the board. Basically the reverse version of <seealso cref="PlayMove(Move)"/>
        /// </summary>
        public void UndoLastMove()
        {
            if (playedMovesStack.Count == 0)
                return;

            foreach(Move move in currentPossibleMoves)
            {
                boardView.ResetHint(move.position);
            }

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
        /// This method fires an event to update the data bound on the UI
        /// https://www.c-sharpcorner.com/UploadFile/020f8f/data-binding-with-inotifypropertychanged-interface/
        /// </summary>
        /// <param name="propertyName">The property's name to update on the UI</param>
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
            Point movePos = new Point(column, line);

            return currentPossibleMoves.Any(move => move.position.Equals(movePos));
        }

        public bool PlayMove(int column, int line, bool isWhite)
        {
            Point movePos = new Point(column, line);
            Move targetMove = currentPossibleMoves.Where(move => move.position.Equals(movePos)).FirstOrDefault();
            if(targetMove != null)
            {
                logicalBoard.ApplyMove(targetMove);
            }

            return false;
        }

        public Tuple<int, int> GetNextMove(int[,] game, int level, bool whiteTurn)
        {
            throw new NotImplementedException();
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

        /// <summary>
        /// This method is called by the Serialization process to populate the values to store.
        /// </summary>
        /// <param name="info">Serialized informations storage object</param>
        /// <param name="context">The stream context used by the serialization process</param>
        /// Will also call <seealso cref="Board.GetObjectData(SerializationInfo, StreamingContext)"/>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Turn", whitePlayerTurn);
            info.AddValue("WhiteTime", GetPlayerStopwatch(true).Elapsed);
            info.AddValue("BlackTime", GetPlayerStopwatch(false).Elapsed);
            info.AddValue("Board", this.logicalBoard, typeof(Board));
            info.AddValue("ScoreWhite", this.ScoreWhite);
            info.AddValue("ScoreBlack", this.ScoreBlack);
        }

    }
}
