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

        /// <summary>
        /// Constructor for the BoardCell object
        /// </summary>
        /// <param name="cellClicked">The reference of the EventHandler that will handle the mouse clicks</param>
        /// <param name="cellHover">The reference of the EventHandler that will handle the mouse hovers</param>
        /// <param name="boardPosition">The position in the board, used so we don't need to search for it when it fires an event</param>
        public BoardCell(MouseButtonEventHandler cellClicked, System.Drawing.Point boardPosition)
        {
            InitializeComponent();

            this.BoardPosition = boardPosition;

            SetHandlers(cellClicked);
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
            //contentLabel.Opacity = 0.6;
        }
        
        /// <summary>
        /// Connects the handlers to the event of the cells and remove (if any) there was before
        /// </summary>
        /// <param name="cellClicked"></param>
        /// <param name="cellHover"></param>
        public void SetHandlers(MouseButtonEventHandler cellClicked)
        {
            if(CellClickHandler != null)
            {
                MouseLeftButtonDown -= CellClickHandler;
            }

            this.CellClickHandler = cellClicked;

            MouseLeftButtonDown += CellClickHandler;
        }

        /// <summary>
        /// Displays one of the pawn on the current cell using the styles
        /// </summary>
        /// <param name="whitePlayer">white or black player(false)</param>
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

            this.contentLabel.Opacity = 1.0;
        }

        public void UnsetPawnPlayer()
        {
            this.contentLabel.Style = FindResource("CellBaseState") as Style;
            this.imageContainer.Style = this.FindResource("PawnBaseState") as Style;
        }
    }
}
