using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootCirclePreview : ShootPreview
{
    /// <summary>
    /// Les cercles prévisualisant la zone touché par les tirs
    /// </summary>
    [SerializeField] Transform[] fixedCircles;
    [SerializeField] SpriteRenderer[] fixedCirclesSprites;
    /// <summary>
    /// Les cerlces prévisualisant le moment auquel le projectile va toucher
    /// </summary>
    [SerializeField] Transform[] growingCircles;
    [SerializeField] SpriteRenderer[] growingCirclesSprites;

    /// <summary>
    /// Line renderer prévisualisant la trajectoire du projectile
    /// </summary>
    [SerializeField] LineRenderer trajectoryLineRenderer;
    [SerializeField] float lineMovingSpeed;
    [SerializeField] float tilePerDistance;
    [SerializeField] Color playerOutColor;
    [SerializeField] Color playerInColor;
    [SerializeField] Color enemyOutColor;
    [SerializeField] Color enemyInColor;
    [SerializeField] float verticalOffset = 0.1f;
    float currentTilingOffset;
    float currentTile;

    [SerializeField] float previewCircleSizeMultiplier = 1;

    [Header("Pansement le temps de corriger")]
    [SerializeField] float lifeTime;

    /// <summary>
    /// La salve en train d'être prévisualisée
    /// </summary>
    Salvo previewingSalvo;
    ProjectileBoulder previewingProjectile;

    /// <summary>
    /// Taille maximale des projcetiles lancés
    /// </summary>
    float maxSize;
    /// <summary>
    /// Temps total d'expansion des cercles (correspond au temps de vol du projectile)
    /// </summary>
    float totalGrowingTime;
    /// <summary>
    /// temps restant avant la fin du vol du projectile
    /// </summary>
    float currentGrowingTime;

    /// <summary>
    /// Renvoie la taille du cercle total de préparation, en fonction de la taille des projectiles et de leur imprécision
    /// </summary>
    public float GetTotalCircleSize
    {
        get
        {
            float maxSpayRadius = previewingSalvo.GetImprecisionParameter;
            float projSize = previewingSalvo.GetProjectileParameters.GetCurrentProjectileSize;

            if (previewingSalvo.GetNumberOfProjectiles > 1)
            {
                return Mathf.Clamp(maxSpayRadius * 2 + projSize / 2, projSize, Mathf.Infinity);
            }
            else
            {
                return projSize;
            }
        }
    }

    /// <summary>
    /// Commence à prévisualiser la préparation
    /// </summary>
    /// <param name="salvo">Salve à prévisualiser</param>
    public override void ShowPreparePreview(Salvo salvo, AttackTag sourceTag)
    {
        base.ShowPreparePreview(salvo, sourceTag);

        float size = salvo.GetProjectileParameters.GetCurrentProjectileSize;
        maxSize = size * previewCircleSizeMultiplier;

        fixedCircles[0].gameObject.SetActive(true);
        trajectoryLineRenderer.gameObject.SetActive(true);

        previewingSalvo = salvo;

        Vector3 scale = Vector3.one * GetTotalCircleSize * previewCircleSizeMultiplier;
        //scale.y = fixedCircles[0].localScale.y;
        scale.z = fixedCircles[0].localScale.z;
        fixedCircles[0].localScale = scale;
        //fixedCirclesSprites[0].color = play

        growingCircles[0].localScale = new Vector3(0, /*growingCircles[0].localScale.y*/0, growingCircles[0].localScale.z);

        previewingProjectile = GameManager.gameManager.PoolManager.GetProjectile(salvo.GetProjectileType, PoolInteractionType.PeekPrefab) as ProjectileBoulder;

        SetPreviewColor(sourceTag);
    }

    /// <summary>
    /// Finit de prévisualiser la préparation
    /// </summary>
    public override void HidePreparePreview()
    {
        //Destroy(gameObject);
        gameObject.SetActive(false);
        GameManager.gameManager.PoolManager.ReturnPreview(this);
    }

    /// <summary>
    /// Actualise la prévisualisation de la préparation
    /// </summary>
    /// <param name="originPos">Position d'ou provient la compétence</param>
    /// <param name="aimedPos">Position visée par la compétence</param>
    public override void UpdatePreparePreview(Vector3 originPos, Vector3 aimedPos)
    {
        fixedCircles[0].position = aimedPos + Vector3.up * verticalOffset;
        growingCircles[0].position = aimedPos + Vector3.up * verticalOffset;

        if (previewingProjectile != null)
        {
            AnimationCurve curve = previewingProjectile.GetYPosCurve;

            for (int i = 0; i < trajectoryLineRenderer.positionCount; i++)
            {
                float coeff = (float)i / (float)(trajectoryLineRenderer.positionCount - 1);
                Vector3 position = Vector3.Lerp(originPos, aimedPos, coeff);
                position.y = Mathf.Lerp(originPos.y, aimedPos.y, coeff) + curve.Evaluate(coeff);

                trajectoryLineRenderer.SetPosition(i, position);
            }
        }

        currentTile = tilePerDistance * Vector3.Distance(originPos, aimedPos);
    }

    /// <summary>
    /// Commence à prévisualiser la compétence lancée
    /// </summary>
    /// <param name="startPos">Position d'ou provient la compétence</param>
    /// <param name="aimPos">Les Positions visées par la compétence</param>
    /// <param name="time">Temps que mettra le boulet pour atteindre la cible</param>
    public override void StartLaunchedPreview(Vector3 startPos, List<Vector3> aimPos, float time)
    {
        trajectoryLineRenderer.gameObject.SetActive(false);
        totalGrowingTime = time;

        if (totalGrowingTime == 0)
            totalGrowingTime = lifeTime;

        currentGrowingTime = totalGrowingTime;

        UpdatePreparePreview(startPos, aimPos[0]);

        for(int i = 0; i < previewingSalvo.GetNumberOfProjectiles; i++)
        {
            if(i >= aimPos.Count)
            {
                Debug.LogWarning("tous les tirs n'ont pas été générés");
                break;
            }

            if (i >= fixedCircles.Length || i >= growingCircles.Length)
            {
                Debug.LogWarning("trop de tirs pour le nombre de cercles de preview");
                break;
            }

            fixedCircles[i].gameObject.SetActive(true);
            growingCircles[i].gameObject.SetActive(true);

            /*fixedCircles[i].localScale = new Vector3(maxSize, fixedCircles[i].localScale.y, maxSize) ;
            growingCircles[i].localScale = new Vector3(0, growingCircles[i].localScale.y, 0);*/
            fixedCircles[i].localScale = new Vector3(maxSize, maxSize, fixedCircles[i].localScale.z);
            growingCircles[i].localScale = new Vector3(0, 0, growingCircles[i].localScale.z);

            fixedCircles[i].position = aimPos[i] + Vector3.up * verticalOffset;
            growingCircles[i].position = aimPos[i] + Vector3.up * verticalOffset;
        }
    }

    /// <summary>
    /// Actualise la prévisualisation de la compétence lancée
    /// </summary>
    public override void UpdateLaunchedPreview()
    {
        if (currentGrowingTime > 0)
        {
            currentGrowingTime -= Time.deltaTime;

            Vector3 scale = Vector3.Lerp(Vector3.zero, Vector3.one * maxSize, (1 - currentGrowingTime / totalGrowingTime));

            for (int i = 0; i < previewingSalvo.GetNumberOfProjectiles; i++)
            {
                //scale.y = growingCircles[i].localScale.y;
                scale.z = growingCircles[i].localScale.z;
                growingCircles[i].localScale = scale;
            }            
        }
        else if (currentGrowingTime < 0)
        {
            currentGrowingTime = 0;
            EndPreview();
        }
    }

    /// <summary>
    /// Finit de prévisualiser la compétence lancée
    /// </summary>
    public void EndPreview()
    {
        gameObject.SetActive(false);
        GameManager.gameManager.PoolManager.ReturnPreview(this);

        foreach (Transform fixedCircle in fixedCircles)
            fixedCircle.gameObject.SetActive(false);

        foreach (Transform growingCircle in growingCircles)
            growingCircle.gameObject.SetActive(false);
    }

    private void Update()
    {
        UpdateLaunchedPreview();

        currentTilingOffset += Time.unscaledDeltaTime * lineMovingSpeed;
        if (currentTilingOffset < 0)
            currentTilingOffset += 1;
        trajectoryLineRenderer.material.mainTextureScale = new Vector2(currentTile, 1);
        trajectoryLineRenderer.material.mainTextureOffset = new Vector2(currentTilingOffset, 0);
    }

    public void SetPreviewColor(AttackTag sourceTag)
    {
        foreach (SpriteRenderer spriteRenderer in fixedCirclesSprites)
            spriteRenderer.color = (sourceTag == AttackTag.Player) ? playerOutColor : enemyOutColor;

        foreach (SpriteRenderer spriteRenderer in growingCirclesSprites)
            spriteRenderer.color = (sourceTag == AttackTag.Player) ? playerInColor : enemyInColor;
    }
}
