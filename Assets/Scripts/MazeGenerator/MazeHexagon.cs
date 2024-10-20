using System.Collections.Generic;
using System.Linq;

namespace MazeGenerator
{
    public class MazeHexagon
    {
        public readonly MazeTransition[] MazeTransitions;
        public bool IsFinish;
        public (int x, int y) MazePosition { get; private set; }

        public void AddMazeTransition(int direction, MazeTransition mazeTransition)
        {
            MazeTransitions[direction] = mazeTransition;
        }

        public MazeHexagon(int x, int y)
        {
            MazePosition = (x,y);
            MazeTransitions = new MazeTransition[6];
            IsFinish = false;
        }
        
        public bool CheckConnectivityTo(MazeHexagon hex2)
        {
            var connectivitySet = new HashSet<MazeHexagon> {this};
            var newConnectivitySet = new HashSet<MazeHexagon> {this};

            while (true)
            {
                foreach (var targetHexagon in from hexagon in connectivitySet
                         from transition in hexagon.MazeTransitions
                         where transition != null && transition.Activated
                         select transition.GetOtherNode(hexagon))
                {
                    newConnectivitySet.Add(targetHexagon);
                }

                if (newConnectivitySet.Contains(hex2)) return true;
                if (connectivitySet.Count == newConnectivitySet.Count) return false;

                connectivitySet = new HashSet<MazeHexagon>(newConnectivitySet);
            }
        }

        public bool CheckConnectivityTo2(MazeHexagon hex2)
        {
            var hexesToCheck = new Queue<MazeHexagon>();
            hexesToCheck.Enqueue(this);
            var hexesChecked = new HashSet<MazeHexagon>();
            
            while (hexesToCheck.TryDequeue(out var mazeHexagon))
            {
                hexesChecked.Add(mazeHexagon);
                foreach (var transition in mazeHexagon.MazeTransitions)
                {
                    if (transition == null || !transition.Activated || hexesChecked.Contains(transition.GetOtherNode(mazeHexagon)))
                    {
                        continue;
                    }

                    var otherNode = transition.GetOtherNode(mazeHexagon);
                    if (otherNode == hex2) return true;
                    hexesToCheck.Enqueue(otherNode);
                }
            }

            return false;
        }

        public string Print()
        {
            return MazeTransitions.Aggregate("", (current, transition) => current + MazePosition + (transition is {Activated: true} ? "+" : "-"));
        }
    }
}