using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Model;
using Model.OutterModel;
using Persistence;
using Persistence.Enums;
using Persistence.GameObjects;
using Persistence.Structures;
using Model.EventArgs;
using Persistence.Monsters;

namespace ViewModel
{
    public class InnerViewModel : ViewModelBase
    {
        #region Fields

        private ModelOfControl _outerModel;
        private Player _player1;
        private Player _player2;
        private Player _player3;
        private int _round;
        private int _rowSize;
        private int _columnSize;
        private bool _isGameStarted;

        #endregion

        #region Properties
        public String GameTime { get { return TimeSpan.FromSeconds(_outerModel.Model.GameTime).ToString("g"); } }

        public PlayingPlayer WinnerOfTheRound { get; set; }

        //public ObservableCollection<PlayingPlayer> PlayingPlayers { get; set; }
        public Player Player1
        {
            get
            {
                return _player1;
            }
            set
            {
                _player1 = value;
                OnPropertyChanged(nameof(Player1));
            }
        }
        public Player Player2
        {
            get
            {
                return _player2;
            }
            set
            {
                _player2 = value;
                OnPropertyChanged(nameof(Player2));
            }
        }
        public Player Player3
        {
            get
            {
                return _player3;
            }
            set
            {
                _player3 = value;
                OnPropertyChanged(nameof(Player3));
            }
        }
        
        public int Round
        {
            get
            {
                return _round;
            }
            set
            {
                _round = value;
                OnPropertyChanged(nameof(Round));
            }
        }

        public int RowSize
        {
            get
            {
                return _rowSize;
            }
            set
            {
                _rowSize = value;
                OnPropertyChanged(nameof(RowSize));
            }
        }

        public int ColumnSize
        {
            get
            {
                return _columnSize;
            }
            set
            {
                _columnSize = value;
                OnPropertyChanged(nameof(ColumnSize));
            }
        }

        public bool IsGameStarted
        {
            get
            {
                return !_isGameStarted;
            }
            set
            {
                _isGameStarted = value;
                OnPropertyChanged(nameof(IsGameStarted));
            }
        }

        public ObservableCollection<GameField> GameFields { get; set; }


        public DelegateCommand StartGameCommand { get; private set; } //command of starting game --- not used yet
        public DelegateCommand PauseGameCommand { get; private set; } //command of pausing game
        public DelegateCommand BackToMenuCommand { get; private set; }
        public DelegateCommand SaveGameCommand { get; private set; }

        public string PauseText { get; set; }

        //commands of stepping
        public DelegateCommand StepUpCommand { get;  private set; }
        public DelegateCommand StepDownCommand { get; private set; }
        public DelegateCommand StepRightCommand { get; private set; }
        public DelegateCommand StepLeftCommand { get; private set; } 
        public DelegateCommand PlantBombCommand { get; private set; }
        public DelegateCommand PlaceBoxCommand { get; private set; }

        public event EventHandler PauseGame;
        public event EventHandler? SaveGame;
        public event EventHandler? BackToMenu;

        #endregion

        #region Constructor
        public InnerViewModel(ModelOfControl outerModel)
        {
            _outerModel = outerModel;

            _outerModel.Model.Step += new EventHandler<GameStepEventArgs>(Model_Stepping);
            _outerModel.Model.GameAdvanced += new EventHandler<GameEventArgs>(Model_GameAdvanced);
            _outerModel.Model.RoundWinner += new EventHandler<GameEventArgs>(Model_RoundWinner);
            _outerModel.NewRoundEvent += new EventHandler<NumberOfSomethingEventArgs>(Model_NewRound);

            StepUpCommand = new DelegateCommand(param => OnStepUp(Convert.ToInt32(param)));
            StepDownCommand = new DelegateCommand(param => OnStepDown(Convert.ToInt32(param)));
            StepRightCommand = new DelegateCommand(param => OnStepRight(Convert.ToInt32(param)));
            StepLeftCommand = new DelegateCommand(param => OnStepLeft(Convert.ToInt32(param)));
            PlantBombCommand = new DelegateCommand(param => OnPlantBomb(Convert.ToInt32(param)));
            PlaceBoxCommand = new DelegateCommand(param => OnPlaceBox(Convert.ToInt32(param)));
            PauseGameCommand = new DelegateCommand(param => OnPauseGame());
            BackToMenuCommand = new DelegateCommand(param => OnBackToMenu());

            SaveGameCommand = new DelegateCommand(param => OnSaveGame());
            PauseText = "Start Game";

            GameFields = new ObservableCollection<GameField>();

            RowSize = _outerModel.Model.Map.Fields.GetLength(0);
            ColumnSize = _outerModel.Model.Map.Fields.GetLength(1);

            for (int i = 0; i < RowSize; i++)
            {
                for (int j = 0; j < ColumnSize; j++)
                {
                    GameFields.Add(new GameField
                    {
                        IsPlayer1 = false,
                        IsPlayer1GhostFull = false,
                        IsPlayer1GhostLow = false,
                        IsPlayer1InvincibleFull = false,
                        IsPlayer1InvincibleLow = false,
                        IsPlayer2 = false,
                        IsPlayer2GhostFull = false,
                        IsPlayer2GhostLow = false,
                        IsPlayer2InvincibleFull = false,
                        IsPlayer2InvincibleLow = false,
                        IsPlayer3 = false,
                        IsPlayer3GhostFull = false,
                        IsPlayer3GhostLow = false,
                        IsPlayer3InvincibleFull = false,
                        IsPlayer3InvincibleLow = false,
                        IsWall = false,
                        IsBomb = false,
                        IsDangerZone = false,
                        IsExtraBombs = false,
                        IsBiggerBlast = false,
                        IsDetonator = false,
                        IsRollerSkate = false,
                        IsInvincibility = false,
                        IsGhostPowerup = false,
                        IsBarrier = false,
                        IsSlower = false,
                        IsSmallRange = false,
                        IsNoBomb = false,
                        IsInstantBomb = false,
                        IsDefault = false,
                        IsGhost = false,
                        IsFrankeinstein = false,
                        IsNoob = false
                    });
                }
            }
            if (outerModel.Players.Count == 2)
            {
                //PlayingPlayers = new ObservableCollection<PlayingPlayer> { new PlayingPlayer(outerModel.Players[0]), new PlayingPlayer(outerModel.Players[1]) };
                Player1 = outerModel.Players[0];
                Player2 = outerModel.Players[1];
            }
            else
            {
                //PlayingPlayers = new ObservableCollection<PlayingPlayer> { new PlayingPlayer(outerModel.Players[0]), new PlayingPlayer(outerModel.Players[1]), new PlayingPlayer(outerModel.Players[2]) };
                Player1 = outerModel.Players[0];
                Player2 = outerModel.Players[1];
                Player3 = outerModel.Players[2];
            }
            _round = 1;

            _isGameStarted = false;

            RefreshTable();
        }


        #endregion

        private void Model_Stepping(object? sender, GameStepEventArgs e)
        {
            RefreshTable();
        }

        private void OnSaveGame()
        {
            SaveGame?.Invoke(this, EventArgs.Empty);
        }

        private void Model_GameAdvanced(object? sender, GameEventArgs e)
        {
            OnPropertyChanged(nameof(GameTime));
            RefreshTable();
        }

        private void Model_RoundWinner(object? sender, GameEventArgs e)
        {
            if(e.Player.Id == Player1.Id)
            {
                Player1.Score++;
            }
            else if(e.Player.Id == Player2.Id)
            {
                Player2.Score++;
            }
            else
            {
                Player3.Score++;
            }
        }

        private void Model_NewRound(object? sender, NumberOfSomethingEventArgs e)
        {
            RefreshTable();
            _round++;
            OnPropertyChanged(nameof(Round));
        }

        #region Public methods

        public void RefreshTable()
        {

            for (int i = 0; i < RowSize; i++)
            {
                for (int j = 0; j < ColumnSize; j++)
                {
                    //Is Player1?
                    GameFields[ColumnSize * i + j]
                        .IsPlayer1 = (_outerModel.Model.Map.IsPlayerWithId(new Point(i,j), 1)
                        && ((Player)_outerModel.Model.Map.GetGameObject(new Point(i, j), typeof(Player))).Ghost == 0 
                        && ((Player)_outerModel.Model.Map.GetGameObject(new Point(i, j), typeof(Player))).Invincible == 0);

                    //Is IsPlayer1GhostFull
                    GameFields[ColumnSize * i + j].IsPlayer1GhostFull = (_outerModel.Model.Map.IsPlayerWithId(new Point(i, j), 1)
                        && ((Player)_outerModel.Model.Map.GetGameObject(new Point(i, j), typeof(Player))).Ghost > 3);

                    //Is IsPlayer1GhostLow
                    GameFields[ColumnSize * i + j].IsPlayer1GhostLow = (_outerModel.Model.Map.IsPlayerWithId(new Point(i, j), 1)
                        && ((Player)_outerModel.Model.Map.GetGameObject(new Point(i, j), typeof(Player))).Ghost <= 3 && ((Player)_outerModel.Model.Map.GetGameObject(new Point(i, j), typeof(Player))).Ghost > 0);
                    
                    //Is IsPlayer1InvincibleFull
                    GameFields[ColumnSize * i + j].IsPlayer1InvincibleFull = (_outerModel.Model.Map.IsPlayerWithId(new Point(i, j), 1)
                        && ((Player)_outerModel.Model.Map.GetGameObject(new Point(i, j), typeof(Player))).Invincible > 3);
                    
                    //Is IsPlayer1InvincibleLow
                    GameFields[ColumnSize * i + j].IsPlayer1InvincibleLow = (_outerModel.Model.Map.IsPlayerWithId(new Point(i, j), 1)
                        && ((Player)_outerModel.Model.Map.GetGameObject(new Point(i, j), typeof(Player))).Invincible <= 3 && ((Player)_outerModel.Model.Map.GetGameObject(new Point(i, j), typeof(Player))).Invincible > 0);

                    //Is Player2?
                    GameFields[ColumnSize * i + j]
                        .IsPlayer2 = (_outerModel.Model.Map.IsPlayerWithId(new Point(i, j), 2) 
                        && ((Player)_outerModel.Model.Map.GetGameObject(new Point(i, j), typeof(Player))).Ghost == 0
                        && ((Player)_outerModel.Model.Map.GetGameObject(new Point(i, j), typeof(Player))).Invincible == 0); ;

                    //Is IsPlayer2GhostFull
                    GameFields[ColumnSize * i + j].IsPlayer2GhostFull = (_outerModel.Model.Map.IsPlayerWithId(new Point(i, j), 2)
                        && ((Player)_outerModel.Model.Map.GetGameObject(new Point(i, j), typeof(Player))).Ghost > 3);

                    //Is IsPlayer2GhostLow
                    GameFields[ColumnSize * i + j].IsPlayer2GhostLow = (_outerModel.Model.Map.IsPlayerWithId(new Point(i, j), 2)
                        && ((Player)_outerModel.Model.Map.GetGameObject(new Point(i, j), typeof(Player))).Ghost <= 3 && ((Player)_outerModel.Model.Map.GetGameObject(new Point(i, j), typeof(Player))).Ghost > 0);

                    //Is IsPlayer2InvincibleFull
                    GameFields[ColumnSize * i + j].IsPlayer2InvincibleFull = (_outerModel.Model.Map.IsPlayerWithId(new Point(i, j), 2)
                        && ((Player)_outerModel.Model.Map.GetGameObject(new Point(i, j), typeof(Player))).Invincible > 3);

                    //Is IsPlayer2InvincibleLow                  
                    GameFields[ColumnSize * i + j].IsPlayer2InvincibleLow = (_outerModel.Model.Map.IsPlayerWithId(new Point(i, j), 2)
                        && ((Player)_outerModel.Model.Map.GetGameObject(new Point(i, j), typeof(Player))).Invincible <= 3 && ((Player)_outerModel.Model.Map.GetGameObject(new Point(i, j), typeof(Player))).Invincible > 0);
                    
                    //Is Player3?
                    GameFields[ColumnSize * i + j]
                        .IsPlayer3 = (_outerModel.Model.Map.IsPlayerWithId(new Point(i, j), 3) 
                        && ((Player)_outerModel.Model.Map.GetGameObject(new Point(i, j), typeof(Player))).Ghost == 0
                        && ((Player)_outerModel.Model.Map.GetGameObject(new Point(i, j), typeof(Player))).Invincible == 0); ;

                    //Is IsPlayer3GhostFull
                    GameFields[ColumnSize * i + j].IsPlayer3GhostFull = (_outerModel.Model.Map.IsPlayerWithId(new Point(i, j), 3)
                        && ((Player)_outerModel.Model.Map.GetGameObject(new Point(i, j), typeof(Player))).Ghost > 3);

                    //Is IsPlayer3GhostLow
                    GameFields[ColumnSize * i + j].IsPlayer3GhostLow = (_outerModel.Model.Map.IsPlayerWithId(new Point(i, j), 3)
                        && ((Player)_outerModel.Model.Map.GetGameObject(new Point(i, j), typeof(Player))).Ghost <= 3 && ((Player)_outerModel.Model.Map.GetGameObject(new Point(i, j), typeof(Player))).Ghost > 0);

                    //Is IsPlayer3InvincibleFull
                    GameFields[ColumnSize * i + j].IsPlayer3InvincibleFull = (_outerModel.Model.Map.IsPlayerWithId(new Point(i, j), 3)
                        && ((Player)_outerModel.Model.Map.GetGameObject(new Point(i, j), typeof(Player))).Invincible > 3);

                    //Is IsPlayer3InvincibleLow
                    GameFields[ColumnSize * i + j].IsPlayer3InvincibleLow = (_outerModel.Model.Map.IsPlayerWithId(new Point(i, j), 3)
                        && ((Player)_outerModel.Model.Map.GetGameObject(new Point(i, j), typeof(Player))).Invincible <= 3 && ((Player)_outerModel.Model.Map.GetGameObject(new Point(i, j), typeof(Player))).Invincible > 0);

                    //Is Wall?
                    GameFields[ColumnSize * i + j]
                        .IsWall = _outerModel.Model.Map.Fields[i,j].CheckGameObject(typeof(Wall)) && !(GameFields[ColumnSize * i + j].IsPlayer3 || GameFields[ColumnSize * i + j].IsPlayer2 || GameFields[ColumnSize * i + j].IsPlayer1);
                    
                    //Is Bomb?
                    GameFields[ColumnSize * i + j]
                        .IsBomb = _outerModel.Model.Map.Fields[i, j].CheckGameObject(typeof(Bomb));
                    
                    //Is DangerZone?
                    GameFields[ColumnSize * i + j]
                        .IsDangerZone = _outerModel.Model.Map.Fields[i, j].CheckGameObject(typeof(DangerZone));

                    //Is ExtraBombs?
                    GameFields[ColumnSize * i + j]
                        .IsExtraBombs = _outerModel.Model.Map.Fields[i, j].CheckGameObject(typeof(PowerUp)) && ((PowerUp)_outerModel.Model.Map.Fields[i, j].GetGameObject(typeof(PowerUp))).PowerUps == PowerUps.EXTRABOMBS && !_outerModel.Model.Map.Fields[i, j].CheckGameObject(typeof(DangerZone));

                    //Is BiggerBlast?
                    GameFields[ColumnSize * i + j]
                        .IsBiggerBlast = _outerModel.Model.Map.Fields[i, j].CheckGameObject(typeof(PowerUp)) && ((PowerUp)_outerModel.Model.Map.Fields[i, j].GetGameObject(typeof(PowerUp))).PowerUps == PowerUps.BIGGERBLAST && !_outerModel.Model.Map.Fields[i, j].CheckGameObject(typeof(DangerZone));

                    //Is Detonator?
                    GameFields[ColumnSize * i + j]
                        .IsDetonator = _outerModel.Model.Map.Fields[i, j].CheckGameObject(typeof(PowerUp)) && ((PowerUp)_outerModel.Model.Map.Fields[i, j].GetGameObject(typeof(PowerUp))).PowerUps == PowerUps.DETONATOR && !_outerModel.Model.Map.Fields[i, j].CheckGameObject(typeof(DangerZone));

                    //Is Rollerskate?
                    GameFields[ColumnSize * i + j]
                        .IsRollerSkate = _outerModel.Model.Map.Fields[i, j].CheckGameObject(typeof(PowerUp)) && ((PowerUp)_outerModel.Model.Map.Fields[i, j].GetGameObject(typeof(PowerUp))).PowerUps == PowerUps.ROLLERSKATE && !_outerModel.Model.Map.Fields[i, j].CheckGameObject(typeof(DangerZone));

                    //Is Invincibility?
                    GameFields[ColumnSize * i + j]
                        .IsInvincibility = _outerModel.Model.Map.Fields[i, j].CheckGameObject(typeof(PowerUp)) && ((PowerUp)_outerModel.Model.Map.Fields[i, j].GetGameObject(typeof(PowerUp))).PowerUps == PowerUps.INVINCIBILITY && !_outerModel.Model.Map.Fields[i, j].CheckGameObject(typeof(DangerZone));

                    //Is GhostPowerup?
                    GameFields[ColumnSize * i + j]
                        .IsGhostPowerup = _outerModel.Model.Map.Fields[i, j].CheckGameObject(typeof(PowerUp)) && ((PowerUp)_outerModel.Model.Map.Fields[i, j].GetGameObject(typeof(PowerUp))).PowerUps == PowerUps.GHOST && !_outerModel.Model.Map.Fields[i, j].CheckGameObject(typeof(DangerZone));

                    //Is Barrier?
                    GameFields[ColumnSize * i + j]
                        .IsBarrier = _outerModel.Model.Map.Fields[i, j].CheckGameObject(typeof(PowerUp)) && ((PowerUp)_outerModel.Model.Map.Fields[i, j].GetGameObject(typeof(PowerUp))).PowerUps == PowerUps.BARRIER && !_outerModel.Model.Map.Fields[i, j].CheckGameObject(typeof(DangerZone));

                    //Is Slower?
                    GameFields[ColumnSize * i + j]
                        .IsSlower = _outerModel.Model.Map.Fields[i, j].CheckGameObject(typeof(PowerUp)) && ((PowerUp)_outerModel.Model.Map.Fields[i, j].GetGameObject(typeof(PowerUp))).PowerUps == PowerUps.SLOWER && !_outerModel.Model.Map.Fields[i, j].CheckGameObject(typeof(DangerZone));

                    //Is SmallRange?
                    GameFields[ColumnSize * i + j]
                        .IsSmallRange = _outerModel.Model.Map.Fields[i, j].CheckGameObject(typeof(PowerUp)) && ((PowerUp)_outerModel.Model.Map.Fields[i, j].GetGameObject(typeof(PowerUp))).PowerUps == PowerUps.SMALLRANGE && !_outerModel.Model.Map.Fields[i, j].CheckGameObject(typeof(DangerZone));

                    //Is NoBomb?
                    GameFields[ColumnSize * i + j]
                        .IsNoBomb = _outerModel.Model.Map.Fields[i, j].CheckGameObject(typeof(PowerUp)) && ((PowerUp)_outerModel.Model.Map.Fields[i, j].GetGameObject(typeof(PowerUp))).PowerUps == PowerUps.NOBOMB && !_outerModel.Model.Map.Fields[i, j].CheckGameObject(typeof(DangerZone));

                    //Is InstantBomb?
                    GameFields[ColumnSize * i + j]
                        .IsInstantBomb = _outerModel.Model.Map.Fields[i, j].CheckGameObject(typeof(PowerUp)) && ((PowerUp)_outerModel.Model.Map.Fields[i, j].GetGameObject(typeof(PowerUp))).PowerUps == PowerUps.INSTANTBOMB && !_outerModel.Model.Map.Fields[i, j].CheckGameObject(typeof(DangerZone));

                    //Is Box?
                    GameFields[ColumnSize * i + j]
                        .IsBox = _outerModel.Model.Map.Fields[i, j].CheckGameObject(typeof(Box)) && !(GameFields[ColumnSize * i + j].IsPlayer3 || GameFields[ColumnSize * i + j].IsPlayer2 || GameFields[ColumnSize * i + j].IsPlayer1);

                    //Is Default?
                    GameFields[ColumnSize * i + j]
                        .IsDefault = _outerModel.Model.Map.Fields[i, j].CheckGameObject(typeof(Default));

                    GameFields[ColumnSize * i + j]
                        .IsGhost = _outerModel.Model.Map.Fields[i, j].CheckGameObject(typeof(Ghost));

                    GameFields[ColumnSize * i + j]
                        .IsFrankeinstein = _outerModel.Model.Map.Fields[i, j].CheckGameObject(typeof(FrankEinstein));

                    GameFields[ColumnSize * i + j]
                        .IsNoob = _outerModel.Model.Map.Fields[i, j].CheckGameObject(typeof(Noob));

                }
            }
            OnPropertyChanged(nameof(Player1));
            OnPropertyChanged(nameof(Player2));
            OnPropertyChanged(nameof(Player3));
            OnPropertyChanged(nameof(PauseText));
            OnPropertyChanged(nameof(GameTime));
        }

        #endregion

        #region Private methods

        private void OnPauseGame()
        {
            if(_outerModel.Model != null)
            {
                if (_outerModel.Model.IsTimerEnabled() && !_outerModel.Model.IsGameOver)
                {
                    _outerModel.Model.StopTimer();
                    PauseText = "Continue";
                    IsGameStarted = false;
                }
                else if (_outerModel.Model.IsTimerEnabled() && _outerModel.Model.IsGameOver)
                {
                    _outerModel.Model.StopTimer();
                    PauseText = "Start Game";
                    IsGameStarted = false;
                }
                else if (!_outerModel.Model.IsTimerEnabled() && !_outerModel.Model.IsGameOver)
                {
                    _outerModel.Model.StartTimer();
                    PauseText = "Pause";
                    IsGameStarted = true;
                }
                OnPropertyChanged(nameof(PauseText));
            }

            /* WHEN THE GAME IS MAINLY DONE, THIS CODE SHOULD BE USED INSTEAD!
            if (!_outerModel.Model.IsGameOver)
            {
                if (_outerModel.Model.IsTimerEnabled())
                {
                    _outerModel.Model.StopTimer();
                }

                else
                {
                    _outerModel.Model.StartTimer();
                }
            }
            else
            {
                _outerModel.Model.StopTimer();
            }
            */
        }

        private void OnBackToMenu()
        {
            _outerModel.Model.StopTimer();
            _outerModel.Model.StopTimer2();
            BackToMenu?.Invoke(this, EventArgs.Empty);
        }

        private void OnStepUp(int param)
        {
            _outerModel.Model.StepPlayer(_outerModel.GetPlayerByID(param), Direction.UP);
            RefreshTable();
        }

        private void OnStepDown(int param)
        {
            _outerModel.Model.StepPlayer(_outerModel.GetPlayerByID(param), Direction.DOWN);
            RefreshTable();
        }

        private void OnStepRight(int param)
        {
            _outerModel.Model.StepPlayer(_outerModel.GetPlayerByID(param), Direction.RIGHT);
            RefreshTable();
        }

        private void OnStepLeft(int param)
        {
            _outerModel.Model.StepPlayer(_outerModel.GetPlayerByID(param), Direction.LEFT);
            RefreshTable();
        }

        private void OnPlantBomb(int param)
        {
            _outerModel.Model.SetBomb(_outerModel.GetPlayerByID(param));
            RefreshTable();
        }

        private void OnPlaceBox(int param)
        {
            _outerModel.Model.PlaceBox(_outerModel.GetPlayerByID(param));
            RefreshTable();
        }

        #endregion
    }
}
