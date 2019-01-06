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

        public BoardCell()
        {
            InitializeComponent();
        }

        public int CellValue { get => cellValue; set => cellValue = value; }
    }
}
