using System;
using System.Collections.Generic;
using System.Diagnostics;
using DefaultNamespace;
using MazeGenerator;
using MazeGenerator.Labyrinth;
using UnityEngine;

namespace LabManager
{
    public class SquareLabManager : MonoBehaviour, ILabManager
    {
        [SerializeField] private MazeTile mazeTilePrefab;

        public event EventHandler<ILabManager.GameOverEventArgs> OnGameEnd;
        public event EventHandler OnGameReset;

        private Dictionary<(int x, int y), MazeTile> _labyrinth;
        private LabyrinthBase _labyrinthModel;
        private float _hexPlacementTimer;
        private Queue<MazeHexagon> _hexagonsToPlace;
        private Stopwatch _stopwatch;
        private float _gameTimer = 0f;

        public void Awake()
        {
            _labyrinthModel =
                new SquareLabyrinth(new LabyrinthBase.LabOptions(SceneData.Size, SceneData.Seed,
                    SceneData.UseRandomSeed));
            _labyrinthModel.Generate();

            _labyrinth = new Dictionary<(int x, int y), MazeTile>();
            _hexagonsToPlace = new Queue<MazeHexagon>();
        }

        public void PlayerMovedOnNewField(MazeHexagon hexagon)
        {
            foreach (var mazeTransition in hexagon.MazeTransitions)
            {
                if (mazeTransition is not {Activated: true}) continue;

                var hexagonToAdd = mazeTransition.GetOtherNode(hexagon);
                if (_hexagonsToPlace.Contains(hexagonToAdd) ||
                    _labyrinth.ContainsKey((hexagonToAdd.MazePosition.x, hexagonToAdd.MazePosition.y))) continue;
                _hexagonsToPlace.Enqueue(hexagonToAdd);
            }
        }

        public void PlayerTouchedGoal()
        {
            OnGameEnd?.Invoke(this, new ILabManager.GameOverEventArgs {Win = true});
        }

        public float GetGameTime()
        {
            return _gameTimer;
        }

        public void GameStart()
        {
            var mazeHexagon = _labyrinthModel.GetStartHexagon();
            CreateMazeTile(mazeHexagon);
        }

        public Vector3 GetStartPosition()
        {
            var mazePosition = _labyrinthModel.GetStartHexagon().MazePosition;
            return LabUtil.CalculateTilePosition(mazePosition.x, mazePosition.y);
        }

        private void Update()
        {
            _gameTimer += Time.deltaTime;
            if (_hexagonsToPlace.Count == 0)
            {
                return;
            }

            _hexPlacementTimer += Time.deltaTime;
            if (_hexPlacementTimer <= LabUtil.InitTimeMax)
            {
                return;
            }

            var hexagonToPlace = _hexagonsToPlace.Dequeue();
            CreateMazeTile(hexagonToPlace);
            _hexPlacementTimer = 0;
        }

        private void CreateMazeTile(MazeHexagon hexagon)
        {
            var mazePositionX = hexagon.MazePosition.x;
            var mazePositionY = hexagon.MazePosition.y;
            var mazeTile = Instantiate(mazeTilePrefab,
                LabUtil.CalculateTilePosition(mazePositionX, mazePositionY), Quaternion.identity);
            mazeTile.SetMazeHexagon(hexagon, _labyrinthModel.GetEndHexagon().Equals(hexagon));
            _labyrinth.Add((mazePositionX, mazePositionY), mazeTile);
        }
    }
}