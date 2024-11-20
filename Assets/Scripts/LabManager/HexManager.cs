using System.Collections.Generic;
using MazeGenerator;
using MazeGenerator.Labyrinth;
using UnityEngine;

namespace LabManager
{
    public class HexManager : MonoBehaviour
    {
        [SerializeField] private MazeTile mazeTilePrefab;

        private Queue<MazeHexagon> _hexagonsToPlace;
        private Dictionary<(int x, int y), MazeTile> _labyrinth;
        private float _hexPlacementTimer;

        private void Awake()
        {
            _hexagonsToPlace = new Queue<MazeHexagon>();
            _hexPlacementTimer = 0f;
            _labyrinth = new Dictionary<(int x, int y), MazeTile>();
        }

        private void Update()
        {
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

        public void CreateAdjacentHexes(MazeHexagon hexagon)
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

        public void CreateMazeTile(MazeHexagon hexagon)
        {
            var mazePositionX = hexagon.MazePosition.x;
            var mazePositionY = hexagon.MazePosition.y;
            var mazeTile = Instantiate(mazeTilePrefab,
                LabUtil.CalculateTilePosition(mazePositionX, mazePositionY), Quaternion.identity);
            mazeTile.SetMazeHexagon(hexagon);
            _labyrinth.Add((mazePositionX, mazePositionY), mazeTile);
        }

        public void ColorTile(MazeHexagon hexagon, MazeTile.HighlightType color)
        {
            _labyrinth[hexagon.MazePosition].Highlight(color);
        }

        public void Clear()
        {
            _hexagonsToPlace.Clear();
            foreach (var tile in _labyrinth.Values)
            {
                Destroy(tile.gameObject);
            }

            _labyrinth.Clear();
        }
    }
}