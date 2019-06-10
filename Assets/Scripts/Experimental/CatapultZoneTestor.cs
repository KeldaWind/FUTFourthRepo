using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatapultZoneTestor : MonoBehaviour
{
    [SerializeField] GameObject testPrefab;//
    [SerializeField] float objectsRadius;
    [SerializeField] float objectsSpacing;
    [SerializeField] float objectsRandomization;
    [SerializeField] float zoneRadius;
    [SerializeField] int maxCircles;
    [SerializeField] int numberOfObjectsToSpawn;//
    [SerializeField] bool simulateCatapult;//

    private void Start()
    {
        SpawnObjectsInRadius();
    }

    public void SpawnObjectsInRadius()
    {
        List<Vector3> allPossiblePositions = new List<Vector3>();
        allPossiblePositions.Add(Vector3.zero);
        bool outOfRadius = false;
        int circleCounter = 1;
        Vector3 lateralStep = new Vector3(objectsRadius + objectsSpacing, 0, 0);
        Vector3 diagonalStep = Quaternion.Euler(0, 60, 0) * lateralStep;
        while (!outOfRadius && circleCounter < maxCircles)
        {
            int lateralBaseValue = circleCounter/2;
            if (circleCounter % 2 != 0)
                lateralBaseValue++;
            int diagonalBaseValue = circleCounter/2;

            bool alternate = true;
            int alternanceCounter = 0;
            for(int i = 0; i < circleCounter; i++)
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
                if(newPos.magnitude < zoneRadius)
                {
                    allPossiblePositions.Add(newPos);
                    for(int j = 1; j < 6; j++)
                        allPossiblePositions.Add(Quaternion.Euler(0, 60 * j, 0) * newPos);
                }
            }
            circleCounter++;
        }

        Vector3 basePos = transform.position;

        if (simulateCatapult)
        {
            for (int i = 0; i < numberOfObjectsToSpawn; i++)
            {
                int randomIndex = Random.Range(0, allPossiblePositions.Count);
                GameObject newObj = Instantiate(testPrefab, basePos + allPossiblePositions[randomIndex] + new Vector3(Random.Range(-objectsRandomization, objectsRandomization), 0, Random.Range(-objectsRandomization, objectsRandomization)), Quaternion.identity);
                newObj.transform.localScale = Vector3.one * objectsRadius;
                allPossiblePositions.RemoveAt(randomIndex);

                if (allPossiblePositions.Count == 0)
                    break;
            }
        }
        else
        {
            foreach(Vector3 pos in allPossiblePositions)
            {
                GameObject newObj = Instantiate(testPrefab, basePos + pos, Quaternion.identity);
                newObj.transform.localScale = Vector3.one * objectsRadius;
            }
        }
    }
}
