using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Drawing;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TP_Othello.GameLogics;


namespace TP_Othello
{
    /// <summary>
    /// Logique d'interaction pour OthelloBoard.xaml
    /// </summary>
    public partial class BoardView : UserControl
    {
        // Cells components of the board
        private BoardCell[,] boardCells;

        public BoardView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// This method initializes the Grid's layout and the sub-usercontrols board cells
        /// <see cref="BoardCell"/>
        /// </summary>
        public void InitBoardView(System.Drawing.Size boardDimensions, MouseButtonEventHandler cellClickHandler)
        {
            this.GridBoard.Children.Clear();
            this.GridBoard.RowDefinitions.Clear();
            this.GridBoard.ColumnDefinitions.Clear();
            boardCells = new BoardCell[boardDimensions.Width, boardDimensions.Height];

            for(int i = 0; i < boardDimensions.Height; i++)
            {
                RowDefinition rowDefinition = new RowDefinition();

                rowDefinition.Height = new GridLength(1.0, GridUnitType.Star);

                GridBoard.RowDefinitions.Add(rowDefinition);
            }

            for(int i = 0; i < boardDimensions.Width; i++)
            {
                ColumnDefinition columnDefinition = new ColumnDefinition();
                //columnDefinition.Width = new GridLength(1.0, GridUnitType.Star);

                GridBoard.ColumnDefinitions.Add(columnDefinition);
            }


            for (int j = 0; j < boardDimensions.Height; j++)
            { 
                for (int i = 0; i < boardDimensions.Width; i++)
                {
                    BoardCell boardCell = new BoardCell(cellClickHandler, new System.Drawing.Point(i, j));

                    Grid.SetColumn(boardCell, i);
                    Grid.SetRow(boardCell, j);

                    GridBoard.Children.Add(boardCell);
                    boardCells[i, j] = boardCell;
                }
            }
        }

        /// <summary>
        /// Applies the handlers to all board cells
        /// </summary>
        /// <param name="cellClickHandler"></param>
        /// <param name="cellHoverHandler"></param>
        public void SetHandlers(MouseButtonEventHandler cellClickHandler)
        {
            for(int i = 0; i < boardCells.GetLength(0); i++)
            {
                for(int j = 0; j < boardCells.GetLength(1); j++)
                {
                    boardCells[i, j].SetHandlers(cellClickHandler);
                }
            }
        }

        /// <summary>
        /// Displays a pawn on a specific cell
        /// </summary>
        /// <param name="position">The cell's [x,y] position</param>
        /// <param name="whitePlayer">White or black player's pawn</param>
        public void SetPawnCell(System.Drawing.Point position, bool whitePlayer)
        {
            boardCells[position.X, position.Y].SetPawnPlayer(whitePlayer);
        }

        /// <summary>
        /// Removes the pawn's display on a specific cell. Called by the undo
        /// </summary>
        /// <param name="position">Cell's position</param>
        public void UnsetPawnCell(System.Drawing.Point position)
        {
            boardCells[position.X, position.Y].UnsetPawnPlayer();
        }

        /// <summary>
        /// Displays the specific cell as playable for the user
        /// </summary>
        /// <param name="position">Cell's position</param>
        public void SetMoveHint(System.Drawing.Point position)
        {
            boardCells[position.X, position.Y].SetMoveHint();
        }

        /// <summary>
        /// Removes the hint on the cell as it used to be playable for the user
        /// </summary>
        /// <param name="position">Cell's position</param>
        public void ResetHint(System.Drawing.Point position)
        {
            boardCells[position.X, position.Y].ResetHint();
        }
    }
}
