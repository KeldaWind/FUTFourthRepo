using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLootCrate : MonoBehaviour
{
    [SerializeField] LootCratePoolTag lootCratePoolTag;
    public LootCratePoolTag GetLootCratePoolTag { get { return lootCratePoolTag; } }

    int lootedGold;
    public int GetLootedGold { get { return lootedGold; } }
    ShipEquipment lootedEquipment;
    public ShipEquipment GetLootedEquipment { get { return lootedEquipment; } }

    [Header("Looting Feedback")]
    [SerializeField] SphereCollider lootingCollider;
    [SerializeField] float lootingRadius;
    [SerializeField] float lootingDuration;
    [SerializeField] AnimationCurve lootingTowardPlayerCurve;
    [SerializeField] AnimationCurve lootingYCurve;
    [SerializeField] float lootingMaxHeight;
    float remainingLootingDuration;
    PlayerShip lootingPlayer;
    Vector3 baseLootingPosition;
    [SerializeField] float rotationSpeed;
    float rotationCounter;
    [SerializeField] ParticleSystem shinyParticles;
    [SerializeField] AudioSource lootCrateAudioSource;
    [SerializeField] Sound lootSound;

    public void SetBoxLoot(int gold, ShipEquipment equipment)
    {
        lootingPlayer = null;
        baseLootingPosition = Vector3.zero;
        remainingLootingDuration = 0;

        lootedGold = gold;
        lootedEquipment = equipment;

        gameObject.SetActive(true);

        DebugLootContent();

        lootingCollider.radius = lootingRadius;

        //shinyParticles.Play();

        transform.rotation = Quaternion.identity;
    }

    public void DebugLootContent()
    {
        string lootText = "";
        if (lootedGold == 0 && lootedEquipment == null)
            lootText = "rien";
        else if (lootedGold != 0 && lootedEquipment != null)
            lootText = lootedGold + " gold et l'équipement " + lootedEquipment.GetEquipmentInformations.GetEquipmentName;
        else if (lootedGold != 0)
            lootText = lootedGold + " gold";
        else
            lootText = "l'équipement " + lootedEquipment.GetEquipmentInformations.GetEquipmentName;

        //Debug.Log("La caisse " + name + " contient " + lootText);
    }

    void OnTriggerEnter(Collider other)
    {
        if (lootingPlayer != null)
            return;

        PlayerShipHitbox playerShipHitbox = other.GetComponent<PlayerShipHitbox>();
        if (playerShipHitbox != null)
        {
            PlayerShip playerShip = playerShipHitbox.GetRelatedShip as PlayerShip;
            if (playerShip != null)
            {
                StartLootCrate(playerShip);
            }
        }
    }

    private void Update()
    {
        if (remainingLootingDuration != 0)
            UpdateLootCrate();
        else
        {
            UpdateStreamInfluenceValue();
            UpdateFloatingMove();
        }
    } 

    public void StartLootCrate(PlayerShip playerShip)
    {
        lootingPlayer = playerShip;
        remainingLootingDuration = lootingDuration;
        baseLootingPosition = transform.position;

        Vector3 direction = (playerShip.transform.position - transform.position).normalized;
        direction.y = 0;
        direction.Normalize();
        float rotY = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, rotY, 0));

        shinyParticles.Stop();

        lootCrateAudioSource.PlaySound(lootSound);
    }

    public float GetLootingProgressionCoeff { get { return 1 - remainingLootingDuration / lootingDuration; } }

    public void UpdateLootCrate()
    {
        if (remainingLootingDuration > 0)
            remainingLootingDuration -= Time.deltaTime;
        else if (remainingLootingDuration < 0)
            LootCrate();

        Vector3 cratePosition = Vector3.Lerp(baseLootingPosition, lootingPlayer.transform.position, lootingTowardPlayerCurve.Evaluate(GetLootingProgressionCoeff));
        cratePosition += new Vector3(0, lootingYCurve.Evaluate(GetLootingProgressionCoeff) * lootingMaxHeight, 0);
        transform.position = cratePosition;

        Vector3 direction = (lootingPlayer.transform.position - transform.position).normalized;
        direction.y = 0;
        direction.Normalize();
        float rotY = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        rotationCounter += rotationSpeed * Time.deltaTime;

        transform.rotation = Quaternion.Euler(new Vector3(0, rotY, rotationCounter));
    }

    public void LootCrate(PlayerShip playerShip)
    {
        lootingPlayer = playerShip;
        LootCrate();
    }

    public void LootCrate()
    {
        LootPopUpObject lootPopUpObject = GameManager.gameManager.PoolManager.GetLootPopUp(LootPopUpPoolTag.Normal, PoolInteractionType.GetFromPool);
        lootPopUpObject.transform.position = transform.position;

        if (lootedEquipment != null)
        {
            lootingPlayer.PlayerLootManager.AddLootedEquipment(lootedEquipment);
            lootPopUpObject.SetUp(transform.position, lootedEquipment.GetEquipmentInformations.GetEquipmentIcon);
        }

        if (lootedGold != 0)
        {
            lootingPlayer.PlayerLootManager.AddLootedGold(lootedGold);
            lootPopUpObject.SetUp(transform.position, lootedGold);
        }

        ArenaManager.arenaManager.DropManager.RemoveDropCrate(this);
        gameObject.SetActive(false);
        GameManager.gameManager.PoolManager.ReturnLootCrate(this);
    }

    #region
    [Header("Feedbacks : Floating Projectile Feedback")]
    [SerializeField] float floatingSpeed = 0.4f;
    float endFloatingSpeed;
    float currentfloatingSpeed = 0;
    [SerializeField] float maxFloatingOffset = 3f;
    [SerializeField] float minFloatingOffset = 0.5f;
    float currentMaxFloatingOffset;
    [SerializeField] float floatingDesceleration = 1.5f;
    [SerializeField] AnimationCurve floatingCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] AnimationCurve floatingMoveCurve;
    float floatingCounter;
    Vector3 baseFloatingPosition;
    [SerializeField] float floatingMoveDuration;
    float remainingFloatingMoveDuration;

    public void SetUpFloatingMove()
    {
        remainingFloatingMoveDuration = floatingMoveDuration;
        floatingCurve.preWrapMode = WrapMode.PingPong;
        floatingCurve.postWrapMode = WrapMode.PingPong;
        currentMaxFloatingOffset = maxFloatingOffset;
        floatingCounter = -0.5f;
        baseFloatingPosition = transform.position;

        floatingMoveCurve = new AnimationCurve().SetAsFastInSlowOutDescreasingCurve();

        endFloatingSpeed = floatingSpeed / 3;
        currentfloatingSpeed = floatingSpeed;
    }

    public void UpdateFloatingMove()
    {
        if (currentMaxFloatingOffset > minFloatingOffset)
            currentMaxFloatingOffset -= Time.deltaTime * floatingDesceleration;
        else if (currentMaxFloatingOffset < minFloatingOffset)
            currentMaxFloatingOffset = minFloatingOffset;

        if (currentMaxFloatingOffset == minFloatingOffset)
            currentfloatingSpeed = Mathf.Lerp(currentfloatingSpeed, endFloatingSpeed, 0.1f);

        floatingCounter += Time.deltaTime * currentfloatingSpeed;

        float floatCoeff = floatingCurve.Evaluate(floatingCounter);
        floatCoeff = (floatCoeff * 2) - 1;

        float floatingHeight = floatCoeff * currentMaxFloatingOffset;
        transform.position = baseFloatingPosition + new Vector3(0, floatingHeight, 0);
    }
    #endregion

    #region Water Stream Movements
    [Header("Stream Management")]
    [SerializeField] Rigidbody crateBody;
    [SerializeField] float streamCoeff = 0.3f;
    [SerializeField] float streamMultiplier = 3;
    bool beingStreamed;
    Vector3 currentStreamForce;
    float streamInfluence;

    public void StartStreamForce(Vector3 streamForce)
    {
        beingStreamed = true;
        currentStreamForce = streamForce;
    }

    public void UpdateStreamForce(Vector3 streamForce)
    {
        currentStreamForce = streamForce;
    }

    public void StopStreamForce(Vector3 streamForce)
    {
        beingStreamed = false;
        currentStreamForce = streamForce;
    }

    public Vector3 GetCurrentStreamForce
    {
        get
        {
            return streamInfluence * currentStreamForce;
        }
    }

    public void UpdateStreamInfluenceValue()
    {
        crateBody.velocity = GetCurrentStreamForce * streamMultiplier;

        if (beingStreamed)
        {
            if (streamInfluence < 1)
                streamInfluence = Mathf.Lerp(streamInfluence, 1, streamCoeff);

            if (streamInfluence > 0.99f)
                streamInfluence = 1;
        }
        else
        {
            if (streamInfluence > 0)
                streamInfluence = Mathf.Lerp(streamInfluence, 0, streamCoeff);

            if (streamInfluence < 0.01f)
                streamInfluence = 0;
        }
    }
    #endregion
}
