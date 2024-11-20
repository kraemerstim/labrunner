using System.Collections.Generic;

namespace MazeGenerator.Labyrinth
{
    public class Rememberinth : LabyrinthBase
    {
        private List<MazeHexagon> _pathHexagons = new();
        private List<MazeHexagon> _falseHexagons = new();

        public Rememberinth(LabOptions options) : base(options)
        {
        }

        public override void Generate()
        {
            InitLabyrinth(_options.Size);
        }

        public void PrepareStage(int stage)
        {
            switch (stage)
            {
                case 2:
                    GenerateMaze();
                    break;
                case 3:
                    SetAllTransitionsActive();
                    break;
            }
        }

        private void SetAllTransitionsActive()
        {
            foreach (var transition in _transitions)
            {
                transition.Activated = true;
            }
        }

        public List<MazeHexagon> GetPath()
        {
            return new List<MazeHexagon>(_pathHexagons);
        }

        protected override void InitLabyrinth(int size)
        {
            var tempHexagon = new MazeHexagon(0, 0);
            _startHexagon = tempHexagon;
            _hexagons.Add((0, 0), tempHexagon);
            _pathHexagons.Add(_startHexagon);
            for (var i = 1; i <= size; i++)
            {
                var randomFreeTransitionDirection = tempHexagon.GetRandomFreeTransitionDirection(_random);
                var directionalCoordinate =
                    LabUtil.GetDirectionalCoordinate(tempHexagon.MazePosition, randomFreeTransitionDirection);
                var newHexagon = CreateHexagon(directionalCoordinate.x, directionalCoordinate.y, false);
                _pathHexagons.Add(newHexagon);
                tempHexagon.MazeTransitions[randomFreeTransitionDirection].Activated = true;
                tempHexagon = newHexagon;
            }

            tempHexagon.IsGoal = true;
            _endHexagon = tempHexagon;
            CreateSurroundingHexagons(2);
        }

        private void CreateSurroundingHexagons(int size)
        {
            var todoList = new List<MazeHexagon>(_hexagons.Values);
            var nextTodoList = new List<MazeHexagon>();

            for (var i = 0; i < size; i++)
            {
                foreach (var tempHexagon in todoList)
                {
                    for (var direction = 0; direction < 6; direction++)
                    {
                        var directionalCoordinate =
                            LabUtil.GetDirectionalCoordinate(tempHexagon.MazePosition, direction);
                        if (_hexagons.ContainsKey(directionalCoordinate)) continue;
                        var newHexagon = CreateHexagon(directionalCoordinate.x, directionalCoordinate.y, false);
                        _falseHexagons.Add(newHexagon);
                        nextTodoList.Add(newHexagon);
                    }
                }

                todoList = new List<MazeHexagon>(nextTodoList);
                nextTodoList.Clear();
            }
        }

        protected void GenerateMaze()
        {
            var reachedHexagons = new List<MazeHexagon>(_pathHexagons);
            foreach (var hexagon in _falseHexagons)
            {
                hexagon.OpenRandomOpening(_random,
                    transition => transition is {Activated: false} &&
                                  reachedHexagons.Contains(transition.GetOtherNode(hexagon)));
                reachedHexagons.Add(hexagon);
            }
        }
    }
}