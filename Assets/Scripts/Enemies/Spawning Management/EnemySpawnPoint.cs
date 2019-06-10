using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnPoint : MonoBehaviour
{
    [Header("Basic Parameters")]
    [SerializeField] EnemySpawningRarity rarityOfSpawnedEnemy;
    public EnemySpawningRarity GetRarityOfSpawnedEnemy { get { return rarityOfSpawnedEnemy; } }

    [Header("Linked Watching Point")]
    [SerializeField] EnemyWatchingRoundParameters linkedWatchingRoundParameters;
    public EnemyWatchingRoundParameters GetLinkedWatchingRoundParameters { get { return linkedWatchingRoundParameters; } }
}
