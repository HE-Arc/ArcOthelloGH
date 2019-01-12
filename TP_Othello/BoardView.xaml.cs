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
        public void InitBoardView(System.Drawing.Size boardDimensions, MouseButtonEventHandler cellClickHandler, MouseEventHandler cellHoverHandler)
        {
            boardCells = new BoardCell[boardDimensions.Height, boardDimensions.Width];

            for (int j = 0; j < boardDimensions.Height; j++)
            {
                RowDefinition rowDefinition = new RowDefinition();
                rowDefinition.Height = GridLength.Auto;
                GridBoard.RowDefinitions.Add(rowDefinition);

                for (int i = 0; i < boardDimensions.Width; i++)
                {
                    ColumnDefinition columnDefinition = new ColumnDefinition();
                    columnDefinition.Width = GridLength.Auto;

                    GridBoard.ColumnDefinitions.Add(columnDefinition);

                    BoardCell boardCell = new BoardCell(cellClickHandler, cellHoverHandler);

                    Grid.SetColumn(boardCell, i);
                    Grid.SetRow(boardCell, j);

                    GridBoard.Children.Add(boardCell);
                    boardCells[j, i] = boardCell;
                }
            }
        }
    }
}
