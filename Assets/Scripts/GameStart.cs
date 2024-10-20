using System;
using System.Collections.Generic;
using System.Diagnostics;
using MazeGenerator;
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
    private float _hexPlacementTimer;
    private Queue<MazeHexagon> _hexagonsToPlace;
    private Stopwatch _stopwatch;

    private void Start()
    {
        _stopwatch = new Stopwatch();
        _stopwatch.Start();
        
        _labyrinthModel = new Labyrinth();
        _labyrinthModel.Generate(size, seed, loopPercentage);
        _labyrinth = new MazeTile[size, size];
        _hexagonsToPlace = new Queue<MazeHexagon>();

        player.OnMoveToNewHex += PlayerOnOnMoveToNewHex;
        player.OnWin += PlayerOnOnWin;
        
        var mazeHexagon = _labyrinthModel.GetStartHexagon();
        var mazeTile = Instantiate(mazeTilePrefab,
            CalculateTilePosition(mazeHexagon.MazePosition.x, mazeHexagon.MazePosition.y), Quaternion.identity);
        mazeTile.SetMazeHexagon(mazeHexagon);
        _labyrinth[mazeHexagon.MazePosition.x, mazeHexagon.MazePosition.y] = mazeTile;
        player.transform.position = mazeTile.transform.position;
    }

    private void PlayerOnOnWin(object sender, EventArgs e)
    {
        _stopwatch.Stop();
        Debug.Log(_stopwatch.Elapsed);
    }

    private void PlayerOnOnMoveToNewHex(object sender, Player.NewHexEventArgs e)
    {
        foreach (var mazeTransition in e.Hexagon.MazeTransitions)
        {
            if (mazeTransition == null || !mazeTransition.Activated) continue;

            var hexagonToAdd = mazeTransition.GetOtherNode(e.Hexagon);
            if (_hexagonsToPlace.Contains(hexagonToAdd) ||
                _labyrinth[hexagonToAdd.MazePosition.x, hexagonToAdd.MazePosition.y] != null) continue;
            _hexagonsToPlace.Enqueue(hexagonToAdd);
        }
    }

    private void Update()
    {
        if (_hexagonsToPlace.Count == 0)
        {
            return;
        }

        _hexPlacementTimer += Time.deltaTime;
        if (_hexPlacementTimer <= initTimeMax)
        {
            return;
        }

        var hexagonToPlace = _hexagonsToPlace.Dequeue();
        var mazePositionX = hexagonToPlace.MazePosition.x;
        var mazePositionY = hexagonToPlace.MazePosition.y;
        var mazeTile = Instantiate(mazeTilePrefab,
            CalculateTilePosition(mazePositionX, mazePositionY), Quaternion.identity);
        mazeTile.SetMazeHexagon(hexagonToPlace);
        _labyrinth[mazePositionX, mazePositionY] = mazeTile;
        _hexPlacementTimer = 0;
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