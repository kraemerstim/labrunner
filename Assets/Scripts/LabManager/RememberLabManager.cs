using System;
using System.Collections.Generic;
using Cinemachine;
using DefaultNamespace;
using MazeGenerator;
using MazeGenerator.Labyrinth;
using UnityEngine;

namespace LabManager
{
    public class RememberLabManager : MonoBehaviour, ILabManager
    {
        private enum GameState
        {
            PreGame,
            Game,
            Outro
        }

        [SerializeField] HexManager hexManager;
        private CinemachineVirtualCamera _mainCamera;
        private Player _player;
        public event EventHandler<ILabManager.GameOverEventArgs> OnGameEnd;

        private Rememberinth _labyrinthModel;
        private Queue<MazeHexagon> _path;
        private List<MazeHexagon> _alreadyTakenPath;

        private float _gameTimer;
        private float _outroTimer;

        private bool _hasWon;

        private int _level;
        private int _stage;
        private GameState _gameState;

        public void Awake()
        {
            _stage = 1;
            _level = 1;
            _gameState = GameState.PreGame;
        }

        private void GenerateLabyrinth()
        {
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
        }

        private void Start()
        {
            _mainCamera = (CinemachineVirtualCamera) CinemachineCore.Instance.GetVirtualCamera(0);
            _player = Player.Instance;
            _player.OnMoveToNewHex += PlayerMovedOnNewField;
            _player.OnGoalTouched += PlayerTouchedGoal;
            GenerateLabyrinth();
            StageStart();
        }

        public void PlayerMovedOnNewField(object sender, Player.NewHexEventArgs newHexEventArgs)
        {
            var hexagon = newHexEventArgs.Hexagon;

            if (_alreadyTakenPath.Contains(hexagon)) return;
            if (!ValidateStep(hexagon))
            {
                InitiateGameEnd(false);
                return;
            }

            hexManager.CreateAdjacentHexes(hexagon);
        }

        private bool ValidateStep(MazeHexagon hexagon)
        {
            if (_alreadyTakenPath.Contains(hexagon)) return true;
            if (_path.TryDequeue(out var queueHexagon) && queueHexagon == hexagon)
            {
                _alreadyTakenPath.Add(hexagon);
                hexManager.ColorTile(hexagon, MazeTile.HighlightType.Green);
                return true;
            }

            hexManager.ColorTile(hexagon, MazeTile.HighlightType.Red);
            return false;
        }

        public void PlayerTouchedGoal(object sender, EventArgs eventArgs)
        {
            switch (_stage)
            {
                case 3:
                    _gameState = GameState.Outro;
                    InitiateGameEnd(true);
                    return;
                default:
                    PrepareNextStage();
                    break;
            }
        }

        private void PrepareNextStage()
        {
            _stage++;
            _labyrinthModel.PrepareStage(_stage);
            Clear();
            StageStart();
        }

        private void Clear()
        {
            hexManager.Clear();
            _path?.Clear();
            _alreadyTakenPath?.Clear();
        }

        private void InitiateGameEnd(bool hasWon)
        {
            _gameState = GameState.Outro;
            _hasWon = hasWon;
            _player.IsMovementAllowed = false;
            _outroTimer = 0;
        }

        public float GetGameTime()
        {
            return _gameTimer;
        }

        public void StageStart()
        {
            _path = new Queue<MazeHexagon>(_labyrinthModel.GetPath());
            _alreadyTakenPath = new List<MazeHexagon>();
            var mazeHexagon = _labyrinthModel.GetStartHexagon();
            _player.transform.position = GetStartPosition();
            _gameState = GameState.Game;
            hexManager.CreateMazeTile(mazeHexagon);
        }

        public Vector3 GetStartPosition()
        {
            var mazePosition = _labyrinthModel.GetStartHexagon().MazePosition;
            return LabUtil.CalculateTilePosition(mazePosition.x, mazePosition.y);
        }

        private void Update()
        {
            switch (_gameState)
            {
                case GameState.Game:
                    _gameTimer += Time.deltaTime;
                    break;
                case GameState.Outro:
                {
                    _outroTimer += Time.deltaTime;
                    _mainCamera.m_Lens.FieldOfView = Mathf.Lerp(42, 120, _outroTimer / 3);
                    if (_outroTimer >= 3)
                    {
                        OnGameEnd?.Invoke(this, new ILabManager.GameOverEventArgs {Win = _hasWon});
                    }

                    break;
                }
            }
        }
    }
}