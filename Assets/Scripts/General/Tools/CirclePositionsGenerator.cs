using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CirclePositionsGenerator 
{
    static int iterationsLimit = 100;

    public static List<Vector3> GetAllPositionsInCircle(float objectsRadius, float objectsSpacing, float zoneRadius)
    {
        List<Vector3> allPossiblePositions = new List<Vector3>();
        allPossiblePositions.Add(Vector3.zero);
        bool outOfRadius = false;
        int circleCounter = 1;
        Vector3 lateralStep = new Vector3(objectsRadius + objectsSpacing, 0, 0);
        Vector3 diagonalStep = Quaternion.Euler(0, 60, 0) * lateralStep;
        while (!outOfRadius && circleCounter < iterationsLimit)
        {
            int lateralBaseValue = circleCounter / 2;
            if (circleCounter % 2 != 0)
                lateralBaseValue++;
            int diagonalBaseValue = circleCounter / 2;

            bool alternate = true;
            int alternanceCounter = 0;
            for (int i = 0; i < circleCounter; i++)
            {
                int lateralValue = lateralBaseValue;
                int diagonalValue = diagonalBaseValue;
                if (alternate)
                {
                    lateralValue += alternanceCounter;
                    diagonalValue -= alternanceCounter;
                    alternanceCounter++;
                    alternate = false;
                }
                else
                {
                    lateralValue -= alternanceCounter;
                    diagonalValue += alternanceCounter;
                    alternate = true;
                }

                Vector3 newPos = lateralValue * lateralStep + diagonalValue * diagonalStep;
                if (newPos.magnitude < zoneRadius)
                {
                    allPossiblePositions.Add(newPos);
                    for (int j = 1; j < 6; j++)
                        allPossiblePositions.Add(Quaternion.Euler(0, 60 * j, 0) * newPos);
                }
            }
            circleCounter++;
        }

        return allPossiblePositions;
    }
}
