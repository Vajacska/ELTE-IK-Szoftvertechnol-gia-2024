using Persistence.Enums;

namespace Model.EventArgs
{
    public class MapEventArgs : System.EventArgs
    {
        private MapType _mapType;

        /// <summary>
        /// Returns map type
        /// </summary>
        public MapType MapType { get { return _mapType; } }

        /// <summary>
        /// Creating a map event args for creating new map
        /// </summary>
        /// <param name="mapType"></param>
        public MapEventArgs(MapType mapType)
        {
            _mapType = mapType;
        }
    }
}
