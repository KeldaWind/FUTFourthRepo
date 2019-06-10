using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawningManager : MonoBehaviour
{
    public void SetUp()
    {
        GameManager.gameManager.PoolManager.GenerateEnemyShipsPoolDisctionary(GetAllUsedEnemiesTags);

        wavesWaitingToBeLaunched = new List<EnemyWave>();
        wavesToRemoveFromWait = new List<EnemyWave>();

        enemySpawingDropParameters.SetUp();

        foreach (EnemyWave wave in allWaves)
            wave.SetUp(this);

        ArenaManager arenaManager = ArenaManager.arenaManager;

        if (arenaManager != null)
        {
            arenaManager.ChoseNextTimeToCheck();
            arenaManager.ChoseNextKillsToCheck();
        }
    }

    List<EnemyShipPoolTag> allUsedEnemies;
    public List<EnemyShipPoolTag> GetAllUsedEnemiesTags
    {
        get
        {
            if (allUsedEnemies != null)
                return allUsedEnemies;
            else
            {
                List<EnemyShipPoolTag> allEnemies = new List<EnemyShipPoolTag>();

                foreach (EnemyWave wave in allWaves)
                {
                    List<EnemyShipPoolTag> tags = wave.GetAllUsedEnemyTags;
                    foreach (EnemyShipPoolTag tag in tags)
                    {
                        if (!allEnemies.Contains(tag))
                            allEnemies.Add(tag);
                    }
                }
                allUsedEnemies = allEnemies;

                return allEnemies;
            }
        }
    }

    [Header("Waves")]
    [SerializeField] EnemyWave[] allWaves;

    public void CheckIfAllWavesCleared()
    {
        bool allCleared = true;

        foreach(EnemyWave wave in allWaves)
        {
            if (!wave.Cleared)
            {
                allCleared = false;
                break;
            }
        }

        if (allCleared)
        {
            if (ArenaManager.arenaManager != null)
                ArenaManager.arenaManager.AllWavesCleared();
        }
    }

    public void Start()
    {
        SetUp();
    }

    [Header("Drop Management")]
    [SerializeField] EnemySpawingDropParameters enemySpawingDropParameters;
    public EnemySpawingDropParameters GetEnemySpawingDropParameters { get { return enemySpawingDropParameters; } }


    List<EnemyWave> wavesWaitingToBeLaunched;
    List<EnemyWave> wavesToRemoveFromWait;
    public void Update()
    {
        if (wavesWaitingToBeLaunched == null)
            return;

        if (wavesWaitingToBeLaunched.Count == 0)
            return;

        foreach (EnemyWave wave in wavesWaitingToBeLaunched)
            wave.UpdateWaintingTimeBeforeStarWave();

        List<EnemyWave> removedWaves = new List<EnemyWave>();
        foreach (EnemyWave wave in wavesToRemoveFromWait)
        {
            wavesWaitingToBeLaunched.Remove(wave);
            removedWaves.Add(wave);
        }

        foreach (EnemyWave wave in removedWaves)
            wavesWaitingToBeLaunched.Remove(wave);
    }

    public void AddWaveToWaitTime(EnemyWave wave)
    {
        wavesWaitingToBeLaunched.Add(wave);
    }

    public void RemoveWaveFromWaitTime(EnemyWave wave)
    {
        wavesToRemoveFromWait.Add(wave);
    }

    public List<IDamageReceiver> SpawnEnemiesExternaly(int waveIndex)
    {
        return allWaves[waveIndex].SpawnEnemies();
    }
}

public delegate void OnWaveCleared();

[System.Serializable]
public class EnemyWave
{
    [SerializeField] string waveName;
    public string GetWaveName { get { return waveName; } }
    [SerializeField] IntroControler cinematicToPlayOnWaveStart;

    bool spawned;
    bool cleared;
    public bool Cleared { get { return cleared; } }

    EnemySpawingDropParameters dropParameters;

    public void SetUp(EnemySpawningManager spawningManager)
    {
        GenerateSpawningPointsLists();
        spawningConditions.SetUpConditions(this);

        dropParameters = spawningManager.GetEnemySpawingDropParameters;

        StartWaveWaitingTime = spawningManager.AddWaveToWaitTime;
        EndWaveWaitingTime = spawningManager.RemoveWaveFromWaitTime;

        CheckIfReadyToSpawnWave();

        OnWaveCleared = spawningManager.CheckIfAllWavesCleared;
    }

    #region Enemies to Spawn
    [Header("Enemies to Spawn")]
    [SerializeField] bool spawnedEnemiesAreStopped;
    [SerializeField] bool spawnedEnemiesAutoDetectPlayer;
    [SerializeField] int numberOfCommonEnemies;
    [SerializeField] int numberOfRareEnemies;

    [SerializeField] EnemiesProbabilitySystem commonEnemies;
    [SerializeField] HighlightingType commonEnemiesHighlightingType;
    [SerializeField] EnemiesProbabilitySystem rareEnemies;
    [SerializeField] HighlightingType rareEnemiesHighlightingType;

    public List<EnemyShipPoolTag> GetAllUsedEnemyTags
    {
        get
        {
            List<EnemyShipPoolTag> allUsedEnemies = new List<EnemyShipPoolTag>();

            foreach (EnemyShipWithProbabilityWeight item in commonEnemies.GetAllItems)
            {
                if(!allUsedEnemies.Contains(item.GetItem))
                    allUsedEnemies.Add(item.GetItem);
            }

            foreach (EnemyShipWithProbabilityWeight item in rareEnemies.GetAllItems)
            {
                if (!allUsedEnemies.Contains(item.GetItem))
                    allUsedEnemies.Add(item.GetItem);
            }

            return allUsedEnemies;
        }
    }
    #endregion

    #region Spawners
    [Header("Spawners")]
    [SerializeField] Transform spawnersParent;
    List<EnemySpawnPoint> commonEnemiesSpawnPoints;
    List<EnemySpawnPoint> rareEnemiesSpawnPoints;

    public void GenerateSpawningPointsLists()
    {
        commonEnemiesSpawnPoints = new List<EnemySpawnPoint>();
        rareEnemiesSpawnPoints = new List<EnemySpawnPoint>();

        EnemySpawnPoint[] allSpawners = spawnersParent.GetComponentsInChildren<EnemySpawnPoint>();

        foreach (EnemySpawnPoint spawnPoint in allSpawners)
        {
            if (spawnPoint.GetRarityOfSpawnedEnemy == EnemySpawningRarity.Common)
                commonEnemiesSpawnPoints.Add(spawnPoint);
            else if (spawnPoint.GetRarityOfSpawnedEnemy == EnemySpawningRarity.Rare)
                rareEnemiesSpawnPoints.Add(spawnPoint);
        }
    }
    #endregion

    public List<IDamageReceiver> SpawnEnemies()
    {
        spawned = true;

        List<EnemySpawnPoint> availableCommonSpawnPoints = new List<EnemySpawnPoint>(commonEnemiesSpawnPoints);
        List<EnemySpawnPoint> availableRareSpawnPoints = new List<EnemySpawnPoint>(rareEnemiesSpawnPoints);

        spawnedEnemies = new List<IDamageReceiver>();

        bool stopSpawnedEnemies = false;
        if (cinematicToPlayOnWaveStart != null)
            stopSpawnedEnemies = cinematicToPlayOnWaveStart.StopSpawnedEnemies || spawnedEnemiesAreStopped;
        else
            stopSpawnedEnemies = spawnedEnemiesAreStopped;

        List<EnemyShip> allSpawnedEnemies = new List<EnemyShip>();

        PlayerShip playerShip = GameManager.gameManager.Player;

        #region Common
        for (int i = 0; i < numberOfCommonEnemies; i++)
        {
            if (availableCommonSpawnPoints.Count == 0)
            {
                Debug.Log("pas de spawn points disponibles : impossible de spawner l'ennemi commun");
                break;
            }

            EnemySpawnPoint chosenSpawnPoint = availableCommonSpawnPoints.GetRandomMemberOfTheList();

            EnemyShipPoolTag enemyPoolTag = commonEnemies.GetRandomElementFromSystem();

            EnemyShip spawnedEnemy = GameManager.gameManager.PoolManager.GetEnemyShip(enemyPoolTag, PoolInteractionType.GetFromPool);
            spawnedEnemy.transform.position = chosenSpawnPoint.transform.position;
            spawnedEnemy.transform.rotation = chosenSpawnPoint.transform.rotation;

            spawnedEnemy.ExternalSetUp(chosenSpawnPoint.GetLinkedWatchingRoundParameters);

            availableCommonSpawnPoints.Remove(chosenSpawnPoint);

            spawnedEnemies.Add(spawnedEnemy.GetShipDamageReceiver);
            spawnedEnemy.GetShipDamageReceiver.OnDeath += new OnLifeEvent(RemoveSpawnedEnemy);

            spawnedEnemy.SetDropParameters(dropParameters.GetEnemyDropParameters(enemyPoolTag));

            allSpawnedEnemies.Add(spawnedEnemy);

            if (commonEnemiesHighlightingType != HighlightingType.None)
            {
                spawnedEnemy.StartEnemyHighlighting(playerShip, commonEnemiesHighlightingType == HighlightingType.Important);
                //if (!spawnedEnemiesArentHighlighted)
                //playerShip.StartEnemyHighlighting(spawnedEnemy);
            }

            if (stopSpawnedEnemies)
                spawnedEnemy.ShipMvt.InstantStopShip();

            if (spawnedEnemiesAutoDetectPlayer)
                spawnedEnemy.AutoDetectPlayer();
        }
        #endregion

        #region Rare
        for (int i = 0; i < numberOfRareEnemies; i++)
        {
            if (availableRareSpawnPoints.Count == 0)
            {
                Debug.Log("pas de spawn points disponibles : impossible de spawner l'ennemi rare");
                break;
            }

            EnemySpawnPoint chosenSpawnPoint = availableRareSpawnPoints.GetRandomMemberOfTheList();

            EnemyShipPoolTag enemyPoolTag = rareEnemies.GetRandomElementFromSystem();

            EnemyShip spawnedEnemy = GameManager.gameManager.PoolManager.GetEnemyShip(enemyPoolTag, PoolInteractionType.GetFromPool);
            spawnedEnemy.transform.position = chosenSpawnPoint.transform.position;
            spawnedEnemy.transform.rotation = chosenSpawnPoint.transform.rotation;

            spawnedEnemy.ExternalSetUp(chosenSpawnPoint.GetLinkedWatchingRoundParameters);

            availableCommonSpawnPoints.Remove(chosenSpawnPoint);

            spawnedEnemies.Add(spawnedEnemy.GetShipDamageReceiver);
            spawnedEnemy.GetShipDamageReceiver.OnDeath += new OnLifeEvent(RemoveSpawnedEnemy);

            spawnedEnemy.SetDropParameters(dropParameters.GetEnemyDropParameters(enemyPoolTag));

            allSpawnedEnemies.Add(spawnedEnemy);

            if (rareEnemiesHighlightingType != HighlightingType.None)
            {
                spawnedEnemy.StartEnemyHighlighting(playerShip, rareEnemiesHighlightingType == HighlightingType.Important);
            }
            //if (!spawnedEnemiesArentHighlighted)
                //playerShip.StartEnemyHighlighting(spawnedEnemy);

            if (stopSpawnedEnemies)
                spawnedEnemy.ShipMvt.InstantStopShip();

            if (spawnedEnemiesAutoDetectPlayer)
                spawnedEnemy.AutoDetectPlayer();
        }
        #endregion

        if (cinematicToPlayOnWaveStart != null)
        {
            if(stopSpawnedEnemies)
                cinematicToPlayOnWaveStart.PlayCinematic(allSpawnedEnemies);
            else
                cinematicToPlayOnWaveStart.PlayCinematic();
        }
        else
        {
            if (stopSpawnedEnemies)
                GameManager.gameManager.CinematicMng.SetShipsToStart(allSpawnedEnemies);
        }

        return spawnedEnemies;
    }

    #region Spawn Enemies and Clearing
    List<IDamageReceiver> spawnedEnemies;
    public OnWaveCleared OnWaveCleared;

    public void RemoveSpawnedEnemy(IDamageReceiver deadTarget)
    {
        spawnedEnemies.Remove(deadTarget);

        CheckForWaveCleared();
    }

    public void CheckForWaveCleared()
    {
        if(spawnedEnemies.Count == 0)
        {
            cleared = true;
            OnWaveCleared();
        }
    }
    #endregion

    #region Checking
    [Header("Spawning Conditions")]
    [SerializeField] EnemyWaveSpawningConditions spawningConditions;
    public EnemyWaveSpawningConditions GetSpawningConditions { get { return spawningConditions; } }
    [SerializeField] float waitTimeBeforeStartWaveAfterConditionsVerified;

    public OnWaveChange StartWaveWaitingTime; 
    public OnWaveChange EndWaveWaitingTime; 

    public void CheckIfReadyToSpawnWave()
    {
        if (spawned)
            return;

        if (spawningConditions.ReadyToSpawn())
        {
            if (waitTimeBeforeStartWaveAfterConditionsVerified > 0)
                StartWaveWaitingTime(this);
            else
                SpawnEnemies();
        }
    }

    public void UpdateWaintingTimeBeforeStarWave()
    {
        if (waitTimeBeforeStartWaveAfterConditionsVerified > 0)
            waitTimeBeforeStartWaveAfterConditionsVerified -= Time.deltaTime;
        else if (waitTimeBeforeStartWaveAfterConditionsVerified < 0)
        {
            waitTimeBeforeStartWaveAfterConditionsVerified = 0;
            SpawnEnemies();
        }
    }
    #endregion
}

public delegate void OnWaveConditionsChanged();
public delegate void OnWaveChange(EnemyWave wave);

public class EnemyWaveWithNeededTime
{
    public EnemyWaveWithNeededTime(EnemyWave wave, float time)
    {
        enemyWave = wave;
        neededTime = time;
    }

    EnemyWave enemyWave;
    public EnemyWave GetEnemyWave { get { return enemyWave; } }

    float neededTime;
    public float GetNeededTime { get { return neededTime; } }
}

public class EnemyWaveWithNeededKills
{
    public EnemyWaveWithNeededKills(EnemyWave wave, int kills)
    {
        enemyWave = wave;
        neededKills = kills;
    }

    EnemyWave enemyWave;
    public EnemyWave GetEnemyWave { get { return enemyWave; } }

    int neededKills;
    public int GetNeededKills { get { return neededKills; } }
}

[System.Serializable]
public class EnemyWaveSpawningConditions
{
    public event OnWaveConditionsChanged WaveConditionsChanged;

    public void SetUpConditions(EnemyWave wave)
    {
        ArenaManager arenaManager = ArenaManager.arenaManager;

        if (arenaManager == null)
            return;

        WaveConditionsChanged = wave.CheckIfReadyToSpawnWave;

        #region Time
        if (minTimeSinceFightBeginning == 0)
            validatedTime = true;
        else
            arenaManager.AddTimeToCheck(new EnemyWaveWithNeededTime(wave, minTimeSinceFightBeginning));
        #endregion

        #region Kills
        if (minKillsSinceFightBeginning == 0)
            validatedEnemyKilled = true;
        else
            arenaManager.AddKillsToCheck(new EnemyWaveWithNeededKills(wave, minKillsSinceFightBeginning));
        #endregion

        #region Objects to Destroy
        damageListenerToKill = new List<IDamageReceiver>();

        foreach (GameObject targetObject in objectsToDestroy)
        {
            if (targetObject != null)
            {
                IDamageReceiver damageReceiver = targetObject.GetComponent<IDamageReceiver>();
                if (damageReceiver != null)
                {
                    damageListenerToKill.Add(damageReceiver);
                    damageReceiver.OnDeath += new OnLifeEvent(RemoveTargetToDestroy);
                }
            }
        }
        #endregion

        #region Triggers
        remainingZones = new List<TargetZone>();

        foreach (TargetZone zoneToGoTo in zonesToGoTo)
        {
            remainingZones.Add(zoneToGoTo);
            zoneToGoTo.OnPlayerEnter += RemoveZoneToGoTo;
            zoneToGoTo.StartZone();
        }
        #endregion
    }

    [SerializeField] bool spawnOnStart;
    [SerializeField] bool spawnExternaly;

    #region Time
    [SerializeField] float minTimeSinceFightBeginning;
    bool validatedTime;
    public void ValidateTime()
    {
        validatedTime = true;

        WaveConditionsChanged();
    }
    #endregion

    #region Kills
    [SerializeField] int minKillsSinceFightBeginning;
    bool validatedEnemyKilled;
    public void ValidateEnemyKilled()
    {
        validatedEnemyKilled = true;

        WaveConditionsChanged();
    }
    #endregion

    #region Objects To Destroy
    [SerializeField] GameObject[] objectsToDestroy;
    List<IDamageReceiver> damageListenerToKill;

    public void RemoveTargetToDestroy(IDamageReceiver deadTarget)
    {
        damageListenerToKill.Remove(deadTarget);

        WaveConditionsChanged();
    }
    #endregion

    #region Triggers
    [SerializeField] TargetZone[] zonesToGoTo;
    List<TargetZone> remainingZones;

    public void RemoveZoneToGoTo(TargetZone removedZone)
    {
        remainingZones.Remove(removedZone);

        WaveConditionsChanged();
    }
    #endregion

    public void DebugReadyProgression()
    {
        //Debug.Log("validatedTime :");
    }

    public bool ReadyToSpawn()
    {
        Debug.Log("check has been called");
        if (spawnExternaly)
            return false;

        return (validatedTime && validatedEnemyKilled && damageListenerToKill.Count == 0 && remainingZones.Count == 0) || spawnOnStart;
    }
}

[System.Serializable]
public /*struct*/class EnemySpawingDropParameters
{
    public void SetUp()
    {
        foreach (EnemyTagWithDropParameters parameter in allEnemiesDropParameters)
            parameter.SetUp();
    }

    [SerializeField] EnemyTagWithDropParameters[] allEnemiesDropParameters;
    public EnemyDropParameters GetEnemyDropParameters(EnemyShipPoolTag enemyTag)
    {
        EnemyDropParameters parameters = null;

        foreach(EnemyTagWithDropParameters enemyWithDropParams in allEnemiesDropParameters)
        {
            if (enemyTag == enemyWithDropParams.GetEnemyTag)
            {
                parameters = enemyWithDropParams.GetDropParameters;
                break;
            }
        }

        if (parameters == null)
            Debug.LogWarning("Impossible de trouver des paramètres de drop pour l'ennemi spawné : aucun drop sur l'ennemi");

        return parameters;
    }
}

[System.Serializable] 
public /*struct*/class EnemyTagWithDropParameters
{
    public void SetUp()
    {
        if (dropParameters != null)
        {
            dropParametersCopy = Object.Instantiate(dropParameters);
            dropParametersCopy.name = enemyTag + "DropParameters";
        }
        else
            Debug.LogWarning("Attention : aucun paramètre de drop assigné pour ce type d'ennemi");
    }

    [SerializeField] EnemyShipPoolTag enemyTag;
    public EnemyShipPoolTag GetEnemyTag { get { return enemyTag; } }
    [SerializeField] EnemyDropParameters dropParameters;
    [SerializeField] EnemyDropParameters dropParametersCopy;
    public EnemyDropParameters GetDropParameters { get { return dropParametersCopy; } }
}

public enum EnemySpawningRarity
{
    Common, Rare
}
