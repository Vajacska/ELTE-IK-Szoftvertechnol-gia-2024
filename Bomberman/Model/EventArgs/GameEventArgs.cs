using Persistence.GameObjects;

namespace Model.EventArgs
{
    public class GameEventArgs : System.EventArgs
    {
        private bool _isEveryoneOut; 
        private int _gameTime;
        private Player _player = null!;

        /// <summary>
        /// Returns true if every player has died
        /// </summary>
        public bool IsEveryoneOut {  get { return _isEveryoneOut; } }

        public Player Player { get { return _player; } }

        /// <summary>
        /// Game ended event
        /// </summary>
        /// <param name="isEveryoneOut"></param>
        /// <param name="gameTime"></param>
        public GameEventArgs(bool isEveryoneOut, int gameTime)
        {
            _isEveryoneOut = isEveryoneOut;
            _gameTime = gameTime;
        }

        /// <summary>
        /// Game ended event
        /// </summary>
        /// <param name="isEveryoneOut"></param>
        /// <param name="gameTime"></param>
        /// <param name="player"></param>
        public GameEventArgs(bool isEveryoneOut, int gameTime, Player player)
        {
            _isEveryoneOut = isEveryoneOut;
            _gameTime = gameTime;
            _player = player;
        }

        /// <summary>
        /// Game ended event
        /// </summary>
        /// <param name="player"></param>
        public GameEventArgs(Player player)
        {
            _player = player;
        }
    }
}
