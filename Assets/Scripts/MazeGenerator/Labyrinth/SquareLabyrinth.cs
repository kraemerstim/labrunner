using System.Collections.Generic;
using System.Linq;

namespace MazeGenerator.Labyrinth
{
    public class SquareLabyrinth : LabyrinthBase
    {
        public SquareLabyrinth(LabOptions options) : base(options)
        {
        }

        public override void Generate()
        {
            InitLabyrinth(_options.Size);
            GenerateMaze();
            SelectStartAndFinish();
        }

        protected override void InitLabyrinth(int size)
        {
            for (var y = 0; y < size; y++)
            {
                for (var x = 0; x < size; x++)
                {
                    CreateHexagon(x, y);
                }
            }
        }

        protected void GenerateMaze()
        {
            //generate transition set
            var transitionList = LabUtil.ShuffleList(_transitions.ToList(), _random);
            foreach (var transition in transitionList)
            {
                transition.Activated = false;
                transition.Activated = !transition.Node1.CheckPathTo(transition.Node2);
            }

            var hexagonList = LabUtil.ShuffleList(_hexagons.Values.ToList(), _random);

            foreach (var hexagon in hexagonList.Where(hexagon =>
                         hexagon.GetHexagonOpenings(
                             mazeTransition => mazeTransition != null && mazeTransition.Activated) < 2))
            {
                hexagon.OpenRandomOpening(_random, transition => transition is {Activated: false});
            }
        }

        private void SelectStartAndFinish()
        {
            //Select Start Tile
            _startHexagon = _hexagons.Values.ToList()[_random.Next(_hexagons.Count)];

            //Select Finish Tile
            var hexesToCheck = new Queue<MazeHexagon>();
            var hexesChecked = new List<MazeHexagon>();

            hexesToCheck.Enqueue(_startHexagon);

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
                    hexesToCheck.Enqueue(otherNode);
                }
            }

            _endHexagon = hexesChecked.Last();
        }
    }
}