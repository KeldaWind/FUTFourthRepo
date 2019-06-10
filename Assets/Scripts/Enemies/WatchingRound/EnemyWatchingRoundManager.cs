using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyWatchingRoundManager
{
    [SerializeField] EnemyWatchingRoundParameters watchingRoundParameters = new EnemyWatchingRoundParameters(5);
}

[System.Serializable]
public struct EnemyWatchingRoundParameters
{
    public bool IsNull { get { return watchingPositions.Count == 0; } }

    public EnemyWatchingRoundParameters(float minDistanceFromPoint)
    {
        watchingPositions = new List<Transform>();
        minDistanceFromPointToPass = minDistanceFromPoint;
        aimedWatchingPosition = null;
    }

    [SerializeField] List<Transform> watchingPositions;
    public List<Transform> GetAllWatchingPositions
    {
        get
        {
            return watchingPositions;
        }
    }

    float minDistanceFromPointToPass;
    public float GetMinDistanceFromPointToPass { get { return minDistanceFromPointToPass; } }
    Transform aimedWatchingPosition;
    public Vector3 GetAimedWatchingPosition { get { return aimedWatchingPosition.position; } }

    public void SetUp()
    {
        minDistanceFromPointToPass = 10;

        if (watchingPositions.Count > 0)
            aimedWatchingPosition = watchingPositions[0];

        watchingPositions = new List<Transform>(watchingPositions);
    }

    public Vector3 GetNextPointPosition
    {
        get
        {
            if (watchingPositions.Count > 1)
                return watchingPositions[1].position;
            else
                return Vector3.zero;
        }
    }

    public void GoToNextPoint()
    {
        Transform ancientPosition = aimedWatchingPosition;
        watchingPositions.Remove(aimedWatchingPosition);

        if (watchingPositions.Count > 0)
            aimedWatchingPosition = watchingPositions[0];

        watchingPositions.Add(ancientPosition);
    }

    public Vector3 GetPreviousPointPosition
    {
        get
        {
            if (watchingPositions.Count > 0)
                return watchingPositions[watchingPositions.Count - 1].position;
            else
                return Vector3.zero;
        }
    }

    public void GoToPreviousPoint()
    {
        List<Transform> newWatchingPositions = new List<Transform>();

        if (watchingPositions.Count > 0)
            aimedWatchingPosition = watchingPositions[watchingPositions.Count - 1];

        newWatchingPositions.Add(aimedWatchingPosition);
        for (int i = 0; i < watchingPositions.Count - 1; i++)
        {
            newWatchingPositions.Add(watchingPositions[i]);
        }

        watchingPositions = newWatchingPositions;
    }

    public void GoToSpecificPoint(Transform specificPoint)
    {
        aimedWatchingPosition = specificPoint;

        int pointIndex = watchingPositions.IndexOf(specificPoint);
        List<Transform> newWatchingPositions = new List<Transform>();

        for(int i = 0; i < watchingPositions.Count; i++)
        {
            newWatchingPositions.Add(watchingPositions[pointIndex]);
            pointIndex++;

            if (pointIndex >= watchingPositions.Count)
                pointIndex = 0;
        }

        watchingPositions = newWatchingPositions;
    }
}
