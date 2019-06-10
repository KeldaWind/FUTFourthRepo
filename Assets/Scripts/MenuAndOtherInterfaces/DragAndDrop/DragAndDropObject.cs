using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DragAndDropObject : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Image objectIconImage;

    MenuSlot originSlot;
    public MenuSlot GetOriginSlot
    {
        get
        {
            return originSlot;
        }
    }

    public void SetOriginSlot(MenuSlot slot)
    {
        originSlot = slot;
        objectIconImage.sprite = slot.GetSlotIcon();
    }
}
