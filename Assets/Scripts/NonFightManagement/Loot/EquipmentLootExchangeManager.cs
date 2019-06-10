using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EquipmentLootExchangeManager : MonoBehaviour
{
    bool hadToSort;
    public bool HadToSort { get { return hadToSort; } }

    private void Update()
    {
        UpdateExchangePanelManagement();
    }

    #region InputManagement
    bool managerOpen;

    /// <summary>
    /// Graphic raycaster permettant de faire du raycast sur l'UI
    /// </summary>
    [Header("Input Management")]
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
        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {

                pointerEventData = new PointerEventData(eventSystem);
                pointerEventData.position = touch.position;

                List<RaycastResult> results = new List<RaycastResult>();
                graphicRaycaster.Raycast(pointerEventData, results);

                foreach (RaycastResult result in results)
                {
                    MenuSlot slot = result.gameObject.GetComponent<MenuSlot>();
                    if (slot != null)
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
                                equipmentsInformationsManager.OpenInformationsPanel(equipSlot, false);
                                touchedSomething = true;
                                break;
                            }
                        }

                        currentEquipmentSlot = null;

                        equipmentsInformationsManager.CloseInformationsPanel();
                        break;
                    }
                    else if (result.gameObject.GetComponent<Button>() != null)
                    {
                        touchedSomething = true;
                        break;
                    }

                    touchedSomething = false;
                }

                if (!touchedSomething)
                    equipmentsInformationsManager.CloseInformationsPanel();
            }
        }
    }
    #endregion

    [SerializeField] EquipmentsInformationsManager equipmentsInformationsManager;

    [Header("Interface References")]
    [SerializeField] GameObject lootExchangePanel;
    [SerializeField] List<EquipmentSlot> inventorySlots;
    [SerializeField] List<EquipmentSlot> lootedEquipmentSlots;
    [SerializeField] float lootedEquipmentSlotsSpacing;

    List<ShipEquipment> playerInventory;
    public List<ShipEquipment> GetPlayerModifiedInventory { get { return playerInventory; } }
    List<ShipEquipment> lootedEquipments;
    public List<ShipEquipment> GetPlayerModifiedLoot { get { return lootedEquipments; } }

    public void OpenLootExchangePanel(List<ShipEquipment> lootEquipments)
    {
        hadToSort = true;

        equipmentsInformationsManager.SetUpForArena(ThrowSelectedObject);

        lootExchangePanel.gameObject.SetActive(true);

        PlayerEquipmentsDatas equipmentDatas = PlayerDataSaver.LoadPlayerEquipmentsDatas();
        playerInventory = equipmentDatas.GetPlayerEquipmentsInventory;

        lootedEquipments = lootEquipments;

        UpdateInterface();
    }

    public void UpdateInterface()
    {
        #region inventory
        int inventoryCounter = 0;
        for(inventoryCounter = 0; inventoryCounter < playerInventory.Count; inventoryCounter++)
        {
            inventorySlots[inventoryCounter].SetUp(playerInventory[inventoryCounter]);
        }

        while (inventoryCounter < inventorySlots.Count)
        {
            inventorySlots[inventoryCounter].SetUp(null);
            inventoryCounter++;
        }
        #endregion


        #region Loot
        int lootCounter = 0;
        for (lootCounter = 0; lootCounter < lootedEquipments.Count; lootCounter++)
        {
            lootedEquipmentSlots[lootCounter].SetUp(lootedEquipments[lootCounter]);
        }

        while (lootCounter < lootedEquipmentSlots.Count)
        {
            lootedEquipmentSlots[lootCounter].SetUp(null);
            lootCounter++;
        }
        /*float basePosition = 0;
        int numberOfLootedEquipments = lootedEquipments.Count;
        basePosition -= (lootedEquipmentSlotsSpacing * (numberOfLootedEquipments - 1)) / 2;

        int i = 0;
        for (i = 0; i < numberOfLootedEquipments; i++)
        {
            lootedEquipmentSlots[i].gameObject.SetActive(true);
            lootedEquipmentSlots[i].transform.localPosition = new Vector3(basePosition + (lootedEquipmentSlotsSpacing * i), 0, 0);
            lootedEquipmentSlots[i].SetUp(lootedEquipments[i]);
        }

        while (i < lootedEquipmentSlots.Count)
        {
            lootedEquipmentSlots[i].gameObject.SetActive(false);
            i++;
        }*/
        #endregion
    }

    public void UpdateExchangePanelManagement()
    {
        CheckForTouch();
    }

    public void CloseLootExchangePanel()
    {
        lootExchangePanel.gameObject.SetActive(false);

        GameManager.gameManager.ArenaInterfaceManager.OpenLootingPanelAfterSort();
    }

    public void ThrowSelectedObject()
    {
        //l'idée ici, c'est de parcourir les deux listes de slot pour savoir s'il s'agit d'un élément de l'inventaire ou d'un élément de la liste de loot
        //pour ensuite mettre en place le bon update de l'UI 
        EquipmentSlot slotToThrow = equipmentsInformationsManager.GetCurrentEquipmentSlot;

        if(slotToThrow != null)
        {
            bool slotFound = false;
            #region check l'inventaire
            for (int i = 0; i < inventorySlots.Count; i++)
            {
                EquipmentSlot inventorySlot = inventorySlots[i];
                if (inventorySlot == slotToThrow)
                {
                    playerInventory.RemoveAt(i);
                    slotFound = true;
                }
            }
            #endregion

            #region check le loot
            if (!slotFound)
            {
                for (int i = 0; i < lootedEquipmentSlots.Count; i++)
                {
                    EquipmentSlot lootSlot = lootedEquipmentSlots[i];
                    if (lootSlot == slotToThrow)
                    {
                        lootedEquipments.RemoveAt(i);
                        slotFound = true;
                    }
                }
            }
            #endregion
        }

        equipmentsInformationsManager.CloseInformationsPanel();

        CheckIfStillTooMuchEquipments();
    }

    public void CheckIfStillTooMuchEquipments()
    {
        int numberOfTooMuchEquipments = lootedEquipments.Count + playerInventory.Count - IntersceneManager.intersceneManager.GetPlayerDatas.GetPlayerInventoryCapacity;
        if (numberOfTooMuchEquipments > 0)
            UpdateInterface();
        else
        {
            CloseLootExchangePanel();
        }
    }
}
