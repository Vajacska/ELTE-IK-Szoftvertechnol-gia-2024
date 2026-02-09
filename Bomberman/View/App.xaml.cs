using Microsoft.Win32;
using Model;
using Model.EventArgs;
using Model.OutterModel;
using Persistence.Enums;
using System.ComponentModel;
using System.Windows;
using ViewModel;

namespace View
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        #region Fields
        private ModelOfControl? _modelOfControl;
        private InnerViewModel? _gameViewModel;
        private OuterViewModel? _menuViewModel;
        private GameWindow? _gameWindow;
        private MenuGameWindow? _menuWindow;

        // save dialog
        private OpenFileDialog _loadFileDialog;
        private SaveFileDialog _saveFileDialog;
        #endregion

        public App()
        {
            Startup += OnAppStartUp;
        }

        private void OnAppStartUp(object sender, StartupEventArgs e)
        {
            _menuWindow = new MenuGameWindow();
            _menuViewModel = new OuterViewModel();
            _modelOfControl = new ModelOfControl(_menuViewModel.MapType, 1, [new(1), new(2)]);
            _menuWindow.DataContext = _menuViewModel;

            _menuViewModel.MapSchema += OnChangeMap;
            _menuViewModel.NewGame += OnStartGame;
            _menuViewModel.NumberOfPlayers += new EventHandler<NumberOfSomethingEventArgs>(OnNumberOfPlayers);
            _menuViewModel.LoadGame += OnLoadGame;

            _menuViewModel.Exit += OnExitGame;
            _menuViewModel.WinNumber += new EventHandler<NumberOfSomethingEventArgs>(OnNumberOfWins);

            _menuWindow.Show();

            _loadFileDialog = new OpenFileDialog();
            _loadFileDialog.Title = "Load Bomberman map";
            _loadFileDialog.Filter = "GameType|*.bomberman";
        }

        private void OnNumberOfPlayers(object? sender, NumberOfSomethingEventArgs e)
        {
            _modelOfControl?.ChangePlayerNumber(e.Number);
        }

        private void OnNumberOfWins(object? sender, NumberOfSomethingEventArgs e)
        {
            _modelOfControl?.ChangeNumberOfWins(e.Number);
        }

        private void OnExitGame(object? sender, EventArgs e)
        {
            if (_menuWindow != null)
            {
                _menuWindow.Close();
            }

            if (_gameWindow != null)
            {
                _gameWindow.Close();
            }
        }

        private void OnChangeMap(object? sender, MapEventArgs e)
        {
            _modelOfControl?.ChangeMapType(e.MapType);
        }

        private void OnStartGame(object? sender, EventArgs e)
        {
            _modelOfControl?.StartNewRound();
            _gameViewModel = new InnerViewModel(_modelOfControl);
            _modelOfControl.Model.RoundOver += OnRoundOver;
            _gameViewModel.SaveGame += OnSaveGame;
            _gameViewModel.BackToMenu += OnBackToMenu;
            _saveFileDialog = new SaveFileDialog();
            _saveFileDialog.Title = "Save Bomberman map";
            _saveFileDialog.Filter = "GameType|*.bomberman";

            _gameWindow = new GameWindow();
            _gameWindow.DataContext = _gameViewModel;
            _gameWindow.Closed += OnExitGame;

            _gameWindow.Show();
            _menuWindow?.Hide();
        }

        private void RestartGame()
        {
            _modelOfControl?.StartNewRound();
            _gameViewModel = new InnerViewModel(_modelOfControl);
            _modelOfControl.Model.RoundOver += OnRoundOver;
        }

        private void OnRoundOver(object? sender, GameEventArgs e)
        {
            if (e.IsEveryoneOut)
            {
                if (MessageBox.Show("Round Over! Everyone is out!", "Bomberman", MessageBoxButton.OK) == MessageBoxResult.OK)
                {
                    if(_gameViewModel != null)
                    {
                        _gameViewModel.PauseText = "Start Game";
                        _gameViewModel.RefreshTable();
                        RestartGame();
                    }
                }
            }
            else
            {
                if (e.Player.Score + 1 < _modelOfControl?.Model.NumberOfWins)
                {
                    if (MessageBox.Show($"Round Over! Winner: {e.Player.Id}", "Bomberman", MessageBoxButton.OK) == MessageBoxResult.OK)
                    {
                        if(_gameViewModel != null)
                        {
                            _gameViewModel.PauseText = "Start Game";
                            _gameViewModel.RefreshTable();
                            RestartGame();
                        }
                    }
                }
                else
                {
                    if (MessageBox.Show($"Game Over! Winner: {e.Player.Id}", "Bomberman", MessageBoxButton.OK) == MessageBoxResult.OK)
                    {
                        if(_menuViewModel?.MapType != null)
                            _modelOfControl = new ModelOfControl(_menuViewModel.MapType, _menuViewModel?.NumberOfWins, [new(1), new(2)]);
                    }
                }
            }
        }

        private async void OnLoadGame(object? sender, EventArgs e)
        {
            if (_loadFileDialog.ShowDialog() == true)
            {
                try
                {
                    if(_modelOfControl != null)
                    {
                        await _modelOfControl.LoadGameAsync(_loadFileDialog.FileName);
                        MessageBox.Show("Game was loaded!", "Info", MessageBoxButton.OK, MessageBoxImage.Information);

                        OnStartGame(null, EventArgs.Empty);
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Game was not loaded!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async void OnSaveGame(object? sender, EventArgs e)
        {
            if (_saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    if(_modelOfControl != null)
                    {
                        await _modelOfControl.SaveGameAsync(_saveFileDialog.FileName);
                        MessageBox.Show("Game was saved!", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Game was not saved!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void OnBackToMenu(object? sender, EventArgs e)
        {
            _gameWindow.Close();
            _menuWindow.Show();
        }
    }
}
