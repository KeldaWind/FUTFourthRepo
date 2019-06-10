using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootPreview : Preview
{
    /// <summary>
    /// Commence à prévisualiser la préparation
    /// </summary>
    /// <param name="salvo">Salve à prévisualiser</param>
    public virtual void ShowPreparePreview(Salvo salvo, AttackTag sourceTag)
    {
        gameObject.SetActive(true);
    }
}
