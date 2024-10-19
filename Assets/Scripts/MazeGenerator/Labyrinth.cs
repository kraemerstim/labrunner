using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

namespace MazeGenerator
{
    public class Labyrinth
    {
        private MazeHexagon[,] _labyrinth;
        private MazeTransition.MazeTransitionCollection _transitions;

        public MazeHexagon GetMazeHexagon(int x, int y)
        {
            return _labyrinth[x, y];
        }

        private List<MazeTransition> ShuffleList(List<MazeTransition> inputList, int seed)
        {
            var inputListCount = inputList.Count;
            var random = new Random(seed);
            var tempList = new List<MazeTransition>();
            tempList.AddRange(inputList);

            for (var i = 0; i < inputListCount; i++)
            {
                var r = random.Next(i, tempList.Count);
                (tempList[i], tempList[r]) = (tempList[r], tempList[i]);
            }

            return tempList;
        }

        public void Init(int width, int height)
        {
            _labyrinth = new MazeHexagon[width, height];
            _transitions = new MazeTransition.MazeTransitionCollection();
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

        public void Print()
        {
            for (var y = 0; y < _labyrinth.GetLength(1); y++)
            {
                for (var x = 0; x < _labyrinth.GetLength(0); x++)
                {
                    Debug.Log(_labyrinth[x, y].Print());
                }
            }
        }

        public void GenerateMaze(int seed, int loopPercentage)
        {
            //generate transition set
            var transitionList = ShuffleList(_transitions.GetTransitions().ToList(), seed);
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
    }
}