using Model.EventArgs;
using Persistence.Enums;
using Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ViewModel
{
    public class OuterViewModel : ViewModelBase
    {
        #region Fields

        private MapType _mapType;
        private int _numberOfWins;

        #endregion

        #region Properties

        public DelegateCommand MapSchemaCommand { get; private set; }
        public DelegateCommand NewGameCommand { get; private set; }
        public DelegateCommand LoadGameCommand { get; private set; }
        public DelegateCommand SaveGameCommand { get; private set; }
        public DelegateCommand ExitCommand { get; private set; }
        public DelegateCommand NumberOfPlayersCommand { get; private set; }
        public DelegateCommand NumberOfWinsCommand { get; private set; }
        public DelegateCommand MonsterTutorialCommand { get; private set; }
        public MapType MapType { get { return _mapType; } set { _mapType = value; } }
        public int NumberOfWins { get { return _numberOfWins; } set { _numberOfWins = value; } }

        #endregion

        #region Events

        public event EventHandler<MapEventArgs>? MapSchema;
        public event EventHandler? NewGame;
        public event EventHandler? LoadGame;
        public event EventHandler? SaveGame;
        public event EventHandler? Exit;
        public event EventHandler<NumberOfSomethingEventArgs> NumberOfPlayers;
        public event EventHandler<NumberOfSomethingEventArgs> WinNumber;

        #endregion

        #region Constructor

        public OuterViewModel()
        {
            MapSchemaCommand = new DelegateCommand(param => OnMapSchema(Convert.ToInt32(param)));
            NewGameCommand = new DelegateCommand(param => OnNewGame());
            LoadGameCommand = new DelegateCommand(param => OnLoadGame());
            SaveGameCommand = new DelegateCommand(param => OnSaveGame());
            ExitCommand = new DelegateCommand(param => OnExitGame());
            NumberOfPlayersCommand = new DelegateCommand(param => OnNumberOfPlayers(Convert.ToInt32(param)));
            NumberOfWinsCommand = new DelegateCommand(param => OnNumberOfWins(Convert.ToInt32(param)));
            MonsterTutorialCommand = new DelegateCommand(param => OnTutorial());
        }

        #endregion

        #region Event Methods

        private void OnMapSchema(int maptype)
        {
            if (maptype == 1)
            {
                MapSchema?.Invoke(this, new MapEventArgs(MapType.TYPE1));
                MapType = MapType.TYPE1;
            }
            else if (maptype == 2)
            {
                MapSchema?.Invoke(this, new MapEventArgs(MapType.TYPE2));
                MapType = MapType.TYPE2;
            }
            else
            {
                MapSchema?.Invoke(this, new MapEventArgs(MapType.TYPE3));
                MapType = MapType.TYPE3;
            }
        }
        private void OnNewGame()
        {
            NewGame?.Invoke(this, EventArgs.Empty);
        }

        private void OnLoadGame()
        {
            LoadGame?.Invoke(this, EventArgs.Empty);
        }

        private void OnSaveGame()
        {
            SaveGame?.Invoke(this, EventArgs.Empty);
        }

        private void OnExitGame()
        {
            Exit?.Invoke(this, EventArgs.Empty);
        }

        private void OnNumberOfPlayers(int number)
        {
            NumberOfPlayers?.Invoke(this, new NumberOfSomethingEventArgs(number));
        }

        private void OnNumberOfWins(int number)
        {
            _numberOfWins = number;
            WinNumber?.Invoke(this, new NumberOfSomethingEventArgs(number));
        }

        private void OnTutorial()
        {
            MapSchema?.Invoke(this, new MapEventArgs(MapType.TUTORIAL));
            MapType = MapType.TUTORIAL;

            NumberOfPlayers?.Invoke(this, new NumberOfSomethingEventArgs(2));
            WinNumber?.Invoke(this, new NumberOfSomethingEventArgs(1));

            NewGame?.Invoke(this, EventArgs.Empty);
        }
        #endregion
    }
}
