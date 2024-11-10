using System;
using System.Collections.Generic;

namespace MazeGenerator.Labyrinth
{
    public abstract class LabyrinthBase
    {
        public enum LabType
        {
            Square,
            Remember
        }

        public record LabOptions(int Size, int Seed, bool UseRandomSeed = false)
        {
            public int Size { get; } = Size;
            public int Seed { get; } = Seed;
            public bool UseRandomSeed { get; } = UseRandomSeed;
        }

        protected Dictionary<(int x, int y), MazeHexagon> _hexagons;
        protected HashSet<MazeTransition> _transitions;
        protected readonly Random _random;
        protected readonly LabOptions _options;
        protected MazeHexagon _startHexagon;
        protected MazeHexagon _endHexagon;

        public MazeHexagon GetStartHexagon()
        {
            return _startHexagon;
        }

        public MazeHexagon GetEndHexagon()
        {
            return _endHexagon;
        }

        protected LabyrinthBase(LabOptions options)
        {
            _options = options;
            _random = options.UseRandomSeed ? new Random() : new Random(options.Seed);
            _hexagons = new Dictionary<(int x, int y), MazeHexagon>();
            _transitions = new HashSet<MazeTransition>();
        }

        protected MazeHexagon CreateHexagon(int x, int y, bool createTransitionsActive = true)
        {
            if (_hexagons.ContainsKey((x, y))) return null;

            var newHexagon = new MazeHexagon(x, y);
            _hexagons.Add((x, y), newHexagon);

            for (var i = 0; i < 6; i++)
            {
                if (_hexagons.TryGetValue(LabUtil.GetDirectionalCoordinate((x, y), i), out var targetHexagon))
                {
                    var mazeTransition = CreateTransition(newHexagon, i, targetHexagon);
                    mazeTransition.Activated = createTransitionsActive;
                }
            }

            return newHexagon;
        }

        protected abstract void InitLabyrinth(int size);
        public abstract void Generate();

        public MazeTransition CreateTransition(MazeHexagon node1, int directionTo, MazeHexagon node2)
        {
            var mazeTransition = new MazeTransition(node1, directionTo, node2);
            _transitions.Add(mazeTransition);
            node1.AddMazeTransition(directionTo, mazeTransition);
            node2.AddMazeTransition((directionTo + 3) % 6, mazeTransition);
            return mazeTransition;
        }
    }
}