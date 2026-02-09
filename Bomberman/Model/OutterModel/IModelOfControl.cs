using Persistence.GameObjects;

namespace Model.OutterModel
{
    public interface IModelOfControl
    {
        public virtual void StartNewRound(List<Player> players) { }

        public virtual bool CheckEndOfGame() { return false; }
    }
}
