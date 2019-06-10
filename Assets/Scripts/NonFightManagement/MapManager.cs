using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Cinemachine;

public class MapManager : MonoBehaviour
{
    public static MapManager mapManager;

    private void Awake()
    {
        mapManager = this;
        CheckForIntersceneManager();
        CheckAllGameEquipmentsManager();
    }

    private void Start()
    {
        SetUp();
    }

    public void SetUp()
    {
        playerShip = GameManager.gameManager.Player;
        playerShip.ExtrenalSetUp();

        mapDocksManager.SetUp(this);
        mapArenaManager.SetUp(this);

        equipmentsInformationsManager.SetUp(this);

        menuInputManager.SetUp(this);

        mapProgressionManager.SetUp(this);
        mapProgressionManager.CheckMapProgression();

        confirmationPanel = GameManager.gameManager.ConfirmPanel;

        //set player pos and rot and don't trigger spot
        GetPlayerBackOnRightSpot();
    }

    /// <summary>
    /// Prefab du manager inter-scènes
    /// </summary>
    [Header("Interscene Management")]
    [SerializeField] IntersceneManager intersceneManagerPrefab;
    /// <summary>
    /// Vérifie au lancement de la scène de menu si le manager inter-scènes n'existe pas déjà. S'il n'existe pas, l'instantie.
    /// </summary>
    public void CheckForIntersceneManager()
    {
        if (IntersceneManager.intersceneManager == null)
        {
            IntersceneManager newIntersceneManager = Instantiate(intersceneManagerPrefab);
            newIntersceneManager.SetUp();
        }
    }

    [Header("Equipments Management")]
    [SerializeField] AllGameEquipmentsManager allGameEquipmentsManagerPrefab;
    public void CheckAllGameEquipmentsManager()
    {
        if(AllGameEquipmentsManager.manager == null)
        {
            AllGameEquipmentsManager newAllEquipManager = Instantiate(allGameEquipmentsManagerPrefab);
            newAllEquipManager.SetUp();
        }
    }
    

    [Header("Important Modules")]
    [SerializeField] MenuInputManager menuInputManager;
    #region Progression
    [SerializeField] MapProgressionManager mapProgressionManager;
    public MapProgressionManager MapProgressManager { get { return mapProgressionManager; } }
    #endregion
    ConfirmationPanel confirmationPanel;
    public ConfirmationPanel GetConfirmationPanel { get { return confirmationPanel; } }

    #region Player Management    
    [Header("Player and Equipments Management")]
    [SerializeField] ShipCompositionManager shipCompositionManager;
    public ShipCompositionManager ShipCompoManager
    {
        get
        {
            return shipCompositionManager;
        }
    }

    #region Equipments Actions Management
    [SerializeField] EquipmentsInformationsManager equipmentsInformationsManager;
    public EquipmentsInformationsManager EquipmentsInfoManager
    {
        get
        {
            return equipmentsInformationsManager;
        }
    }
    #endregion

    PlayerShip playerShip;

    public void SetPlayerNotPlayable()
    {
        playerShip.ShipMvt.StopShip();
        GameManager.gameManager.HidePlayerInterface();
        GameManager.gameManager.DisableMainVirtualCamera();
    }

    public void SetPlayerPlayable()
    {
        playerShip.ShipMvt.StartShip();
        GameManager.gameManager.ShowPlayerInterface();
        GameManager.gameManager.EnableMainVirtualCamera();
        GameManager.gameManager.PlrHighlightCameraManagement.DesactivateHighlightCamera();
    }

    public void UpdatePlayerEquipments()
    {
        EquipmentsSet equipmentsSet = shipCompositionManager.ComposeEquipmentSet();
        IntersceneManager.intersceneManager.SetPlayerDatas(equipmentsSet);
        EquipmentsSet set = IntersceneManager.intersceneManager.GetPlayerDatas.GetPlayerEquipedEquipments;
        playerShip.EqpmntManager.SetUpEquipments(set);

        playerShip.PlrInterface.UpdateEquipments(playerShip, playerShip.EqpmntManager.EquipedEquipments);
    }

    public void SavePlayerDatas()
    {
        PlayerDataSaver.SavePlayerEquipmentsDatas(shipCompositionManager.ComposeEquipmentSet(), mapDocksManager.InvtrManager.GetPlayerInventoryEquipments, IntersceneManager.intersceneManager.GetPlayerDatas.GetPlayerCurrentArmorValue, IntersceneManager.intersceneManager.GetPlayerDatas.GetPlayerGoldAmount);
    }

    [ContextMenu("DeletePlayerDatas")]
    public void DeletePlayerDatas()
    {
        PlayerDataSaver.ErasePlayerDatas();
    }
    #endregion

    private void Update()
    {
        menuInputManager.CheckForTouch();
    }

    #region Special Places Management
    MapSpecialPlaceSpot openedSpecialPlace;
    public void SetOpenedSpecialPlace(MapSpecialPlaceSpot specialPlace)
    {
        if (openedSpecialPlace != null && specialPlace == null)
            openedSpecialPlace.EndSpotInteraction();

        openedSpecialPlace = specialPlace;
    }
    public MapSpecialPlaceSpot GetOpenedSpecialPlace
    {
        get
        {
            return openedSpecialPlace;
        }
    }

    CinemachineVirtualCamera lastActivatedVirtualCamera;
    public void SetLastActivatedVirtualCamera(CinemachineVirtualCamera virtualCamera)
    {
        lastActivatedVirtualCamera = virtualCamera;
    }
    public CinemachineVirtualCamera GetLastActivatedVirtualCamera
    {
        get
        {
            return lastActivatedVirtualCamera;
        }
    }

    #region Docks
    [Header("Docks")]
    [SerializeField] MapDocksManager mapDocksManager;
    public MapDocksManager DocksManager
    {
        get
        {
            return mapDocksManager;
        }
    }

    /*public void ChangeDocksModeToPlayerInventory()
    {
        mapDocksManager.ChangeDocksPanelMode(DocksMode.PlayersInventory);
    }*/

    public void ChangeDocksModeToSellerInventory()
    {
        mapDocksManager.ChangeDocksPanelMode(DocksMode.SellersInventory);
    }

    public void ChangeDocksModeToSellerRebuyInventory()
    {
        mapDocksManager.ChangeDocksPanelMode(DocksMode.Rebuy);
    }

    public void CloseDocksPanel()
    {
        mapDocksManager.CloseDocksPanel();
    }

    public void TryEquipSelectedEquipment()
    {
        mapDocksManager.TryEquipEquipment(equipmentsInformationsManager.GetCurrentEquipmentSlot);
    }

    public void TryUpgradeSelectedEquipment()
    {
        mapDocksManager.TryUpgradeEquipment(equipmentsInformationsManager.GetCurrentEquipmentSlot);
    }

    public void TryBuySelectedEquipment()
    {
        mapDocksManager.TryBuyEquipment(equipmentsInformationsManager.GetCurrentEquipmentSlot);
    }

    public void AskSellSelectedEquipment()
    {
        confirmationPanel.OpenConfirmationPanel();
        confirmationPanel.Ask(TrySellSelectedEquipment);
    }

    public void TrySellSelectedEquipment()
    {
        mapDocksManager.TrySellEquipment(equipmentsInformationsManager.GetCurrentEquipmentSlot);
    }
    #endregion

    #region Arena
    [Header("Arena")]
    [SerializeField] MapArenaManager mapArenaManager;
    public MapArenaManager ArenaManager
    {
        get
        {
            return mapArenaManager;
        }
    }

    public void CloseArenaPanel()
    {
        mapArenaManager.CloseArenaPanel();
    }

    public void LaunchArena()
    {
        mapArenaManager.StartLaunchArena();
    }

    public void GetPlayerBackOnRightSpot()
    {
        if (IntersceneManager.intersceneManager != null)
        {
            ArenaParameters parameters = IntersceneManager.intersceneManager.ArenaInterscInformations.GetLaunchedArenaParameters;
            if (parameters != null)
            {
                MapArenaSpot arenaSpot = null;

                foreach (MapArenaSpot spot in mapProgressionManager.GetAllMapArenaSpots)
                {
                    if(spot.GetArenaParameters == parameters)
                    {
                        arenaSpot = spot;
                        arenaSpot.SetDontActivateSincePlayerOut();
                        break;
                    }
                }

                ShipMovements playerShipMvt = playerShip.ShipMvt;
                playerShip.transform.position = arenaSpot.GetPlayerTransformOnceStopped.position;
                playerShipMvt.SetCurrentRotation(arenaSpot.GetPlayerTransformOnceStopped.rotation.eulerAngles.y);
            }
        }
    }
    #endregion
    #endregion
}
