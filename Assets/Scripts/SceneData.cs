using MazeGenerator.Labyrinth;

namespace DefaultNamespace
{
    public static class SceneData
    {
        //Labtype
        public static LabyrinthBase.LabType LabType = LabyrinthBase.LabType.Remember;

        //Maze-Generation
        public static int Size = 5;
        public static bool UseRandomSeed = true;
        public static int Seed = 124;

        //loadingScreen
        public static float SecondsPassed = 0;

        //endScreen
        public static bool Win = false;
    }
}