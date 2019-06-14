using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MainMenuManager : MonoBehaviour
{
    bool passedTutorial;

    private void Awake()
    {
        SetUp();
    }

    private void Update()
    {
        loadingScreenManager.UpdateLoadingScreen();
    }

    public void SetUp()
    {
        Application.targetFrameRate = 60;
        Screen.orientation = ScreenOrientation.Landscape;

        CheckForIntersceneManager();
        CheckAllGameEquipmentsManager();
        CheckForSaveFiles();

        SetUpMainButtons();
        SetUpOptionsButtons();
        SetUpExtrasButtons();
    }

    #region Main Panel
    [Header("Main Panel")]
    [SerializeField] GameObject menuMainPanel;
    [SerializeField] Image gameIcon;
    [SerializeField] GameButton launchGameButton;
    [SerializeField] GameButton optionsButton;
    [SerializeField] GameButton extrasButton;
    [SerializeField] GameButton quitButton;

    public void OpenMainPanel()
    {
        menuMainPanel.SetActive(true);
    }

    public void CloseMainPanel()
    {
        menuMainPanel.SetActive(false);
    }

    public void SetUpMainButtons()
    {
        launchGameButton.Interaction = StartLaunchGame;

        optionsButton.Interaction = OpenOptionsPanel;
        optionsButton.Interaction += CloseMainPanel;

        extrasButton.Interaction = OpenExtrasPanel;
        extrasButton.Interaction += CloseMainPanel;

        quitButton.Interaction = AskQuitGame;
    }

    #region Button Methods
    bool launchingGame;
    public void StartLaunchGame()
    {
        loadingScreenManager.StartEndLoad(LaunchGame);
    }

    public void LaunchGame()
    {
        if (launchingGame)
            return;

        if (passedTutorial)
        {
            SceneManager.LoadScene(mapSceneName);
        }
        else
        {
            IntersceneManager.intersceneManager.ArenaInterscInformations.SetNeedToPassTutorial(true);
            SceneManager.LoadScene(tutorialSceneName);
        }
    }

    public void AskQuitGame()
    {
        confirmationPanel.OpenConfirmationPanel();
        confirmationPanel.Ask(QuitGame);
    }

    bool quitting;
    public void QuitGame()
    {
        if (quitting)
            return;
        quitting = true;

        #if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    #endregion
    #endregion

    #region Options Panel
    [Header("Options Panel")]
    [SerializeField] GameObject optionsPanel;
    [SerializeField] GameButton soundOnOffButton;
    [SerializeField] GameButton sideHandedButton;
    [SerializeField] GameButton optionsBackButton;

    public void OpenOptionsPanel()
    {
        optionsPanel.SetActive(true);
    }

    public void CloseOptionsPanel()
    {
        optionsPanel.SetActive(false);
    }

    public void SetUpOptionsButtons()
    {
        #region Saved Parameters
        if (PlayerPrefs.HasKey("SoundOff"))
        {
            if (PlayerPrefs.GetInt("SoundOff") == 1)
                ChangeSoundState();
        }
        else
            PlayerPrefs.SetInt("SoundOff", 0);

        if (PlayerPrefs.HasKey("HandSide"))
        {
            SetPlayerHandedType((HandedType)PlayerPrefs.GetInt("HandSide"));
        }
        else
            PlayerPrefs.SetInt("HandSide", 0);
        #endregion

        soundOnOffButton.Interaction = ChangeSoundState;
        sideHandedButton.Interaction = ChangePlayerHandedType;

        optionsBackButton.Interaction = CloseOptionsPanel;
        optionsBackButton.Interaction += OpenMainPanel;
    }

    #region Button Methods
    bool soundOff;

    public void ChangeSoundState()
    {
        if (soundOff)
        {
            soundOff = false;
            AudioListener.volume = 1f;
            soundOnOffButton.SetButtonLabel("Volume : On");
        }
        else
        {
            soundOff = true;
            AudioListener.volume = 0f;
            soundOnOffButton.SetButtonLabel("Volume : Off");
        }

        PlayerPrefs.SetInt("SoundOff", soundOff ? 1 : 0);
    }

    HandedType playerHandedType;

    public void SetPlayerHandedType(HandedType handedType)
    {
        playerHandedType = handedType;

        sideHandedButton.SetButtonLabel((handedType == HandedType.LeftHanded ? "Left-Handed" : "Right-Handed"));

        PlayerPrefs.SetInt("HandSide", (int)playerHandedType);
    }

    public void ChangePlayerHandedType()
    {
        if (playerHandedType == HandedType.LeftHanded)
            SetPlayerHandedType(HandedType.RightHanded);
        else if (playerHandedType == HandedType.RightHanded)
            SetPlayerHandedType(HandedType.LeftHanded);
    }
    #endregion
    #endregion

    #region Extras Panel
    [Header("Extras Panel")]
    [SerializeField] GameObject extrasPanel;
    [SerializeField] GameButton creditsButton;
    [SerializeField] GameButton reinitializeButton;
    [SerializeField] GameButton skipTutorialButton;
    [SerializeField] GameButton extrasBackButton;

    public void OpenExtrasPanel()
    {
        extrasPanel.SetActive(true);
        gameIcon.enabled = false;
    }

    public void CloseExtrasPanel()
    {
        extrasPanel.SetActive(false);
        gameIcon.enabled = true;
    }

    public void SetUpExtrasButtons()
    {
        creditsButton.Interaction = OpenCreditsPanel;
        creditsButton.Interaction += CloseExtrasPanel;

        reinitializeButton.Interaction = AskReinitialize;

        extrasBackButton.Interaction = CloseExtrasPanel;
        extrasBackButton.Interaction += OpenMainPanel;

        creditsBackButton.Interaction = CloseCreditsPanel;
        creditsBackButton.Interaction += OpenExtrasPanel;

        if (skipTutorialButton != null)
        {
            skipTutorialButton.Interaction = AskSkipTutorial;
        }
    }
    #region Button Methods
    public void AskReinitialize()
    {
        confirmationPanel.OpenConfirmationPanel();
        confirmationPanel.Ask(Reinitialize);
    }

    public void Reinitialize()
    {
        IntersceneManager intersceneManager = IntersceneManager.intersceneManager;
        PlayerDataSaver.ErasePlayerDatas();
        intersceneManager.GetPlayerDatas.Reinitialize();
        intersceneManager.ArenaInterscInformations.Reinitialize();
        intersceneManager.ArenaInterscInformations.SetNeedToPassTutorial(true);
        intersceneManager.MapInterscInformations.Reinitialize();
        intersceneManager.MapInterscInformations.SetMapIntersceneInfos(mapSceneName, Vector3.zero, null);
        passedTutorial = false;
        CheckForSaveFiles();
    }

    public void AskSkipTutorial()
    {
        confirmationPanel.OpenConfirmationPanel();
        confirmationPanel.Ask(SkipTutorial);
    }

    public void SkipTutorial()
    {
        PlayerProgressionDatas data = PlayerDataSaver.LoadProgressionDatas();
        if(data != null)
        {
            data.SetPassedTutorial();
            PlayerDataSaver.SavePlayerProgressionDatas(data);
            passedTutorial = true;
        }
    }
    #endregion
    #endregion

    #region Confirmation Panel
    [Header("Confirmation Panel")]
    [SerializeField] ConfirmationPanel confirmationPanel;
    #endregion

    #region Credits 
    [Header("Credits")]
    [SerializeField] GameObject creditsPanel;
    [SerializeField] GameButton creditsBackButton;

    public void OpenCreditsPanel()
    {
        creditsPanel.SetActive(true);
    }

    public void CloseCreditsPanel()
    {
        creditsPanel.SetActive(false);
    }
    #endregion

    #region Save and Gameplay
    [Header("Save and Gameplay")]
    [SerializeField] IntersceneManager intersceneManagerPrefab;
    [SerializeField] AllGameEquipmentsManager allGameEquipmentsManagerPrefab;
    [SerializeField] EquipmentsSet gameBeginningEquipmentSet;
    [SerializeField] string mapSceneName;
    [SerializeField] string tutorialSceneName;

    public void CheckForIntersceneManager()
    {
        if (IntersceneManager.intersceneManager == null)
        {
            IntersceneManager newIntersceneManager = Instantiate(intersceneManagerPrefab);
            newIntersceneManager.SetUp();
        }

        IntersceneManager.intersceneManager.MapInterscInformations.SetMapIntersceneInfos(mapSceneName, Vector3.zero, null);
    }

    public void CheckAllGameEquipmentsManager()
    {
        if (AllGameEquipmentsManager.manager == null)
        {
            AllGameEquipmentsManager newAllEquipManager = Instantiate(allGameEquipmentsManagerPrefab);
            newAllEquipManager.SetUp();
        }
        else
        {
            loadingScreenManager.StartBeginLoad(null);
        }
    }

    public void CheckForSaveFiles()
    {
        PlayerEquipmentsDatas equipmentsData = PlayerDataSaver.LoadPlayerEquipmentsDatas();

        if(equipmentsData == null)
        {
            Debug.Log("Pas de fichier d'équipements, application des équipements de base");

            equipmentsData = new PlayerEquipmentsDatas();
            equipmentsData.SetPlayerEquipmentsData(gameBeginningEquipmentSet, new List<ShipEquipment>(), 10, 0);

            PlayerDataSaver.SavePlayerEquipmentsDatas(equipmentsData);

            IntersceneManager.intersceneManager.GetPlayerDatas.SetEquipedEquipements(gameBeginningEquipmentSet);
        }
        else
        {
            IntersceneManager.intersceneManager.GetPlayerDatas.SetEquipedEquipements(equipmentsData.GetPlayerEquipmentsSet);
        }

        PlayerProgressionDatas progressionData = PlayerDataSaver.LoadProgressionDatas();

        if (progressionData == null)
        {
            Debug.Log("Pas de fichier de progression");
            PlayerDataSaver.SavePlayerProgressionDatas(new List<PassedArenaData>(), false);
        }
        else
        {
            if (progressionData.GetPassedTuto)
                passedTutorial = true;
        }
    }
    #endregion

    #region Loading
    [Header("Loading")]
    [SerializeField] LoadingScreenManager loadingScreenManager;
    #endregion
}
