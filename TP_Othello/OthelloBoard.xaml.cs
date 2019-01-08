using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        // Those are the event handlers passed to the cells so the event fired for them is handled here
        private event MouseButtonEventHandler CellClicked;
        private event MouseEventHandler CellHover;

        // Cells components of the board
        // represented as a matrix (row, column)
        private BoardCell[,] boardCells;


        public OthelloBoard()
        {
            InitializeComponent();

            CellClicked = new MouseButtonEventHandler(CellClickedHandler);
            CellHover = new MouseEventHandler(CellHoverHandler);

            InitGrid();
        }

        private void InitGrid()
        {
            boardCells = new BoardCell[(int)BOARD_DIMENSIONS.Height,(int)BOARD_DIMENSIONS.Width];

            for (int j = 0; j < BOARD_DIMENSIONS.Height; j++)
            {
                GridBoard.RowDefinitions.Add(new RowDefinition());

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
        /// <param name="Sender"></param>
        /// <param name="e"></param>
        private void CellClickedHandler(object Sender, MouseEventArgs e)
        {

        }

        /// <summary>
        /// This is the handler function of the hover on a cell. It is called by the BoardCell objects
        /// </summary>
        /// <see cref="BoardCell"/>
        /// <param name="Sender"></param>
        /// <param name="e"></param>
        private void CellHoverHandler(object Sender, MouseEventArgs e)
        {

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
