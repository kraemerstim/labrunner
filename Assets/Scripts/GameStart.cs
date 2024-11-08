using System;
using System.Collections.Generic;
using System.Diagnostics;
using DefaultNamespace;
using MazeGenerator;
using MazeGenerator.Labyrinth;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStart : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private MazeTile mazeTilePrefab;
    [SerializeField] private Player player;
    [SerializeField] private VisualConfig visualConfig;

    private Dictionary<(int x, int y), MazeTile> _labyrinth;
    private SquareLabyrinth _labyrinthModel;
    private float _hexPlacementTimer;
    private Queue<MazeHexagon> _hexagonsToPlace;
    private Stopwatch _stopwatch;

    private void Start()
    {
        player.OnWin += PlayerOnOnWin;
        _stopwatch = new Stopwatch();
        _stopwatch.Start();

        _labyrinthModel =
            new SquareLabyrinth(new LabyrinthBase.LabOptions(SceneData.size, SceneData.seed, SceneData.useRandomSeed));
        _labyrinthModel.Generate();
        _labyrinth = new Dictionary<(int x, int y), MazeTile>();
        _hexagonsToPlace = new Queue<MazeHexagon>();

        player.OnMoveToNewHex += PlayerOnOnMoveToNewHex;

        var mazeHexagon = _labyrinthModel.GetStartHexagon();
        var mazeTile = CreateMazeTile(mazeHexagon);
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
                _labyrinth.ContainsKey((hexagonToAdd.MazePosition.x, hexagonToAdd.MazePosition.y))) continue;
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
        CreateMazeTile(hexagonToPlace);
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

    private MazeTile CreateMazeTile(MazeHexagon hexagon)
    {
        var mazePositionX = hexagon.MazePosition.x;
        var mazePositionY = hexagon.MazePosition.y;
        var mazeTile = Instantiate(mazeTilePrefab,
            CalculateTilePosition(mazePositionX, mazePositionY), Quaternion.identity);
        mazeTile.SetMazeHexagon(hexagon, _labyrinthModel.GetEndHexagon().Equals(hexagon));
        _labyrinth.Add((mazePositionX, mazePositionY), mazeTile);
        return mazeTile;
    }
}