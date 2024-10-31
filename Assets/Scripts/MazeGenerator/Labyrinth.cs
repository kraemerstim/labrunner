using System.Collections.Generic;
using System.Linq;
using Random = System.Random;

namespace MazeGenerator
{
    public class Labyrinth
    {
        private MazeHexagon[,] _labyrinth;
        private MazeTransition.MazeTransitionCollection _transitions;
        private Random _random;
        private MazeHexagon _startHexagon;

        public MazeHexagon GetMazeHexagon(int x, int y)
        {
            return _labyrinth[x, y];
        }

        public MazeHexagon GetStartHexagon()
        {
            return _startHexagon;
        }

        public void Generate(int size, Random random)
        {
            _labyrinth = new MazeHexagon[size, size];
            _transitions = new MazeTransition.MazeTransitionCollection();
            _random = random;

            InitLabyrinth();
            GenerateMaze();
            SelectStartAndFinish();
        }

        private void InitLabyrinth()
        {
            var height = _labyrinth.GetLength(1);
            var width = _labyrinth.GetLength(0);
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    var tempHexagon = new MazeHexagon(x, y);
                    if (x > 0)
                    {
                        var target = _labyrinth[x - 1, y];
                        if (x % 2 == 1)
                        {
                            _transitions.CreateTransition(tempHexagon, 4, target, 1);
                        }
                        else
                        {
                            _transitions.CreateTransition(tempHexagon, 5, target, 2);
                        }
                    }

                    if (y > 0)
                    {
                        var target = _labyrinth[x, y - 1];
                        _transitions.CreateTransition(tempHexagon, 3, target, 0);
                        if (x % 2 == 0)
                        {
                            if (x > 0)
                            {
                                target = _labyrinth[x - 1, y - 1];
                                _transitions.CreateTransition(tempHexagon, 4, target, 1);
                            }

                            if (x < width - 1)
                            {
                                target = _labyrinth[x + 1, y - 1];
                                _transitions.CreateTransition(tempHexagon, 2, target, 5);
                            }
                        }
                    }

                    _labyrinth[x, y] = tempHexagon;
                }
            }
        }

        private void GenerateMaze()
        {
            //generate transition set
            var transitionList = LabUtil.ShuffleList(_transitions.GetTransitions().ToList(), _random);
            foreach (var transition in transitionList)
            {
                transition.Activated = false;
                transition.Activated = !transition.Node1.CheckConnectivityTo2(transition.Node2);
            }

            var hexagonList = new List<MazeHexagon>();
            var height = _labyrinth.GetLength(1);
            var width = _labyrinth.GetLength(0);
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    hexagonList.Add(_labyrinth[x, y]);
                }
            }
            
            hexagonList = LabUtil.ShuffleList(hexagonList,_random);

            foreach (var hexagon in hexagonList)
            {
                if (hexagon.GetHexagonOpenings() < 2)
                {
                    hexagon.OpenRandomOpening(_random);
                }
            }
        }

        private void SelectStartAndFinish()
        {
            //Select Start Tile
            _startHexagon = _labyrinth[_random.Next(_labyrinth.GetLength(0)), _random.Next(_labyrinth.GetLength(1))];
            
            //Select Finish Tile
            var hexesToCheck = new Queue<MazeHexagon>();
            var hexesChecked = new List<MazeHexagon>();
            
            hexesToCheck.Enqueue(_startHexagon);
            
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
                    hexesToCheck.Enqueue(otherNode);
                }
            }

            hexesChecked.Last().IsFinish = true;
        }
    }
}