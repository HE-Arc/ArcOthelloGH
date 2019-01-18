using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Drawing;
namespace TP_Othello.GameLogics
{
    /// <summary>
    /// This class represents a move with the pawn positions that will be changed if it is applied
    /// </summary>
    class Move
    {
        private List<Point> checksToInvert;
        public Point position { get; }
        public bool whitePlayer { get; }

        public Move(Point position, bool playerId)
        {
            this.position = position;
            this.whitePlayer = playerId;
            checksToInvert = new List<Point>();
        }

        public void AddChecksToInvert(List<Point> positions)
        {
            checksToInvert.AddRange(positions);
        }
       
        public List<Point> GetChecksToInvert()
        {
            return checksToInvert;
        }
    }
}
