using Model.EventArgs;
using Persistence;
using Persistence.Enums;
using Persistence.GameObjects;
using Persistence.Monsters;
using Persistence.Structures;
using System.Drawing;
using System.Timers;

namespace Model
{
    public class GameModel //ONLY THE SCHEMA WAS MADE FOLLOWING THE UML
    {
        #region Fields

        private Map? _map;
        private int _numOfPlayers;
        private List<Player> _players;
        private List<Monster> _monsters;
        private System.Timers.Timer _timer;
        private System.Timers.Timer _timer2;
        private int _gameTime;
        private List<Player> _deadPlayers;
        private int _timeAfterDeath;
        private int _deadPlayerCounter;
        private bool _isGameOver;
        private int? _numberOfWins;
        private int _stepTime;
        private int[] _steppedPlayers;

        static int currentRow = 1;
        static int currentCol = 1;
        static int targetRow = 3;
        static int targetCol = 3;
        static int directionIndex = 0;
        static int[] dRow = { 0, 1, 0, -1 }; // Right, Down, Left, Up
        static int[] dCol = { 1, 0, -1, 0 }; // Right, Down, Left, Up
        static int rankOfCircle = 0;
        static int counter = 0;
        static bool battleRoyalInProgress;
        static int phaseCounter;

        #endregion

        #region Properties

        /// <summary>
        /// Returns current map
        /// </summary>
        public Map? Map { get { return _map; } }

        /// <summary>
        /// Returns number of players
        /// </summary>
        public int NumOfPlayers { get { return _numOfPlayers; } }

        /// <summary>
        /// Returns players
        /// </summary>
        public List<Player> Players { get { return _players; } }

        /// <summary>
        /// Returns monsters
        /// </summary>
        public List<Monster> Monsters { get { return _monsters; } }

        /// <summary>
        /// Returns game time
        /// </summary>
        public int GameTime { get { return _gameTime; }  set { _gameTime = value; } }

        /// <summary>
        /// Returns true if game is over
        /// </summary>
        public bool IsGameOver { get { return _isGameOver; } set { _isGameOver = value; } }

        /// <summary>
        /// Returns number of wins
        /// </summary>
        public int? NumberOfWins { get { return _numberOfWins; } set { _numberOfWins = value; } }

        #endregion

        #region Constructor

        /// <summary>
        /// Creating Gamemodel object
        /// </summary>
        /// <param name="map"></param>
        /// <param name="players"></param>
        /// <param name="numberOfWins"></param>
        public GameModel(Map? map, List<Player>? players, int? numberOfWins)
        {
            _map = map;
            _numOfPlayers = players.Count;
            _players = players;
            //_playersToMove = new Player[4];
            battleRoyalInProgress = false;
            phaseCounter = 0;
            _monsters = new List<Monster> { };

            //making a timer which controls the enemystepping and shows the elapsed time
            _timer = new System.Timers.Timer(500);
            _timer.Elapsed += new ElapsedEventHandler(Timer_Tick);
            _timer.AutoReset = true;

            _timer2 = new System.Timers.Timer(1000);
            _timer2.Elapsed += new ElapsedEventHandler(Timer_Tick_After_Death);
            _timer2.AutoReset = true;

            _gameTime = 180;

            _deadPlayers = new List<Player>();
            _timeAfterDeath = 3;
            _deadPlayerCounter = 0;
            _isGameOver = false;
            _numberOfWins = numberOfWins;
            _stepTime = 0;
            _steppedPlayers = [0, 0, 0];

        }

        #endregion

        #region Events

        public event EventHandler<GameStepEventArgs>? Step;
        public event EventHandler<GameEventArgs>? RoundOver;
        public event EventHandler<GameEventArgs>? RoundWinner;
        public event EventHandler<GameEventArgs>? GameAdvanced;
        public event EventHandler PauseGame;

        #endregion

        #region Timer methods

        /// <summary>
        /// Gamemodel timer tick event, main gametime
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_Tick(Object? sender, ElapsedEventArgs e) //if the timer 'ticks', one enemy stepping will reveal
        {
            if (_stepTime % 2 == 0)
            {
                if (_gameTime > 0)
                {
                    _gameTime--;
                }
                if (_gameTime == 0)
                {
                    phaseCounter++;
                    battleRoyalInProgress = true;
                    _gameTime = 120;
                    if (phaseCounter == 2)
                    {
                        _gameTime = 0;
                        _timer.Stop();
                        OnRoundOver(true); // ???
                    }
                }
                if (battleRoyalInProgress == true)
                {

                    if (currentRow == targetRow && currentCol == targetCol)
                    {
                        battleRoyalInProgress = false;
                    }
                    else
                    {
                        if (Map.CheckGameObjectType(new Point(currentRow, currentCol), typeof(Player)))
                        {
                            Player? player = (Map.GetGameObject(new Point(currentRow, currentCol), typeof(Player)) as Player);
                            foreach (var p in _players)
                            {
                                if (p.Id == player?.Id)
                                {
                                    p.IsAlive = false;
                                }
                            }
                            _deadPlayerCounter++;
                            if(player != null)
                                _deadPlayers.Add(player);
                            if (_deadPlayerCounter == _numOfPlayers - 1)
                            {
                                StartTimer2();
                            }
                        }
                        Map.RemoveAllGameObject(new Point(currentRow, currentCol));
                        Map.AddGameObject(new Point(currentRow, currentCol), new Wall());

                        int nextRow = currentRow + dRow[directionIndex];
                        int nextCol = currentCol + dCol[directionIndex];

                        if (nextRow > 0 + rankOfCircle && nextRow < 12 - rankOfCircle && nextCol > 0 + rankOfCircle && nextCol < 12 - rankOfCircle)
                        {
                            currentRow = nextRow;
                            currentCol = nextCol;

                        }
                        else
                        {
                            directionIndex = (directionIndex + 1) % 4;
                            counter++;
                            currentRow += dRow[directionIndex];
                            currentCol += dCol[directionIndex];
                            if (counter == 4)
                            {
                                counter = 0;
                                rankOfCircle++;
                                currentRow = rankOfCircle + 1;
                                currentCol = rankOfCircle + 1;
                            }
                        }
                    }
                }
            }

            _stepTime++;

            for (int i = 0; i < 3; i++)
            {
                if (_steppedPlayers[i] > 0)
                    _steppedPlayers[i]--;
            }
            OnGameAdvanced();
        }

        /// <summary>
        /// Time that allepsed after 1 player left
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_Tick_After_Death(Object? sender, ElapsedEventArgs e)
        {
            if (_timeAfterDeath > 0)
            {
                _timeAfterDeath--;
            }
            else
            {
                bool isEveryOneOut = true;
                Player winner = null!;
                foreach (var player in _players)
                {
                    if (player.IsAlive)
                    {
                        winner = player;
                        isEveryOneOut = false;
                        break;
                    }
                }

                StopTimer();
                StopTimer2();

                if (isEveryOneOut)
                {
                    _isGameOver = true;
                    OnPauseGame();
                    OnRoundOver(isEveryOneOut);
                }
                else
                {
                    _isGameOver = true;
                    OnPauseGame();
                    OnRoundOver(winner);
                    OnRoundWinner(winner);
                }
            }
        }

        /// <summary>
        /// Start main timer
        /// </summary>
        public void StartTimer()
        {
            if (_timer.Enabled)
                return;
            _timer.Start();
            StartMonsters();
        }

        /// <summary>
        /// Stop main timer
        /// </summary>
        public void StopTimer()
        {
            if (!_timer.Enabled)
                return;
            _timer.Stop();
            StopMonsters();
        }

        /// <summary>
        /// Start wait some time till end timer starter
        /// </summary>
        public void StartTimer2()
        {
            if (_timer2.Enabled)
                return;
            _timer2.Start();
        }

        /// <summary>
        /// Start wait some time till end timer stopper
        /// </summary>
        public void StopTimer2()
        {
            if (!_timer2.Enabled)
                return;
            _timer2.Stop();
            StopMonsters();
        }

        /// <summary>
        /// Returns true if main timer is enabled
        /// </summary>
        /// <returns></returns>
        public Boolean IsTimerEnabled()
        {
            return _timer.Enabled;
        }

        #endregion

        #region Public methods
        /// <summary>
        /// Generates a hardcoded map, based on the chosen map design and starts the game
        /// </summary>
        public void NewGame(int? playingPlayers)
        {
            _isGameOver = false;
            for (int i = 0; i < playingPlayers; i++)
            {
                _players[i].IsAlive = true;
            }
            switch (Map?.MapType)
            {
                case MapType.TYPE1:
                    GenerateMapType1(playingPlayers);
                    break;
                case MapType.TYPE2:
                    GenerateMapType2(playingPlayers);
                    break;
                case MapType.TYPE3:
                    GenerateMapType3(playingPlayers);
                    break;
                case MapType.TUTORIAL:
                    GenerateTestMap(playingPlayers);
                    break;
            }
        }

        /// <summary>
        /// Stepping with the player on the field
        /// </summary>
        /// <param name="player"></param>
        /// <param name="direction"></param>
        public void StepPlayer(Player? player, Direction direction)
        {
            if (_timer.Enabled && player.IsAlive) // if the timer does not elapse, the game is in pause, so nothing can move
            {
                try
                {
                    player.DieInStructure += DieInStructure;
                    //Finding the player
                    Point playerCoords = new Point(0, 0);
                    if (_map != null)
                        playerCoords = _map.FindPlayer(player);

                    //Calculating the destination
                    Point coord = new Point();
                    switch (direction)
                    {
                        case Direction.UP:
                            coord = new Point(-1, 0);
                            break;

                        case Direction.DOWN:
                            coord = new Point(1, 0);
                            break;
                        case Direction.LEFT:
                            coord = new Point(0, -1);
                            break;

                        case Direction.RIGHT:
                            coord = new Point(0, 1);
                            break;
                    }

                    Point newCoordinate = new Point(playerCoords.X + coord.X, playerCoords.Y + coord.Y);

                    if (_steppedPlayers[player.Id - 1] == 0)
                    {
                        bool bomb = _map.MovePlayer(playerCoords, newCoordinate, player);
                        if (bomb)
                        {
                            SetBomb(player);
                        }
                        OnStepping(newCoordinate);
                        if (player.RollerSakte && player.Slower == 0)
                        {
                            _steppedPlayers[player.Id - 1] = 1;
                        }
                        else if (player.Slower > 0)
                        {
                            _steppedPlayers[player.Id - 1] = 3;
                        }
                        else
                        {
                            _steppedPlayers[player.Id - 1] = 2;
                        }
                    }
                }
                catch { return; }
                // if there was something wrong (could not find the player, player cannot step on the field), nothing happens
            }
        }

        #region Monster methods
        /// <summary>
        /// Setting random monster on the field to the given coordinates
        /// </summary>
        /// <param name="coord"></param>
        public void SetMonster(Point coord) //sets a random smonster
        {
            Random random = new Random();

            Monster monster;
            switch (random.Next(1, 4))
            {
                case 1:
                    monster = new Default(1, coord);
                    Map.AddGameObject(coord, monster);
                    _monsters.Add(monster);
                    monster.MonsterStep += new EventHandler<Persistence.Monsters.MonsterEventArgs>(StepMonster);
                    break;

                case 2:
                    monster = new Ghost(1, coord);
                    Map.AddGameObject(coord, monster);
                    _monsters.Add(monster);
                    monster.MonsterStep += new EventHandler<Persistence.Monsters.MonsterEventArgs>(StepMonster);
                    break;

                case 3:
                    monster = new FrankEinstein(1, coord);
                    Map.AddGameObject(coord, monster);
                    _monsters.Add(monster);
                    monster.MonsterStep += new EventHandler<Persistence.Monsters.MonsterEventArgs>(StepMonster);
                    break;

                case 4:
                    monster = new Noob(1, coord);
                    Map.AddGameObject(coord, monster);
                    _monsters.Add(monster);
                    monster.MonsterStep += new EventHandler<Persistence.Monsters.MonsterEventArgs>(StepMonster);
                    break;
            }
        }

        /// <summary>
        /// Setting up, initialize the given monster
        /// </summary>
        /// <param name="monster"></param>
        public void SetMonster(Monster monster) //sets a specific monster
        {
            switch (monster)
            {
                case Default:
                    monster = new Default(1, monster.Coord);
                    Map.AddGameObject(monster.Coord, monster);
                    _monsters.Add(monster);
                    monster.MonsterStep += new EventHandler<Persistence.Monsters.MonsterEventArgs>(StepMonster);
                    break;

                case Ghost:
                    monster = new Ghost(1, monster.Coord);
                    Map.AddGameObject(monster.Coord, monster);
                    _monsters.Add(monster);
                    monster.MonsterStep += new EventHandler<Persistence.Monsters.MonsterEventArgs>(StepMonster);
                    break;

                case FrankEinstein:
                    monster = new FrankEinstein(1, monster.Coord);
                    Map.AddGameObject(monster.Coord, monster);
                    _monsters.Add(monster);
                    monster.MonsterStep += new EventHandler<Persistence.Monsters.MonsterEventArgs>(StepMonster);
                    break;

                case Noob:
                    monster = new Noob(1, monster.Coord);
                    Map.AddGameObject(monster.Coord, monster);
                    _monsters.Add(monster);
                    monster.MonsterStep += new EventHandler<Persistence.Monsters.MonsterEventArgs>(StepMonster);
                    break;
            }
        }

        /// <summary>
        /// Step with the monsters logic
        /// </summary>
        /// <param name="monster"></param>
        private void StepMonster(Monster monster)
        {
            // DEFAULT MONSTER
            if (monster is Default @default) //case of the monster is a default monster
            {
                CasualMonsterStep(@default);
            }

            //GHOST MONSTER
            else if (monster is Ghost ghost) //case of the monster is a ghost
            {
                Direction? direction = ghost.Direction;

                Point oldCoord = ghost.Coord;

                Point coord = new Point();
                switch (direction)
                {
                    case Direction.UP:
                        coord = new Point(-1, 0);
                        break;

                    case Direction.DOWN:
                        coord = new Point(1, 0);
                        break;

                    case Direction.LEFT:
                        coord = new Point(0, -1);
                        break;

                    case Direction.RIGHT:
                        coord = new Point(0, 1);
                        break;
                }

                Point newCoordinate = new Point(oldCoord.X + coord.X, oldCoord.Y + coord.Y);


                // case of ghost is heading to the edge of the map
                if (newCoordinate.X * newCoordinate.Y == 0
                    ||
                    newCoordinate.X * newCoordinate.Y % (Map.Size - 1) == 0)
                {
                    switch (direction)
                    {
                        case Direction.UP:
                            ghost.ChangeDirection(Direction.RIGHT);
                            break;

                        case Direction.DOWN:
                            ghost.ChangeDirection(Direction.LEFT);
                            break;

                        case Direction.LEFT:
                            ghost.ChangeDirection(Direction.UP);
                            break;

                        case Direction.RIGHT:
                            ghost.ChangeDirection(Direction.DOWN);
                            break;
                    }
                    CasualMonsterStep(ghost);
                    return;
                }

                Random random = new Random();

                if (

                       (
                           Map.Fields[newCoordinate.X, newCoordinate.Y].CheckGameObject(typeof(Wall))
                           ||
                           Map.Fields[newCoordinate.X, newCoordinate.Y].CheckGameObject(typeof(Box))
                       )
                       &&
                       !
                       (
                           newCoordinate.X * newCoordinate.Y == 0
                           ||
                           newCoordinate.X * newCoordinate.Y % (Map.Size - 1) == 0
                       )
                       &&
                       (
                           random.Next(1, 10) != 10 //surprise factor: 10% to not going through the wall
                       )
                    )
                {
                    switch (direction)
                    {
                        case Direction.UP:
                            int k = newCoordinate.X;

                            while ((k > 0)
                                && !(Map.Fields[k, newCoordinate.Y].CheckGameObject(typeof(Bomb)))
                                && !(Map.Fields[k, newCoordinate.Y].IsEmpty
                                    || Map.Fields[k, newCoordinate.Y].IsPlayerOrPowerUp
                                    || Map.Fields[k, newCoordinate.Y].CheckGameObject(typeof(Monster))))
                            {
                                k--;
                            }

                            if (k == 0 || Map.Fields[k, newCoordinate.Y].CheckGameObject(typeof(Bomb)))
                            {
                                ghost.ChangeDirection(Direction.RIGHT);
                            }
                            else
                            {
                                newCoordinate.X = k;
                                Map.MoveGameObject(oldCoord, newCoordinate, ghost);
                                ghost.ChangeCoord(newCoordinate);
                                OnStepping(newCoordinate);
                            }
                            break;

                        case Direction.DOWN:
                            k = newCoordinate.X;

                            while ((k < (Map.Size - 1))
                                && !(Map.Fields[k, newCoordinate.Y].CheckGameObject(typeof(Bomb)))
                                && !(Map.Fields[k, newCoordinate.Y].IsEmpty
                                    || Map.Fields[k, newCoordinate.Y].IsPlayerOrPowerUp
                                    || Map.Fields[k, newCoordinate.Y].CheckGameObject(typeof(Monster))))
                            {
                                k++;
                            }

                            if (k == (Map.Size - 1) || Map.Fields[k, newCoordinate.Y].CheckGameObject(typeof(Bomb)))
                            {
                                // hitting the wall
                                ghost.ChangeDirection(Direction.LEFT);
                            }
                            else
                            {
                                newCoordinate.X = k;
                                Map.MoveGameObject(oldCoord, newCoordinate, ghost);
                                ghost.ChangeCoord(newCoordinate);
                                OnStepping(newCoordinate);
                            }
                            break;

                        case Direction.LEFT:
                            k = newCoordinate.Y;
                            while ((k > 0)
                                && !(Map.Fields[newCoordinate.X, k].CheckGameObject(typeof(Bomb)))
                                && !(Map.Fields[newCoordinate.X, k].IsEmpty
                                    || Map.Fields[newCoordinate.X, k].IsPlayerOrPowerUp
                                    || Map.Fields[newCoordinate.X, k].CheckGameObject(typeof(Monster))))
                            {
                                k--;
                            }

                            if (k == 0 || Map.Fields[newCoordinate.X, k].CheckGameObject(typeof(Bomb)))
                            {
                                // hitting the wall
                                ghost.ChangeDirection(Direction.UP);
                            }
                            else
                            {
                                newCoordinate.Y = k;
                                Map.MoveGameObject(oldCoord, newCoordinate, ghost);
                                ghost.ChangeCoord(newCoordinate);
                                OnStepping(newCoordinate);
                            }
                            break;

                        case Direction.RIGHT:
                            k = newCoordinate.Y;
                            while ((k < (Map.Size - 1))
                                && !(Map.Fields[newCoordinate.X, k].CheckGameObject(typeof(Bomb)))
                                && !(Map.Fields[newCoordinate.X, k].IsEmpty
                                    || Map.Fields[newCoordinate.X, k].IsPlayerOrPowerUp
                                    || Map.Fields[newCoordinate.X, k].CheckGameObject(typeof(Monster))))
                            {
                                k++;

                            }

                            if (k == (Map.Size - 1) || Map.Fields[newCoordinate.X, k].CheckGameObject(typeof(Bomb)))
                            {
                                // hitting the wall
                                ghost.ChangeDirection(Direction.DOWN);
                            }
                            else
                            {
                                newCoordinate.Y = k;
                                Map.MoveGameObject(oldCoord, newCoordinate, ghost);
                                ghost.ChangeCoord(newCoordinate);
                                OnStepping(newCoordinate);
                            }
                            break;
                    }
                }
                else
                {
                    CasualMonsterStep(ghost);
                }
            }

            //FRANKEINSTEIN MONSTER
            else if (monster is FrankEinstein frankEinstein)
            {
                Point coord = new Point();
                switch (frankEinstein.Direction)
                {
                    case Direction.UP:
                        coord = new Point(-1, 0);
                        break;

                    case Direction.DOWN:
                        coord = new Point(1, 0);
                        break;

                    case Direction.LEFT:
                        coord = new Point(0, -1);
                        break;

                    case Direction.RIGHT:
                        coord = new Point(0, 1);
                        break;
                }

                Point newCoordinate = new Point(frankEinstein.Coord.X + coord.X, frankEinstein.Coord.Y + coord.Y);
                if (Map.Fields[newCoordinate.X, newCoordinate.Y].IsStructure && !frankEinstein.IsFollowingAPlayer)
                {

                    //Finding the closest player to the monster

                    Player closestPlayer = new Player(-1); //fake player for initializing, this variable will be a real player
                    double shortestDistance = -1; //fake distance for initializing

                    for (int i = 0; i < _players.Count; i++)
                    {
                        if (_players[i].IsAlive)
                        {
                            double currentDistance =
                            Math.Sqrt(Math.Pow(_map.FindPlayer(_players[i]).X - frankEinstein.Coord.X, 2)
                            +
                            Math.Pow(_map.FindPlayer(_players[i]).Y - frankEinstein.Coord.Y, 2));

                            if (_players[i].IsAlive && (i == 0 || shortestDistance > currentDistance))
                            {
                                closestPlayer = _players[i];
                                shortestDistance = currentDistance;
                            }
                        }
                    }
                    if (shortestDistance >= 0)
                    {
                        Point newStepDifference = new Point(-1 * (newCoordinate.X - frankEinstein.Coord.X), -1 * (newCoordinate.Y - frankEinstein.Coord.Y));

                        if (newStepDifference == new Point(0, 1))
                            frankEinstein.ChangeDirection(Direction.DOWN);
                        else if (newStepDifference == new Point(0, -1))
                            frankEinstein.ChangeDirection(Direction.UP);
                        else if (newStepDifference == new Point(1, 0))
                            frankEinstein.ChangeDirection(Direction.RIGHT);
                        else
                            frankEinstein.ChangeDirection(Direction.LEFT);

                        newCoordinate = new Point(monster.Coord.X + newStepDifference.X, monster.Coord.Y + newStepDifference.Y);
                        
                        if (!Map.Fields[newCoordinate.X, newCoordinate.Y].IsStructure)
                        {
                            Map.MoveGameObject(monster.Coord, newCoordinate, monster);
                            monster.ChangeCoord(newCoordinate);
                            OnStepping(newCoordinate);
                        }

                        frankEinstein.MonsterStartsToFollowAPlayer(closestPlayer);

                        return;
                    }

                }

                if (Map.Fields[newCoordinate.X, newCoordinate.Y].IsStructure)
                {

                    if (coord == new Point(0, 1))
                    {
                        frankEinstein.ChangeDirection(Direction.LEFT);
                        coord = new Point(-1, 0);
                    }

                    else if (coord == new Point(0, -1))
                    {
                        frankEinstein.ChangeDirection(Direction.RIGHT);
                        coord = new Point(1, 0);
                    }

                    else if (coord == new Point(1, 0))
                    {
                        frankEinstein.ChangeDirection(Direction.DOWN);
                        coord = new Point(0, 1);
                    }

                    else
                    {
                        frankEinstein.ChangeDirection(Direction.UP);
                        coord = new Point(0, -1);
                    }


                    newCoordinate = new Point(monster.Coord.X + coord.X, monster.Coord.Y + coord.Y);

                    if (!Map.Fields[newCoordinate.X, newCoordinate.Y].IsStructure)
                    {
                        Map.MoveGameObject(monster.Coord, newCoordinate, monster);
                        monster.ChangeCoord(newCoordinate);
                        OnStepping(newCoordinate);
                    }

                    return;
                }

                if (frankEinstein.IsFollowingAPlayer)
                {
                    Point playerCoord = _map.FindPlayer(frankEinstein.FollowedPlayer);

                    if (Math.Abs(playerCoord.X - frankEinstein.Coord.X) >= Math.Abs(playerCoord.Y - frankEinstein.Coord.Y))
                    {
                        if (playerCoord.X > frankEinstein.Coord.X)
                            frankEinstein.ChangeDirection(Direction.DOWN);
                        else
                            frankEinstein.ChangeDirection(Direction.UP);
                    }
                    else
                    {
                        if (playerCoord.Y > frankEinstein.Coord.Y)
                            frankEinstein.ChangeDirection(Direction.RIGHT);
                        else
                            frankEinstein.ChangeDirection(Direction.LEFT);
                    }



                    Map.MoveGameObject(monster.Coord, newCoordinate, monster);
                    monster.ChangeCoord(newCoordinate);
                    OnStepping(newCoordinate);



                    frankEinstein.TakenStepByMonsterWhenFollowsPlayer();

                    if (frankEinstein.ActualStepWhenFollowingPlayer == 0)
                        frankEinstein.MonsterStopsToFollowAPlayer();
                }

                else
                {
                    CasualMonsterStep(frankEinstein);
                }
            }

            //NOOB MONSTER
            else if (monster is Noob noob)
            {
                Point coord = new Point();
                switch (noob.Direction)
                {
                    case Direction.UP:
                        coord = new Point(-1, 0);
                        break;

                    case Direction.DOWN:
                        coord = new Point(1, 0);
                        break;

                    case Direction.LEFT:
                        coord = new Point(0, -1);
                        break;

                    case Direction.RIGHT:
                        coord = new Point(0, 1);
                        break;
                }

                Point newCoordinate = new Point(noob.Coord.X + coord.X, noob.Coord.Y + coord.Y);
                if (Map.Fields[newCoordinate.X, newCoordinate.Y].IsStructure && !noob.IsFollowingAPlayer)
                {

                    //Finding the closest player to the monster

                    Player closestPlayer = new Player(-1); //fake player for initializing, this variable will be a real player
                    double shortestDistance = -1; //fake distance for initializing

                    for (int i = 0; i < _players.Count; i++)
                    {
                        if (_players[i].IsAlive)
                        {
                            double currentDistance =
                            Math.Sqrt(Math.Pow(_map.FindPlayer(_players[i]).X - noob.Coord.X, 2)
                            +
                            Math.Pow(_map.FindPlayer(_players[i]).Y - noob.Coord.Y, 2));

                            if (_players[i].IsAlive && (i == 0 || shortestDistance > currentDistance))
                            {
                                closestPlayer = _players[i];
                                shortestDistance = currentDistance;
                            }
                        }
                    }
                    if (shortestDistance >= 0)
                    {
                        Point newStepDifference = new Point(-1 * (newCoordinate.X - noob.Coord.X), -1 * (newCoordinate.Y - noob.Coord.Y));

                        if (newStepDifference == new Point(0, 1))
                            noob.ChangeDirection(Direction.DOWN);
                        else if (newStepDifference == new Point(0, -1))
                            noob.ChangeDirection(Direction.UP);
                        else if (newStepDifference == new Point(1, 0))
                            noob.ChangeDirection(Direction.RIGHT);
                        else
                            noob.ChangeDirection(Direction.LEFT);

                        newCoordinate = new Point(monster.Coord.X + newStepDifference.X, monster.Coord.Y + newStepDifference.Y);

                        if (!Map.Fields[newCoordinate.X, newCoordinate.Y].IsStructure)
                        {
                            Map.MoveGameObject(monster.Coord, newCoordinate, monster);
                            monster.ChangeCoord(newCoordinate);
                            OnStepping(newCoordinate);
                        }
                        noob.MonsterStartsToFollowAPlayer(closestPlayer);

                        return;
                    }

                }

                if (Map.Fields[newCoordinate.X, newCoordinate.Y].IsStructure)
                {

                    if (coord == new Point(0, 1))
                    {
                        noob.ChangeDirection(Direction.LEFT);
                        coord = new Point(-1, 0);
                    }

                    else if (coord == new Point(0, -1))
                    {
                        noob.ChangeDirection(Direction.RIGHT);
                        coord = new Point(1, 0);
                    }

                    else if (coord == new Point(1, 0))
                    {
                        noob.ChangeDirection(Direction.DOWN);
                        coord = new Point(0, 1);
                    }

                    else
                    {
                        noob.ChangeDirection(Direction.UP);
                        coord = new Point(0, -1);
                    }

                    newCoordinate = new Point(monster.Coord.X + coord.X, monster.Coord.Y + coord.Y);

                    if (!Map.Fields[newCoordinate.X, newCoordinate.Y].IsStructure)
                    {
                        Map.MoveGameObject(monster.Coord, newCoordinate, monster);
                        monster.ChangeCoord(newCoordinate);
                        OnStepping(newCoordinate);
                    }
                    
                    return;
                }

                Random rnd = new Random();

                if (noob.IsFollowingAPlayer && rnd.Next(1,10) > 2) //80% chance for going the wrong way.
                {
                    Point playerCoord = _map.FindPlayer(noob.FollowedPlayer);

                    if (Math.Abs(playerCoord.X - noob.Coord.X) >= Math.Abs(playerCoord.Y - noob.Coord.Y))
                    {
                        if (playerCoord.X < noob.Coord.X)
                            noob.ChangeDirection(Direction.DOWN);
                        else
                            noob.ChangeDirection(Direction.UP);
                    }
                    else
                    {
                        if (playerCoord.Y < noob.Coord.Y)
                            noob.ChangeDirection(Direction.RIGHT);
                        else
                            noob.ChangeDirection(Direction.LEFT);
                    }



                    Map.MoveGameObject(monster.Coord, newCoordinate, monster);
                    monster.ChangeCoord(newCoordinate);
                    OnStepping(newCoordinate);



                    noob.TakenStepByMonsterWhenFollowsPlayer();

                    if (noob.ActualStepWhenFollowingPlayer == 0)
                        noob.MonsterStopsToFollowAPlayer();
                }

                else
                {
                    CasualMonsterStep(noob);
                }
            }

            //case of a monster catches player
            if (_map.CheckGameObjectType(monster.Coord, typeof(Player)))
            {
                Player player = (Player)Map.GetGameObject(monster.Coord, typeof(Player));
                int id = player.Id;

                if (player.IsAlive)
                {
                    foreach (var p in _players)
                    {
                        if (p.Id == id)
                        {
                            p.IsAlive = false;
                        }
                    }
                    _deadPlayerCounter++;
                    _deadPlayers.Add(player);

                    if (_deadPlayerCounter == _numOfPlayers - 1)
                    {
                        StopTimer();
                        StartTimer2();
                    }
                }
            }
        }
        

        #endregion
        /// <summary>
        /// Setting a bomb with the given player to the field
        /// </summary>
        /// <param name="player"></param>
        public void SetBomb(Player player)
        {
            if (_timer.Enabled && player.IsAlive) // if the timer does not elapse, the game is in pause, so nothing can move
            {
                Point rowcol = _map.FindPlayer(player);

                var bomb = player.Plant(rowcol, _map, player);
                if (bomb != null)
                    bomb.Explode += SetCentralDangerZone;
            }
        }

        /// <summary>
        /// Setting a box with the given player to the field
        /// </summary>
        /// <param name="player"></param>
        public void PlaceBox(Player player)
        {
            if (_timer.Enabled)
            {
                Point rowcol = _map.FindPlayer(player);

                player.PlaceBox(rowcol, _map, player);
            }
        }

        #endregion

        #region Private methods

        #region Map generation methods based on mapType

        // Test Map Type - this is just for testing, and this won't be in the game.

        private void GenerateTestMap(int? playingPlayers)
        {
            for (int i = 0; i < 13; i++)
            {
                for (int j = 0; j < 13; j++)
                {
                    if ((i == 0 || i == 12 || j == 0 || j == 12) || (i == 5 && j < 10 && j > 3) || (j == 5 && i > 5 && i < 12))
                    {
                        Map.SetGameObject(new Point(i, j), new Wall());
                    }
                }
            }

            Map.SetGameObject(new Point(1, 1), new Player(1));
            Map.SetGameObject(new Point(1, 11), new Player(2));
            if (playingPlayers == 3)
                Map.SetGameObject(new Point(11, 1), new Player(3));

            SetMonster(new Ghost(1, new Point(5, 1)));
            SetMonster(new FrankEinstein(1, new Point(7, 1)));
            SetMonster(new Default(1, new Point(5, 11)));
            SetMonster(new Noob(1, new Point(7, 11)));
        }

        private void GenerateMapType1(int? playingPlayers)
        {
            for (int i = 0; i < 13; i++)
            {
                for (int j = 0; j < 13; j++)
                {
                    if ((i == 0 || i == 12 || j == 0 || j == 12) || i % 2 == 0 && j % 2 == 0)
                    {
                        Map.SetGameObject(new Point(i, j), new Wall());
                    }
                    else if (i % 3 == 0 && j % 3 == 0)
                    {
                        Map.SetGameObject(new Point(i, j), new Box(0, true));
                    }
                }
            }

            Map.SetGameObject(new Point(1, 1), new Player(1));
            Map.SetGameObject(new Point(1, 11), new Player(2));
            if (playingPlayers == 3)
                Map.SetGameObject(new Point(11, 1), new Player(3));

            Map.RemoveAllGameObject(new Point(11, 5));
            SetMonster(new Point(11, 5));

            Map.RemoveAllGameObject(new Point(1, 2));
            Map.RemoveAllGameObject(new Point(1, 3));
            Map.RemoveAllGameObject(new Point(1, 5));
            Map.RemoveAllGameObject(new Point(1, 7));
            Map.RemoveAllGameObject(new Point(1, 9));
            Map.RemoveAllGameObject(new Point(1, 10));
            Map.RemoveAllGameObject(new Point(2, 1));
            Map.RemoveAllGameObject(new Point(2, 11));
            Map.RemoveAllGameObject(new Point(3, 1));
            Map.RemoveAllGameObject(new Point(3, 11));
            Map.RemoveAllGameObject(new Point(5, 1));
            Map.RemoveAllGameObject(new Point(5, 11));
            Map.RemoveAllGameObject(new Point(7, 1));
            Map.RemoveAllGameObject(new Point(7, 11));
            Map.RemoveAllGameObject(new Point(9, 1));
            Map.RemoveAllGameObject(new Point(9, 11));
            if (playingPlayers != 3)
                Map.RemoveAllGameObject(new Point(11, 1));
            else
            {
                Map.RemoveAllGameObject(new Point(10, 1));
                Map.RemoveAllGameObject(new Point(11, 2));
            }
            Map.RemoveAllGameObject(new Point(11, 3));
            Map.RemoveAllGameObject(new Point(11, 7));
            Map.RemoveAllGameObject(new Point(11, 9));
            Map.RemoveAllGameObject(new Point(11, 11));
        }

        private void GenerateMapType2(int? playingPlayers)
        {
            for (int i = 0; i < 13; i++)
            {
                for (int j = 0; j < 13; j++)
                {
                    if (i == 0 || j == 0)
                    {
                        Map.SetGameObject(new Point(i, j), new Wall());
                    }
                    else
                    {
                        Map.SetGameObject(new Point(i, j), new Box(0, true));
                    }
                }
            }

            Map.SetGameObject(new Point(1, 1), new Player(1));
            Map.SetGameObject(new Point(1, 11), new Player(2));
            if (playingPlayers == 3)
                Map.SetGameObject(new Point(11, 1), new Player(3));

            for (int i = 2; i < 6; i++)
            {
                Map.SetGameObject(new Point(2, i), new Wall());
                Map.SetGameObject(new Point(10, i), new Wall());
                Map.SetGameObject(new Point(2, 12 - i), new Wall());
                Map.SetGameObject(new Point(10, 12 - i), new Wall());
                Map.SetGameObject(new Point(i, 2), new Wall());
                Map.SetGameObject(new Point(i, 10), new Wall());
                Map.SetGameObject(new Point(12 - i, 2), new Wall());
                Map.SetGameObject(new Point(12 - i, 10), new Wall());
            }

            Map.SetGameObject(new Point(5, 5), new Wall());
            Map.SetGameObject(new Point(5, 9), new Wall());
            Map.SetGameObject(new Point(9, 5), new Wall());
            Map.SetGameObject(new Point(9, 9), new Wall());

            Map.RemoveAllGameObject(new Point(1, 2));
            Map.RemoveAllGameObject(new Point(1, 3));
            Map.RemoveAllGameObject(new Point(1, 5));
            Map.RemoveAllGameObject(new Point(1, 7));
            Map.RemoveAllGameObject(new Point(1, 9));
            Map.RemoveAllGameObject(new Point(1, 10));
            Map.RemoveAllGameObject(new Point(2, 1));
            Map.RemoveAllGameObject(new Point(3, 1));
            Map.RemoveAllGameObject(new Point(5, 1));
            Map.RemoveAllGameObject(new Point(7, 1));
            Map.RemoveAllGameObject(new Point(9, 1));
            Map.RemoveAllGameObject(new Point(10, 1));
            Map.RemoveAllGameObject(new Point(11, 2));
            Map.RemoveAllGameObject(new Point(11, 3));
            Map.RemoveAllGameObject(new Point(11, 5));
            Map.RemoveAllGameObject(new Point(11, 7));
            Map.RemoveAllGameObject(new Point(11, 9));
            Map.RemoveAllGameObject(new Point(11, 10));
            Map.RemoveAllGameObject(new Point(2, 11));
            Map.RemoveAllGameObject(new Point(3, 11));
            Map.RemoveAllGameObject(new Point(5, 11));
            Map.RemoveAllGameObject(new Point(7, 11));
            Map.RemoveAllGameObject(new Point(9, 11));
            Map.RemoveAllGameObject(new Point(10, 11));

            SetMonster(new Point(11, 5));
        }

        private void GenerateMapType3(int? playingPlayers)
        {
            for (int i = 0; i < 13; i++)
            {
                for (int j = 0; j < 13; j++)
                {
                    if (i == 0 || j == 0)
                    {
                        Map.SetGameObject(new Point(i, j), new Wall());
                    }
                    else
                    {
                        Map.SetGameObject(new Point(i, j), new Box(0, true));
                    }
                }
            }

            Map.SetGameObject(new Point(1, 1), new Player(1));
            Map.SetGameObject(new Point(1, 11), new Player(2));
            if (playingPlayers == 3)
                Map.SetGameObject(new Point(11, 1), new Player(3));

            for (int i = 2; i < 11; i++)
            {
                Map.SetGameObject(new Point(2, i), new Wall());
                Map.SetGameObject(new Point(10, i), new Wall());
                Map.SetGameObject(new Point(i, 10), new Wall());
            }

            for (int i = 5; i < 8; i++)
            {
                Map.SetGameObject(new Point(5, i), new Wall());
                Map.SetGameObject(new Point(7, i), new Wall());
                Map.SetGameObject(new Point(i, 7), new Wall());
            }

            Map.RemoveAllGameObject(new Point(1, 2));
            Map.RemoveAllGameObject(new Point(1, 3));
            Map.RemoveAllGameObject(new Point(1, 5));
            Map.RemoveAllGameObject(new Point(1, 7));
            Map.RemoveAllGameObject(new Point(1, 9));
            Map.RemoveAllGameObject(new Point(1, 10));
            Map.RemoveAllGameObject(new Point(2, 1));
            Map.RemoveAllGameObject(new Point(3, 1));
            Map.RemoveAllGameObject(new Point(5, 1));
            Map.RemoveAllGameObject(new Point(7, 1));
            Map.RemoveAllGameObject(new Point(9, 1));
            Map.RemoveAllGameObject(new Point(10, 1));
            Map.RemoveAllGameObject(new Point(11, 2));
            Map.RemoveAllGameObject(new Point(11, 3));
            Map.RemoveAllGameObject(new Point(11, 5));
            Map.RemoveAllGameObject(new Point(11, 7));
            Map.RemoveAllGameObject(new Point(11, 9));
            Map.RemoveAllGameObject(new Point(11, 10));
            Map.RemoveAllGameObject(new Point(2, 11));
            Map.RemoveAllGameObject(new Point(3, 11));
            Map.RemoveAllGameObject(new Point(5, 11));
            Map.RemoveAllGameObject(new Point(7, 11));
            Map.RemoveAllGameObject(new Point(9, 11));
            Map.RemoveAllGameObject(new Point(10, 11));

            SetMonster(new Point(11, 5));
        }

        #endregion

        /// <summary>
        /// Setting central dangerzone to the field
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetCentralDangerZone(object? sender, System.EventArgs e)
        {
            Bomb? bomb = (Bomb?)sender;
            (Map.GetGameObject(Map.FindPlayer(bomb.Player), typeof(Player)) as Player).PlantedBombs--;
            //Player player = (Player)Map.GetGameobject(Map.FindPlayer(bomb.Player), typeof(Player));

            if (bomb != null && Map.CheckGameObjectType(bomb.Position, typeof(Bomb)))
            {
                _map.RemoveGameObject(bomb.Position, typeof(Bomb));

                DangerZone dangerZone = new DangerZone(bomb.Position, bomb.Range, true);
                dangerZone.ExpandDangerZone += ExpandDangerZone;
                dangerZone.RemoveDangerZone += RemoveDangerZones;

                Map.SetGameObject(bomb!.Position, dangerZone);
            }
        }

        /// <summary>
        /// Removing dangerzone from the field
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoveDangerZones(object? sender, System.EventArgs e)
        {
            Point startPosition = (sender as DangerZone).Position;
            List<Direction> newDirectionList = new List<Direction> { Direction.RIGHT, Direction.LEFT, Direction.UP, Direction.DOWN };
            Point newPoint;

            _map.RemoveGameObject(startPosition, typeof(DangerZone));
            foreach (var direction in (sender as DangerZone).CanSpread)
            {
                switch (direction)
                {
                    case Direction.LEFT:
                        newPoint = new Point(startPosition.X, startPosition.Y - (sender as DangerZone).CurrentRange);

                        if (_map.CheckGameObjectType(newPoint, typeof(Wall))
                            || _map.CheckGameObjectType(newPoint, typeof(Bomb)))
                        {
                            newDirectionList.Remove(Direction.LEFT);
                        }
                        else
                        {
                            _map.RemoveGameObject(newPoint, typeof(DangerZone));
                        }

                        break;

                    case Direction.RIGHT:
                        newPoint = new Point(startPosition.X, startPosition.Y + (sender as DangerZone).CurrentRange);

                        if (_map.CheckGameObjectType(newPoint, typeof(Wall))
                            || _map.CheckGameObjectType(newPoint, typeof(Bomb)))
                        {
                            newDirectionList.Remove(Direction.RIGHT);
                        }
                        else
                        {
                            _map.RemoveGameObject(newPoint, typeof(DangerZone));
                        }
                        break;

                    case Direction.UP:
                        newPoint = new Point(startPosition.X - (sender as DangerZone).CurrentRange, startPosition.Y);

                        if (_map.CheckGameObjectType(newPoint, typeof(Wall))
                        || _map.CheckGameObjectType(newPoint, typeof(Bomb)))
                        {
                            newDirectionList.Remove(Direction.UP);
                        }
                        else
                        {
                            _map.RemoveGameObject(newPoint, typeof(DangerZone));
                        }
                        break;

                    case Direction.DOWN:
                        newPoint = new Point(startPosition.X + (sender as DangerZone).CurrentRange, startPosition.Y);

                        if (_map.CheckGameObjectType(newPoint, typeof(Wall))
                        || _map.CheckGameObjectType(newPoint, typeof(Bomb)))
                        {
                            newDirectionList.Remove(Direction.DOWN);
                        }
                        else
                        {
                            _map.RemoveGameObject(newPoint, typeof(DangerZone));
                        }
                        break;

                    default:
                        break;
                }
            }

            (sender as DangerZone).CanSpread = newDirectionList;
            OnGameAdvanced();
        }

        /// <summary>
        /// Expanding dangerzone on the field
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExpandDangerZone(object? sender, System.EventArgs e)
        {
            Point startPosition = (sender as DangerZone).Position;
            List<Direction> newDirectionList = new List<Direction>(); ;
            Point newPoint;
            DangerZone dangerZone;

            foreach (var direction in (sender as DangerZone).CanSpread)
            {
                PowerUps? powerup = null;
                Player? invPlayer = null;

                switch (direction)
                {
                    case Direction.LEFT:
                        newPoint = new Point(startPosition.X, startPosition.Y - (sender as DangerZone).CurrentRange);

                        dangerZone = new DangerZone(newPoint, 0, false);
                        if (!_map.CheckGameObjectType(newPoint, typeof(Wall)))
                        {
                            if (_map.CheckGameObjectType(newPoint, typeof(Bomb)))
                            {
                                Bomb bomb = (Bomb)Map.GetGameObject(newPoint, typeof(Bomb));
                                bomb.BlowUp();
                            }
                            if (_map.CheckGameObjectType(newPoint, typeof(Box)))
                            {
                                Box box = (Box)Map.GetGameObject(newPoint, typeof(Box));
                                powerup = box.GeneratePowerUp();
                                if (box.PlayerId != 0)
                                {
                                    Point? playerCoord = Map.FindPlayerByID(box.PlayerId);
                                    if (playerCoord != null)
                                    {
                                        Player player = (Player)Map.GetGameObject((Point)playerCoord, typeof(Player));
                                        player.PlacedBoxes--;
                                    }

                                }
                            }
                            if (_map.CheckGameObjectType(newPoint, typeof(DangerZone)))
                            {
                                Map.SetGameObject(newPoint, dangerZone);
                            }
                            else if (_map.CheckGameObjectType(newPoint, typeof(Player)))
                            {
                                Player player = (Player)Map.GetGameObject(newPoint, typeof(Player));
                                if (player.Invincible == 0)
                                {
                                    int id = player.Id;
                                    foreach (var p in _players)
                                    {
                                        if (p.Id == id)
                                        {
                                            p.IsAlive = false;
                                        }
                                    }
                                    _deadPlayerCounter++;
                                    _deadPlayers.Add(player);
                                    if (_deadPlayerCounter == _numOfPlayers - 1)
                                    {
                                        StartTimer2();
                                    }
                                }
                                else
                                {
                                    invPlayer = player;
                                }
;
                                Map.SetGameObject(newPoint, dangerZone);
                                if (invPlayer != null)
                                {
                                    Map.AddGameObject(newPoint, invPlayer);
                                }
                                newDirectionList.Add(Direction.LEFT);
                            }
                            else if (_map.CheckGameObjectType(newPoint, typeof(Monster)))
                            {
                                Monster monster = (Monster)Map.GetGameObject(newPoint, typeof(Monster));
                                foreach (Monster m in _monsters)
                                {
                                    if (m == monster)
                                    {
                                        m.IsAlive = false;
                                        _monsters.Remove(m);
                                        Map.RemoveGameObject(m.Coord, typeof(Monster));
                                    }
                                }
                            }
                            else
                            {
                                Map.SetGameObject(newPoint, dangerZone);
                                if (powerup != null)
                                {
                                    Map.AddGameObject(newPoint, new PowerUp((PowerUps)powerup));
                                }

                                newDirectionList.Add(Direction.LEFT);
                            }

                        }
                        break;

                    case Direction.RIGHT:
                        newPoint = new Point(startPosition.X, startPosition.Y + (sender as DangerZone).CurrentRange);

                        dangerZone = new DangerZone(newPoint, 0, false);
                        if (!_map.CheckGameObjectType(newPoint, typeof(Wall)))
                        {
                            if (_map.CheckGameObjectType(newPoint, typeof(Bomb)))
                            {
                                Bomb bomb = (Bomb)Map.GetGameObject(newPoint, typeof(Bomb));
                                bomb.BlowUp();
                            }
                            if (_map.CheckGameObjectType(newPoint, typeof(Box)))
                            {
                                Box box = (Box)Map.GetGameObject(newPoint, typeof(Box));
                                powerup = box.GeneratePowerUp();

                                if (powerup != null)
                                {
                                    Map.AddGameObject(newPoint, new PowerUp((PowerUps)powerup));
                                }
                                if (box.PlayerId != 0)
                                {
                                    Point? playerCoord = Map.FindPlayerByID(box.PlayerId);
                                    if (playerCoord != null)
                                    {
                                        Player player = (Player)Map.GetGameObject((Point)playerCoord, typeof(Player));
                                        player.PlacedBoxes--;
                                    }

                                }
                            }
                            if (_map.CheckGameObjectType(newPoint, typeof(DangerZone)))
                            {
                                Map.SetGameObject(newPoint, dangerZone);
                            }
                            else if (_map.CheckGameObjectType(newPoint, typeof(Player)))
                            {
                                Player player = (Player)Map.GetGameObject(newPoint, typeof(Player));
                                if (player.Invincible == 0)
                                {
                                    int id = player.Id;
                                    foreach (var p in _players)
                                    {
                                        if (p.Id == id)
                                        {
                                            p.IsAlive = false;
                                        }
                                    }
                                    _deadPlayerCounter++;
                                    _deadPlayers.Add(player);
                                    if (_deadPlayerCounter == _numOfPlayers - 1)
                                    {
                                        StartTimer2();
                                    }
                                }
                                else
                                {
                                    invPlayer = player;
                                }

                                Map.SetGameObject(newPoint, dangerZone);
                                if (invPlayer != null)
                                {
                                    Map.AddGameObject(newPoint, invPlayer);
                                }
                                newDirectionList.Add(Direction.LEFT);
                            }
                            else if (_map.CheckGameObjectType(newPoint, typeof(Monster)))
                            {
                                Monster monster = (Monster)Map.GetGameObject(newPoint, typeof(Monster));
                                foreach (Monster m in _monsters)
                                {
                                    if (m == monster)
                                    {
                                        m.IsAlive = false;
                                        _monsters.Remove(m);
                                        Map.RemoveGameObject(m.Coord, typeof(Monster));
                                    }
                                }
                            }
                            else
                            {
                                Map.SetGameObject(newPoint, dangerZone);
                                if (powerup != null)
                                {
                                    Map.AddGameObject(newPoint, new PowerUp((PowerUps)powerup));
                                }
                                newDirectionList.Add(Direction.RIGHT);
                            }
                        }
                        break;

                    case Direction.UP:
                        newPoint = new Point(startPosition.X - (sender as DangerZone).CurrentRange, startPosition.Y);

                        dangerZone = new DangerZone(newPoint, 0, false);
                        if (!_map.CheckGameObjectType(newPoint, typeof(Wall)))
                        {
                            if (_map.CheckGameObjectType(newPoint, typeof(Bomb)))
                            {
                                Bomb bomb = (Bomb)Map.GetGameObject(newPoint, typeof(Bomb));
                                bomb.BlowUp();
                            }
                            if (_map.CheckGameObjectType(newPoint, typeof(Box)))
                            {
                                Box box = (Box)Map.GetGameObject(newPoint, typeof(Box));
                                powerup = box.GeneratePowerUp();

                                if (powerup != null)
                                {
                                    Map.AddGameObject(newPoint, new PowerUp((PowerUps)powerup));
                                }
                                if (box.PlayerId != 0)
                                {
                                    Point? playerCoord = Map.FindPlayerByID(box.PlayerId);
                                    if (playerCoord != null)
                                    {
                                        Player player = (Player)Map.GetGameObject((Point)playerCoord, typeof(Player));
                                        player.PlacedBoxes--;
                                    }

                                }
                            }
                            if (_map.CheckGameObjectType(newPoint, typeof(DangerZone)))
                            {
                                Map.SetGameObject(newPoint, dangerZone);
                            }
                            else if (_map.CheckGameObjectType(newPoint, typeof(Player)))
                            {
                                Player player = (Player)Map.GetGameObject(newPoint, typeof(Player));
                                if (player.Invincible == 0)
                                {
                                    int id = player.Id;
                                    foreach (var p in _players)
                                    {
                                        if (p.Id == id)
                                        {
                                            p.IsAlive = false;
                                        }
                                    }
                                    _deadPlayerCounter++;
                                    _deadPlayers.Add(player);
                                    if (_deadPlayerCounter == _numOfPlayers - 1)
                                    {
                                        StartTimer2();
                                    }
                                }
                                else
                                {
                                    invPlayer = player;
                                }

                                Map.SetGameObject(newPoint, dangerZone);
                                if (invPlayer != null)
                                {
                                    Map.AddGameObject(newPoint, invPlayer);
                                }
                                newDirectionList.Add(Direction.LEFT);
                            }
                            else if (_map.CheckGameObjectType(newPoint, typeof(Monster)))
                            {
                                Monster monster = (Monster)Map.GetGameObject(newPoint, typeof(Monster));
                                foreach (Monster m in _monsters)
                                {
                                    if (m == monster)
                                    {
                                        m.IsAlive = false;
                                        _monsters.Remove(m);
                                        Map.RemoveGameObject(m.Coord, typeof(Monster));
                                    }
                                }
                            }
                            else
                            {
                                Map.SetGameObject(newPoint, dangerZone);
                                if (powerup != null)
                                {
                                    Map.AddGameObject(newPoint, new PowerUp((PowerUps)powerup));
                                }
                                newDirectionList.Add(Direction.UP);
                            }
                        }
                        break;

                    case Direction.DOWN:
                        newPoint = new Point(startPosition.X + (sender as DangerZone).CurrentRange, startPosition.Y);

                        dangerZone = new DangerZone(newPoint, 0, false);
                        if (!_map.CheckGameObjectType(newPoint, typeof(Wall)))
                        {
                            if (_map.CheckGameObjectType(newPoint, typeof(Bomb)))
                            {
                                Bomb bomb = (Bomb)Map.GetGameObject(newPoint, typeof(Bomb));
                                bomb.BlowUp();
                            }
                            if (_map.CheckGameObjectType(newPoint, typeof(Box)))
                            {
                                Box box = (Box)Map.GetGameObject(newPoint, typeof(Box));
                                powerup = box.GeneratePowerUp();

                                if (powerup != null)
                                {
                                    Map.AddGameObject(newPoint, new PowerUp((PowerUps)powerup));
                                }
                                if (box.PlayerId != 0)
                                {
                                    Point? playerCoord = Map.FindPlayerByID(box.PlayerId);
                                    if (playerCoord != null)
                                    {
                                        Player player = (Player)Map.GetGameObject((Point)playerCoord, typeof(Player));
                                        player.PlacedBoxes--;
                                    }

                                }
                            }
                            if (_map.CheckGameObjectType(newPoint, typeof(DangerZone)))
                            {
                                Map.SetGameObject(newPoint, dangerZone);
                            }
                            else if (_map.CheckGameObjectType(newPoint, typeof(Player)))
                            {
                                Player player = (Player)Map.GetGameObject(newPoint, typeof(Player));
                                if (player.Invincible == 0)
                                {
                                    int id = player.Id;
                                    foreach (var p in _players)
                                    {
                                        if (p.Id == id)
                                        {
                                            p.IsAlive = false;
                                        }
                                    }
                                    _deadPlayerCounter++;
                                    _deadPlayers.Add(player);
                                    if (_deadPlayerCounter == _numOfPlayers - 1)
                                    {
                                        StartTimer2();
                                    }
                                }
                                else
                                {
                                    invPlayer = player;
                                }

                                Map.SetGameObject(newPoint, dangerZone);
                                if (invPlayer != null)
                                {
                                    Map.AddGameObject(newPoint, invPlayer);
                                }
                                newDirectionList.Add(Direction.LEFT);
                            }
                            else if (_map.CheckGameObjectType(newPoint, typeof(Monster)))
                            {
                                Monster monster = (Monster)Map.GetGameObject(newPoint, typeof(Monster));
                                foreach (Monster m in _monsters)
                                {
                                    if (m == monster)
                                    {
                                        m.IsAlive = false;
                                        _monsters.Remove(m);
                                        Map.RemoveGameObject(m.Coord, typeof(Monster));
                                    }
                                }
                            }
                            else
                            {
                                Map.SetGameObject(newPoint, dangerZone);
                                if (powerup != null)
                                {
                                    Map.AddGameObject(newPoint, new PowerUp((PowerUps)powerup));
                                }
                                newDirectionList.Add(Direction.DOWN);
                            }
                        }
                        break;

                    default:
                        break;
                }
            }
            (sender as DangerZone).CanSpread = newDirectionList;
            OnGameAdvanced();
        }

        /// <summary>
        /// Step with monster
        /// </summary>
        /// <param name="monster"></param>
        private void CasualMonsterStep(Monster monster)
        {
            Point coord = new Point();
            switch (monster.Direction)
            {
                case Direction.UP:
                    coord = new Point(-1, 0);
                    break;

                case Direction.DOWN:
                    coord = new Point(1, 0);
                    break;

                case Direction.LEFT:
                    coord = new Point(0, -1);
                    break;

                case Direction.RIGHT:
                    coord = new Point(0, 1);
                    break;
            }

            Point newCoordinate = new Point(monster.Coord.X + coord.X, monster.Coord.Y + coord.Y);
            
            while (Map.Fields[newCoordinate.X, newCoordinate.Y].IsStructure)
            {
                switch (monster.Direction)
                {
                    case Direction.UP:
                        monster.ChangeDirection(Direction.RIGHT);
                        coord = new Point(0, 1);
                        break;

                    case Direction.DOWN:
                        monster.ChangeDirection(Direction.LEFT);
                        coord = new Point(0, -1);
                        break;

                    case Direction.LEFT:
                        monster.ChangeDirection(Direction.UP);
                        coord = new Point(-1, 0);
                        break;

                    case Direction.RIGHT:
                        monster.ChangeDirection(Direction.DOWN);
                        coord = new Point(1, 0);
                        break;
                }

                newCoordinate = new Point(monster.Coord.X + coord.X, monster.Coord.Y + coord.Y);
            }

            Map.MoveGameObject(monster.Coord, newCoordinate, monster);
            monster.ChangeCoord(newCoordinate);
            OnStepping(newCoordinate);
        }

        /// <summary>
        /// Player dies in structure
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DieInStructure(Object? sender, System.EventArgs e)
        {
            Point playerCoord = _map.FindPlayer(sender as Player);
            if (_map.CheckGameObjectType(playerCoord, typeof(Wall)) || _map.CheckGameObjectType(playerCoord, typeof(Box)))
            {
                int id = (sender as Player).Id;
                foreach (var p in _players)
                {
                    if (p.Id == id)
                    {
                        p.IsAlive = false;
                    }
                }
                _deadPlayerCounter++;
                _deadPlayers.Add(sender as Player);
                _map.RemoveGameObject(playerCoord, typeof(Player));
                if (_deadPlayerCounter == _numOfPlayers - 1)
                {
                    StartTimer2();
                }
            }
        }

        #endregion

        #region Event methods

        private void StepMonster(object? obj, MonsterEventArgs e)
        {
            StepMonster(e.Monster);
        }

        private void OnStepping(Point newCoords)
        {
            Step?.Invoke(this, new GameStepEventArgs(newCoords.X, newCoords.Y));
        }

        private void OnGameAdvanced()
        {
            GameAdvanced?.Invoke(this, new GameEventArgs(false, _gameTime));
        }

        private void OnRoundOver(bool isEveryOneOut)
        {
            RoundOver?.Invoke(this, new GameEventArgs(isEveryOneOut, _gameTime));
        }

        private void OnRoundOver(Player player) //it only be called when a player won the round
        {
            RoundOver?.Invoke(this, new GameEventArgs(false, _gameTime, player));
        }

        private void OnRoundWinner(Player player)
        {
            RoundWinner?.Invoke(this, new GameEventArgs(player));
        }

        private void OnPauseGame()
        {
            PauseGame?.Invoke(this, new System.EventArgs());
        }

        private void StartMonsters()
        {
            foreach (Monster monster in _monsters)
            {
                monster.StartMoving();
            }
        }

        private void StopMonsters()
        {
            foreach (Monster monster in _monsters)
            {
                monster.StopMoving();
            }
        }
        #endregion
    }
}
