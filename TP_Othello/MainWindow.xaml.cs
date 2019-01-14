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
using System.IO;
using Microsoft.Win32;
using System.Diagnostics;
using TP_Othello.GameLogics;

namespace TP_Othello
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Game game;

        public MainWindow()
        {
            InitializeComponent();
            
            this.game = new Game(boardView);
            this.DataContext = game;
            game.StartGame();
        }

        private void btnOpenCredits_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Othello Game made for the C# .NET and AI course HE-Arc 2018-2019.\nGrava Maxime, Herbelin Ludovic", "About this app");
        }

        private void btnSaveGame_Click(object sender, RoutedEventArgs e)
        {
            int[,] serializableBoard = { { 1, 2, 3, 4 },{ 5, 6, 7, 8 },{ 9, 10, 11, 12 } };
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "Arc Othello files (*.arcgh)|All files (*.*)";
            if(saveDialog.ShowDialog().HasValue == true)
            {
            }
        }

        private void btnLoadGame_Click(object sender, RoutedEventArgs e)
        {
            Debug.Write("Load game");
        }

        private void btnUndo_Click(object sender, RoutedEventArgs e)
        {
            game.UndoLastMove();
        }
    }
}
