using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyShipsPool : Pool<EnemyShip>
{
    [SerializeField] EnemyShipPoolTag enemyPoolTag;
    public EnemyShipPoolTag GetEnemyPoolTag { get { return enemyPoolTag; } }

    public override void SetUpPool()
    {
        objectsQueue = new Queue<EnemyShip>();

        for (int i = 0; i < initialPoolSize; i++)
        {
            EnemyShip instantiatedObj = Object.Instantiate(objectPrefab, poolParent);
            objectsQueue.Enqueue(instantiatedObj);
            instantiatedObj.gameObject.SetActive(false);
            instantiatedObj.FirstSetUp();
        }
    }

    public override void CheckForQueueEmpty()
    {
        if (objectsQueue.Count < 1)
        {
            EnemyShip instantiatedObj = Object.Instantiate(objectPrefab, poolParent);
            objectsQueue.Enqueue(instantiatedObj);
            instantiatedObj.gameObject.SetActive(false);
            instantiatedObj.FirstSetUp();
        }
    }
}

public enum EnemyShipPoolTag
{
    SideCanonsNormal,
    SideCanonsSlowing,
    SideCanonBordee,
    SideRepetitiveCanon,

    FrontCanon,
    FrontRepetitiveCanon,
    FrontShotgun,
    FrontPin,

    CatapultNormal,
    FragtapultNormal,
    CollTapult,
    BarragePult,

    FrontChargeNormal,
    ArmoredFrontCharge,

    BossArena4,
}

