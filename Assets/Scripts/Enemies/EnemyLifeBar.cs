using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLifeBar : MonoBehaviour
{
    int shipMaxLife;
    int shipCurrentLife;

    float scalePerLifePoint = 0.5f;

    [Header("References")]
    [SerializeField] Transform lifeBarEmpty;
    [SerializeField] Transform lifeBarFill;
    [SerializeField] Transform fillPlacement;

    public void SetUp(int maxLife)
    {
        shipMaxLife = maxLife;

        lifeBarEmpty.localScale = new Vector3(0.5f * maxLife, lifeBarEmpty.localScale.y, lifeBarEmpty.localScale.z);
        lifeBarFill.localScale = new Vector3(0.5f * maxLife, lifeBarFill.localScale.y, lifeBarFill.localScale.z);

        lifeBarFill.position = fillPlacement.position;
    }

    public void UpdateLifeBar(int currentLife)
    {
        shipCurrentLife = currentLife;
        lifeBarFill.localScale = new Vector3(0.5f * currentLife, lifeBarFill.localScale.y, lifeBarFill.localScale.z);
    }

    public float GetLifePercentage
    {
        get
        {
            return (float)shipCurrentLife / (float)shipMaxLife;
        }
    }

    private void FixedUpdate()
    {
        transform.rotation = Quaternion.identity;
    }
}
