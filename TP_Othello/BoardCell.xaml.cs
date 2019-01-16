using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        private MouseButtonEventHandler CellClickHandler;
        private MouseEventHandler CellHoverHandler;

        /// <summary>
        /// Constructor for the BoardCell object
        /// </summary>
        /// <param name="cellClicked">The reference of the EventHandler that will handle the mouse clicks</param>
        /// <param name="cellHover">The reference of the EventHandler that will handle the mouse hovers</param>
        /// <param name="boardPosition">The position in the board, used so we don't need to search for it when it fires an event</param>
        public BoardCell(MouseButtonEventHandler cellClicked, MouseEventHandler cellHover, System.Drawing.Point boardPosition)
        {
            InitializeComponent();

            this.BoardPosition = boardPosition;

            SetHandlers(cellClicked, cellHover);
        }


        /// <summary>
        /// This method removes the highlight of the cell. The cell is highlighted when the user hovers on it and can play here.
        /// It is called when the user's mouse leaves the cell or the user has played.
        /// </summary>
        public void ResetHint()
        {
            this.contentLabel.Style = FindResource("CellBaseState") as Style;
        }

        public void SetMoveHint()
        {
            this.contentLabel.Style = FindResource("CellHint") as Style;
        }

        public void SetHandlers(MouseButtonEventHandler cellClicked, MouseEventHandler cellHover)
        {
            if(CellHoverHandler != null && CellClickHandler != null)
            {
                MouseLeftButtonDown -= CellClickHandler;
                MouseEnter -= CellHoverHandler;
            }

            this.CellHoverHandler = cellHover;
            this.CellClickHandler = cellClicked;

            MouseLeftButtonDown += CellClickHandler;
            MouseEnter += CellHoverHandler;
        }

        public void SetPawnPlayer(bool whitePlayer)
        {

            this.contentLabel.Style = this.FindResource("CellBaseState") as Style;
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
            this.imageContainer.Style = this.FindResource("PawnBaseState") as Style;
            this.contentLabel.Style = FindResource("CellBaseState") as Style;
        }
    }
}
