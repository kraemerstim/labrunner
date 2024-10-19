using System;
using System.Collections.Generic;

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

        private MazeTransition(MazeHexagon node1, MazeHexagon node2)
        {
            Node1 = node1;
            Node2 = node2;
            Activated = true;
        }

        public class MazeTransitionCollection
        {
            private readonly HashSet<MazeTransition> _transitionSet = new HashSet<MazeTransition>();

            public HashSet<MazeTransition> GetTransitions()
            {
                return _transitionSet;
            }

            public MazeTransition CreateTransition(MazeHexagon node1, int node1Direction, MazeHexagon node2, int node2Direction)
            {
                var mazeTransition = new MazeTransition(node1, node2);
                _transitionSet.Add(mazeTransition);
                node1.AddMazeTransition(node1Direction, mazeTransition);
                node2.AddMazeTransition(node2Direction, mazeTransition);
                return mazeTransition;
            }
            
        }
    }
}