using Persistence.Enums;
using Persistence.Structures;
using System.Drawing;
using System.Timers;

namespace Persistence.GameObjects
{
    public class Player : GameObject
    {
        #region fields
        private bool _isAlive;
        private int _score;
        private int _id;
        private int _plantedBombs;
        private int _boxes;
        private int _placedBoxes;
        private List<Bomb> _bombs;
        private Point _nextStep;
        private System.Timers.Timer _timer;

        //powerups
        private int _numberOfBombs;
        private int _bombRange;
        private bool _detonator;
        private bool _rollerSkate;
        private int _invincible;
        private int _ghost;
        private int _slower;
        private int _smallBomb;
        private int _noBomb;
        private int _instantBomb;
        #endregion

        public event EventHandler? DieInStructure;

        #region properties
        /// <summary>
        /// Range of Player's bomb
        /// </summary>
        public int BombRange
        {
            get { return _bombRange; }
            set { _bombRange = value; }
        }

        /// <summary>
        /// Number of setting down bombs
        /// </summary>
        public int NumberOfBombs
        {
            get { return _numberOfBombs; }
            set { _numberOfBombs = value; }
        }

        /// <summary>
        /// Player's detonator
        /// </summary>
        public bool Detonator
        {
            get { return _detonator; }
            set { _detonator = value; }
        }

        /// <summary>
        /// Player's rollerskate
        /// </summary>
        public bool RollerSakte
        {
            get { return _rollerSkate; }
            set { _rollerSkate = value; }
        }

        /// <summary>
        /// Returns true if the player live, else false
        /// </summary>
        public bool IsAlive
        {
            get { return _isAlive; }
            set { _isAlive = value; }
        }

        /// <summary>
        /// Player's score
        /// </summary>
        public int Score
        {
            get { return _score; }
            set { _score = value; }
        }

        /// <summary>
        /// Player's ID
        /// </summary>
        public int Id
        {
            get { return _id; }
            private set { _id = value; }
        }

        /// <summary>
        /// Number of planted bombs
        /// </summary>
        public int PlantedBombs
        {
            get { return _plantedBombs; }
            set { _plantedBombs = value; }
        }

        /// <summary>
        /// Number of Player's maximum planteble boxes
        /// </summary>
        public int Boxes
        {
            get { return _boxes; }
            private set { _boxes = value; }
        }
        /// <summary>
        /// Player's planted boxes number
        /// </summary>
        public int PlacedBoxes
        {
            get { return _placedBoxes; }
            set { _placedBoxes = value; }
        }

        public int Invincible
        {
            get { return _invincible; }
            set { _invincible = value; }
        }

        public int Ghost
        {
            get { return _ghost; }
            set { _ghost = value; }
        }

        public int Slower
        {
            get { return _slower; }
            set { _slower = value; }
        }

        public int InstantBomb
        {
            get { return _instantBomb; }
            set { _instantBomb = value; }
        }
        #endregion

        #region constructor
        /// <summary>
        /// Creating player object
        /// </summary>
        /// <param name="id"></param>
        public Player(int id)
        {
            _isAlive = true;
            _score = 0;
            _id = id;
            _plantedBombs = 0;
            _boxes = 0;
            _nextStep = new Point(0, 0);
            _bombs = new List<Bomb>();
            _numberOfBombs = 1;
            _bombRange = 2;
            _detonator = false;
            _rollerSkate = false;
            _slower = 0;
            _smallBomb = 0;
            _noBomb = 0;
            _instantBomb = 0;
            _timer = new System.Timers.Timer(1000);
            _timer.Elapsed += new ElapsedEventHandler(OnTimeElapsed);
        }

        #endregion

        private void OnTimeElapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            if(_ghost == 1)
            {
                DieInStructure?.Invoke(this, EventArgs.Empty);
            }
            if (_ghost > 0)
            {
                _ghost--;
            }
          
            if (_invincible > 0)
            { 
                _invincible--;
            }
            if (_slower > 0)
            {
                _slower--;
            }
            if (_smallBomb > 0)
            {
                _smallBomb--;
            }
            if (_noBomb > 0)
            {
                _noBomb--;
            }
            if(_instantBomb > 0)
            {
                _instantBomb--;
                //event to communicate with map?
            }
            if (_ghost == 0 && _invincible == 0 && _slower == 0 && _smallBomb == 0 && _noBomb == 0 && _instantBomb == 0)
            { 
                _timer.Stop();
            }
        }
        

        #region public methods
        /// <summary>
        /// Player plant bomb
        /// </summary>
        /// <param name="rowcol"></param>
        /// <param name="map"></param>
        /// <param name="player"></param>
        /// <returns></returns>
        public Bomb? Plant(Point rowcol, Map map, Player player)
        {
            if (_noBomb > 0)
            {
                return null;
            }
            if (PlantedBombs < NumberOfBombs)
            {
                Bomb bomb;
                if (_smallBomb == 0)
                {
                    bomb = new Bomb(BombRange, rowcol, player);
                }
                else 
                {
                    bomb = new Bomb(1, rowcol, player);
                }

                map.AddGameObject(rowcol, bomb);
                PlantedBombs++;

                if (Detonator)
                {
                    _bombs.Add(bomb);
                }

                return bomb;
            }
            else if (PlantedBombs == NumberOfBombs && Detonator)
            {
                foreach (Bomb bomb in _bombs)
                { 
                    bomb.BlowUp();
                }

                _bombs.Clear();
            }
            return null;
        }

        /// <summary>
        /// Player place box
        /// </summary>
        /// <param name="rowcol"></param>
        /// <param name="map"></param>
        /// <param name="player"></param>
        public void PlaceBox(Point rowcol, Map map, Player player)
        {
            if (PlacedBoxes < Boxes)
            {
                Box box = new Box(Id, false);
                map.AddGameObject(rowcol, box);
                PlacedBoxes++;
            }
        }

        /// <summary>
        /// Picks up a powerup and modifies the appropriate fields
        /// </summary>
        /// <param name="powerup">The type of powerup</param>
        public void PickUpPowerup(PowerUp? powerup)
        {
            switch (powerup?.PowerUps)
            {
                case PowerUps.EXTRABOMBS:
                    _numberOfBombs++;
                    break;
                case PowerUps.BIGGERBLAST:
                    _bombRange++;
                    break;
                case PowerUps.DETONATOR:
                    _detonator = true;
                    break;
                case PowerUps.ROLLERSKATE:
                    _rollerSkate = true;
                    break;
                case PowerUps.INVINCIBILITY:
                    _invincible = 7;
                    if (!_timer.Enabled)
                    {
                        _timer.Start();
                    }
                    break;
                case PowerUps.GHOST:
                    _ghost = 7;
                    if (!_timer.Enabled)
                    { 
                        _timer.Start();
                    }
                    break;
                case PowerUps.BARRIER:
                    _boxes += 3;
                    break;
                case PowerUps.SLOWER:
                    _slower = 7;
                    if (!_timer.Enabled)
                    {
                        _timer.Start();
                    }
                    break;
                case PowerUps.SMALLRANGE:
                    _smallBomb = 7;
                    if (!_timer.Enabled)
                    {
                        _timer.Start();
                    }
                    break;
                case PowerUps.NOBOMB:
                    _noBomb = 7;
                    if (!_timer.Enabled)
                    {
                        _timer.Start();
                    }
                    break;
                default:
                    _instantBomb = 7;
                    if (!_timer.Enabled)
                    {
                        _timer.Start();
                    }
                    break;
            }
        }
        #endregion
    }
}

