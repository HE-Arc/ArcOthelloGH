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
    /// Logique d'interaction pour BoardCell.xaml
    /// </summary>
    ///
    public partial class BoardCell : UserControl
    {
        int cellValue = -1;

        /// <summary>
        /// Constructor for the BoardCell object
        /// </summary>
        /// <param name="cellClicked">The reference of the EventHandler that will handle the mouse clicks</param>
        /// <param name="cellHover">The reference of the EventHandler that will handle the mouse hovers</param>
        public BoardCell(MouseButtonEventHandler cellClicked, MouseEventHandler cellHover)
        {
            InitializeComponent();

            MouseLeftButtonDown += cellClicked;
            MouseEnter += cellHover;

            // forging
            MouseLeave += new MouseEventHandler((sender, args) => ResetHighlight());

            this.imageContainer.Visibility = Visibility.Hidden;
            /*MouseLeave += new MouseEventHandler(delegate (Object o, MouseEventArgs e)
            {

                ResetDisplay();
            });*/
        }

        public int CellValue { get => cellValue; set => cellValue = value; }

        public void Play(int cellValue)
        {
            // debug
            this.imageContainer.Visibility = Visibility.Visible;
            this.imageContainer.Source = new BitmapImage(new System.Uri("http://www.he-arc.ch/sites/www.he-arc.ch/files/ING/competences/collaborateurs/francois_tieche.jpg"));
            this.imageContainer.Stretch = Stretch.Fill;
            /*if (cellValue == 0)
                pawnImage.UriSource = new System.Uri("http://www.he-arc.ch/sites/www.he-arc.ch/files/ING/competences/collaborateurs/stephane_gobron.jpg");
            else
                pawnImage.UriSource = new System.Uri("http://www.he-arc.ch/sites/www.he-arc.ch/files/ING/competences/collaborateurs/francois_tieche.jpg");*/
        }

        /// <summary>
        /// This method removes the highlight of the cell. The cell is highlighted when the user hovers on it and can play here.
        /// It is called when the user's mouse leaves the cell or the user has played.
        /// </summary>
        public void ResetHighlight()
        {

        }
    }
}
