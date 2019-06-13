using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuSlot : MonoBehaviour
{
    [Header("Invariable References")]
    [SerializeField] protected Image objectImage;

    private void Start()
    {
        ActualizeObjectImage();
    }

    public virtual bool IsEmpty()
    {
        return false;
    }

    #region Rendering
    public void ActualizeObjectImage()
    {
        objectImage.sprite = GetSlotIcon();
        if (GetSlotIcon() == null)
            objectImage.enabled = false;
        else
            objectImage.enabled = true;
    }

    public virtual Sprite GetSlotIcon()
    {
        return null;
    }
    #endregion
}
