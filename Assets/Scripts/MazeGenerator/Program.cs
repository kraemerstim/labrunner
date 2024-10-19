using System;
using System.Diagnostics;

namespace MazeGenerator
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var labyrinth = new Labyrinth();
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            labyrinth.Init(30, 30);
            labyrinth.GenerateMaze(124,0);
            stopwatch.Stop();
            Console.WriteLine($"time needed: {stopwatch.Elapsed:g}");
        }
    }
}