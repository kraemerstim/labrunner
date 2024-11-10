using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace MazeGenerator.Labyrinth
{
    public class LabUtil
    {
        private const float TileOffsetY = 17.4f;
        private const float TileOffsetX = 15.06f;
        public static float InitTimeMax = 0.1f;

        public static Vector3 CalculateTilePosition(int x, int y)
        {
            var xPosition = x * TileOffsetX;
            var yPosition = y * TileOffsetY;
            if (x % 2 != 0)
            {
                yPosition += TileOffsetY / 2;
            }

            return new Vector3(xPosition, 0, yPosition);
        }

        public static List<T> ShuffleList<T>(List<T> inputList, Random random)
        {
            var inputListCount = inputList.Count;
            var tempList = new List<T>();
            tempList.AddRange(inputList);

            for (var i = 0; i < inputListCount; i++)
            {
                var r = random.Next(i, tempList.Count);
                (tempList[i], tempList[r]) = (tempList[r], tempList[i]);
            }

            return tempList;
        }

        public static (int x, int y) GetDirectionalCoordinate((int x, int y) source, int direction)
        {
            return direction switch
            {
                0 => (source.x, source.y + 1),
                1 => (source.x + 1, source.x % 2 == 0 ? source.y : source.y + 1),
                2 => (source.x + 1, source.x % 2 == 0 ? source.y - 1 : source.y),
                3 => (source.x, source.y - 1),
                4 => (source.x - 1, source.x % 2 == 0 ? source.y - 1 : source.y),
                5 => (source.x - 1, source.x % 2 == 0 ? source.y : source.y + 1),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}