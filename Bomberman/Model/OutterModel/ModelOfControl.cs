using Model.EventArgs;
using Persistence;
using Persistence.Enums;
using Persistence.GameObjects;

namespace Model.OutterModel
{
    public class ModelOfControl : IModelOfControl
    {
        #region fields
        private GameModel? _model;

        private Map _map;

        private MapType _fieldType;

        private int? _numberOfWins;

        private List<Player> _players;

        private int _roundCounter;
        #endregion

        #region events
        public event EventHandler<NumberOfSomethingEventArgs>? NewRoundEvent;
        #endregion

        #region properties

        public List<Player> Players { get { return _players; } set { _players = value; } }

        public GameModel? Model { get { return _model; } }

        public Map Map { get { return _map; } }

        public int RoundCounter { get { return _roundCounter; } }

        #endregion

        #region constructor
        public ModelOfControl(MapType fieldType, int? numberOfWins, List<Player> players)
        {
            _fieldType = fieldType;
            _numberOfWins = numberOfWins;
            _players = players;
            _roundCounter = 0;
        }
        #endregion

        #region public methods
        /// <summary>
        /// Starts a new round in the game by reinstancing the inner model
        /// </summary>
        public void StartNewRound()
        {
            _model = new GameModel(new Map(_players.Count, _fieldType), _players, _numberOfWins);
            if(Model?.Map != null)
                _map = Model.Map;
            _model.NewGame(Players.Count);
            OnNewRound();
        }

        /// <summary>
        /// todo: implement
        /// </summary>
        public bool CheckEndOfGame()
        {
            return false;
        }

        /// <summary>
        /// todo: implement
        /// </summary>
        public void OnNewRound()
        {
            NewRoundEvent?.Invoke(this, new NumberOfSomethingEventArgs(_roundCounter));
        }

        /// <summary>
        /// todo: implement
        /// </summary>
        public void OnGameEnded()
        {

        }

        public Player GetPlayerByID(int id)
        {
            foreach (Player player in _players)
            {
                if (player.Id == id)
                {
                    return player;
                }
            }

            throw new ArgumentException(nameof(id), "There is no player with this id!");
        }

        public void ChangePlayerNumber(int number)
        {
            if (_players.Count < number)
                _players.Add(new Player(number));
            else if (_players.Count > number)
                _players.Remove(GetPlayerByID(number));
        }

        public void ChangeNumberOfWins(int number)
        {
            _numberOfWins = number;
        }

        public void ChangeMapType(MapType type)
        {
            _fieldType = type;
        }

        public async Task LoadGameAsync(string path)
        {
            try
            {
                using (StreamReader reader = new StreamReader(path))
                {
                    string line;
                    line = await reader.ReadLineAsync();
                    Enum.TryParse(line, out _fieldType);

                    line = await reader.ReadLineAsync();
                    int.TryParse(line, out _roundCounter);

                    _players.Clear();

                    while ((line = await reader.ReadLineAsync()) != null)
                    {
                        string[] numbers = line.Split(' ');
                        if (numbers.Count() == 2)
                        {
                            var p = new Player(int.Parse(numbers[0]));
                            p.Score = int.Parse(numbers[1]);
                            _players.Add(p);

                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new NullReferenceException(e.Message);
            }
        }

        public async Task SaveGameAsync(string path)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(path))
                {
                    await writer.WriteLineAsync(_fieldType.ToString());
                    await writer.WriteLineAsync(_roundCounter.ToString());

                    for (int i = 0; i < _players.Count(); i++)
                    {
                        await writer.WriteAsync(_players[i].Id + " " + _players[i].Score);
                        await writer.WriteLineAsync();
                    }
                }
            }
            catch (Exception e)
            {
                throw new NullReferenceException(e.Message);
            }
        }

        #endregion
    }
}
