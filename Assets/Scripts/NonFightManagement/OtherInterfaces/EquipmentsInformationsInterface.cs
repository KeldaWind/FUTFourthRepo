using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentsInformationsInterface : MonoBehaviour
{
    public void SetUp(GameButton.InteractionDeleguate equipInteraction, GameButton.InteractionDeleguate upgradeInteraction, GameButton.InteractionDeleguate buyInteraction, GameButton.InteractionDeleguate sellInteraction, GameButton.InteractionDeleguate closeInteraction)
    {
        equipButton.Interaction = equipInteraction;
        upgradeButton.Interaction = upgradeInteraction;
        sellButton.Interaction = sellInteraction;
        buyButton.Interaction = buyInteraction;
        /*equipWithSellButton.Interaction = equipInteraction;
        equipWithUpgradeAndSellButton.Interaction = equipInteraction;

        sellWithEquipButton.Interaction = sellInteraction;
        sellWithEquipAndUpgradeButton.Interaction = sellInteraction;

        upgradeButton.Interaction = upgradeInteraction;
        upgradeWithEquipAndSellButton.Interaction = upgradeInteraction;

        buyButton.Interaction = buyInteraction;*/

        if (closeButton != null)
            closeButton.Interaction = closeInteraction;
    }

    public void SetUpOnlyThrow(GameButton.InteractionDeleguate throwInteraction)
    {
        throwButton.Interaction = throwInteraction;
    }

    [Header("References")]
    [SerializeField] GameObject equipmentInformationsPanel;
    [SerializeField] Text equipmentName;
    [SerializeField] Text equipmentType;
    [SerializeField] Text equipmentDescription;
    [SerializeField] Image equipmentIcon;
    [SerializeField] GameButton closeButton;

    public void OpenInformationPanel(EquipmentInformations equipmentInformations, EquipmentType type)
    {
        equipmentInformationsPanel.SetActive(true);

        equipmentName.text = equipmentInformations.GetEquipmentName;
        equipmentDescription.text = equipmentInformations.GetEquipmentDescription;
        equipmentIcon.sprite = equipmentInformations.GetEquipmentIcon;

        equipmentType.text = type == EquipmentType.Hull ? "Coque" : type == EquipmentType.Canon ? "Cannon" : "Catapulte";
    }

    public void CloseInformationsPanel()
    {
        equipmentInformationsPanel.SetActive(false);

        /*equipOrSellParent.SetActive(false);
        upgradeOrEquipOrSellParent.SetActive(false);
        upgradeParent.SetActive(false);
        buyParent.SetActive(false);
        throwParent.SetActive(false);*/
    }

    public void CloseAllActions()
    {
        equipButton.gameObject.SetActive(false);
        upgradeButton.gameObject.SetActive(false);
        sellButton.gameObject.SetActive(false);
        buyButton.gameObject.SetActive(false);
        /*equipOrSellParent.SetActive(false);
        upgradeOrEquipOrSellParent.SetActive(false);
        upgradeParent.SetActive(false);
        buyParent.SetActive(false);
        throwParent.SetActive(false);*/
    }

    #region V1
    /*[Header("Actions Buttons : Equip or Sell")]
    [SerializeField] GameObject equipOrSellParent;
    [SerializeField] GameButton equipWithSellButton;
    [SerializeField] GameButton sellWithEquipButton;
    [SerializeField] Text onlySellPriceText;

    public void OpenEquipOrSellOption(int sellPrice)
    {
        equipOrSellParent.SetActive(true);
        upgradeOrEquipOrSellParent.SetActive(false);
        upgradeParent.SetActive(false);
        buyParent.SetActive(false);
        throwParent.SetActive(false);

        onlySellPriceText.text = sellPrice + " Gold";
    }


    [Header("Actions Buttons : Equip or Upgrade or Sell")]
    [SerializeField] GameObject upgradeOrEquipOrSellParent;
    [SerializeField] GameButton upgradeWithEquipAndSellButton;
    [SerializeField] GameButton equipWithUpgradeAndSellButton;
    [SerializeField] GameButton sellWithEquipAndUpgradeButton;
    [SerializeField] Text upgradeOrEquipCostText;
    [SerializeField] Text sellOrUpgradePriceText;

    public void OpenEquipOrUpgradeOrSellOption(float upgradeCost, int sellPrice)
    {
        equipOrSellParent.SetActive(false);
        upgradeOrEquipOrSellParent.SetActive(true);
        upgradeParent.SetActive(false);
        buyParent.SetActive(false);
        throwParent.SetActive(false);

        upgradeOrEquipCostText.text = upgradeCost + " Gold";
        sellOrUpgradePriceText.text = sellPrice + " Gold";
    }


    [Header("Actions Buttons : Only Upgrade")]
    [SerializeField] GameObject upgradeParent;
    [SerializeField] Text upgradeCostText;
    [SerializeField] GameButton upgradeButton;

    public void OpenOnlyUpgradeOption(float cost)
    {
        equipOrSellParent.SetActive(false);
        upgradeOrEquipOrSellParent.SetActive(false);
        upgradeParent.SetActive(true);
        buyParent.SetActive(false);
        throwParent.SetActive(false);

        upgradeCostText.text = cost + " Gold";
    }


    [Header("Actions Buttons : Buy")]
    [SerializeField] GameObject buyParent;
    [SerializeField] Text buyCostText;
    [SerializeField] GameButton buyButton;

    public void OpenBuyOption(float cost)
    {
        equipOrSellParent.SetActive(false);
        upgradeOrEquipOrSellParent.SetActive(false);
        upgradeParent.SetActive(false);
        buyParent.SetActive(true);
        throwParent.SetActive(false);

        buyCostText.text = cost + " Gold";
    }

    [Header("Actions Buttons : Throw")]
    [SerializeField] GameObject throwParent;
    [SerializeField] GameButton throwButton;

    public void OpenThrowOption()
    {
        equipOrSellParent.SetActive(false);
        upgradeOrEquipOrSellParent.SetActive(false);
        upgradeParent.SetActive(false);
        buyParent.SetActive(false);
        throwParent.SetActive(true);
    }*/
    #endregion

    #region V2
    [Header("Actions Buttons")]
    [SerializeField] GameButton equipButton;
    [SerializeField] GameButton upgradeButton;
    [SerializeField] Text upgradeValueText;
    [SerializeField] Text nonUpgradeText;
    [SerializeField] Image upgradeValueImage;
    [SerializeField] GameButton sellButton;
    [SerializeField] Text sellValueText;
    [SerializeField] GameButton buyButton;
    [SerializeField] Text buyValueText;
    [SerializeField] Image buyValueImage;
    [SerializeField] GameButton throwButton;
    [SerializeField] Color canColor;
    [SerializeField] Color canImageColor;
    [SerializeField] Color cantColor;
    [SerializeField] Color cantImageColor;

    public void OpenPlayerInventoryEquipmentActions(bool upgradable, float upgradeValue, float sellValue)
    {
        int playerGoldAmount = IntersceneManager.intersceneManager.GetPlayerDatas.GetPlayerGoldAmount;

        equipButton.gameObject.SetActive(true);
        upgradeButton.gameObject.SetActive(true);
        sellButton.gameObject.SetActive(true);
        buyButton.gameObject.SetActive(false);
        if (!upgradable)
        {
            upgradeButton.SetButtonInteractable(false);
            upgradeValueText.gameObject.SetActive(false);
            upgradeValueImage.gameObject.SetActive(false);
            nonUpgradeText.gameObject.SetActive(true);
            //upgradeValueText.text = "Max Level";
            nonUpgradeText.color = cantColor;
        }
        else
        {
            upgradeValueText.gameObject.SetActive(true);
            upgradeValueImage.gameObject.SetActive(true);
            nonUpgradeText.gameObject.SetActive(false);

            upgradeValueText.text = upgradeValue.ToString();
            if (playerGoldAmount >= upgradeValue)
            {
                upgradeButton.SetButtonInteractable(true);
                upgradeValueText.color = canColor;
                upgradeValueImage.color = canImageColor;
            }
            else
            {
                upgradeButton.SetButtonInteractable(false);
                upgradeValueText.color = cantColor;
                upgradeValueImage.color = cantImageColor;
            }
        }

        sellValueText.text = sellValue.ToString();
    }

    public void OpenPlayerEquippedEquipmentActions(bool upgradable, float upgradeValue)
    {
        int playerGoldAmount = IntersceneManager.intersceneManager.GetPlayerDatas.GetPlayerGoldAmount;

        equipButton.gameObject.SetActive(false);
        upgradeButton.gameObject.SetActive(true);
        sellButton.gameObject.SetActive(false);
        buyButton.gameObject.SetActive(false);
        if (!upgradable)
        {
            upgradeButton.SetButtonInteractable(false);
            upgradeValueText.gameObject.SetActive(false);
            upgradeValueImage.gameObject.SetActive(false);
            nonUpgradeText.gameObject.SetActive(true);
            //upgradeValueText.text = "Max Level";
            nonUpgradeText.color = cantColor;
        }
        else
        {
            upgradeValueText.gameObject.SetActive(true);
            upgradeValueImage.gameObject.SetActive(true);
            nonUpgradeText.gameObject.SetActive(false);

            upgradeValueText.text = upgradeValue.ToString();
            if (playerGoldAmount >= upgradeValue)
            {
                upgradeButton.SetButtonInteractable(true);
                upgradeValueText.color = canColor;
                upgradeValueImage.color = canImageColor;
            }
            else
            {
                upgradeButton.SetButtonInteractable(false);
                upgradeValueText.color = cantColor;
                upgradeValueImage.color = cantImageColor;
            }
        }
    }

    public void OpenSellerEquipmentActions(float buyValue)
    {
        int playerGoldAmount = IntersceneManager.intersceneManager.GetPlayerDatas.GetPlayerGoldAmount;

        equipButton.gameObject.SetActive(false);
        upgradeButton.gameObject.SetActive(false);
        sellButton.gameObject.SetActive(false);
        buyButton.gameObject.SetActive(true);

        buyValueText.text = buyValue.ToString();

        bool playerFull = false;
        if (MapManager.mapManager != null && IntersceneManager.intersceneManager != null)
            playerFull = MapManager.mapManager.DocksManager.InvtrManager.GetPlayerInventoryEquipments.Count >= IntersceneManager.intersceneManager.GetPlayerDatas.GetPlayerInventoryCapacity;

        if (playerGoldAmount >= buyValue && !playerFull)
        {
            buyButton.SetButtonInteractable(true);
            buyValueText.color = canColor;
            buyValueImage.color = canImageColor;
        }
        else
        {
            buyButton.SetButtonInteractable(false);
            buyValueText.color = cantColor;
            buyValueImage.color = cantImageColor;

            if (playerFull)
                buyValueText.text = "Full";
        }
    }

    public void OpenThrowEquipmentActions()
    {
        int playerGoldAmount = IntersceneManager.intersceneManager.GetPlayerDatas.GetPlayerGoldAmount;

        equipButton.gameObject.SetActive(false);
        upgradeButton.gameObject.SetActive(false);
        sellButton.gameObject.SetActive(false);
        buyButton.gameObject.SetActive(false);
        throwButton.gameObject.SetActive(true);
    }
    #endregion

    #region Hull Informations
    [Header("Hull Informations")]
    [SerializeField] GameObject hullInformationsParent;
    [SerializeField] Text hullLifeText;
    [SerializeField] Text hullArmorText;
    public void OpenHullInformations(int life, int armor)
    {
        hullInformationsParent.SetActive(true);
        weaponInformationsParent.SetActive(false);

        hullLifeText.text = life.ToString();
        hullArmorText.text = armor.ToString();
    }
    #endregion

    #region Weapon Informations
    [Header("Weapon Informations")]
    [SerializeField] GameObject weaponInformationsParent;
    [SerializeField] Text weaponTypeText;
    [SerializeField] Text weaponShotTypeText;
    [SerializeField] Text weaponEffectText;

    public void OpenWeaponInformations(WeaponInformationType weaponType, WeaponInformationShotType shotType, WeaponInformationEffect effect)
    {
        hullInformationsParent.SetActive(false);
        weaponInformationsParent.SetActive(true);

        weaponTypeText.text =
            weaponType == WeaponInformationType.Catapult ? "Catapult" :
            weaponType == WeaponInformationType.FrontCanons ? "Front Canon" :
            weaponType == WeaponInformationType.SideCanons ? "Side Canon" :
            weaponType == WeaponInformationType.MultiCanons ? "Multi Canon" :
            "";

        weaponShotTypeText.text = shotType == WeaponInformationShotType.SingleShot ? "Single Shot" : shotType.ToString();

        weaponEffectText.text = effect == WeaponInformationEffect.NoEffect ? "No Effect" : effect.ToString();
    }
    #endregion
}


public enum WeaponInformationType
{
    FrontCanons, SideCanons, Catapult, MultiCanons
}

public enum WeaponInformationShotType
{
    SingleShot, Fragmented, Repetitive
}

public enum WeaponInformationEffect
{
    NoEffect, Slowing, Explosive, Piercing, Skewering, Blinding
}
