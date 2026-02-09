using Persistence.GameObjects;
using System;

namespace Model.OutterModel
{
    public class GameEndedEventArgs : System.EventArgs
    {
        public Player Winner { get; set; }
        public List<Player> Players { get; set; }

        public GameEndedEventArgs(Player winner, List<Player> players)
        {
            Winner = winner;
            Players = players;
        }
    }
}
