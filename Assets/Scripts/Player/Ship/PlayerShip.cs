using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Bateau du joueur, qu'il peut contrôler pendant les niveaux
/// </summary>
public class PlayerShip : Ship
{
    bool setUp;
    bool lost;
    public bool Lost
    {
        get
        {
            return lost;
        }
    }

    /// <summary>
    /// Rigidbody du bateau
    /// </summary>
    [Header("Player Control References")]
    /// <summary>
    /// La barre du bateau, permettant de contrôler sa direction
    /// </summary>
    [SerializeField] ShipsWheel relatedWheel;    
    
    [Header("Player's Modules")]
    /// <summary>
    /// Module de gestion des équipements du joueur
    /// </summary>
    [SerializeField] EquipmentsManager equipmentsManager;
    /// <summary>
    /// Module de gestion des équipements du joueur
    /// </summary>
    public EquipmentsManager EqpmntManager
    {
        get
        {
            return equipmentsManager;
        }
    }

    [SerializeField] PlayerInterface playerInterface;
    public PlayerInterface PlrInterface
    {
        get
        {
            return playerInterface;
        }
    }
   
    public void ExtrenalSetUp()
    {
        SetUp(!startMoving);
    }

    /// <summary>
    /// Initialisation du bateau du joueur
    /// </summary>
    public override void SetUp(bool stopped)
    {
        if (setUp)
            return;

        setUp = true;

        base.SetUp(stopped);

        relatedWheel.SetUp(shipMovements);

        equipmentsManager.SetUpSystem(this);

        if(IntersceneManager.intersceneManager != null)
        {
            equipmentsManager.SetUpEquipments(IntersceneManager.intersceneManager.GetPlayerDatas.GetPlayerEquipedEquipments);

            /*if(MapManager.mapManager != null)
            {
                transform.position = IntersceneManager.intersceneManager.MapInterscInformations.GetPlayerPositionOnMap;
            }*/
        }

        playerInterface.SetUp(this, equipmentsManager.EquipedEquipments);

        lifeManager.OnLifeChange += PlayDamageVibrationFeedback;
    }

    public override void Die()
    {
        if (!alreadyDead)
        {
            //base.Die();
            alreadyDead = true;
            shipMovements.StopShip();

            GameManager.gameManager.Lose();

            lost = true;

            #region Resets
            remainingStunDuration = 0;
            skeweringProjectile = null;

            shipMovements.Reset();
            knockbackManager.Reset();
            #endregion
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void Update()
    {
        base.Update();


        if (shipMovements.Stopped)
            shipMovements.UpdateMovementValues(0);

        if (enemiesToHighlight != null || importantEnemiesToHighlight != null)
        {
            //if (enemiesToHighlight.Count > 0)
                UpdateEnemyHighlighting();
        }
    }

    #region LootManager
    [Header("Loot")]
    [SerializeField] LootManager lootManager;
    public LootManager PlayerLootManager { get { return lootManager; } }
    #endregion

    #region Blinding
    public override void Blind()
    {
        base.Blind();

        playerInterface.Blind();
    }

    public override void Blind(float duration)
    {
        base.Blind(duration);

        playerInterface.Blind(duration);
    }

    public override void Unblind()
    {
        base.Unblind();

        playerInterface.Unblind();
    }
    #endregion

    #region ScoreManagement
    //public void Che
    #endregion

    [Header("Enemy Highlighting")]
    [SerializeField] float minXDistanceToHighlight;
    [SerializeField] float minYUpDistanceToHighlight;
    [SerializeField] float minYDownDistanceToHighlight;
    List<Transform> enemiesToHighlight;
    List<Transform> importantEnemiesToHighlight;

    public void StartEnemyHighlighting(EnemyShip enemyShip, bool important)
    {
        if (important)
        {
            if (importantEnemiesToHighlight == null)
                importantEnemiesToHighlight = new List<Transform>();

            importantEnemiesToHighlight.Add(enemyShip.transform);
        }
        else
        {
            if (enemiesToHighlight == null)
                enemiesToHighlight = new List<Transform>();

            enemiesToHighlight.Add(enemyShip.transform);
        }

    }

    public void UpdateEnemyHighlighting()
    {
        #region Normal
        List<Vector3> highlightedDirections = new List<Vector3>();

        if (enemiesToHighlight != null)
        {
            foreach (Transform enemyToHighlight in enemiesToHighlight)
            {
                bool showEnemy = false;
                Vector3 vectorToEnemy = enemyToHighlight.position - transform.position;
                if (Mathf.Abs(vectorToEnemy.x) > minXDistanceToHighlight)
                    showEnemy = true;
                else if (vectorToEnemy.z > minYUpDistanceToHighlight)
                    showEnemy = true;
                else if (vectorToEnemy.z < minYDownDistanceToHighlight)
                    showEnemy = true;

                if (showEnemy)
                {
                    vectorToEnemy.y = 0;
                    vectorToEnemy.Normalize();
                    highlightedDirections.Add(vectorToEnemy);
                }
            }
        }
        #endregion

        #region Important
        List<Vector3> importantHighlightedDirections = new List<Vector3>();

        if (importantEnemiesToHighlight != null)
        {
            foreach (Transform importantEnemyToHighlight in importantEnemiesToHighlight)
            {
                bool showEnemy = false;
                Vector3 vectorToEnemy = importantEnemyToHighlight.position - transform.position;
                if (Mathf.Abs(vectorToEnemy.x) > minXDistanceToHighlight)
                    showEnemy = true;
                else if (vectorToEnemy.z > minYUpDistanceToHighlight)
                    showEnemy = true;
                else if (vectorToEnemy.z < minYDownDistanceToHighlight)
                    showEnemy = true;

                if (showEnemy)
                {
                    vectorToEnemy.y = 0;
                    vectorToEnemy.Normalize();
                    importantHighlightedDirections.Add(vectorToEnemy);
                }
            }
        }
        #endregion

        playerInterface.UpdateEnemyHighlight(highlightedDirections, importantHighlightedDirections);
    }

    public void EndEnemyHighlighting(EnemyShip enemy)
    {
        enemiesToHighlight.Remove(enemy.transform);
    }

    public void EndImportantEnemyHighlighting(EnemyShip enemy)
    {
        importantEnemiesToHighlight.Remove(enemy.transform);
    }

    [Header("Other Feedbacks")]
    [SerializeField] long milisecondsDamageVibrationTime;
    [SerializeField] ParticleSystem dangerParticles;

    public void PlayDamageVibrationFeedback(IDamageReceiver receiver)
    {
        Vibration.Vibrate(milisecondsDamageVibrationTime);
    }

    public void PlayDangerParticles()
    {
        if (dangerParticles != null)
            dangerParticles.Play();
    }

    public void StopDangerParticles()
    {
        if (dangerParticles != null)
            dangerParticles.Stop();
    }
}
