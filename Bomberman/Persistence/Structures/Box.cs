using Persistence.Enums;

namespace Persistence.Structures
{
    public class Box : Structure
    {
        #region fields
        private bool _canGeneratePowerUp;
        private int _playerId;
        #endregion

        #region properties

        /// <summary>
        /// returns the box's player id
        /// </summary>
        public int PlayerId
        {
            get { return _playerId; }
            private set { _playerId = value; }
        }
        #endregion

        #region constructor
        /// <summary>
        /// creating box object
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="canGeneratePowerUp"></param>
        public Box(int playerId, bool canGeneratePowerUp)
        {
            _playerId = playerId;
            _canGeneratePowerUp = canGeneratePowerUp;
        }
        #endregion

        #region public methods
        /// <summary>
        /// generate power up
        /// </summary>
        /// <returns></returns>
        public PowerUps? GeneratePowerUp()
        {
            PowerUps? powerup = null;

            if (_canGeneratePowerUp)
            { 
                Random rnd = new Random();
                int chance = rnd.Next(1,11);

                // 
                if (chance >= 1 && chance < 6)
                { 
                    int powerupType = rnd.Next(1,12);
                    

                    switch (powerupType) 
                    {
                        case 1:
                            powerup = PowerUps.EXTRABOMBS;
                            break;
                        case 2:
                            powerup= PowerUps.BIGGERBLAST;
                            break;
                        case 3:
                            powerup = PowerUps.DETONATOR;
                            break;
                        case 4:
                            powerup = PowerUps.ROLLERSKATE;
                            break;
                        case 5:
                            powerup = PowerUps.INVINCIBILITY;
                            break;
                        case 6:
                            powerup = PowerUps.GHOST;
                            break;
                        case 7:
                            powerup = PowerUps.BARRIER;
                            break;
                        case 8:
                            powerup = PowerUps.SLOWER;
                            break;
                        case 9:
                            powerup = PowerUps.SMALLRANGE;
                            break;
                        case 10:
                            powerup = PowerUps.NOBOMB;
                            break;
                        default:
                            powerup = PowerUps.INSTANTBOMB;
                            break;
                    }
                }
            }

            return powerup;
        }
        #endregion
    }
}
