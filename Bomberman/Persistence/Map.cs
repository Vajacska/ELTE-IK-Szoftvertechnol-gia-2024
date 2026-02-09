using Persistence.Enums;
using Persistence.GameObjects;
using Persistence.Monsters;
using Persistence.Structures;
using System.Drawing;

namespace Persistence
{
    public class Map
    {
        private int? _playersAlive;
        private MapType? _mapType;

        private Field[,]? _fields;
        
        private int[,]? _pathGraph = new int[169, 169];

        /// <summary>
        /// returns map type
        /// </summary>
        public MapType? MapType { get { return _mapType; } }

        /// <summary>
        /// returns field matrix
        /// </summary>
        public Field[,]? Fields { get { return _fields; } }

        /// <summary>
        /// returns map size
        /// </summary>
        public int? Size {  get { return _fields?.GetLength(0); } }

        /// <summary>
        /// creating map object
        /// </summary>
        /// <param name="playerNumber"></param>
        /// <param name="mapType"></param>
        public Map(int playerNumber, MapType? mapType)
        {
            _playersAlive = playerNumber;
            _mapType = mapType;
            //Map size, whether it's fixed or not is not yet decided
            _fields = new Field[13, 13];

            for (int i = 0; i < 13; i++)
            {
                for (int j = 0; j < 13; j++)
                {
                    _fields[i, j] = new Field(new Point(i, j));
                }
            }

            InitializeGraph();
        }

        /// <summary>
        /// Checking the gameobject type on the field
        /// </summary>
        /// <param name="point"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool CheckGameObjectType(Point point, Type type) 
        { 
            if(point.X < _fields?.GetLength(1) && point.X > -1 && point.Y < _fields.GetLength(0) && point.Y > -1)
                return _fields[point.X, point.Y].CheckGameObject(type);
            return false;
        }

        /// <summary>
        /// Adds a GameObject to the given coordinate
        /// </summary>
        /// <param name="xy"></param>
        /// <param name="gameobject"></param>
        public void AddGameObject(Point xy, GameObject gameObject)
        {
            _fields?[xy.X,xy.Y].AddGameObject(gameObject);

            if (CheckGameObjectType(xy, typeof(Bomb)) || CheckGameObjectType(xy, typeof(Box)))
            { 
                UpdateGraph(xy,false);
            }
        }

        /// <summary>
        /// returns a selected typeof gameobject
        /// </summary>
        /// <param name="xy"></param>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        public GameObject? GetGameObject(Point xy, Type gameObject)
        {
            return _fields?[xy.X, xy.Y].GetGameObject(gameObject);
        }

        /// <summary>
        /// Removes all GameObjects from the coordinate and sets a new one
        /// </summary>
        /// <param name="xy"></param>
        /// <param name="gameObject"></param>
        public void SetGameObject(Point xy, GameObject gameObject)
        {
            bool before = (CheckGameObjectType(xy, typeof(Bomb)) || CheckGameObjectType(xy, typeof(Box)));

            if (xy.X < _fields?.GetLength(1) && xy.X > -1 && xy.Y < _fields.GetLength(0) && xy.Y > -1)
            {
                _fields[xy.X, xy.Y].RemoveAllGameObject();
                _fields[xy.X, xy.Y].AddGameObject(gameObject);
            }

            if (before && !(CheckGameObjectType(xy, typeof(Bomb)) || CheckGameObjectType(xy, typeof(Box))))
            {
                UpdateGraph(xy, true);
            }
        }

        /// <summary>
        /// remove all gameobejct from the selected coordinates
        /// </summary>
        /// <param name="xy"></param>
        public void RemoveAllGameObject(Point xy)
        {
            bool before = (CheckGameObjectType(xy, typeof(Bomb)) || CheckGameObjectType(xy, typeof(Box)));

            if (xy.X < _fields?.GetLength(1) && xy.X > -1 && xy.Y < _fields.GetLength(0) && xy.Y > -1)
                _fields[xy.X, xy.Y].RemoveAllGameObject();

            if (before)
            {
                UpdateGraph(xy, true);
            }
        }

        /// <summary>
        /// Removes the first occurance of the given GameObject
        /// </summary>
        /// <param name="xy"></param>
        /// <param name="gameObject"></param>
        public void RemoveGameObject(Point xy, Type gameObject)
        {
            bool before = (CheckGameObjectType(xy, typeof(Bomb)) || CheckGameObjectType(xy, typeof(Box)));
            _fields?[xy.X, xy.Y].RemoveGameObject(gameObject);

            if (before && !(CheckGameObjectType(xy, typeof(Bomb)) || CheckGameObjectType(xy, typeof(Box))))
            {
                UpdateGraph(xy, true);
            }
        }

        /// <summary>
        /// Move game objects
        /// </summary>
        /// <param name="oldPoint"></param>
        /// <param name="newPoint"></param>
        /// <param name="gameObject"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void MoveGameObject(Point oldPoint, Point newPoint, GameObject gameObject)
        {
            if (oldPoint.X >= _fields?.Length || oldPoint.X < 0)
                throw new ArgumentOutOfRangeException(nameof(oldPoint.X), "X is out of range!");
            if (oldPoint.Y >= _fields?.Length || oldPoint.Y < 0)
                throw new ArgumentOutOfRangeException(nameof(oldPoint.Y), "Y is out of range!");
            if (newPoint.X >= _fields?.Length || newPoint.X < 0)
                throw new ArgumentOutOfRangeException(nameof(oldPoint.X), "NewX is out of range!");
            if (newPoint.Y >= _fields?.Length || newPoint.Y < 0)
                throw new ArgumentOutOfRangeException(nameof(oldPoint.Y), "NewY is out of range!");

            if (_fields?[newPoint.X, newPoint.Y] != null)
            {
                if (!(_fields[newPoint.X, newPoint.Y].IsStructure)
                ||
                (gameObject is Ghost && !_fields[newPoint.X, newPoint.Y].CheckGameObject(typeof(Bomb)))
                ||
                (gameObject is Player && ((Player)gameObject).Ghost > 0))
                //if there is no structure on the field, any gameobject can step on that field
                //except ghost, cuz they can go through the walls and boxes
                {
                    _fields[newPoint.X, newPoint.Y].AddGameObject(gameObject);
                    _fields[oldPoint.X, oldPoint.Y].RemoveGameObject(gameObject.GetType());

                    if (gameObject is Player && CheckGameObjectType(newPoint, typeof(PowerUp)))
                    {
                        PowerUp? powerup = GetGameObject(newPoint, typeof(PowerUp)) as PowerUp;
                        (gameObject as Player)?.PickUpPowerup(powerup);
                        RemoveGameObject(newPoint, typeof(PowerUp));
                    }

                }
            }

        }

        /// <summary>
        /// Move player
        /// </summary>
        /// <param name="oldPoint"></param>
        /// <param name="newPoint"></param>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public bool MovePlayer(Point oldPoint, Point newPoint, GameObject gameObject)
        {
            if (oldPoint.X >= _fields?.Length || oldPoint.X < 0)
                throw new ArgumentOutOfRangeException(nameof(oldPoint.X), "X is out of range!");
            if (oldPoint.Y >= _fields?.Length || oldPoint.Y < 0)
                throw new ArgumentOutOfRangeException(nameof(oldPoint.Y), "Y is out of range!");
            if (newPoint.X >= _fields?.Length || newPoint.X < 0)
                throw new ArgumentOutOfRangeException(nameof(oldPoint.X), "NewX is out of range!");
            if (newPoint.Y >= _fields?.Length || newPoint.Y < 0)
                throw new ArgumentOutOfRangeException(nameof(oldPoint.Y), "NewY is out of range!");

            if (_fields?[newPoint.X, newPoint.Y] != null)
                if (!_fields[newPoint.X, newPoint.Y].IsStructure
                    ||
                    (gameObject is Ghost && !_fields[newPoint.X, newPoint.Y].CheckGameObject(typeof(Bomb)))
                    ||
                    (gameObject is Player && ((Player)gameObject).Ghost > 0))
                    //if there is no structure on the field, any gameobject can step on that field
                    //except ghost, cuz they can go through the walls and boxes
                {
                    _fields[newPoint.X, newPoint.Y].AddGameObject(gameObject);
                    _fields[oldPoint.X, oldPoint.Y].RemoveGameObject(gameObject.GetType());

                    if (gameObject is Player && CheckGameObjectType(newPoint, typeof(PowerUp)))
                    {
                        PowerUp? powerup = GetGameObject(newPoint, typeof(PowerUp)) as PowerUp;
                        (gameObject as Player)?.PickUpPowerup(powerup);
                        RemoveGameObject(newPoint, typeof(PowerUp));
                    }

                    if (gameObject is Player && ((Player)gameObject).InstantBomb > 0)
                    {
                   
                        return true;
                    }
                
                }
            return false;

        }


        #region Monster methods
       /// <summary>
       /// 
       /// </summary>
       /// <param name="start"></param>
       /// <returns></returns>
        private int[] Dijkstra(Point start)
        {
            int[] distance = new int[169];
            int[] parent = new int[169];

            //initializing parent and distance arrays, we will use 30000/-1 insted of infinity/none
            for (int i = 0; i < 169; i++)
            {
                distance[i] = 30000;
                parent[i] = -1;
            }

            //the start point will come from the 13*13 matrix, to convert it, we do 13*x+y, which will result in a maximum of 13*12+12=168
            int s = 13 * start.X + start.Y;
            distance[s] = 0;

            PriorityQueue<int, int> priorityQueue = new PriorityQueue<int, int>();
            for (int i = 0; i < 169; i++)
            {
                if (i != s)
                {
                    priorityQueue.Enqueue(i, distance[i]);
                }
            }

            int u = s;

            while (distance[u] < 30000 && priorityQueue.Count != 0)
            {
                PriorityQueue<int, int> newPriorityQueue = new PriorityQueue<int, int>();
                while (priorityQueue.Count != 0)
                {
                    int removed = priorityQueue.Dequeue();
                    if (removed != u - 1 && removed != u + 1 && removed != u - 13 && removed != u + 13)
                    {
                        newPriorityQueue.Enqueue(removed, distance[removed]);
                    }
                }

                if (u - 1 >= 0 && _pathGraph?[u, u - 1] == 1 && distance[u - 1] > distance[u] + 1)
                {
                    distance[u - 1] = distance[u] + 1;
                    parent[u - 1] = u;
                    newPriorityQueue.Enqueue(u - 1, distance[u] + 1);
                }
                if (u + 1 < 169 && _pathGraph?[u, u + 1] == 1 && distance[u + 1] > distance[u] + 1)
                {
                    distance[u + 1] = distance[u] + 1;
                    parent[u + 1] = u;
                    newPriorityQueue.Enqueue(u + 1, distance[u] + 1);
                }
                if (u - 13 >= 0 && _pathGraph?[u, u - 13] == 1 && distance[u - 13] > distance[u] + 1)
                {
                    distance[u - 13] = distance[u] + 1;
                    parent[u - 13] = u;
                    newPriorityQueue.Enqueue(u - 13, distance[u] + 1);
                }
                if (u + 13 < 169 && _pathGraph?[u, u + 13] == 1 && distance[u + 13] > distance[u] + 1)
                {
                    distance[u + 13] = distance[u] + 1;
                    parent[u + 13] = u;
                    newPriorityQueue.Enqueue(u + 13, distance[u] + 1);
                }

                priorityQueue = newPriorityQueue;

                u = priorityQueue.Dequeue();
            }

            return parent;
        }

        public List<Direction> CreateShortestPath(Point monsterCoord, Point playerCoord)
        {
            List<Direction> path = new List<Direction>();   

            int[] parent = Dijkstra(monsterCoord);

            int convertedPlayerCoord = playerCoord.X*13+playerCoord.Y;

            int currentCoord = convertedPlayerCoord;
            while (currentCoord != monsterCoord.X * 13 + monsterCoord.Y)
            {
                for (int i = 0; i < 169; i++)
                {
                    if (parent[currentCoord] == i)
                    {
                        switch (currentCoord - i)
                        {
                            case 1:
                                path.Add(Direction.RIGHT);
                                break;
                            case -1:
                                path.Add(Direction.LEFT);
                                break;
                            case 13:
                                path.Add(Direction.DOWN);
                                break;
                            default:
                                path.Add(Direction.UP);
                                break;
                        }

                        currentCoord = i;
                    }
                }
            }

            path.Reverse();
            return path;
        }

        private void InitializeGraph()
        {
            for (int k = 0; k < 169; k++)
            {
                for (int l = 0; l < 169; l++)
                {
                    if (_pathGraph?[k, l] != null)
                        _pathGraph[k, l] = 1;
                }
            }

            for (int i = 0; i < 13; i++)
            {
                for (int j = 0; j < 13; j++)
                {
                    int graphCoord = i * 13 + j;

                    if (CheckGameObjectType(new Point(i, j), typeof(Wall)) || CheckGameObjectType(new Point(i, j), typeof(Box)) || CheckGameObjectType(new Point(i, j), typeof(Bomb)))
                    {
                        if (graphCoord + 1 < 169 && _pathGraph?[graphCoord + 1, graphCoord] != null)
                        {
                            _pathGraph[graphCoord + 1, graphCoord] = 0;
                        }
                        if (graphCoord - 1 > 0 && _pathGraph?[graphCoord - 1, graphCoord] != null)
                        {
                            _pathGraph[graphCoord - 1, graphCoord] = 0;
                        }
                        if (graphCoord + 13 < 169 && _pathGraph?[graphCoord + 13, graphCoord] != null)
                        {
                            _pathGraph[graphCoord + 13, graphCoord] = 0;
                        }
                        if (graphCoord - 13 > 0 && _pathGraph?[graphCoord - 13, graphCoord] != null)
                        {
                            _pathGraph[graphCoord - 13, graphCoord] = 0;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// updates the graph after a box/bomb has been placed or removed
        /// </summary>
        /// <param name="coord"></param>
        /// <param name="removed"></param>
        private void UpdateGraph(Point coord, bool removed)
        {
            int isPath;
            if (removed)
            {
                isPath = 1;
            }
            else
            {
                isPath = 0;
            }

            int graphCoord = coord.X * 13 + coord.Y;

            if (graphCoord + 1 < 169 && _pathGraph?[graphCoord + 1, graphCoord] != null)
            {
                _pathGraph[graphCoord + 1, graphCoord] = isPath;
            }
            if (graphCoord - 1 > 0 && _pathGraph?[graphCoord - 1, graphCoord] != null)
            {
                _pathGraph[graphCoord - 1, graphCoord] = isPath;
            }
            if (graphCoord + 13 < 169 && _pathGraph?[graphCoord + 13, graphCoord] != null)
            {
                _pathGraph[graphCoord + 13, graphCoord] = isPath;
            }
            if (graphCoord - 13 > 0 && _pathGraph?[graphCoord - 13, graphCoord] != null)
            {
                _pathGraph[graphCoord - 13, graphCoord] = isPath;
            }
        }
        #endregion

        #region Player methods
        /// <summary>
        /// returns if a specific player is on a specific field 
        /// </summary>
        /// <param name="coord"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool IsPlayerWithId(Point coord, int id)
        {
            if (Fields?[coord.X, coord.Y] != null)
                if (Fields[coord.X, coord.Y].CheckGameObject(typeof(Player)))
                {
                    var gameObject = Fields[coord.X, coord.Y].GetGameObject(typeof(Player)) as Player;
                    return gameObject != null && gameObject.Id == id;
                }

            return false;
        }

        /// <summary>
        /// Returns the Point of the given player
        /// </summary>
        public Point FindPlayer(Player player)
        {
            for(int i = 0; i < _fields?.GetLength(0); i++)
            {
                for(int j = 0; j < _fields.GetLength(1); j++)
                {
                    if (_fields[i, j].CheckGameObject(typeof(Player)) 
                        &&
                        (_fields[i, j].GetGameObject(typeof(Player)) as Player)?.Id == player.Id)
                        return new Point(i, j);
                }
            }

            throw new Exception("There is no player with this id!");
        }

        public Point? FindPlayerByID(int id)
        {
            for (int i = 0; i < _fields?.GetLength(0); i++)
            {
                for (int j = 0; j < _fields.GetLength(1); j++)
                {
                    if (_fields[i, j].CheckGameObject(typeof(Player))
                        &&
                        (_fields[i, j].GetGameObject(typeof(Player)) as Player)?.Id == id)
                        return new Point(i, j);
                }
            }

            return null;
        }

        #endregion
    }
}

