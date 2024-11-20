using System;

namespace MazeGenerator
{
    public class MazeTransition
    {
        public MazeHexagon Node1 { get; set; }
        public MazeHexagon Node2 { get; set; }
        public int NodeDirection1 { get; set; }
        public int NodeDirection2 { get; set; }
        public bool Activated { get; set; }

        public MazeHexagon GetOtherNode(MazeHexagon origin)
        {
            if (origin == null) return null;
            if (origin == Node1) return Node2;
            if (origin == Node2) return Node1;
            throw new Exception("Origin not found in transition");
        }

        public int GetNodeDirection(MazeHexagon origin)
        {
            return origin == Node1 ? NodeDirection1 : NodeDirection2;
        }

        public MazeTransition(MazeHexagon node1, int nodeDirection, MazeHexagon node2)
        {
            Node1 = node1;
            NodeDirection1 = nodeDirection;
            Node2 = node2;
            NodeDirection2 = (nodeDirection + 3) % 6;
            Activated = true;
        }
    }
}