using System.Collections.Generic;
using UnityEngine;

namespace AStar
{
    public class Node
    {
        public Vector2Int Position;
        public Node Parent;
        public float G, H, F;

        public Node(Vector2Int pos, Node parent = null)
        {
            Position = pos;
            Parent = parent;
            G = H = F = 0;
        }
    }

}