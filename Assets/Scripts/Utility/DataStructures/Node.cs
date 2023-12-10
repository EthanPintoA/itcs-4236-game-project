using System;
using UnityEngine;

namespace DataStructures
{
    public class Node : IComparable<Node>
    {
#nullable enable
        public Node? parent;
        public Vector2Int position;

        /// <summary>
        /// The travel cost - The distance from the start node to the current node.
        /// </summary>
        public float g;

        /// <summary>
        /// The heuristic - the distance from the current node to the end node.
        /// </summary>
        public float h;

        /// <summary>
        /// The sum of g and h.
        /// </summary>
        public float F => g + h;

        public Node(Node? parent, Vector2Int position)
        {
            this.parent = parent;
            this.position = position;

            g = 0;
            h = 0;
        }

        /// <summary>
        /// Compare the F value of this node to the F value of another node.
        /// <br/>
        /// This is used for the priority queue.
        /// </summary>
        public int CompareTo(Node other)
        {
            return F.CompareTo(other.F);
        }

        public override bool Equals(object obj)
        {
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                var node = (Node)obj;
                return position == node.position;
            }
        }

        public override int GetHashCode()
        {
            return position.GetHashCode();
        }
    }
}
