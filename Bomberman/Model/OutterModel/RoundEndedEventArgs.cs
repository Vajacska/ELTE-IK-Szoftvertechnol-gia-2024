using Persistence.GameObjects;

namespace Model.OutterModel
{
    public class RoundEndedEventArgs : System.EventArgs
    {
        public int Round { get; set; }

        public List<Player> Players { get; set; }

        public RoundEndedEventArgs(int currentRound, List<Player> players)
        {
            Round = currentRound;
            Players = players;
        }
    }
}
