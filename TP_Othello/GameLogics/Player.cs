using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TP_Othello.GameLogics
{
    class Player : System.Runtime.Serialization.ISerializable
    {
        Stack<Object> lastPlays;

        public Player()
        {
            lastPlays = new Stack<object>();
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
        }
    }
}
