using Persistence.Enums;
using Persistence.GameObjects;
using System.Drawing;

namespace Persistence.Monsters
{
    public class FrankEinstein : Monster 
    {
        private const int DefaultSpeed = 500; // the default speed of the monster

        private const int TotalStepWhenFollowingPlayer = 20;

        private int _actualStepWhenFollowingPlayer;

        private bool _isFollowingAPlayer;

        private Player _followedPlayer;

        //private List<Direction> _shortestPath = new List<Direction>();

        public bool IsFollowingAPlayer { get { return _isFollowingAPlayer; } private set { _isFollowingAPlayer = value; } }

        public Player FollowedPlayer { get { return _followedPlayer; } private set { _followedPlayer = value; } }

        public int ActualStepWhenFollowingPlayer { get { return _actualStepWhenFollowingPlayer; } private set { _actualStepWhenFollowingPlayer = value; } }

        //public List<Direction> ShortestPath { get { return _shortestPath; } private set { _shortestPath = value; } }

        public FrankEinstein(int id, Point coord) : base(id, coord) 
        {
            Speed.Interval = DefaultSpeed;
            _isFollowingAPlayer = false;
            _followedPlayer = null!;
            _actualStepWhenFollowingPlayer = 0;
        }

        /// <summary>
        /// Sets the monster's speed back to the original one.
        /// </summary>
        public void ResetDefaultSpeed()
        {
            Speed.Interval = DefaultSpeed;
        }

        public void MonsterStartsToFollowAPlayer(Player followedPlayer)
        {
            if (!_isFollowingAPlayer)
            {
                _isFollowingAPlayer = true;
                _followedPlayer = followedPlayer;
                _actualStepWhenFollowingPlayer = TotalStepWhenFollowingPlayer;
            }
        }

        public void MonsterStopsToFollowAPlayer()
        {
            if (_isFollowingAPlayer)
            {
                _isFollowingAPlayer = false;
                _followedPlayer = null!;
            }
        }

        public void TakenStepByMonsterWhenFollowsPlayer()
        {
            if (_actualStepWhenFollowingPlayer > 0)
                _actualStepWhenFollowingPlayer--;
        }
    }
}
