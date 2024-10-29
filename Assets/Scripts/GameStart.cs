using System;
using System.Collections.Generic;
using System.Diagnostics;
using DefaultNamespace;
using MazeGenerator;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Debug = UnityEngine.Debug;

public class GameStart : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private MazeTile mazeTilePrefab;
    [SerializeField] private Player player;
    [FormerlySerializedAs("config")] [SerializeField] private VisualConfig visualConfig;

    private MazeTile[,] _labyrinth;
    private Labyrinth _labyrinthModel;
    private float _hexPlacementTimer;
    private Queue<MazeHexagon> _hexagonsToPlace;
    private Stopwatch _stopwatch;

    private void Start()
    {
        player.OnWin += PlayerOnOnWin;
        _stopwatch = new Stopwatch();
        _stopwatch.Start();
        
        _labyrinthModel = new Labyrinth();
        _labyrinthModel.Generate(SceneData.size, SceneData.useRandomSeed ? -1 : SceneData.seed, SceneData.loopPercentage);
        _labyrinth = new MazeTile[SceneData.size, SceneData.size];
        _hexagonsToPlace = new Queue<MazeHexagon>();

        player.OnMoveToNewHex += PlayerOnOnMoveToNewHex;
        
        var mazeHexagon = _labyrinthModel.GetStartHexagon();
        var mazeTile = Instantiate(mazeTilePrefab,
            CalculateTilePosition(mazeHexagon.MazePosition.x, mazeHexagon.MazePosition.y), Quaternion.identity);
        mazeTile.SetMazeHexagon(mazeHexagon);
        _labyrinth[mazeHexagon.MazePosition.x, mazeHexagon.MazePosition.y] = mazeTile;
        player.transform.position = mazeTile.transform.position;
    }
    
    private void PlayerOnOnWin(object sender, EventArgs e)
    {
        SceneManager.LoadScene(2);
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
        if (_hexPlacementTimer <= visualConfig.initTimeMax)
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
        var xPosition = x * visualConfig.tileOffsetX;
        var yPosition = y * visualConfig.tileOffsetY;
        if (x % 2 != 0)
        {
            yPosition += visualConfig.tileOffsetY / 2;
        }

        return new Vector3(xPosition, 0, yPosition);
    }
}