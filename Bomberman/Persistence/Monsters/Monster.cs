using Persistence.Enums;
using Persistence.GameObjects;
using System.Drawing;
using System.Timers;

namespace Persistence.Monsters
{
    public abstract class Monster : GameObject
	{
		#region Fields

		private int _id;
		private Point _coord;
		private Direction _direction; // the last direction where the monster went
		private bool _isAlive;
		private System.Timers.Timer _speed; 

		#endregion

		#region Properties

		/// <summary>
		/// Monster position on the map
		/// </summary>
        public Point Coord
        {
            get { return _coord; }
            private set { _coord = value; }
        }

		/// <summary>
		/// Direction to which the monster moves
		/// </summary>
        public Direction Direction
		{
			get { return _direction; }
			private set
			{
				_direction = value;
			}
		}

		public bool IsAlive { get { return _isAlive; } set { _isAlive = value; } }

        public System.Timers.Timer Speed 
		{ 
			get { return _speed; } 
			private set
			{
				_speed = value;
			}
		}
		

        public Boolean IsMoving()
        {
            return _speed.Enabled;
        }
		

        #endregion

        public event EventHandler<MonsterEventArgs>? MonsterStep;

		#region Constructor

		/// <summary>
		/// creating monster object
		/// </summary>
		/// <param name="id"></param>
		/// <param name="coord"></param>
		public Monster(int id, Point coord) 
		{ 
			_id = id;
			_coord = coord;
			Direction = Direction.RIGHT; // the default direction is right at instancing 
			_isAlive = true;			
            _speed = new System.Timers.Timer();
            _speed.Elapsed += new ElapsedEventHandler(SpeedTimer_Tick);
            _speed.AutoReset = true;			
        } 

        #endregion

        #region Methods

        /// <summary>
        /// changes the direction when moving
        /// </summary>
        /// <param name="direction"></param>
        public void ChangeDirection(Direction direction)
		{
			Direction = direction;
		}

        /// <summary>
        ///  changes the actual coordinates of the monster
        /// </summary>
        /// <param name="newCoord"></param>
        public void ChangeCoord(Point newCoord)
		{
			_coord = newCoord;
		}

		/// <summary>
		/// killing monster
		/// </summary>
		public void KillMonster()
		{
			if (_isAlive)
			{
				_isAlive = false;
			}
		}
		
        private void SpeedTimer_Tick(Object? sender, ElapsedEventArgs e)
        {
			MonsterStep?.Invoke(this,new MonsterEventArgs(this));
        }

        public void StartMoving() //starting timer
        {
            if (_speed.Enabled)
                return;
            _speed.Start();
        }

        public void StopMoving() //stopping timer
        {
            if (!_speed.Enabled)
                return;
            _speed.Stop();
        }

		/// <summary>
		/// Changes the speed of the monster
		/// </summary>
		/// <param name="TimerInterval">The interval of the timer in miliseconds which manages the speed. The more interval it uses, the slower the monster will be. For example when the timer is set to 1000 miliseconds, the monster will step in every seconds.</param>
		public void ChangeSpeed(int TimerInterval)
		{
			Speed.Interval = TimerInterval;
		}
		
        #endregion
    }
}
