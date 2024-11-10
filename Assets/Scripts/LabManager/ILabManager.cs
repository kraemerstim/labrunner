using System;
using MazeGenerator;
using UnityEngine;

namespace LabManager
{
    public interface ILabManager
    {
        public class GameOverEventArgs
        {
            public bool Win;
        }

        event EventHandler<GameOverEventArgs> OnGameEnd;
        event EventHandler OnGameReset;
        Vector3 GetStartPosition();
        void GameStart();
        void PlayerMovedOnNewField(MazeHexagon hexagon);
        void PlayerTouchedGoal();
        float GetGameTime();
    }
}