using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TP_Othello.GameLogics.AI
{
    /// <summary>
    /// Tree structure for the AI minimax / alphabeta algorithm
    /// Structure inspired by some post on stackoverflow https://stackoverflow.com/questions/66893/tree-data-structure-in-c-sharp
    /// </summary>
    /// <typeparam name="T"></typeparam>
    class TreeNode<T>
    {
        private T data;

        private LinkedList<TreeNode<T>> children;

        public TreeNode(T data)
        {
            this.data = data;
            children = new LinkedList<TreeNode<T>>();
        }

        public void AddChild(T data)
        {
            children.AddLast(new TreeNode<T>(data));
        }

        public TreeNode<T> GetChild(int index)
        {
            foreach(TreeNode<T> child in children)
            {
                if (--index == 0)
                    return child;
            }

            return null;
        }
    }
}
