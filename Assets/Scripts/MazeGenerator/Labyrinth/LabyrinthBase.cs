using System;
using System.Collections.Generic;

namespace MazeGenerator.Labyrinth
{
    public abstract class LabyrinthBase
    {
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

        protected LabyrinthBase(LabOptions options)
        {
            _options = options;
            _random = options.UseRandomSeed ? new Random() : new Random(options.Seed);
            _hexagons = new Dictionary<(int x, int y), MazeHexagon>();
            _transitions = new HashSet<MazeTransition>();
        }

        protected abstract void InitLabyrinth(int size);
        protected abstract void GenerateMaze();

        public MazeTransition CreateTransition(MazeHexagon node1, int directionTo, MazeHexagon node2)
        {
            var mazeTransition = new MazeTransition(node1, node2);
            _transitions.Add(mazeTransition);
            node1.AddMazeTransition(directionTo, mazeTransition);
            node2.AddMazeTransition((directionTo + 3) % 6, mazeTransition);
            return mazeTransition;
        }
    }
}