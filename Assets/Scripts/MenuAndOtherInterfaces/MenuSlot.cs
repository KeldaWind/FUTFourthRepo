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
    }

    public virtual Sprite GetSlotIcon()
    {
        return null;
    }
    #endregion
}
