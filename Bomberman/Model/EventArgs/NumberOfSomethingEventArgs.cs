namespace Model.EventArgs
{
    public class NumberOfSomethingEventArgs : System.EventArgs
    {
        private int _number;
        public int Number { get { return _number; } }

        /// <summary>
        /// Number of something, do something, ask Levi!
        /// </summary>
        /// <param name="number"></param>
        public NumberOfSomethingEventArgs(int number)
        {
            _number = number;
        }
    }
}
