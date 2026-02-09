namespace Model.EventArgs
{
    public class GameStepEventArgs : System.EventArgs
    {
        private int _changedX;

        private int _changedY;

        /// <summary>
        /// Game step event when gameobject steps
        /// </summary>
        /// <param name="changedX"></param>
        /// <param name="changedY"></param>
        public GameStepEventArgs(int changedX, int changedY)
        {
            _changedX = changedX;
            _changedY = changedY;
        }
    }
}
