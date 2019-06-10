using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void OnFleeingEnemyCatched(FleeingEnemyShip fleeingEnemy);
public class FleeingEnemyShip : EnemyShip
{
    public OnFleeingEnemyCatched OnFleeingEnemyCatched;

    PlayerShip playerShip;

    public override void SetUp(bool stopped)
    {
        base.SetUp(stopped);

        playerShip = GameManager.gameManager.Player;

        SetCurrentProtectionParameters(ProtectionParameters.UnlimitedLifeProtection);

        relatedShipHitbox.OnOtherShipCollision = CheckIfHitShipIsPlayer;
        //relatedShipHitbox.on

        StartEnemyHighlighting(playerShip, true);
    }

    public override void UpdateEnemyBehavior()
    {
        if (!watchingRoundParameters.IsNull)
            UpdateWatchingRoundBehaviour();
        /*if (targetShip == null)
        {
            if (!watchingRoundParameters.IsNull)
                UpdateWatchingRoundBehaviour();

            targetShip = relatedDetectionZone.GetTargetShip;

            if (targetShip == null)
                return;
            else
            {
                
                //shipMovements.StartShip();
                //UpdateEnemyAcquiredDatas();
            }
        }*/

        if (remainingTimeBeforeNextHit > 0)
            remainingTimeBeforeNextHit -= Time.deltaTime;
        else if (remainingTimeBeforeNextHit < 0)
            remainingTimeBeforeNextHit = 0;
    }

    int reachedWatchingPointCounter;
    bool nearWatchingPoint;
    public override void UpdateWatchingRoundBehaviour()
    {
        Vector3 shipPosition = transform.position;
        shipPosition.y = 0;
        Vector3 pointPosition = watchingRoundParameters.GetAimedWatchingPosition;
        pointPosition.y = 0;

        if (currentReactionTime > 0)
            currentReactionTime -= Time.deltaTime;
        else if (currentReactionTime < 0)
        {
            lastPredictedPosition = watchingRoundParameters.GetAimedWatchingPosition;
            CheckForObstacleOnWatchRoundWay();
        }

        float distance = Vector3.Distance(pointPosition, shipPosition);
        if (distance < watchingRoundParameters.GetMinDistanceFromPointToPass)
        {
            if (!nearWatchingPoint)
            {
                nearWatchingPoint = true;
                reachedWatchingPointCounter++;
                if (reachedWatchingPointCounter >= 2)
                {
                    ChooseBestNextWatchingPoint();
                    reachedWatchingPointCounter = 0;
                }
            }
        }
        else
            nearWatchingPoint = false;
    }

    #region Fleeing Management
    /// <summary>
    /// Dès que le bateau a fini de faire son tour du watching point, il choisit le point suivant en prenant en compre la position du joueur 
    /// </summary>
    public void ChooseBestNextWatchingPoint()
    {
        float nextPointDistanceFromPlayer = Vector3.Distance(playerShip.transform.position, watchingRoundParameters.GetNextPointPosition);
        float previousPointDistanceFromPlayer = Vector3.Distance(playerShip.transform.position, watchingRoundParameters.GetPreviousPointPosition);

        if (nextPointDistanceFromPlayer > previousPointDistanceFromPlayer)
            watchingRoundParameters.GoToNextPoint();
        else
            watchingRoundParameters.GoToPreviousPoint();

        lastPredictedPosition = watchingRoundParameters.GetAimedWatchingPosition;
        CheckForObstacleOnWatchRoundWay();
    }

    public void Accelerate()
    {
        shipMovements.SetGlobalAcceleration(movementsModifierCoeffWhenHit, movementsModifierCoeffWhenHit, movementsModifierTimeWhenHit);

        float maxDistance = 0;
        Transform pickedTransform = null;
        foreach(Transform point in watchingRoundParameters.GetAllWatchingPositions)
        {
            float distance = Vector3.Distance(playerShip.transform.position, point.position);
            if(distance > maxDistance)
            {
                maxDistance = distance;
                pickedTransform = point;
            }
        }

        if (pickedTransform != null)
            lastPredictedPosition = pickedTransform.position;

        watchingRoundParameters.GoToSpecificPoint(pickedTransform);

        CheckForObstacleOnWatchRoundWay();
        reachedWatchingPointCounter = 0;
    }

    [Header("Fleeing Parameters")]
    [SerializeField] float movementsModifierCoeffWhenHit = 3;
    [SerializeField] float movementsModifierTimeWhenHit = 3;

    [Header("Chasing Management")]
    [SerializeField] int numberOfTimeToTouchEnemyToWin;
    int touchedCount;
    [SerializeField] float minimumTimeBetweenTwoHit = 5;
    float remainingTimeBeforeNextHit;

    public void CheckIfHitShipIsPlayer(Ship hitShip)
    {
        if (remainingTimeBeforeNextHit != 0)
            return;

        if (hitShip as PlayerShip != null)
            IncreaseNumberOfHit();
    }

    public void IncreaseNumberOfHit()
    {
        touchedCount++;
        remainingTimeBeforeNextHit = minimumTimeBetweenTwoHit;

        foreach (CinematicControlerWithIndex cinematicWithIndex in cinematicsToPlay)
        {
            if (touchedCount == cinematicWithIndex.index)
            {
                if (cinematicWithIndex.cinematicToPlay != null)
                    cinematicWithIndex.cinematicToPlay.PlayCinematic();
                break;
            }
        }

        if (touchedCount >= numberOfTimeToTouchEnemyToWin)
        {
            if (OnFleeingEnemyCatched != null)
                OnFleeingEnemyCatched(this);

            lastPredictedPosition = transform.position + (playerShip.transform.position - transform.position).normalized * 10000;
            shipMovements.SetGlobalAcceleration(movementsModifierCoeffWhenHit, movementsModifierCoeffWhenHit, movementsModifierTimeWhenHit * 3);
        }
        else
            Accelerate();
    }
    #endregion

    #region Feedbacks
    [Header("Feedbacks")]
    [SerializeField] CinematicControlerWithIndex[] cinematicsToPlay;
    #endregion
}

[System.Serializable]
public struct CinematicControlerWithIndex
{
    public int index;
    public IntroControler cinematicToPlay;
}
