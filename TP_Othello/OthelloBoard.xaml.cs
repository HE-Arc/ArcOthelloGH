using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace TP_Othello
{
    /// <summary>
    /// Logique d'interaction pour OthelloBoard.xaml
    /// </summary>
    public partial class OthelloBoard : UserControl, IPlayable.IPlayable
    {
        private Size BOARD_DIMENSIONS = new Size(9, 7);

        private Stopwatch[] playersTimer;
        private DispatcherTimer refreshTimer;

        private bool currentPlayerId;

        // Those are the event handlers passed to the cells so the event fired for them is handled here
        private event MouseButtonEventHandler CellClicked;
        private event MouseEventHandler CellHover;

        // Cells components of the board
        // represented as a matrix (row, column)
        private BoardCell[,] boardCells;


        public OthelloBoard()
        {
            InitializeComponent();

            // We handle the hover and click events in this class
            CellClicked = new MouseButtonEventHandler(CellClickedHandler);
            CellHover = new MouseEventHandler(CellHoverHandler);

            refreshTimer = new DispatcherTimer();
            refreshTimer.Interval = new TimeSpan(0, 0, 0, 1);
            refreshTimer.Tick += new EventHandler(OnTimerEvent);

            playersTimer = new Stopwatch[2];
            playersTimer[0] = new Stopwatch();
            playersTimer[1] = new Stopwatch();

            InitGrid();

            refreshTimer.Start();
        }

        /// <summary>
        /// This method initializes the Grid's layout and the sub-usercontrols board cells
        /// <see cref="BoardCell"/>
        /// </summary>
        private void InitGrid()
        {
            boardCells = new BoardCell[(int)BOARD_DIMENSIONS.Height,(int)BOARD_DIMENSIONS.Width];

            for (int j = 0; j < BOARD_DIMENSIONS.Height; j++)
            {
                RowDefinition rowDefinition = new RowDefinition();
                rowDefinition.Height = GridLength.Auto;
                GridBoard.RowDefinitions.Add(rowDefinition);

                for (int i = 0; i < BOARD_DIMENSIONS.Width; i++)
                {
                    ColumnDefinition columnDefinition = new ColumnDefinition();
                    columnDefinition.Width = GridLength.Auto;

                    GridBoard.ColumnDefinitions.Add(columnDefinition);

                    BoardCell boardCell = new BoardCell(CellClicked, CellHover);

                    Grid.SetColumn(boardCell, i);
                    Grid.SetRow(boardCell, j);

                    GridBoard.Children.Add(boardCell);
                    boardCells[j, i] = boardCell;
                }
            }
        }

        /// <summary>
        /// Returns a list of valid plays for the current player.
        /// 
        /// </summary>
        /// <param name="playerValue"></param>
        /// <returns>A list of BoardCell objects </returns>
        public List<BoardCell> GetValidPlays(int playerValue)
        {
            // check vertically
            // he can play if there would be one or more opponent pawns between this cell and another of his pawns

            List<BoardCell> validPlays = new List<BoardCell>();

            Point startPos = new Point(-1, -1);

            // there is probably a better way to do this

            for(int column = 0; column < BOARD_DIMENSIONS.Width; column++)
            {
                for (int row = 0; row < BOARD_DIMENSIONS.Height; row++)
                {
                    // if this is another player's pawn we don't care
                    if (boardCells[row, column].CellValue != playerValue)
                        continue;

                    // if this is the first time we encouter a player's pawn in this sequence
                    if(startPos.X == -1 && startPos.Y == -1)
                    {
                        startPos.X = column; startPos.Y = row;
                    }
                    else
                    {
                        validPlays.Add(boardCells[row, column]);
                    }
                }
            }

            // check horizontally
            // check diagonally

            return validPlays;
        }

        /// <summary>
        /// This is the handler function of the click on a cell. It is called by the BoardCell objects
        /// </summary>
        /// <see cref="BoardCell"/>
        /// <param name="Sender">Sender of the event, should be a BoardCell</param>
        /// <param name="e">Event arguments</param>
        private void CellClickedHandler(object Sender, MouseEventArgs e)
        {
            // if the sender object is a BoardCell we cast it and null checks (equivalent as ...  != null)
            if(Sender is BoardCell senderCell)
            {
                Debug.WriteLine("Cell clicked");
                Debug.WriteLine(senderCell.CellValue);
                senderCell.Play(0);
            }
        }

        /// <summary>
        /// This is the handler function of the hover on a cell. It is called by the BoardCell objects
        /// </summary>
        /// <see cref="BoardCell"/>
        /// <param name="Sender">Sender of the event, should be a BoardCell</param>
        /// <param name="e">Event arguments</param>
        private void CellHoverHandler(object Sender, MouseEventArgs e)
        {
            // if the sender object is a BoardCell we cast it and null checks (equivalent as ...  != null)
            if (Sender is BoardCell senderCell)
            {
                Debug.WriteLine("Cell hovered");
                Debug.WriteLine(senderCell.CellValue);
            }
        }

        /// <summary>
        /// This function changes the variables to change turn
        /// </summary>
        private void ChangeTurn()
        {
            playersTimer[currentPlayerId ? 1 : 0].Stop();
            currentPlayerId = !currentPlayerId;
            playersTimer[currentPlayerId ? 1 : 0].Start();
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
                if (currentPlayerId)
                {
                    String a = playersTimer[1].Elapsed.ToString("HH:mm:ss");
                    
                }
                else
                {
                    String a = playersTimer[0].Elapsed.ToString("HH:mm:ss");
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
