using Persistence.GameObjects;
using Persistence.Monsters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Monsters
{
    public class MonsterEventArgs : System.EventArgs
    {
        private Monster _monster;

        public Monster Monster { get { return _monster; } }

        public MonsterEventArgs(Monster monster) 
        {
            _monster = monster;
        }
    }
}
