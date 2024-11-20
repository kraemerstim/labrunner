using System;

namespace LabManager
{
    public interface ILabManager
    {
        public class GameOverEventArgs
        {
            public bool Win;
        }

        event EventHandler<GameOverEventArgs> OnGameEnd;
        void StageStart();
        float GetGameTime();
    }
}