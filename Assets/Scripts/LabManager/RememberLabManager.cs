using System;
using System.Collections.Generic;
using System.Diagnostics;
using DefaultNamespace;
using MazeGenerator;
using MazeGenerator.Labyrinth;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace LabManager
{
    public class RememberLabManager : MonoBehaviour, ILabManager
    {
        [SerializeField] private MazeTile mazeTilePrefab;

        public event EventHandler<ILabManager.GameOverEventArgs> OnGameEnd;
        public event EventHandler OnGameReset;

        private Dictionary<(int x, int y), MazeTile> _labyrinth;
        private Rememberinth _labyrinthModel;
        private float _hexPlacementTimer;
        private Queue<MazeHexagon> _hexagonsToPlace;
        private Stopwatch _stopwatch;
        private Queue<MazeHexagon> _path;

        private float _gameTimer = 0f;

        private int _stage;

        public void Awake()
        {
            _stage = 1;
            var isGenerated = false;

            while (!isGenerated)
            {
                _labyrinthModel =
                    new Rememberinth(new LabyrinthBase.LabOptions(SceneData.Size, SceneData.Seed,
                        SceneData.UseRandomSeed));
                try
                {
                    _labyrinthModel.Generate();
                    isGenerated = true;
                }
                catch (Exception)
                {
                    if (!SceneData.UseRandomSeed)
                    {
                        SceneData.Seed++;
                    }
                }
            }

            _labyrinth = new Dictionary<(int x, int y), MazeTile>();
            _hexagonsToPlace = new Queue<MazeHexagon>();
        }

        public void PlayerMovedOnNewField(MazeHexagon hexagon)
        {
            Debug.Log($"PlayerMovedOnNewField {hexagon.MazePosition}");
            if (_path.Dequeue() != hexagon)
            {
                OnGameEnd?.Invoke(this, new ILabManager.GameOverEventArgs {Win = false});
                return;
            }

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
            switch (_stage)
            {
                case 3:
                    OnGameEnd?.Invoke(this, new ILabManager.GameOverEventArgs {Win = true});
                    break;
                default:
                    _labyrinthModel.OnStageChange(_stage);
                    break;
            }

            _stage++;
            ResetGame();
        }

        private void ResetGame()
        {
            _hexagonsToPlace.Clear();
            foreach (var tile in _labyrinth.Values)
            {
                Destroy(tile.gameObject);
            }

            _labyrinth.Clear();

            OnGameReset?.Invoke(this, EventArgs.Empty);
        }

        public float GetGameTime()
        {
            return _gameTimer;
        }

        public void GameStart()
        {
            _path = new Queue<MazeHexagon>(_labyrinthModel.GetPath());
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
            if (IsGameTimerRunning())
            {
                _gameTimer += Time.deltaTime;
            }

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

        private bool IsGameTimerRunning()
        {
            return true;
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