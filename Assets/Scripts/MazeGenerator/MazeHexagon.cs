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

        public MazeHexagon(int x, int y)
        {
            MazePosition = (x, y);
            MazeTransitions = new MazeTransition[6];
        }

        public void AddMazeTransition(int direction, MazeTransition mazeTransition)
        {
            MazeTransitions[direction] = mazeTransition;
        }

        public int GetHexagonOpenings(Func<MazeTransition, bool> selection)
        {
            return MazeTransitions.Count(selection);
        }

        public void OpenRandomOpening(Random random, Func<MazeTransition, bool> selection)
        {
            var mazeTransition = LabUtil.ShuffleList(MazeTransitions.ToList(), random)
                .First(selection);
            mazeTransition.Activated = true;
        }

        public bool CheckPathTo(MazeHexagon targetHexagon)
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
                    if (otherNode == targetHexagon) return true;
                    hexesToCheck.Enqueue(otherNode);
                }
            }

            return false;
        }

        public int GetRandomFreeTransitionDirection(Random random)
        {
            var nullTransitions = new List<int>();
            for (var i = 0; i < MazeTransitions.Length; i++)
            {
                if (MazeTransitions[i] == null)
                {
                    nullTransitions.Add(i);
                }
            }

            if (nullTransitions.Count == 0) return -1;
            return LabUtil.ShuffleList(nullTransitions, random)
                .First();
        }
    }
}