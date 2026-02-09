using Persistence.GameObjects;
using System.Drawing;

namespace Persistence.Monsters
{
    public class Ghost : Monster 
    {
        /// <summary>
        /// A monster which can go through walls and boxes...
        /// </summary>
        /// <param name="id"></param>
        /// <param name="coord"></param>

        const int DefaultSpeed = 2000; // the default speed of the monster

        public Ghost(int id, Point coord) : base(id, coord) 
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
