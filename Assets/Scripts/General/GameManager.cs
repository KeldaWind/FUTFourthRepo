using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;
using UnityEngine.UI;

/// <summary>
/// Classe permettant de gérer les informations importantes au sein d'une arène
/// </summary>
public class GameManager : MonoBehaviour
{
    [Header("Development Tools")]
    public float normalTimeScale = 1;

    /// <summary>
    /// Instance unique du game manager sur cette scène
    /// </summary>
    public static GameManager gameManager;

    /// <summary>
    /// Le bateau du joueur
    /// </summary>
    [Header("Important References")]
    [SerializeField] PlayerShip player;
    /// <summary>
    /// Le bateau du joueur
    /// </summary>
    public PlayerShip Player
    {
        get
        {
            return player;
        }
    }

    public bool CheckIfPlayerHasControl
    {
        get
        {
            return /*!player.ShipMvt.Stopped && playerInterface.gameObject.activeInHierarchy*/!player.ShipMvt.Stopped && !playerInterface.Hidden;
        }
    }

    /// <summary>
    /// La caméra principale suivant le joueur
    /// </summary>
    [SerializeField] Camera mainCamera;
    /// <summary>
    /// La caméra principale suivant le joueur
    /// </summary>
    public Camera MainCamera
    {
        get
        {
            return mainCamera;
        }
    }

    [SerializeField] CinemachineVirtualCamera mainVirtualCamera;
    public CinemachineVirtualCamera MainVirtualCamera
    {
        get
        {
            return mainVirtualCamera;
        }
    }

    [SerializeField] suiviPourLaCam lockerCam;
    public void StopPlayerCamFollow()
    {
        lockerCam.enabled = false;
    }

    [SerializeField] PlayerInterface playerInterface;
    public PlayerInterface PlrInterface
    {
        get
        {
            return playerInterface;
        }
    }

    [SerializeField] GameObject dialogueInterface;
    public GameObject DialogueInterface
    {
        get
        {
            return dialogueInterface;
        }
    }

    [Header("Important Variables")]
    [SerializeField] float seaLevel;
    public float GetSeaLevel { get { return seaLevel; } }

    [Header("Feedbacks and Interfaces Modules")]
    [SerializeField] ScreenshakeManager screenshakeManager;
    public ScreenshakeManager ScrshkManager
    {
        get
        {
            return screenshakeManager;
        }
    }

    [SerializeField] SlowMoManager slowMoManager;
    public SlowMoManager SlwMoManager
    {
        get
        {
            return slowMoManager;
        }
    }

    [SerializeField] ArenaBeginEndInterfaceManager arenaInterfaceManager;
    public ArenaBeginEndInterfaceManager ArenaInterfaceManager { get { return arenaInterfaceManager; } }

    [SerializeField] PauseInterfaceManager pauseInterfaceManager;
    public PauseInterfaceManager PauseIntrfcManager { get { return pauseInterfaceManager; } }

    [SerializeField] PlayerHighlightCameraManagement playerHighlightCameraManagement;
    public PlayerHighlightCameraManagement PlrHighlightCameraManagement { get { return playerHighlightCameraManagement; } }

    [SerializeField] CinematicManager cinematicManager;
    public CinematicManager CinematicMng { get { return cinematicManager; } }

    [SerializeField] LoadingScreenManager loadingScreenManager;
    public LoadingScreenManager LoadScreenManager { get { return loadingScreenManager; } }

    private void Awake()
    {
        Time.timeScale = normalTimeScale;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        gameManager = this;
        screenshakeManager.SetUpScreenshake();
        poolingManager.SetUpPooling();
        pauseInterfaceManager.SetUp();
        playerHighlightCameraManagement.Initialize();
        cinematicManager.SetUp();
        outMapManager.SetUp();
        player.ExtrenalSetUp();

        SetUpCheats();

        loadingScreenManager.StartBeginLoad(null);
    }

    private void Start()
    {
        if (ArenaManager.arenaManager == null)
        {
            poolingManager.GenerateProjectilePoolDisctionary();
        }
        else
        {
            if (ArenaManager.arenaManager.IsTutorialArena)
                pauseInterfaceManager.DisableGoBackToMap();

            pauseInterfaceManager.HideInventoryButton();
        }
    }

    bool backingToMap;
    public void StartBackingToMap()
    {
        if (!backingToMap)
        {
            backingToMap = true;
            loadingScreenManager.StartEndLoad(GoBackToMap);
        }
    }
    public void GoBackToMap()
    {
        /*if (backingToMap)
            return;*/

        if (IntersceneManager.intersceneManager == null)
        {
            Debug.LogWarning("Attention : il n'y a pas d'IntersceneManager actuellement, impossible de retourner sur la map");
        }

        string menuSceneName = IntersceneManager.intersceneManager.MapInterscInformations.GetMapSceneName;

        try
        {
            PlayerEquipmentsDatas equipmentsData = PlayerDataSaver.LoadPlayerEquipmentsDatas();
            if (equipmentsData == null)
            {
                equipmentsData = new PlayerEquipmentsDatas();

                List<ShipEquipment> playerInventoryEquipments = new List<ShipEquipment>();
                if (equipmentLootExchangeManager.HadToSort)
                {
                    foreach (ShipEquipment equip in equipmentLootExchangeManager.GetPlayerModifiedLoot)
                        playerInventoryEquipments.Add(equip);
                }
                else
                {
                    foreach (ShipEquipment equip in player.PlayerLootManager.GetAllLootedEquipments)
                        playerInventoryEquipments.Add(equip);
                }

                equipmentsData.SetPlayerEquipmentsData(new EquipmentsSet(), playerInventoryEquipments, player.LfManager.GetCurrentArmorAmount, IntersceneManager.intersceneManager.GetPlayerDatas.GetPlayerGoldAmount);
            }
            else
            {
                equipmentsData.SetPlayerArmorAmount(player.LfManager.GetCurrentArmorAmount);
                equipmentsData.SetPlayerGoldAmount(IntersceneManager.intersceneManager.GetPlayerDatas.GetPlayerGoldAmount);

                List<ShipEquipment> playerInventoryEquipments = new List<ShipEquipment>();
                if (equipmentLootExchangeManager.HadToSort)
                {
                    foreach (ShipEquipment equip in equipmentLootExchangeManager.GetPlayerModifiedInventory)
                        playerInventoryEquipments.Add(equip);

                    foreach (ShipEquipment equip in equipmentLootExchangeManager.GetPlayerModifiedLoot)
                        playerInventoryEquipments.Add(equip);
                }
                else
                {
                    playerInventoryEquipments = equipmentsData.GetPlayerEquipmentsInventory;
                    foreach (ShipEquipment equip in player.PlayerLootManager.GetAllLootedEquipments)
                        playerInventoryEquipments.Add(equip);
                }

                equipmentsData.SetPlayerInventory(playerInventoryEquipments);
            }

            PlayerDataSaver.SavePlayerEquipmentsDatas(equipmentsData);

            SceneManager.LoadSceneAsync(menuSceneName);
            //backingToMap = true;
        }
        catch
        {
            Debug.LogWarning("Attention : le nom de scène n'a pas été assigné sur l'interscene manager, ou la scene n'existe pas (ou n'as pas été buildée)");
        }
    }

    bool restarting;
    public void StartRestarting()
    {
        if (!restarting)
        {
            restarting = true;
            loadingScreenManager.StartEndLoad(Restart);
        }
    }
    public void Restart()
    {
        /*if (!restarting)
        {*/
            PlayerEquipmentsDatas equipmentsData = PlayerDataSaver.LoadPlayerEquipmentsDatas();
            if (equipmentsData == null)
            {
                equipmentsData = new PlayerEquipmentsDatas();
                equipmentsData.SetPlayerEquipmentsData(new EquipmentsSet(), new List<ShipEquipment>(), player.LfManager.GetCurrentArmorAmount, IntersceneManager.intersceneManager.GetPlayerDatas.GetPlayerGoldAmount);
            }
            else
            {
                equipmentsData.SetPlayerArmorAmount(player.LfManager.GetCurrentArmorAmount);
                equipmentsData.SetPlayerGoldAmount(IntersceneManager.intersceneManager.GetPlayerDatas.GetPlayerGoldAmount);
            }
            IntersceneManager.intersceneManager.GetPlayerDatas.SetPlayerCurrentArmorValue(player.LfManager.GetCurrentArmorAmount);

            PlayerDataSaver.SavePlayerEquipmentsDatas(equipmentsData);

            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
            //restarting = true;
        //}
    }

    private void Update()
    {
        screenshakeManager.UpdateScreenshake();
        slowMoManager.UpdateSlowMoManagement();
        playerHighlightCameraManagement.UpdateManagement();
        cinematicManager.UpdateCinematicPart();
        outMapManager.UpdateOutMapManagement();

        loadingScreenManager.UpdateLoadingScreen();

        if (arenaInterfaceManager.OpeningCrates)
            arenaInterfaceManager.UpdateCratesOpening();

        if (arenaInterfaceManager.WaitingToOpenAfterCrateInterface)
            arenaInterfaceManager.UpdateWaitToOpenAfterCrateInterface();
    }

    #region Party Management
    public void StartFight()
    {
        arenaInterfaceManager.HideBeginPanel();
        ArenaManager.arenaManager.StartGame();
    }

    public bool StartedFight
    {
        get
        {
            if (ArenaManager.arenaManager != null)
                return ArenaManager.arenaManager.StartedFight;
            else
                return true;
        }
    }

    public bool Won
    {
        get
        {
            if (ArenaManager.arenaManager != null)
                return ArenaManager.arenaManager.Won;
            else
                return true;
        }
    }

    public void Lose()
    {
        if (ArenaManager.arenaManager != null)
            arenaInterfaceManager.PlayDeathCinematic(ArenaManager.arenaManager.IsTutorialArena);
        else
            arenaInterfaceManager.PlayDeathCinematic(false);

        HidePlayerInterface();
    }

    public bool Lost
    {
        get
        {
            return player.Lost;
        }
    }

    [Header("Other")]
    [SerializeField] EquipmentLootExchangeManager equipmentLootExchangeManager;
    public EquipmentLootExchangeManager EquipLootExchangeManager { get { return equipmentLootExchangeManager; } }
    public void OpenEquipLootExchangeManager()
    {
        //arenaInterfaceManager.HideEndPanel();
        equipmentLootExchangeManager.OpenLootExchangePanel(player.PlayerLootManager.GetAllLootedEquipments);
    }

    [SerializeField] OutMapManager outMapManager;
    public OutMapManager OutMapMng { get { return outMapManager; } }
    #endregion

    [SerializeField] ConfirmationPanel confirmationPanel;
    public ConfirmationPanel ConfirmPanel { get { return confirmationPanel; } }

    #region Display Management
    public void ShowPlayerInterface()
    {
        playerInterface.ShowPlayerInterface();
    }

    public void HidePlayerInterface()
    {
        playerInterface.HidePlayerInterface();
    }

    public void EnableMainVirtualCamera()
    {
        mainVirtualCamera.gameObject.SetActive(true);
    }

    public void DisableMainVirtualCamera()
    {
        mainVirtualCamera.gameObject.SetActive(false);
    }
    #endregion

    [Header("Pooling")]
    [SerializeField] PoolingManager poolingManager;
    public PoolingManager PoolManager
    {
        get
        {
            return poolingManager;
        }
    }

    #region Cheats
    [Header("Cheats")]
    [SerializeField] GameButton giveMoneyButton;
    public void EarnLotOfMoney()
    {
        IntersceneManager.intersceneManager.GetPlayerDatas.EarnMoney(1000);
        MapManager.mapManager.DocksManager.UpdateGoldText();
    }

    public void SetUpCheats()
    {
        giveMoneyButton.Interaction = EarnLotOfMoney;
    }
    #endregion

    #region Debug
    [Header("Debug")]
    [SerializeField] Text debugText;
    public void ShowDebugText(string messageToShow)
    {
        debugText.text = messageToShow;
    }
    #endregion
}

/// <summary>
/// Type d'appareil actuellement utilisé
/// </summary>
public enum DeviceType
{
    PC, Mobile
}