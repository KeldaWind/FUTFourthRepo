using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootLinePreview : ShootPreview
{
    /// <summary>
    /// Ligne prévisualisant la trajectoir du projectile
    /// </summary>
    [SerializeField] LineRenderer previewRenderer;
    [SerializeField] float lineMovingSpeed;
    [SerializeField] float tilePerDistance;
    float currentTilingOffset;
    float currentTile;

    /// <summary>
    /// La salve en train d'être prévisualisée
    /// </summary>
    Salvo previewSalvo;

    /// <summary>
    /// Commence à prévisualiser la préparation
    /// </summary>
    /// <param name="salvo">Salve à prévisualiser</param>
    public override void ShowPreparePreview(Salvo salvo, AttackTag sourceTag)
    {
        base.ShowPreparePreview(salvo, sourceTag);

        float size = salvo.GetProjectileParameters.GetCurrentProjectileSize;

        previewRenderer.gameObject.SetActive(true);
        previewRenderer.startWidth = size;
        previewRenderer.endWidth = size;

        previewSalvo = salvo;

        currentTilingOffset = 0;
    }

    /// <summary>
    /// Finit de prévisualiser la préparation
    /// </summary>
    public override void HidePreparePreview()
    {
        gameObject.SetActive(false);
        GameManager.gameManager.PoolManager.ReturnPreview(this);
    }

    /// <summary>
    /// Actualise la prévisualisation de la préparation
    /// </summary>
    /// <param name="originPos">Position d'ou provient la compétence</param>
    /// <param name="direction">Direction visée par la compétence</param>
    public override void UpdatePreparePreview(Vector3 originPos, Vector3 direction)
    {
        transform.position = originPos;

        float rotY = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(new Vector3(90, rotY - 90, 0));

        float distance = previewSalvo.GetProjectileParameters.GetEvolutiveSpeed() * previewSalvo.GetProjectileParameters.GetTotalLifeTime * 1.1f;

        currentTile = distance * tilePerDistance;

        previewRenderer.SetPosition(1, new Vector3(/*0*/distance, 0, /*distance*/0));

        Vector3 leftVector = (Quaternion.Euler(0, -previewSalvo.GetImprecisionParameter / 2, 0) * new Vector3(0, 0, 1)) * distance + new Vector3(-previewSalvo.GetProjectileParameters.GetCurrentProjectileSize/2, 0, 0);
        Vector3 rightVector = (Quaternion.Euler(0, previewSalvo.GetImprecisionParameter / 2, 0) * new Vector3(0, 0, 1)) * distance + new Vector3(previewSalvo.GetProjectileParameters.GetCurrentProjectileSize/2, 0, 0);

        previewRenderer.endWidth = Vector3.Distance(leftVector, rightVector);
    }

    /// <summary>
    /// Commence à prévisualiser la compétence lancée
    /// </summary>
    /// <param name="startPos"></param>
    /// <param name="aimPos"></param>
    /// <param name="time"></param>
    public override void StartLaunchedPreview(Vector3 startPos, List<Vector3> aimPos, float nothing)
    {
        gameObject.SetActive(false);
        GameManager.gameManager.PoolManager.ReturnPreview(this);
    }

    #region Animation Test
    private void Update()
    {
        currentTilingOffset += Time.unscaledDeltaTime * lineMovingSpeed;
        if (currentTilingOffset < 0)
            currentTilingOffset += 1;
        previewRenderer.material.mainTextureScale = new Vector2(currentTile, 1);
        previewRenderer.material.mainTextureOffset = new Vector2(currentTilingOffset, 0);
    }
    #endregion
}
