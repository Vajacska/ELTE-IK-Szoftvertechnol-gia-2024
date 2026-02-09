using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Persistence.GameObjects;

namespace ViewModel
{
    public class PlayingPlayer : ViewModelBase
    {
        private int _winnings;
        private Player _player;
        /*
        private int _x;
        private int _y;
        */
        public PlayingPlayer(Player player)
        {
            _player = player;
            _winnings = Player.Score;
        }

        public Player Player
        {
            get
            {
                return _player;
            }
            set
            {
                if (_player != value)
                {
                    _player = value;
                    OnPropertyChanged();
                }
            }
        }

        public int Winnings
        {
            get
            {
                return _winnings;
            }
            set
            {
                if (_winnings != value)
                {
                    _winnings = value;
                    OnPropertyChanged();
                }
            }
        }
        /*
        public int X 
        { 
            get { return _x; }
            set
            {
                if (_x != value)
                {
                    _x = value;
                }
            }
        }

        public int Y
        {
            get { return _y; }
            set
            {
                if (_y != value)
                {
                    _y = value;
                }
            }
        }

        public Point XY {  get { return new Point(X, Y); } }
        */
    }
}
