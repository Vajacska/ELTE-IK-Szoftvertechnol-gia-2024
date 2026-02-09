using Persistence.Enums;

namespace Persistence.GameObjects
{
    public class PowerUp : GameObject
    {
        private PowerUps _powerUp;

        /// <summary>
        /// Returns powerup
        /// </summary>
        public PowerUps PowerUps { get { return _powerUp; } private set { _powerUp = value; } }

        /// <summary>
        /// Creating power up
        /// </summary>
        /// <param name="type"></param>
        public PowerUp(PowerUps type) 
        {
            _powerUp = type;
        }
    }
}
