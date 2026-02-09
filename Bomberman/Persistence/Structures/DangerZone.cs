using Persistence.GameObjects;
using System.Drawing;
using Timer = System.Timers.Timer;
using Persistence.Enums;

namespace Persistence.Structures
{
    public class DangerZone : Structure
    {
        #region fields
        private bool _isAlive;
        private Timer _timer;
        private int _maxRange;
        private int _currentRange;
        private Point _position;
        private bool _isCentral;
        private List<Direction> _canSpread = new List<Direction> { Direction.LEFT, Direction.RIGHT, Direction.DOWN, Direction.UP};
        private int _remainingTime;
        #endregion

        #region properties
        public int MaxRange
        {
            get { return _maxRange; }
            set { _maxRange = value; _remainingTime = 2 * _maxRange + 2; }
        }

        public int CurrentRange
        {
            get { return _currentRange; }
            set { _currentRange = value; }
        }

        public Point Position
        {
            get { return _position; }
            set { _position = value; }
        }

        public bool IsCentral
        {
            get { return _isCentral; }
            set { _isCentral = value; }
        }

        public List<Direction> CanSpread
        {
            get { return _canSpread; }
            set { _canSpread = value; }
        }

        public int RemainingTime
        {
            get { return _remainingTime; }
            set
            {
                if (value == 0)
                {
                    _remainingTime = value;
                    RemoveDangerZone?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    _remainingTime = value;
                }
            }
        }

        public bool Alive
        {
            get { return _isAlive; }
            private set { _isAlive = value; }
        }

        public event EventHandler? ExpandDangerZone;
        public event EventHandler? RemoveDangerZone;

        #endregion

        #region constructor
        public DangerZone(Point position, int maxRange, bool isCentral)
        {
            _position = position;
            _maxRange = maxRange;
            _isCentral = isCentral;

            _currentRange = 0;
            _remainingTime = 2*maxRange + 2;

            if (isCentral)
            {
                _timer = new(250);
                _timer.Enabled = true;
                _timer.Elapsed += OnTimerElapsed;
                _timer.Start();
                /// todo: timer.stop();
            }
        }

        private void OnTimerElapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            _timer.Stop();
            if (_remainingTime > _maxRange + 2)
            {
                _remainingTime--;
                _currentRange++;
                ExpandDangerZone?.Invoke(this, EventArgs.Empty);
            }
            else if (_remainingTime > _maxRange)
            {
                _remainingTime--;
                CurrentRange = 0;
                CanSpread = new List<Direction> { Direction.RIGHT, Direction.LEFT, Direction.UP, Direction.DOWN };
            }
            else if (_remainingTime >= 0)
            {
                _remainingTime--;
                _currentRange++;
                if (_currentRange <= _maxRange)
                {
                    RemoveDangerZone?.Invoke(this, EventArgs.Empty);
                }
            }
            _timer.Start();
        }
        #endregion
    }
}
