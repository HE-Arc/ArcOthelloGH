﻿using System;
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
    /// Logique d'interaction pour BoardCell.xaml
    /// </summary>
    ///
    public partial class BoardCell : UserControl
    {
        System.Drawing.Point boardPosition;

        public System.Drawing.Point BoardPosition { get => boardPosition; private set => boardPosition = value; }

        /// <summary>
        /// Constructor for the BoardCell object
        /// </summary>
        /// <param name="cellClicked">The reference of the EventHandler that will handle the mouse clicks</param>
        /// <param name="cellHover">The reference of the EventHandler that will handle the mouse hovers</param>
        /// <param name="boardPosition">The position in the board, used so we don't need to search for it when it fires an event</param>
        public BoardCell(MouseButtonEventHandler cellClicked, MouseEventHandler cellHover, System.Drawing.Point boardPosition)
        {
            InitializeComponent();

            MouseLeftButtonDown += cellClicked;
            MouseEnter += cellHover;

            // forging
            MouseLeave += new MouseEventHandler((sender, args) => ResetHighlight());

            this.BoardPosition = boardPosition;
        }


        /// <summary>
        /// This method removes the highlight of the cell. The cell is highlighted when the user hovers on it and can play here.
        /// It is called when the user's mouse leaves the cell or the user has played.
        /// </summary>
        public void ResetHighlight()
        {
            this.contentLabel.Background = Brushes.Transparent;
        }

        public void Highlight(bool whitePlayer)
        {
            this.contentLabel.Background = Brushes.DarkRed;
        }

        public void SetPawnPlayer(bool whitePlayer)
        {
            if(whitePlayer)
            {
                this.imageContainer.Style = this.FindResource("PawnWhite") as Style;
            }
            else
            {
                this.imageContainer.Style = this.FindResource("PawnBlack") as Style;
            }
        }

        public void UnsetPawnPlayer()
        {
            imageContainer.Visibility = Visibility.Hidden;
        }
    }
}
