using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Drawing;
namespace TP_Othello.Game
{
    class Move
    {
        private List<Point> checksToInvert;
        public Point position { get; }
        public bool playerId { get; }

        public Move(Point position, bool playerId)
        {
            this.position = position;
            this.playerId = playerId;
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
