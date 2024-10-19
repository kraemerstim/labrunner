using System.Diagnostics;
using MazeGenerator;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class GameStart : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private MazeTile mazeTile;
    [SerializeField] private MazeTile mazeTilePrefab;
    [SerializeField] private float tileOffsetY = 17.4f;
    [SerializeField] private float tileOffsetX = 15.06f;
    [SerializeField] private int size = 4;
    [SerializeField] private int seed = 124;
    [SerializeField] private int loopPercentage;
    void Start()
    {
        var labyrinth = new Labyrinth();
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        labyrinth.Init(size, size);
        labyrinth.GenerateMaze(seed, loopPercentage);
        for (var i = 0; i < size; i++)
        {
            for (var j = 0; j < size; j++)
            {
                var mazeHexagon = labyrinth.GetMazeHexagon(j, i);
                mazeTile = Instantiate(mazeTilePrefab,
                    CalculateTilePosition(mazeHexagon.MazePosition.x, mazeHexagon.MazePosition.y), Quaternion.identity);
                mazeTile.SetMazeHexagon(mazeHexagon);
            }
        }

        stopwatch.Stop();
        Debug.Log($"Generation time needed: {stopwatch.Elapsed:g}");
        //labyrinth.Print();
    }

    private Vector3 CalculateTilePosition(int x, int y)
    {
        var xPosition = x * tileOffsetX;
        var yPosition = y * tileOffsetY;
        if (x % 2 != 0)
        {
            yPosition += tileOffsetY/2;
        }
        return new Vector3(xPosition, 0, yPosition);
    }
    
}
