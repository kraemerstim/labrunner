using System.Collections.Generic;
using System.Linq;
using Random = System.Random;

namespace MazeGenerator
{
    public class Labyrinth
    {
        private Dictionary<(int x, int y), MazeHexagon> _hexagons;
        private HashSet<MazeTransition> _transitions;
        private Random _random;
        private MazeHexagon _startHexagon;

        public MazeHexagon GetStartHexagon()
        {
            return _startHexagon;
        }

        public void Generate(int size, Random random)
        {
            _hexagons = new Dictionary<(int x, int y), MazeHexagon>();
            _transitions = new HashSet<MazeTransition>();
            _random = random;

            InitLabyrinth(size);
            GenerateMaze();
            SelectStartAndFinish();
        }

        private void InitLabyrinth(int size)
        {
            for (var y = 0; y < size; y++)
            {
                for (var x = 0; x < size; x++)
                {
                    var tempHexagon = new MazeHexagon(x, y);
                    if (x > 0)
                    {
                        var target = _hexagons[(x - 1, y)];
                        CreateTransition(tempHexagon, x % 2 == 1 ? 4 : 5, target);
                    }

                    if (y > 0)
                    {
                        var target = _hexagons[(x, y - 1)];
                        CreateTransition(tempHexagon, 3, target);
                        if (x % 2 == 0)
                        {
                            if (x > 0)
                            {
                                target = _hexagons[(x - 1, y - 1)];
                                CreateTransition(tempHexagon, 4, target);
                            }

                            if (x < size - 1)
                            {
                                target = _hexagons[(x + 1, y - 1)];
                                CreateTransition(tempHexagon, 2, target);
                            }
                        }
                    }

                    _hexagons.Add((x, y), tempHexagon);
                }
            }
        }

        public MazeTransition CreateTransition(MazeHexagon node1, int directionTo, MazeHexagon node2)
        {
            var mazeTransition = new MazeTransition(node1, node2);
            _transitions.Add(mazeTransition);
            node1.AddMazeTransition(directionTo, mazeTransition);
            node2.AddMazeTransition((directionTo + 3) % 6, mazeTransition);
            return mazeTransition;
        }

        private void GenerateMaze()
        {
            //generate transition set
            var transitionList = LabUtil.ShuffleList(_transitions.ToList(), _random);
            foreach (var transition in transitionList)
            {
                transition.Activated = false;
                transition.Activated = !transition.Node1.CheckConnectivityTo(transition.Node2);
            }

            var hexagonList = LabUtil.ShuffleList(_hexagons.Values.ToList(), _random);

            foreach (var hexagon in hexagonList.Where(hexagon => hexagon.GetHexagonOpenings() < 2))
            {
                hexagon.OpenRandomOpening(_random);
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

            hexesChecked.Last().IsFinish = true;
        }
    }
}