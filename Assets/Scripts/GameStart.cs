using System;
using MazeGenerator;
using Unity.VisualScripting;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class GameStart : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private MazeTile mazeTilePrefab;
    [SerializeField] private Player player;
    [SerializeField] private float tileOffsetY = 17.4f;
    [SerializeField] private float tileOffsetX = 15.06f;
    [SerializeField] private int size = 4;
    [SerializeField] private int seed = 124;
    [SerializeField] private int loopPercentage;
    [SerializeField] private float initTimeMax = 0.1f;
    
    private MazeTile[,] _labyrinth;
    private Labyrinth _labyrinthModel;
    private bool _initFinished;
    private float _initTimer;

    private void Start()
    {
        _labyrinthModel = new Labyrinth();
        _labyrinthModel.Generate(size, seed, loopPercentage);
        _labyrinth = new MazeTile[size, size];
        var mazeHexagon = _labyrinthModel.GetStartHexagon();
        var mazeTile = Instantiate(mazeTilePrefab,
            CalculateTilePosition(mazeHexagon.MazePosition.x, mazeHexagon.MazePosition.y), Quaternion.identity);
        mazeTile.SetMazeHexagon(mazeHexagon);
        _labyrinth[mazeHexagon.MazePosition.x, mazeHexagon.MazePosition.y] = mazeTile;
        player.transform.position = mazeTile.transform.position;
    }

    private void Update()
    {
        if (_initFinished)
        {
            return;
        }

        _initTimer += Time.deltaTime;
        if (_initTimer <= initTimeMax)
        {
            return;
        }
        
        for (var i = 0; i < size; i++)
        {
            for (var j = 0; j < size; j++)
            {
                if (_labyrinth[i, j])
                {
                    continue;
                }

                var mazeHexagon = _labyrinthModel.GetMazeHexagon(j, i);
                var mazeTile = Instantiate(mazeTilePrefab,
                    CalculateTilePosition(j, i), Quaternion.identity);
                mazeTile.SetMazeHexagon(mazeHexagon);
                _labyrinth[i, j] = mazeTile;
                _initTimer = 0;
                return;
            }
        }
        _initFinished = true;
    }

    private Vector3 CalculateTilePosition(int x, int y)
    {
        var xPosition = x * tileOffsetX;
        var yPosition = y * tileOffsetY;
        if (x % 2 != 0)
        {
            yPosition += tileOffsetY / 2;
        }

        return new Vector3(xPosition, 0, yPosition);
    }
}