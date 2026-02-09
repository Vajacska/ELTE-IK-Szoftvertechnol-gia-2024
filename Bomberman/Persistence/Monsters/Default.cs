using System.Drawing;

namespace Persistence.Monsters
{
    public class Default : Monster 
    {
        const int DefaultSpeed = 1000; // the default speed of the monster

        /// <summary>
        /// default monster which has no any special ability
        /// </summary>
        /// <param name="id"></param>
        /// <param name="coord"></param>
        public Default(int id, Point coord) : base(id, coord) 
        {
            Speed.Interval = DefaultSpeed;
        }

        /// <summary>
        /// Sets the monster's speed back to the original one.
        /// </summary>
        public void ResetDefaultSpeed()
        {
            Speed.Interval = DefaultSpeed;
        }
    }
}
