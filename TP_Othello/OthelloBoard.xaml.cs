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
    public partial class OthelloBoard : UserControl
    {
        private Size BOARD_DIMENSIONS = new Size(7, 9);

        public delegate void CellClickedEventHandler(object Sender);
        public event CellClickedEventHandler CellClicked;

        public OthelloBoard()
        {
            InitializeComponent();

            InitGrid();
        }

        private void InitGrid()
        {
            for (int i = 0; i < BOARD_DIMENSIONS.Width; i++)
                GridBoard.ColumnDefinitions.Add(new ColumnDefinition());

            for (int i = 0; i < BOARD_DIMENSIONS.Height; i++)
                GridBoard.RowDefinitions.Add(new RowDefinition());


            for(int i = 0; i < BOARD_DIMENSIONS.Width; i++)
            {
                for(int j = 0; j < BOARD_DIMENSIONS.Height; j++)
                {
                    BoardCell boardCell = new BoardCell();

                    Grid.SetColumn(boardCell, i);
                    Grid.SetRow(boardCell, j);

                    GridBoard.Children.Add(boardCell);
                }
            }
        }

        protected virtual void OnCellClicked(object sender)
        {
            // TODO
        }

    }
}
