using System;

namespace MazeGenerator
{
    public class MazeTransition
    {
        public MazeHexagon Node1 { get; set; }
        public MazeHexagon Node2 { get; set; }
        public bool Activated { get; set; }

        public MazeHexagon GetOtherNode(MazeHexagon origin)
        {
            if (origin == null) return null;
            if (origin == Node1) return Node2;
            if (origin == Node2) return Node1;
            throw new Exception("Origin not found in transition");
        }

        public MazeTransition(MazeHexagon node1, MazeHexagon node2)
        {
            Node1 = node1;
            Node2 = node2;
            Activated = true;
        }
    }
}