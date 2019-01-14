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
using System.Runtime.Serialization.Formatters.Binary;

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
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "Arc Othello files (*.arcgh)|*.arcgh|All files (*.*)|*.*";
            if(saveDialog.ShowDialog().HasValue == true)
            {
                // using code from :
                // https://www.dotnetperls.com/serialize-list
                try
                {
                    using (Stream stream = File.Open(saveDialog.FileName + ".arcgh", FileMode.Create))
                    {
                        BinaryFormatter binaryFormatter = new BinaryFormatter();
                        binaryFormatter.Serialize(stream, game);
                    }
                }
                catch(IOException exception)
                {
                    MessageBox.Show($"Unable to save the game : {exception.Message}", "Save file error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btnLoadGame_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.Filter = "Arc Othello files (*.arcgh)|*.arcgh|All files (*.*)|*.*";
            if (openDialog.ShowDialog().HasValue)
            {
                try
                {
                    using (Stream stream = File.Open(openDialog.FileName, FileMode.Open))
                    {
                        BinaryFormatter binaryFormatter = new BinaryFormatter();
                        Game unserializeGame = (Game)binaryFormatter.Deserialize(stream);

                        game.GetBoard();
                    }
                }
                catch (IOException exception)
                {
                    MessageBox.Show($"Unable to load the game : {exception.Message}", "Load file error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            Debug.Write("Load game");
        }

        private void btnUndo_Click(object sender, RoutedEventArgs e)
        {
            game.UndoLastMove();
        }
    }
}
