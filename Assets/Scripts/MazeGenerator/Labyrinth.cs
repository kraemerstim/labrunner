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

        public void Generate(int size, int seed, int loopPercentage)
        {
            _labyrinth = new MazeHexagon[size, size];
            _transitions = new MazeTransition.MazeTransitionCollection();
            _random = seed == -1 ? new Random() : new Random(seed);

            InitLabyrinth();
            GenerateMaze(loopPercentage);
            SelectStartAndFinish();
        }

        private List<MazeTransition> ShuffleList(List<MazeTransition> inputList)
        {
            var inputListCount = inputList.Count;
            var tempList = new List<MazeTransition>();
            tempList.AddRange(inputList);

            for (var i = 0; i < inputListCount; i++)
            {
                var r = _random.Next(i, tempList.Count);
                (tempList[i], tempList[r]) = (tempList[r], tempList[i]);
            }

            return tempList;
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

        private void GenerateMaze(int loopPercentage)
        {
            //generate transition set
            var transitionList = ShuffleList(_transitions.GetTransitions().ToList());
            var disposableTransitions = new List<MazeTransition>();
            foreach (var transition in transitionList)
            {
                transition.Activated = false;
                var checkConnectivityTo2 = transition.Node1.CheckConnectivityTo2(transition.Node2);
                transition.Activated = !checkConnectivityTo2;
                if (checkConnectivityTo2) disposableTransitions.Add(transition);
            }

            for (var i = 0; i < (int) (disposableTransitions.Count * (loopPercentage / 100.0)); i++)
            {
                disposableTransitions[i].Activated = true;
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