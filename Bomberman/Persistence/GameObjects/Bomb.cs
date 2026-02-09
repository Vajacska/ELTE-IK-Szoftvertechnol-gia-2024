using System.Drawing;
using System.Timers;
using Timer = System.Timers.Timer;

namespace Persistence.GameObjects
{
    public class Bomb : GameObject
    {
        #region FIELDS
        private int _range;
        private Point _position;
        private Timer _timer;
        private int _remainingSeconds;
        private Player _player;
        private bool _waitsTrigger;
        #endregion

        #region EVENTS
        /// <summary>
        /// This event is invoked when the bomb exploded
        /// </summary>
        public event EventHandler? Explode;
        #endregion

        #region PROPERTIES
        /// <summary>
        /// Returns the bomb's coordinates in the map
        /// </summary>
        public Point Position
        {
            get { return _position; }
            set { _position = value; }
        }

        /// <summary>
        /// Returns the remaining time till the bomb explodes
        /// </summary>
        public int RemainingTime
        {
            get { return _remainingSeconds; }
            set { _remainingSeconds = value; }
        }

        /// <summary>
        /// Returns the bomb's explosion range
        /// </summary>
        public int Range
        {
            get { return _range; }
            private set { _range = value; }
        }
        /// <summary>
        /// The owner player of the current bomb 
        /// </summary>
        public Player Player { get { return _player; } }
        #endregion

        #region CONSTRUCTOR
        /// <summary>
        /// Creating bomb object
        /// </summary>
        /// <param name="range"></param>
        /// <param name="position"></param>
        /// <param name="player"></param>
        public Bomb(int range, Point position, Player player)
        {
            _range = range;
            _position = position;
            _player = player;
            _waitsTrigger = player.Detonator;

            if (_waitsTrigger)
            {
                _remainingSeconds = 0;
            }
            else
            {
                _remainingSeconds = 5;
            }

            _timer = new(1000);
            _timer.Enabled = true;
            _timer.Elapsed += OnTimerElapsed;
            _timer.Start();
        }

        /// <summary>
        /// Bomb's timer countdown method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTimerElapsed(object? sender, ElapsedEventArgs e)
        {
            if(!_waitsTrigger)
            {
                RemainingTime--;
                if (RemainingTime == 0 && Explode != null)
                {
                    BlowUp();
                }
            }      
        }
        #endregion

        #region PUBLIC METHODS
        /// <summary>
        /// Stop timer and invoke bomb's explode event
        /// </summary>
        public void BlowUp()
        {
            _timer.Stop();
            Explode?.Invoke(this, EventArgs.Empty);
        }
        #endregion
    }
}

