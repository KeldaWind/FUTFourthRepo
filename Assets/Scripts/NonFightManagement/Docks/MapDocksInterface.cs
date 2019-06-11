using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapDocksInterface : MonoBehaviour
{
    #region Global
    [SerializeField] GameObject docksPanel;
    [SerializeField] GameButton playerInventoryButton;
    [SerializeField] GameButton sellerInventoryButton;
    [SerializeField] GameButton sellerRebuyInventoryButton;
    [SerializeField] GameButton closeButton;
    [SerializeField] GameButton repairButton;
    [SerializeField] GameButton openInventoryButton;

    public void SetUp(/*GameButton.InteractionDeleguate openPlayerInteraction,*/ GameButton.InteractionDeleguate openSellerInteraction, GameButton.InteractionDeleguate openSellerRebuyInteraction, GameButton.InteractionDeleguate closeInteraction, GameButton.InteractionDeleguate repairInteraction, GameButton.InteractionDeleguate openPlayerOnlyInteraction)
    {
        //playerInventoryButton.Interaction = openPlayerInteraction;
        sellerInventoryButton.Interaction = openSellerInteraction;
        sellerRebuyInventoryButton.Interaction = openSellerRebuyInteraction;
        closeButton.Interaction = closeInteraction;
        repairButton.Interaction = repairInteraction;
        openInventoryButton.Interaction = openPlayerOnlyInteraction;
        openInventoryButton.gameObject.SetActive(true);
    }

    public void OpenDocksPanel(bool withUtilities)
    {
        docksPanel.SetActive(true);

        if (withUtilities)
        {
            playerInventoryButton.gameObject.SetActive(true);
            sellerInventoryButton.gameObject.SetActive(true);
            sellerRebuyInventoryButton.gameObject.SetActive(true);
            //repairButton.gameObject.SetActive(true);
            openInventoryButton.gameObject.SetActive(false);
        }
        else
        {
            playerInventoryButton.gameObject.SetActive(false);
            sellerInventoryButton.gameObject.SetActive(false);
            sellerRebuyInventoryButton.gameObject.SetActive(false);
            //repairButton.gameObject.SetActive(false);
            openInventoryButton.gameObject.SetActive(true);
        }
    }

    public void CloseDocksPanel()
    {
        docksPanel.SetActive(false);
        openInventoryButton.gameObject.SetActive(true);
    }
    #endregion

    public List<ShipEquipment> GetSortedEquipments(List<ShipEquipment> equipments)
    {
        List<ShipEquipment> sortedEquipments = new List<ShipEquipment>();

        if (equipments == null)
            return sortedEquipments;

        foreach (ShipEquipment equip in equipments)
            if (equip.GetEquipmentType == EquipmentType.Hull)
                sortedEquipments.Add(equip);

        foreach (ShipEquipment equip in equipments)
            if (equip.GetEquipmentType == EquipmentType.Canon)
                sortedEquipments.Add(equip);

        foreach (ShipEquipment equip in equipments)
            if (equip.GetEquipmentType == EquipmentType.Catapult)
                sortedEquipments.Add(equip);

        return sortedEquipments;
    }
    #region Player Inventory
    [Header("Player Inventory")]
    [SerializeField] GameObject playerInventoryPanel;
    [SerializeField] List<EquipmentSlotInventory> playerInventorySlots;
    public List<EquipmentSlotInventory> GetPlayerInventorySlots { get { return playerInventorySlots; } }

    public void OpenPlayerInventoryPanel(List<ShipEquipment> equipments)
    {
        List<ShipEquipment> sortedEquipments = GetSortedEquipments(equipments);

        ClearPlayerInventory();
        playerInventoryPanel.SetActive(true);

        for (int i = 0; i < sortedEquipments.Count && i < playerInventorySlots.Count; i++)
        {
            playerInventorySlots[i].SetUp(sortedEquipments[i]);
        }
    }

    public void ClosePlayerInventoryPanel()
    {
        playerInventoryPanel.SetActive(false);
        ClearPlayerInventory();
    }

    public void ClearPlayerInventory()
    {
        foreach (EquipmentSlotInventory slot in playerInventorySlots)
        {
            slot.SetUp(null);
        }
    }

    public bool PlayerInventoryOpened
    {
        get
        {
            return playerInventoryPanel.activeInHierarchy;
        }
    }


    #region Player Values
    [SerializeField] Text playerGoldText;
    public void UpdatePlayerGoldText()
    {
        playerGoldText.text = IntersceneManager.intersceneManager.GetPlayerDatas.GetPlayerGoldAmount.ToString() /*+ " G"*/;
    }

    #region V1
    /*[SerializeField] Text playerHullLifeText;
    public void UpdatePlayerHullLifeText(int lifeAmount)
    {
        playerHullLifeText.text = lifeAmount.ToString();
    }

    [SerializeField] Text playerHullCurrentArmorText;
    [SerializeField] Text playerHullMaxArmorText;
    public void UpdatePlayerHullArmorText(int currentArmorAmount, int maxArmorAmount)
    {
        playerHullCurrentArmorText.text = currentArmorAmount.ToString();
        playerHullMaxArmorText.text = maxArmorAmount.ToString();
    }

    public void UpdatePlayerHullArmorText(int currentArmorAmount)
    {
        playerHullCurrentArmorText.text = currentArmorAmount.ToString();
    }*/
    #endregion

    #region V2
    [SerializeField] Image armorFillImage;
    int maxArmorValue;
    public void UpdatePlayerHullArmorText(int currentArmorAmount, int maxArmorAmount)
    {
        armorFillImage.fillAmount = (float)currentArmorAmount / (float)maxArmorAmount;
        maxArmorValue = maxArmorAmount;
        /*playerHullCurrentArmorText.text = currentArmorAmount.ToString();
        playerHullMaxArmorText.text = maxArmorAmount.ToString();*/
    }

    public void UpdatePlayerHullArmorText(int currentArmorAmount)
    {
        armorFillImage.fillAmount = (float)currentArmorAmount / (float)maxArmorValue;
    }
    #endregion
    #endregion
    #endregion

    #region Seller Inventory
    [Header("Seller Inventory")]
    [SerializeField] GameObject sellerInventoryPanel;
    [SerializeField] List<EquipmentSlotInventory> sellerInventorySlots;

    public void OpenSellerInventoryPanel(List<ShipEquipment> equipments)
    {
        List<ShipEquipment> sortedEquipments = GetSortedEquipments(equipments);

        ClearSellerInventory();
        sellerInventoryPanel.SetActive(true);

        for (int i = 0; i < sortedEquipments.Count && i < sellerInventorySlots.Count; i++)
        {
            sellerInventorySlots[i].SetUp(sortedEquipments[i]);
        }
    }

    public void CloseSellerInventoryPanel()
    {
        sellerInventoryPanel.SetActive(false);
        ClearSellerInventory();
    }

    public void ClearSellerInventory()
    {
        foreach (EquipmentSlotInventory slot in sellerInventorySlots)
        {
            slot.SetUp(null);
        }
    }

    public bool SellerInventoryOpened
    {
        get
        {
            return sellerInventoryPanel.activeInHierarchy;
        }
    }
    #endregion

    #region Seller Rebuy Inventory
    [Header("Seller Rebuy Inventory")]
    [SerializeField] GameObject sellerRebuyInventoryPanel;
    [SerializeField] List<EquipmentSlotInventory> sellerRebuyInventorySlots;
    public int GetSoldEquipmentsCapacity { get { return sellerRebuyInventorySlots.Count; } }

    public void OpenSellerRebuyInventoryPanel(List<ShipEquipment> equipments)
    {
        ClearSellerRebuyInventory();
        sellerRebuyInventoryPanel.SetActive(true);

        for (int i = 0; i < equipments.Count && i < sellerRebuyInventorySlots.Count; i++)
        {
            sellerRebuyInventorySlots[i].SetUp(equipments[i]);
        }
    }

    public void CloseSellerRebuyInventoryPanel()
    {
        sellerRebuyInventoryPanel.SetActive(false);
        ClearSellerRebuyInventory();
    }

    public void ClearSellerRebuyInventory()
    {
        foreach (EquipmentSlotInventory slot in sellerRebuyInventorySlots)
        {
            slot.SetUp(null);
        }
    }

    public bool SellerRebuyInventoryOpened
    {
        get
        {
            return sellerRebuyInventoryPanel.activeInHierarchy;
        }
    }
    #endregion

}
