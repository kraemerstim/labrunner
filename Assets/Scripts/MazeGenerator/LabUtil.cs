using System;
using System.Collections.Generic;

namespace MazeGenerator
{
    public class LabUtil
    {
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
    }
}