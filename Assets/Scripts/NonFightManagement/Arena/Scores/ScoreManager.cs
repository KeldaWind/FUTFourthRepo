using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ScoreManager
{
    [SerializeField] ArenaParameters currentArenaParameters;
    public ArenaParameters GetCurrentArenaParameters { get { return currentArenaParameters; } }

    public void SetUp(ArenaParameters parameters)
    {
        if(parameters != null)
            currentArenaParameters = parameters;
    }

    #region Time
    float timeSinceArenaBeginning;
    public float GetTimeSinceArenaBeginning { get { return timeSinceArenaBeginning; } }
    public void IncreaseTimer()
    {
        timeSinceArenaBeginning += Time.deltaTime;
    }

    public bool HasTimeStar
    {
        get
        {
            if (currentArenaParameters != null)
                return timeSinceArenaBeginning < currentArenaParameters.GetMaximumArenaTimeToHaveStar;
            else
                return false;
        }
    }
    #endregion

    #region Damages
    int takenDamagesSinceArenaBeginning;
    public int GetTakenDamagesSinceArenaBeginning { get { return takenDamagesSinceArenaBeginning; } }
    public void IncreaseTakenDamages(int damages)
    {
        takenDamagesSinceArenaBeginning += damages;
    }

    public bool HasDamagesStar
    {
        get
        {
            if (currentArenaParameters != null)
                return takenDamagesSinceArenaBeginning < currentArenaParameters.GetMaximumNumberOfDamagesToHaveStar;
            else
                return false;
        }
    }
    #endregion

    #region Don't Hit Obstacles
    bool playerHitObstacle;
    public void SetPlayerHasHitObstacle()
    {
        playerHitObstacle = true;
    }

    public bool HasNoObstacleHitStar
    {
        get
        {
            return !playerHitObstacle;
        }
    }
    #endregion

    public int CheckPlayerScore()
    {
        int starNumber = 1;

        if (currentArenaParameters == null)
            return starNumber;

        /*if(timeSinceArenaBeginning < currentArenaParameters.GetMaximumArenaTimeToHaveStar)
        {
            starNumber++;
        }
        */
        if (!playerHitObstacle)
            starNumber++;

        if (takenDamagesSinceArenaBeginning < currentArenaParameters.GetMaximumNumberOfDamagesToHaveStar)
        {
            starNumber++;
        }

        return starNumber;
    }
}
