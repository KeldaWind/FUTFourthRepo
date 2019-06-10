using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[System.Serializable]
public class PauseInterfaceManager 
{
    [SerializeField] GameButton pauseButton;
    [SerializeField] GameButton openInventoryButton;
    [SerializeField] string mainMenuSceneName;

    public void SetUp()
    {
        pauseButton.Interaction = PauseOrUnpauseGame;

        inGameResumeButton.Interaction = PauseOrUnpauseGame;
        onMapResumeButton.Interaction = PauseOrUnpauseGame;

        inGameRestartButton.Interaction = AskRestart;
        inGameRestartButton.Interaction += CloseMainPanel;

        inGameMapButton.Interaction = AskMap;
        inGameMapButton.Interaction += CloseMainPanel;
        onMapMenuButton.Interaction = AskMainMenu;
        onMapMenuButton.Interaction += CloseMainPanel;
        inGameMenuButton.Interaction = AskMainMenu;
        inGameMenuButton.Interaction += CloseMainPanel;
        inGameMenuButton.gameObject.SetActive(false);

        inGameOptionsButton.Interaction = OpenOptionsPanel;
        inGameOptionsButton.Interaction += CloseMainPanel;
        onMapOptionsButton.Interaction = OpenOptionsPanel;
        onMapOptionsButton.Interaction += CloseMainPanel;

        #region Options
        soundOnOffButton.Interaction = ChangeSoundState;
        sideHandedButton.Interaction = ChangePlayerHandedType;
        playerHandedType = HandedType.RightHanded;

        optionsReturnButton.Interaction = CloseOptionsPanel;
        optionsReturnButton.Interaction += OpenMainPanel;
        #endregion

        #region Saved Parameters
        if (PlayerPrefs.HasKey("SoundOff"))
        {
            if (PlayerPrefs.GetInt("SoundOff") == 1)
                ChangeSoundState();
        }
        else
            PlayerPrefs.SetInt("SoundOff", 0);

        if(PlayerPrefs.HasKey("HandSide"))
        {
            SetPlayerHandedType((HandedType)PlayerPrefs.GetInt("HandSide"));
        }
        else
            PlayerPrefs.SetInt("HandSide", 0);
        #endregion

        confirmationPanel = GameManager.gameManager.ConfirmPanel;
    }

    public void DisableGoBackToMap()
    {
        inGameMapButton.gameObject.SetActive(false);
        inGameMenuButton.gameObject.SetActive(true);
    }

    bool disabledInventoryButton;
    public void PauseOrUnpauseGame()
    {
        if (!GameManager.gameManager.SlwMoManager.Paused)
        {
            GameManager.gameManager.SlwMoManager.PauseGame();
            OpenMainPanel();

            if (openInventoryButton.gameObject.activeInHierarchy)
            {
                openInventoryButton.gameObject.SetActive(false);
                disabledInventoryButton = true;
            }
        }
        else
        {
            GameManager.gameManager.SlwMoManager.UnpauseGame();
            CloseMainPanel();
            confirmationPanel.CloseConfirmationPanel();
            CloseOptionsPanel();

            if (disabledInventoryButton)
            {
                openInventoryButton.gameObject.SetActive(true);
                disabledInventoryButton = false;
            }
        }
    }

    #region Main Panel
    #region In Game
    [Header("Main Panel : In Game")]
    [SerializeField] GameObject inGameMainPanel;
    [SerializeField] GameButton inGameResumeButton;
    [SerializeField] GameButton inGameRestartButton;
    [SerializeField] GameButton inGameMapButton;
    [SerializeField] GameButton inGameMenuButton;
    [SerializeField] GameButton inGameOptionsButton;

    public void OpenMainPanel()
    {
        if (MapManager.mapManager == null)
            inGameMainPanel.SetActive(true);
        else
            onMapMainPanel.SetActive(true);

    }

    public void CloseMainPanel()
    {
        if (MapManager.mapManager == null)
            inGameMainPanel.SetActive(false);
        else
            onMapMainPanel.SetActive(false);
    }
    #endregion

    #region On Map
    [Header("Main Panel : On Map")]
    [SerializeField] GameObject onMapMainPanel;
    [SerializeField] GameButton onMapResumeButton;
    [SerializeField] GameButton onMapMenuButton;
    [SerializeField] GameButton onMapOptionsButton;
    #endregion
    #endregion

    #region Confirmation Panel
    [Header("Confirmation Panel")]
    ConfirmationPanel confirmationPanel;

    public void AskRestart()
    {
        confirmationPanel.OpenConfirmationPanel();
        confirmationPanel.Ask(Restart, OpenMainPanel);
    }
    public void Restart()
    {
        GameManager.gameManager.StartRestarting();
        PauseOrUnpauseGame();
    }

    public void AskMap()
    {
        confirmationPanel.OpenConfirmationPanel();
        confirmationPanel.Ask(GoBackToMap, OpenMainPanel);
    }
    public void GoBackToMap()
    {
        GameManager.gameManager.StartBackingToMap();
        PauseOrUnpauseGame();
    }

    public void AskMainMenu()
    {
        confirmationPanel.OpenConfirmationPanel();
        confirmationPanel.Ask(GoBackToMainMenu, OpenMainPanel);
    }

    public void GoBackToMainMenu()
    {
        GameManager.gameManager.LoadScreenManager.StartEndLoad(TrueGoBackToMenu);
        PauseOrUnpauseGame();
    }

    public void TrueGoBackToMenu()
    {
        SceneManager.LoadSceneAsync(mainMenuSceneName);
    }
    #endregion

    #region Options
    [Header("Options")]
    [SerializeField] GameObject optionsPanel;
    [SerializeField] GameButton soundOnOffButton;
    [SerializeField] GameButton sideHandedButton;
    [SerializeField] GameButton optionsReturnButton;

    public void OpenOptionsPanel()
    {
        optionsPanel.SetActive(true);
    }

    public void CloseOptionsPanel()
    {
        optionsPanel.SetActive(false);
    }

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
        GameManager.gameManager.PlrInterface.SetUpSideHanded(playerHandedType);

        sideHandedButton.SetButtonLabel((handedType == HandedType.LeftHanded ? "Left-Handed" : "Right-Handed"));

        PlayerPrefs.SetInt("HandSide", (int)playerHandedType);
    }

    public void ChangePlayerHandedType()
    {
        if (playerHandedType == HandedType.LeftHanded)
            SetPlayerHandedType(HandedType.RightHanded);
        else if (playerHandedType == HandedType.RightHanded)
            SetPlayerHandedType(HandedType.LeftHanded);
        else
            SetPlayerHandedType(HandedType.LeftHanded);
    }
    #endregion

    public void HideInventoryButton()
    {
        openInventoryButton.gameObject.SetActive(false);
    }
}
