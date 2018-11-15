using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.TP_Othello
{
    interface IPlayable
    {
        Tuple<char, int> GetNextMove();
        bool IsPlayable(int x, int y);
    }
}
