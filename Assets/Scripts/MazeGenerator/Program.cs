using System;
using System.Diagnostics;
using MazeGenerator.Labyrinth;

namespace MazeGenerator
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var labyrinth = new SquareLabyrinth(new LabyrinthBase.LabOptions(30, 124, true));
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            labyrinth.Generate();
            stopwatch.Stop();
            Console.WriteLine($"time needed: {stopwatch.Elapsed:g}");
        }
    }
}