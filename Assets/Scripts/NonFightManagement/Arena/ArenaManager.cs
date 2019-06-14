using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaManager : MonoBehaviour
{
    public static ArenaManager arenaManager;

    private void Awake()
    {
        arenaManager = this;
        goalParameters.SetUpGameGoal();

        if (IntersceneManager.intersceneManager != null)
            scoreManager.SetUp(IntersceneManager.intersceneManager.ArenaInterscInformations.GetLaunchedArenaParameters);
    }

    private void Start()
    {
        gameManager = GameManager.gameManager;
        gameManager.HidePlayerInterface();
        SetUpProjectilesPooling();

        if (IsTutorialArena)
            gameManager.MscManager.PlayTutorialMusic();
        else
            gameManager.MscManager.PlayArenaMusic();
    }

    private void Update()
    {
        if (introCinematicProgress == ProgressionState.NotStarted)
        {
            if (waitTimeBeforeIntro > 0)
                waitTimeBeforeIntro -= Time.deltaTime;
            else 
            {
                waitTimeBeforeIntro = 0;
                if (!IsTutorialArena)
                    StartIntro();
                else
                {
                    introCinematicProgress = ProgressionState.Ended;
                    StartGame();
                    GameManager.gameManager.ArenaInterfaceManager.SetUp(scoreManager);
                }
            }
        }
        else if(introCinematicProgress == ProgressionState.InProgress)
        {
            UpdateIntro();
        }
        else if(arenaProgress == ProgressionState.InProgress)
        {
            UpdateGame();
        }
        else if (outroCinematicProgress == ProgressionState.InProgress)
        {
            UpdateOutro();
        }
    }

    public bool StartedFight
    {
        get
        {
            return arenaProgress == ProgressionState.InProgress || arenaProgress == ProgressionState.Ended;
        }
    }

    public bool Won
    {
        get
        {
            return outroCinematicProgress == ProgressionState.InProgress || outroCinematicProgress == ProgressionState.Ended;
        }
    }

    #region Intro
    [Header("Intro")]
    [SerializeField] IntroControler introCinematic;
    [SerializeField] float waitTimeBeforeIntro = 2;
    ProgressionState introCinematicProgress;

    public void StartIntro()
    {
        introCinematicProgress = ProgressionState.InProgress;
        gameManager.Player.ShipMvt.StopShip();

        if (introCinematic != null)
        {
            if (!introCinematic.Ended)
                introCinematic.PlayCinematic();
            else
                EndIntro();
        }
        else
            EndIntro();
    }

    public void UpdateIntro()
    {
        if (introCinematic.Ended)
            EndIntro();
    }

    public void EndIntro()
    {
        introCinematicProgress = ProgressionState.Ended;
        gameManager.ArenaInterfaceManager.ShowBeginPanel(IsTutorialArena);

        gameManager.Player.ShipMvt.StopShip();
    }
    #endregion

    #region Game and Resolution
    GameManager gameManager;
    [Header("Game and Resolution")]
    [SerializeField] ArenaGoalParameters goalParameters;
    [SerializeField] float waitTimeBeforeOutro = 1;
    float remainingTimeBeforeOutro;
    ProgressionState arenaProgress;
    public ArenaGameMode GetArenaType { get { return goalParameters.GetArenaGameMode; } }

    public void StartGame()
    {
        arenaProgress = ProgressionState.InProgress;
        if (!IsTutorialArena)
        {
            gameManager.Player.ShipMvt.StartShip();
            gameManager.ShowPlayerInterface();
        }
    }

    public void UpdateGame()
    {
        if (gameManager.CheckIfPlayerHasControl && remainingTimeBeforeOutro == 0)
        {
            UpdateTimePassedSinceFightBeginning();
            scoreManager.IncreaseTimer();
        }

        if (!gameManager.Lost)
            if (goalParameters.CheckForGoalAchieved() && remainingTimeBeforeOutro == 0)
                StartEndGame();

        if (remainingTimeBeforeOutro > 0)
            remainingTimeBeforeOutro -= Time.deltaTime;
        else if (remainingTimeBeforeOutro < 0)
            EndGame();
    }

    public void AllWavesCleared()
    {
        if(goalParameters.GetArenaGameMode == ArenaGameMode.WavesClearing && remainingTimeBeforeOutro == 0)
            StartEndGame();
    }

    public void StartEndGame()
    {
        remainingTimeBeforeOutro = waitTimeBeforeOutro;
    }

    public void EndGame()
    {
        remainingTimeBeforeOutro = 0;

        arenaProgress = ProgressionState.Ended;

        int starNumber = scoreManager.CheckPlayerScore();
        
        if (IntersceneManager.intersceneManager != null)
            IntersceneManager.intersceneManager.ArenaInterscInformations.SetArenaPassed(true, starNumber);

        dropManager.LootAllRemainingCrates();

        gameManager.Player.PlayerLootManager.EarnLoot(dropManager, starNumber);

        if (IsTutorialArena)
        {
            PlayerProgressionDatas data = PlayerDataSaver.LoadProgressionDatas();
            data.SetPassedTutorial();
            PlayerDataSaver.SavePlayerProgressionDatas(data);
        }

        StartOutro();
    }
    #endregion

    #region Outro
    [Header("Outro")]
    [SerializeField] IntroControler outroCinematic;
    ProgressionState outroCinematicProgress;

    public void StartOutro()
    {
        outroCinematicProgress = ProgressionState.InProgress;

        if (goalParameters.GetArenaGameMode != ArenaGameMode.Escape)
            gameManager.Player.ShipMvt.StopShip();
        else
            gameManager.StopPlayerCamFollow();

        if (outroCinematic != null)
        {
            if (!outroCinematic.Ended)
                outroCinematic.PlayCinematic();
            else
                EndOutro();
        }
        else
            EndOutro();

        gameManager.HidePlayerInterface();
    }

    public void UpdateOutro()
    {
        if (outroCinematic.Ended)
            EndOutro();
    }

    public void EndOutro()
    {
        outroCinematicProgress = ProgressionState.Ended;

        //gameManager.ArenaInterfaceManager.OpenEndPanel(ShowEndPanelType.Win);
        gameManager.MscManager.PlayVictoryMusic();
        gameManager.ArenaInterfaceManager.OpenStarsAndGlobalLootPanel(IsTutorialArena);

        gameManager.HidePlayerInterface();
    }
    #endregion

    #region Drop Management
    [Header("Drop Management")]
    [SerializeField] ArenaDropManager dropManager;
    public ArenaDropManager DropManager { get { return dropManager; } }
    #endregion

    #region Scores 
    [Header("Scores")]
    [SerializeField] ScoreManager scoreManager;
    public ScoreManager ScoreMng { get { return scoreManager; } }
    #endregion

    #region Increasing Values
    #region Time
    float timePassedSinceFightBeginning;
    public void UpdateTimePassedSinceFightBeginning()
    {
        timePassedSinceFightBeginning += Time.deltaTime;

        if (nextTimeToCheck == null)
            return;

        if (timePassedSinceFightBeginning > nextTimeToCheck.GetNeededTime)
        {
            nextTimeToCheck.GetEnemyWave.GetSpawningConditions.ValidateTime();
            ChoseNextTimeToCheck();
        }
    }

    List<EnemyWaveWithNeededTime> timesToCheck;
    EnemyWaveWithNeededTime nextTimeToCheck;
    public void AddTimeToCheck(EnemyWaveWithNeededTime timeToCheck)
    {
        if (timesToCheck == null)
            timesToCheck = new List<EnemyWaveWithNeededTime>();

        timesToCheck.Add(timeToCheck);
    }

    public void ChoseNextTimeToCheck()
    {
        nextTimeToCheck = null;

        if (timesToCheck == null)
            return;

        if (timesToCheck.Count == 0)
            return;

        foreach(EnemyWaveWithNeededTime elementToCheck in timesToCheck)
        {
            if (nextTimeToCheck == null)
                nextTimeToCheck = elementToCheck;
            else
            {
                if (elementToCheck.GetNeededTime < nextTimeToCheck.GetNeededTime)
                    nextTimeToCheck = elementToCheck;
            }
        }

        if (nextTimeToCheck != null)
            timesToCheck.Remove(nextTimeToCheck);
    }
    #endregion

    #region Kills
    int numberOfKilledEnemies;
    public void IncreaseNumberOfKilledEnemies()
    {
        numberOfKilledEnemies++;

        if (nextKillsToCheck == null)
            return;

        if (numberOfKilledEnemies >= nextKillsToCheck.GetNeededKills)
        {
            nextKillsToCheck.GetEnemyWave.GetSpawningConditions.ValidateEnemyKilled();
            ChoseNextKillsToCheck();
        }
    }

    List<EnemyWaveWithNeededKills> killsToCheck;
    EnemyWaveWithNeededKills nextKillsToCheck;
    public void AddKillsToCheck(EnemyWaveWithNeededKills killsToChk)
    {
        if (killsToCheck == null)
            killsToCheck = new List<EnemyWaveWithNeededKills>();

        killsToCheck.Add(killsToChk);
    }

    public void ChoseNextKillsToCheck()
    {
        nextKillsToCheck = null;

        if (killsToCheck == null)
            return;

        if (killsToCheck.Count == 0)
            return;

        foreach (EnemyWaveWithNeededKills elementToCheck in killsToCheck)
        {
            if (nextKillsToCheck == null)
                nextKillsToCheck = elementToCheck;
            else
            {
                if (elementToCheck.GetNeededKills < nextKillsToCheck.GetNeededKills)
                    nextKillsToCheck = elementToCheck;
            }
        }

        killsToCheck.Remove(nextKillsToCheck);
    }
    #endregion
    #endregion

    #region
    [Header("Projectiles Pooling")]
    [SerializeField] EnemySpawningManager enemySpawningManager;
    [SerializeField] EnemyShip[] alreadyPlacedEnemies;

    public void SetUpProjectilesPooling()
    {
        if (enemySpawningManager == null)
        {
            gameManager.PoolManager.GenerateProjectilePoolDisctionary();
            return;
        }

        List<ProjectilePoolTag> allProjectilesUsed = new List<ProjectilePoolTag>();

        #region Spawning
        List<EnemyShipPoolTag> allUsedEnemies = enemySpawningManager.GetAllUsedEnemiesTags;
        foreach (EnemyShipPoolTag enemyTag in allUsedEnemies)
        {
            EnemyShip ship = gameManager.PoolManager.GetEnemyShip(enemyTag, PoolInteractionType.PeekPrefab);

            if (ship == null)
                break;

            ProjectilePoolTag usedProj = ship.GetUsedProjectileTag;
            if (!allProjectilesUsed.Contains(usedProj))
                allProjectilesUsed.Add(usedProj);
        }
        #endregion

        #region Already Placed
        if (alreadyPlacedEnemies != null)
        {
            foreach (EnemyShip ship in alreadyPlacedEnemies)
            {
                ProjectilePoolTag projTag = ship.GetUsedProjectileTag;
                if (!allProjectilesUsed.Contains(projTag))
                    allProjectilesUsed.Add(projTag);
            }
        }
        #endregion

        #region Player
        EquipmentsSet playerEquipments = gameManager.Player.EqpmntManager.EquipedEquipments;

        ShipEquipment canonEquipment = playerEquipments.GetMainWeaponEquipment;
        if (canonEquipment != null)
        {
            CompetenceShoot canonShoot = canonEquipment.GetPrimaryComp as CompetenceShoot;
            if (canonShoot != null)
            {
                if (!allProjectilesUsed.Contains(canonShoot.GetUsedProjectilePoolTag))
                    allProjectilesUsed.Add(canonShoot.GetUsedProjectilePoolTag);
            }
        }

        ShipEquipment catapultEquipment = playerEquipments.GetSecondaryWeaponEquipment;
        if (catapultEquipment != null)
        {
            CompetenceShoot catapultShoot = catapultEquipment.GetPrimaryComp as CompetenceShoot;
            if (catapultShoot != null)
            {
                if (!allProjectilesUsed.Contains(catapultShoot.GetUsedProjectilePoolTag))
                    allProjectilesUsed.Add(catapultShoot.GetUsedProjectilePoolTag);
            }
        }
        #endregion

        gameManager.PoolManager.GenerateProjectilePoolDisctionary(allProjectilesUsed);
    }
    #endregion

    #region Tutorial
    [Header("Tutorial")]
    [SerializeField] TutorialManager tutorialManager;
    public bool IsTutorialArena { get { return tutorialManager != null; } }
    #endregion
}

[System.Serializable]
public struct ArenaGoalParameters
{
    [SerializeField] ArenaGameMode arenaGameMode;
    public ArenaGameMode GetArenaGameMode { get { return arenaGameMode; } }

    #region Targets to Destroy
    [Header("Targets to Destroy")]
    [SerializeField] List<GameObject> targetsToDestroy;
    List<IDamageReceiver> targetsToDestroyReceivers;
    #endregion

    #region Zones to Go To
    [SerializeField] List<TargetZone> zonesToGoTo;
    #endregion

    #region Ships To Catch
    [SerializeField] List<FleeingEnemyShip> shipsToCatch;
    #endregion

    #region SetUp
    public void SetUpGameGoal()
    {
        targetsToDestroyReceivers = new List<IDamageReceiver>();

        foreach (GameObject targetObject in targetsToDestroy)
        {
            if (targetObject != null)
            {
                IDamageReceiver damageReceiver = targetObject.GetComponent<IDamageReceiver>();
                if (damageReceiver != null)
                {
                    targetsToDestroyReceivers.Add(damageReceiver);
                    damageReceiver.OnDeath += new OnLifeEvent(RemoveTargetToDestroy);
                }
            }
        }

        foreach(TargetZone targetZone in zonesToGoTo)
        {
            targetZone.OnPlayerEnter += RemoveZoneToGoTo;
            targetZone.StartZone();
        }

        foreach(FleeingEnemyShip ship in shipsToCatch)
        {
            ship.OnFleeingEnemyCatched = RemoveCatchedShip;
        }
    }
    #endregion

    #region
    public bool CheckForGoalAchieved()
    {
        switch (arenaGameMode)
        {
            case (ArenaGameMode.Anihilation):
                return targetsToDestroyReceivers.Count == 0;

            case (ArenaGameMode.Assassination):
                return targetsToDestroyReceivers.Count == 0;

            case (ArenaGameMode.WavesClearing):
                return false;

            case (ArenaGameMode.Escape):
                return zonesToGoTo.Count == 0;

            case (ArenaGameMode.EnemyShipPursuit):
                return shipsToCatch.Count == 0;
        }

        return true;
    }

    public void RemoveTargetToDestroy(IDamageReceiver deadTarget)
    {
        targetsToDestroyReceivers.Remove(deadTarget);
    }

    public void RemoveZoneToGoTo(TargetZone zone)
    {
        if (zonesToGoTo.Contains(zone))
            zonesToGoTo.Remove(zone);
    }

    public void RemoveCatchedShip(FleeingEnemyShip enemyShip)
    {
        if (shipsToCatch.Contains(enemyShip))
            shipsToCatch.Remove(enemyShip);
    }
    #endregion
}

public enum ArenaGameMode
{
    Anihilation, Assassination, WavesClearing, Escape, EnemyShipPursuit
}

public enum ProgressionState
{
    NotStarted, InProgress, Ended
}
