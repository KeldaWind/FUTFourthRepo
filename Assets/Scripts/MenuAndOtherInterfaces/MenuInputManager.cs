using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[System.Serializable]
public class MenuInputManager
{
    #region Setting Up
    MapManager mapManager;
    public void SetUp(MapManager mpManager)
    {
        mapManager = mpManager;
    }
    #endregion

    /// <summary>
    /// Graphic raycaster permettant de faire du raycast sur l'UI
    /// </summary>
    [Header("General References")]
    [SerializeField] GraphicRaycaster graphicRaycaster;
    /// <summary>
    /// Event System permettant de faire du raycast sur l'UI
    /// </summary>
    [SerializeField] EventSystem eventSystem;
    /// <summary>
    /// Ponter Event Data permettant de faire du raycast sur l'UI
    /// </summary>
    PointerEventData pointerEventData;

    EquipmentSlot currentEquipmentSlot;

    public void CheckForTouch()
    {
        if (mapManager.GetConfirmationPanel.ConfirmingSomething)
            return;

        foreach (Touch touch in Input.touches)
        {
            if(touch.phase == TouchPhase.Began)
            {
                
                pointerEventData = new PointerEventData(eventSystem);
                pointerEventData.position = touch.position;

                List<RaycastResult> results = new List<RaycastResult>();
                graphicRaycaster.Raycast(pointerEventData, results);

                foreach (RaycastResult result in results)
                {
                    MenuSlot slot = result.gameObject.GetComponent<MenuSlot>();
                    if(slot != null)
                    {
                        currentEquipmentSlot = slot as EquipmentSlot;
                        break;
                    }
                }
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                bool touchedSomething = false;

                pointerEventData = new PointerEventData(eventSystem);
                pointerEventData.position = touch.position;

                List<RaycastResult> results = new List<RaycastResult>();
                graphicRaycaster.Raycast(pointerEventData, results);

                foreach (RaycastResult result in results)
                {
                    MenuSlot slot = result.gameObject.GetComponent<MenuSlot>();
                    if (slot != null && slot == currentEquipmentSlot)
                    {
                        EquipmentSlot equipSlot = (slot as EquipmentSlot);
                        if (equipSlot != null)
                        {
                            if (equipSlot.GetPlacedEquipment != null && !equipSlot.JustBeenPlace)
                            {
                                /*if (!mapManager.DocksManager.PlayerInventoryOnly)
                                    mapManager.EquipmentsInfoManager.OpenInformationsPanel(equipSlot, false);
                                else
                                    mapManager.EquipmentsInfoManager.OpenInformationsPanel(equipSlot);*/
                                mapManager.EquipmentsInfoManager.OpenInformationsPanel(equipSlot, mapManager.DocksManager.PlayerInventoryOnly);

                                touchedSomething = true;

                                /*if (equipSlot as EquipmentSlotInventory != null)
                                    mapManager.ShipCompoManager.ShowEquipButton(equipSlot as EquipmentSlotInventory);*/

                                break;
                            }
                        }

                        currentEquipmentSlot = null;

                        mapManager.EquipmentsInfoManager.CloseInformationsPanel();
                        break;
                    }
                    else if (result.gameObject.GetComponent<Button>() != null)
                    {
                        touchedSomething = true;
                        break;
                    }

                    touchedSomething = false;
                }

                if(!touchedSomething)
                    mapManager.EquipmentsInfoManager.CloseInformationsPanel();
            }
        }
    }

    public MenuSlot CheckForTouchedSlot(Vector3 position)
    {
        pointerEventData = new PointerEventData(eventSystem);
        pointerEventData.position = position;

        List<RaycastResult> results = new List<RaycastResult>();
        graphicRaycaster.Raycast(pointerEventData, results);

        foreach (RaycastResult result in results)
        {
            MenuSlot slot = result.gameObject.GetComponent<MenuSlot>();

            if (slot != null)
            {
                return slot;
            }
        }

        return null;
    }
}
