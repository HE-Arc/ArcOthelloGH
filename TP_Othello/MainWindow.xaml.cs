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
        private GameWithBoardInItsName game;

        public MainWindow()
        {
            InitializeComponent();

            CreateGame();
        }

        /// <summary>
        /// Creates a new game object and passes the board's display to it
        /// </summary>
        public void CreateGame()
        {
            this.game = new GameWithBoardInItsName(boardView, this);
            this.DataContext = game;

            game.StartGame();
        }

        /// <summary>
        /// Toolbar's button or shortcut to create a new gmae
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            CreateGame();
        }

        /// <summary>
        /// Displays the save dialog to save the current game
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "Arc Othello files (*.arcgh)|*.arcgh|All files (*.*)|*.*";
            saveDialog.DefaultExt = "arcgh";
            saveDialog.AddExtension = true;

            if (saveDialog.ShowDialog() == true)
            {
                // using code from :
                // https://www.dotnetperls.com/serialize-list
                try
                {
                    using (Stream stream = File.Open(saveDialog.FileName, FileMode.Create))
                    {
                        BinaryFormatter binaryFormatter = new BinaryFormatter();
                        binaryFormatter.Serialize(stream, game);
                    }
                }
                catch (IOException exception)
                {
                    MessageBox.Show($"Unable to save the game : {exception.Message}", "Save file error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        /// <summary>
        /// Shows the open dialog to load a game
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.Filter = "Arc Othello files (*.arcgh)|*.arcgh|All files (*.*)|*.*";
            if (openDialog.ShowDialog() == true)
            {
                try
                {
                    using (Stream stream = File.Open(openDialog.FileName, FileMode.Open))
                    {
                        BinaryFormatter binaryFormatter = new BinaryFormatter();
                        GameWithBoardInItsName unserializedGame = (GameWithBoardInItsName)binaryFormatter.Deserialize(stream);

                        unserializedGame.SetViews(this.boardView, this);

                        this.game = unserializedGame;
                        this.DataContext = this.game;

                        game.StartGame();
                    }
                }
                catch (IOException exception)
                {
                    MessageBox.Show($"Unable to load the game : {exception.Message}", "Load file error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        /// <summary>
        /// Undo the last move
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UndoCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            game.UndoLastMove();
        }

        /// <summary>
        /// This method is called when the game is over and one of the players has won. It asks the user(s) for rematch
        /// </summary>
        /// <param name="playerName">The player's name by default black or white</param>
        /// <returns>True if the user(s) wants to play again</returns>
        public bool DisplayReplayDialog(string playerName)
        {
            MessageBoxResult result = MessageBox.Show($"Game is over. {playerName} has won !.\nPlay again ?", "Game over", MessageBoxButton.YesNo);
            return result == MessageBoxResult.Yes;
        }

        /// <summary>
        /// This method displays a hint on the UI depending on the player's turn
        /// </summary>
        /// <param name="whitePlayer">Whether it's the white player's turn</param>
        public void DisplayPlayerTurnHighlight(bool whitePlayer)
        {
            Border targetPlayerInfos = whitePlayer ? InfosWhitePlayer : InfosBlackPlayer;
            Border other = whitePlayer ? InfosBlackPlayer : InfosWhitePlayer;

            targetPlayerInfos.Style = FindResource("PlayerTurnHighlight") as Style;
            other.Style = FindResource("PlayerTurnBase") as Style;
        }

        /// <summary>
        /// Shows that we made dis
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HelpCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            MessageBox.Show("Othello Game made for the C# .NET and AI course HE-Arc 2018-2019.\nGrava Maxime, Herbelin Ludovic\nIf you're looking for the Othello rules you better check google.", "About this app");
        }
    }
}
