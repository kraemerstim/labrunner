using System;
using System.Collections.Generic;
using System.Linq;
using MazeGenerator.Labyrinth;

namespace MazeGenerator
{
    public class MazeHexagon
    {
        public readonly MazeTransition[] MazeTransitions;
        public (int x, int y) MazePosition { get; private set; }

        public void AddMazeTransition(int direction, MazeTransition mazeTransition)
        {
            MazeTransitions[direction] = mazeTransition;
        }

        public int GetHexagonOpenings()
        {
            return MazeTransitions.Count(mazeTransition => mazeTransition != null && mazeTransition.Activated);
        }

        public void OpenRandomOpening(Random random)
        {
            var mazeTransition = LabUtil.ShuffleList(MazeTransitions.ToList(), random)
                .First(transition => transition is {Activated: false});
            mazeTransition.Activated = true;
        }

        public MazeHexagon(int x, int y)
        {
            MazePosition = (x, y);
            MazeTransitions = new MazeTransition[6];
        }

        public bool CheckConnectivityTo(MazeHexagon hex2)
        {
            var hexesToCheck = new Queue<MazeHexagon>();
            hexesToCheck.Enqueue(this);
            var hexesChecked = new HashSet<MazeHexagon>();

            while (hexesToCheck.TryDequeue(out var mazeHexagon))
            {
                hexesChecked.Add(mazeHexagon);
                foreach (var transition in mazeHexagon.MazeTransitions)
                {
                    if (transition is not {Activated: true} ||
                        hexesChecked.Contains(transition.GetOtherNode(mazeHexagon)))
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
    }
}