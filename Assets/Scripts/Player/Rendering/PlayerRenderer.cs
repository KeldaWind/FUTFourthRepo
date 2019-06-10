using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Ancienne classe de renderer du joueur
/// </summary>
[System.Serializable]
public class PlayerRenderer 
{
    [Header("References")]
    [SerializeField] Transform rendererParent;
    Vector3 normalRendererRotation;

    public void SetUp()
    {
        normalRendererRotation = rendererParent.localRotation.eulerAngles;
    }

    public void TurnRendererTowardVector(Vector3 lookDirection)
    {
        float rotY = Mathf.Atan2(lookDirection.x, lookDirection.z) * Mathf.Rad2Deg;

        if (rendererParent != null)
            rendererParent.localRotation = Quaternion.Euler(normalRendererRotation + new Vector3(0, rotY, 0));
    }
}
