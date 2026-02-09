using Persistence.GameObjects;
using Persistence.Structures;
using System.Drawing;

namespace Persistence
{
    public class Field
    {
        private Point? _coord;

        private List<GameObject>? _gameObjects;

        /// <summary>
        /// Returns true if field is empty
        /// </summary>
        public bool IsEmpty {
            get {
                return _gameObjects?.Count == 0;
            }
        }

        /// <summary>
        /// Returns true if there is at least one structure on the field
        /// </summary>
        public bool IsStructure 
        {
            get
            {
                return (CheckGameObject(typeof(Wall)) || CheckGameObject(typeof(Box)) || CheckGameObject(typeof(Bomb)));
            }
        }

        /// <summary>
        /// Player or powerup is on the selected field
        /// </summary>
        public bool IsPlayerOrPowerUp
        {
            get
            {
                return (CheckGameObject(typeof(Player)) || CheckGameObject(typeof(PowerUp)));
            }
        }

        /// <summary>
        /// Create field object
        /// </summary>
        /// <param name="coord"></param>
        public Field(Point coord) 
        {
            _coord = coord;
            _gameObjects = new List<GameObject>();
        }

        /// <summary>
        /// Addig gameobject to the field
        /// </summary>
        /// <param name="gameObject"></param>
        public void AddGameObject(GameObject gameObject)
        { 
            _gameObjects?.Add(gameObject);        
        }

        /// <summary>
        /// Removes the first occurance of the given GameObject type
        /// </summary>
        /// <param name="gameObject"></param>
        public void RemoveGameObject(Type gameObject) 
        {
            if(_gameObjects != null)
            {
                foreach (GameObject obj in _gameObjects)
                {
                    if (obj.GetType() == gameObject)
                    {
                        _gameObjects.Remove(obj);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Remove all gameobject from the field
        /// </summary>
        public void RemoveAllGameObject()
        { 
            _gameObjects = new List<GameObject>();
        }

        /// <summary>
        /// Returns true if an object of the specified type is present in the list
        /// </summary>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        public bool CheckGameObject(Type gameObject)
        {
            if(_gameObjects != null)
            {
                foreach (GameObject obj in _gameObjects)
                {
                    if (obj.GetType() == gameObject)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Returns all gameobject on the field
        /// </summary>
        /// <returns></returns>
        public List<GameObject>? GetAllGameObject()
        {
            return _gameObjects;
        }

        /// <summary>
        /// Returns the selected gameobject
        /// </summary>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        public GameObject? GetGameObject(Type gameObject)
        {
            if(_gameObjects != null)
            {
                foreach (GameObject obj in _gameObjects)
                {
                    if (obj.GetType() == gameObject)
                    {
                        return obj;
                    }
                }
            }

            return null;
        }
    }
}
