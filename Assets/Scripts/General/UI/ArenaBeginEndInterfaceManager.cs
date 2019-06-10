using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ArenaBeginEndInterfaceManager
{
    ScoreManager scoreManager;
    LootManager playerLootManager;
    public void SetUp(ScoreManager scManager)
    {
        scoreManager = scManager;
        mainCamera = GameManager.gameManager.MainCamera;

        SetUpStarsAndGoldPanel();
        SetUpLosePanel();
        SetUpLootingPanel();

        playerLootManager = GameManager.gameManager.Player.PlayerLootManager;
    }

    #region Begin Interface
    [Header("Begin Interface")]
    [SerializeField] GameObject beginPanel;
    [SerializeField] Text timeLimitScoreText;
    [SerializeField] Text damageLimitScoreText;

    public void ShowBeginPanel(bool isTutorial)
    {
        SetUp(ArenaManager.arenaManager.ScoreMng);

        beginPanel.SetActive(true);

        ArenaParameters arenaParameters = scoreManager.GetCurrentArenaParameters;

        if (arenaParameters != null)
        {
            float totalTimeInSeconds = scoreManager.GetCurrentArenaParameters.GetMaximumArenaTimeToHaveStar;
            float seconds = totalTimeInSeconds % 60;
            float minutes = (int)(totalTimeInSeconds / 60);

            //timeLimitScoreText.text = "Finish level in less than " + minutes + "'" + (seconds < 10 ? ("0" + seconds.ToString()) : seconds.ToString());
            timeLimitScoreText.text = "Finish the level without hitting any obstacle";
            damageLimitScoreText.text = "Take less than " + scoreManager.GetCurrentArenaParameters.GetMaximumNumberOfDamagesToHaveStar + " damages";
        }
    }

    public void HideBeginPanel()
    {
        beginPanel.SetActive(false);
    }
    #endregion

    #region End Interface V1
    /*[Header("End Interface")]
    [SerializeField] GameObject endPanel;
    [SerializeField] GameObject winPanelInventoryNotFull;
    [SerializeField] GameObject winPanelInventoryFull;
    [SerializeField] GameObject losePanelButtons;

    [Header("End Interface : Loots")]
    [SerializeField] Text wonGoldText;
    [SerializeField] Image[] lootedEquipmentImages;
    [SerializeField] float lootedEquipmentImagesSpacing;
    [SerializeField] Text tooMuchEquipmentText;*/

    /*[Header("End Interface : Scores")]
    [SerializeField] GameObject scorePanel;
    [SerializeField] Text timeLimitScoreEndText;
    [SerializeField] Image timeLimitScoreEndStar;
    [SerializeField] Text damageLimitScoreEndText;
    [SerializeField] Image damageLimitScoreEndStar;*/

    /*public void OpenEndPanel(ShowEndPanelType endPanelType)
    {
        GameManager.gameManager.PlrInterface.HidePlayerInterface();

        //LootManager playerLootManager = GameManager.gameManager.Player.PlayerLootManager;

        if (endPanelType == ShowEndPanelType.Lose)
            playerLootManager.LoseLoot();

        PlayerEquipmentsDatas playerEquipmentsData = PlayerDataSaver.LoadPlayerEquipmentsDatas();

        endPanel.SetActive(true);

        wonGoldText.text = playerLootManager.GetAllLootedGold + " Gold";

        List<ShipEquipment> modifiedLoot = GameManager.gameManager.EquipLootExchangeManager.GetPlayerModifiedLoot;
        if (modifiedLoot == null)
            modifiedLoot = new List<ShipEquipment>();
        int numberOfLootedEquipments = endPanelType == ShowEndPanelType.Win ? playerLootManager.GetAllLootedEquipments.Count : (endPanelType == ShowEndPanelType.AfterSort ? modifiedLoot.Count : 0);

        #region Equipment loot
        float basePosition = 0;
        basePosition -= (lootedEquipmentImagesSpacing * (numberOfLootedEquipments - 1)) / 2;

        int i = 0;
        for (i = 0; i < numberOfLootedEquipments; i++)
        {
            lootedEquipmentImages[i].gameObject.SetActive(true);
            lootedEquipmentImages[i].transform.localPosition = new Vector3(basePosition + (lootedEquipmentImagesSpacing * i), 0, 0);
            if (endPanelType == ShowEndPanelType.Win)
                lootedEquipmentImages[i].sprite = playerLootManager.GetAllLootedEquipments[i].GetEquipmentInformations.GetEquipmentIcon;
            else if (endPanelType == ShowEndPanelType.AfterSort)
                lootedEquipmentImages[i].sprite = modifiedLoot[i].GetEquipmentInformations.GetEquipmentIcon;
        }

        while (i < lootedEquipmentImages.Length)
        {
            lootedEquipmentImages[i].gameObject.SetActive(false);
            i++;
        }
        #endregion

        switch (endPanelType)
        {
            case (ShowEndPanelType.Win):
                if (playerLootManager.GetAllLootedEquipments.Count == 0)
                {
                    winPanelInventoryNotFull.SetActive(true);
                }
                else
                {
                    int numberOfTooMuchEquipments = playerLootManager.GetAllLootedEquipments.Count + playerEquipmentsData.GetPlayerEquipmentsInventory.Count - IntersceneManager.intersceneManager.GetPlayerDatas.GetPlayerInventoryCapacity;
                    if (numberOfTooMuchEquipments > 0)
                    {
                        winPanelInventoryFull.SetActive(true);
                        tooMuchEquipmentText.text =  "Vous avez récupéré trop d'équipements pour votre inventaire. Vous devez en jeter " + numberOfTooMuchEquipments + ".";
                    }
                    else
                    {
                        winPanelInventoryNotFull.SetActive(true);
                    }
                }

                break;

            case (ShowEndPanelType.Lose):
                losePanelButtons.SetActive(true);
                break;

            case (ShowEndPanelType.AfterSort):
                winPanelInventoryFull.SetActive(false);
                winPanelInventoryNotFull.SetActive(true);
                break;
        }
    }

    public void HideEndPanel()
    {
        endPanel.SetActive(false);
    }*/
    #endregion

    #region End Interface V2
    #region Lose Panel
    [Header("End Interface : Lose Panel")]
    [SerializeField] IntroControler playerDeathCinematic;
    [SerializeField] GameObject losePanel;
    [SerializeField] GameObject loseTutorialPanel;
    [SerializeField] GameButton loseRetryButton;
    [SerializeField] GameButton loseTutoRetryButton;
    [SerializeField] GameButton loseMapButton;
    [SerializeField] Text loseGoldText;

    public void SetUpLosePanel()
    {
        loseRetryButton.Interaction = GameManager.gameManager.StartRestarting;
        loseMapButton.Interaction = GameManager.gameManager.StartBackingToMap;
        loseTutoRetryButton.Interaction = GameManager.gameManager.StartRestarting;
    }

    public void PlayDeathCinematic(bool isTutorial)
    {
        if (isTutorial)
            playerDeathCinematic.PlayCinematic(OpenLosePanelTutorial, true);
        else
            playerDeathCinematic.PlayCinematic(OpenLosePanelNotTutorial, true);
    }

    public void OpenLosePanelTutorial() { OpenLosePanel(true); }
    public void OpenLosePanelNotTutorial() { OpenLosePanel(false); }
    public void OpenLosePanel(bool isTutorial)
    {
        if (isTutorial)
        {
            loseTutorialPanel.gameObject.SetActive(true);
            playerLootManager.LoseLoot();
        }
        else
        {
            losePanel.gameObject.SetActive(true);

            playerLootManager.LoseLoot();

            loseGoldText.text = "All you could save was " + playerLootManager.GetAllLootedGold + " gold... Better than nothing, I guess";
        }
        /*losePanel.gameObject.SetActive(true);

        playerLootManager.LoseLoot();

        loseGoldText.text = "All you could save was " + playerLootManager.GetAllLootedGold + " gold... Better than nothing, I guess";*/
    }
    #endregion

    #region Score and Global Loot
    [Header("End Interface : Score and Global Loot")]
    [SerializeField] GameObject starsAndGoldPanel;
    [SerializeField] GameButton openLootButton;
    #region Score
    //[SerializeField] Image[] firstPanelStarsImages;
    [SerializeField] Image firstPanelPassedStar;
    [SerializeField] Image firstPanelTimeLimitStar;
    [SerializeField] Image firstPanelDamageLimitStar;
    [SerializeField] Text timeLimitScoreEndText;
    [SerializeField] Image timeLimitScoreEndStar;
    [SerializeField] Text damageLimitScoreEndText;
    [SerializeField] Image damageLimitScoreEndStar;
    #endregion
    #region Global loot
    [SerializeField] Text firstPanelGoldAmountText;
    [SerializeField] Text firstPanelEquipmentsAmountText;
    #endregion
    #region Tutorial
    [SerializeField] GameObject winTutorialPanel;
    [SerializeField] GameButton openTutorialLootButton;
    [SerializeField] Text firstPanelTutorialGoldAmountText;
    [SerializeField] Text firstPanelTutorialEquipmentsAmountText;
    #endregion

    public void SetUpStarsAndGoldPanel()
    {
        openLootButton.Interaction = CloseStarsAndGoldPanel;
        openLootButton.Interaction += OpenLootingPanelFirstTime;

        openTutorialLootButton.Interaction = CloseStarsAndGoldPanel;
        openTutorialLootButton.Interaction += OpenLootingPanelFirstTime;
    }

    public void OpenStarsAndGlobalLootPanel(bool isTutorial)
    {
        if (isTutorial)
        {
            winTutorialPanel.SetActive(true);

            firstPanelTutorialGoldAmountText.text = playerLootManager.GetAllLootedGold.ToString();
            firstPanelTutorialEquipmentsAmountText.text = "x " + playerLootManager.GetAllLootedEquipments.Count;
        }
        else
        {
            starsAndGoldPanel.SetActive(true);

            #region Score
            int starsNumber = 1;

            timeLimitScoreEndText.text = timeLimitScoreText.text;
            damageLimitScoreEndText.text = damageLimitScoreText.text;

            firstPanelPassedStar.gameObject.SetActive(true);
            if (/*scoreManager.HasTimeStar*/scoreManager.HasNoObstacleHitStar)
            {
                firstPanelTimeLimitStar.gameObject.SetActive(true);
                timeLimitScoreEndStar.gameObject.SetActive(true);
                starsNumber++;
            }
            else
            {
                firstPanelTimeLimitStar.gameObject.SetActive(false);
                timeLimitScoreEndStar.gameObject.SetActive(false);
            }

            if (scoreManager.HasDamagesStar)
            {
                firstPanelDamageLimitStar.gameObject.SetActive(true);
                damageLimitScoreEndStar.gameObject.SetActive(true);
                starsNumber++;
            }
            else
            {
                firstPanelDamageLimitStar.gameObject.SetActive(false);
                damageLimitScoreEndStar.gameObject.SetActive(false);
            }

            /*for (int i = 0; i < starsNumber; i++)
            {
                firstPanelStarsImages[i].gameObject.SetActive(true);
            }

            for (int i = starsNumber; i < firstPanelStarsImages.Length; i++)
            {
                firstPanelStarsImages[i].gameObject.SetActive(false);
            }*/
            #endregion

            #region Global Loot
            //LootManager playerLootManager = GameManager.gameManager.Player.PlayerLootManager;

            firstPanelGoldAmountText.text = playerLootManager.GetAllLootedGold.ToString();
            firstPanelEquipmentsAmountText.text = "x " + playerLootManager.GetAllLootedEquipments.Count;
            #endregion
        }
    }

    public void CloseStarsAndGoldPanel()
    {
        starsAndGoldPanel.SetActive(false);
        winTutorialPanel.SetActive(false);
    }
    #endregion

    #region Looting
    [Header("End Interface : Looting")]
    [SerializeField] GameObject lootingPanel;
    [SerializeField] GameObject cratesOpeningObject;
    [SerializeField] Transform cratesParent;
    [SerializeField] EndArenaOpenableCrate[] openableCrates;
    [SerializeField] float cratesSpacing;
    [SerializeField] float waitTimeBeforeTwoCrates;
    [SerializeField] float waitTimeAfterAllCrates;
    float remainingWaitTimeAfterAllCrates;
    [SerializeField] Transform cratesStartPosition;
    [SerializeField] Transform cratesEndPosition;
    AnimationCurve cratesEndMoveCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] GameButton continueAfterLootButton;
    [SerializeField] GameButton tooMuchLootButton;
    [SerializeField] GameObject afterLootFinishedPanel;
    [SerializeField] Image[] afterLootFinishedStars;
    [SerializeField] Text afterLootFinishedGoldAmount;
    Camera mainCamera;
    List<EndArenaOpenableCrate> cratesToOpen;
    List<EndArenaOpenableCrate> openedCrates;

    public void SetUpLootingPanel()
    {
        tooMuchLootButton.Interaction = GameManager.gameManager.OpenEquipLootExchangeManager;
        tooMuchLootButton.Interaction += CloseLootingPanel;
        continueAfterLootButton.Interaction = GameManager.gameManager.StartBackingToMap;
    }

    public void SetUpLootingCrates(List<ShipEquipment> lootedEquipments)
    {
        int numberOfLootedEquipments = lootedEquipments.Count;

        cratesOpeningObject.SetActive(true);

        if (numberOfLootedEquipments == 0)
        {
            for (int i = lootedEquipments.Count; i < openableCrates.Length; i++)
                openableCrates[i].gameObject.SetActive(false);

            OpenReadyToContinuePanel();
        }
        else
        {
            float basePosition = 0;
            basePosition -= (cratesSpacing * (numberOfLootedEquipments - 1)) / 2;

            cratesToOpen = new List<EndArenaOpenableCrate>();
            openedCrates = new List<EndArenaOpenableCrate>();

            int numberOfEquipments = lootedEquipments.Count;
            for (int i = 0; i < numberOfEquipments; i++)
            {
                if (i >= openableCrates.Length || i >= lootedEquipments.Count)
                    break;

                openableCrates[i].gameObject.SetActive(true);
                openableCrates[i].transform.localPosition = new Vector3(basePosition + (cratesSpacing * i), 0, 0);
                openableCrates[i].SetUpCrate(lootedEquipments[i], mainCamera.transform, i * waitTimeBeforeTwoCrates);
                openableCrates[i].OnCrateOpened = RemoveCrateToOpen;

                cratesToOpen.Add(openableCrates[i]);
            }

            for (int i = lootedEquipments.Count; i < openableCrates.Length; i++)
                openableCrates[i].gameObject.SetActive(false);

            afterLootFinishedPanel.SetActive(false);
            tooMuchLootButton.gameObject.SetActive(false);
        }
    }

    public bool OpeningCrates
    {
        get
        {
            if (cratesToOpen == null)
                return false;
            else
                return cratesToOpen.Count != 0;
        }
    }

    EndArenaOpenableCrate currentlyTouchedCrate;
    EndArenaOpenableCrate currentlyMaintainedCrate;
    int currentlyTouchedCrateFingerId = -1;
    public void UpdateCratesOpening()
    {
        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                if (currentlyTouchedCrate == null)
                {
                    currentlyTouchedCrate = CheckForTouchedCrate(touch.position);
                    currentlyMaintainedCrate = currentlyTouchedCrate;
                    if (currentlyTouchedCrate != null)
                    {
                        currentlyTouchedCrate.StartCrateHighlight();
                        currentlyTouchedCrateFingerId = touch.fingerId;
                    }
                }
            }
            else if (touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Moved)
            {
                if (touch.fingerId == currentlyTouchedCrateFingerId)
                {
                    EndArenaOpenableCrate newTouchedCrate = CheckForTouchedCrate(touch.position);
                    if (currentlyTouchedCrate == currentlyMaintainedCrate)
                    {
                        if (currentlyTouchedCrate != newTouchedCrate)
                        {
                            currentlyMaintainedCrate = newTouchedCrate;
                            currentlyTouchedCrate.EndCrateHighlight();
                        }
                    }
                    else
                    {
                        if (currentlyTouchedCrate == newTouchedCrate)
                        {
                            currentlyMaintainedCrate = currentlyTouchedCrate;
                            currentlyTouchedCrate.StartCrateHighlight();
                        }
                    }
                }
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                if (touch.fingerId == currentlyTouchedCrateFingerId)
                {
                    if (currentlyTouchedCrate == currentlyMaintainedCrate)
                    {
                        currentlyTouchedCrate.OpenCrate();
                    }
                    currentlyTouchedCrateFingerId = -1;
                    currentlyTouchedCrate = null;
                    currentlyMaintainedCrate = null;
                }
            }
        }        
    }

    public EndArenaOpenableCrate CheckForTouchedCrate(Vector3 screenPosition)
    {
        RaycastHit hit = new RaycastHit();
        Ray targetRay = mainCamera.ScreenPointToRay(screenPosition);
        if (Physics.Raycast(targetRay, out hit))
        {
            EndArenaOpenableCrate hitCrate = hit.collider.GetComponent<EndArenaOpenableCrate>();
            if (hitCrate != null)
            {
                return hitCrate;
            }
        }
        return null;
    }

    public void RemoveCrateToOpen(EndArenaOpenableCrate openedCrate)
    {
        cratesToOpen.Remove(openedCrate);
        openedCrates.Add(openedCrate);

        if (cratesToOpen.Count == 0)
        {
            remainingWaitTimeAfterAllCrates = waitTimeAfterAllCrates;
        }
            //OpenAfterCrateOpeningInterface();
    }

    public bool WaitingToOpenAfterCrateInterface { get { return remainingWaitTimeAfterAllCrates != 0; } }
    public void UpdateWaitToOpenAfterCrateInterface()
    {
        if (remainingWaitTimeAfterAllCrates > 0)
        {
            cratesParent.transform.localPosition = Vector3.Lerp(cratesStartPosition.localPosition, cratesEndPosition.localPosition, cratesEndMoveCurve.Evaluate(1 - (remainingWaitTimeAfterAllCrates/waitTimeAfterAllCrates)));
            remainingWaitTimeAfterAllCrates -= Time.deltaTime;
        }
        else if (remainingWaitTimeAfterAllCrates < 0)
        {
            remainingWaitTimeAfterAllCrates = 0;
            OpenAfterCrateOpeningInterface();
        }
    }

    public void OpenAfterCrateOpeningInterface()
    {
        PlayerEquipmentsDatas playerEquipmentsData = PlayerDataSaver.LoadPlayerEquipmentsDatas();

        //cratesParent.position = Vector3.SmoothDamp(cratesParent.position, cratesPositionWhenAllOpened.position, ref Vector3.zero, 1.5f);

        if (IntersceneManager.intersceneManager != null)
        {
            int numberOfTooMuchEquipments = playerLootManager.GetAllLootedEquipments.Count + playerEquipmentsData.GetPlayerEquipmentsInventory.Count - IntersceneManager.intersceneManager.GetPlayerDatas.GetPlayerInventoryCapacity;
            if (numberOfTooMuchEquipments > 0)
            {
                tooMuchLootButton.gameObject.SetActive(true);
                afterLootFinishedPanel.SetActive(false);
                tooMuchLootButton.SetButtonLabel("There's too much for us to carry. We have to throw " + numberOfTooMuchEquipments + " equipments.");
            }
            else
            {
                OpenReadyToContinuePanel();
            }
        }
        else
        {
            OpenReadyToContinuePanel();
        }
    }

    public void OpenReadyToContinuePanel()
    {
        tooMuchLootButton.gameObject.SetActive(false);
        afterLootFinishedPanel.SetActive(true);

        int starsNumber = 1;
        if (/*scoreManager.HasTimeStar*/scoreManager.HasNoObstacleHitStar)
            starsNumber++;
        if (scoreManager.HasDamagesStar)
            starsNumber++;

        for (int i = 0; i < starsNumber; i++)
            afterLootFinishedStars[i].gameObject.SetActive(true);

        for (int i = starsNumber; i < afterLootFinishedStars.Length; i++)
            afterLootFinishedStars[i].gameObject.SetActive(false);

        afterLootFinishedGoldAmount.text = playerLootManager.GetAllLootedGold.ToString();
    }

    public void ChangeLootingCratesAfterSort(List<ShipEquipment> conservedEquipments)
    {
        List<EndArenaOpenableCrate> newOpenedCrates = new List<EndArenaOpenableCrate>();
        foreach(EndArenaOpenableCrate openedCrate in openedCrates)
        {
            if (!conservedEquipments.Contains(openedCrate.GetContainedEquipment))
                openedCrate.gameObject.SetActive(false);
            else
                newOpenedCrates.Add(openedCrate);

        }

        openedCrates = newOpenedCrates;

        int numberOfLootedEquipments = newOpenedCrates.Count;
        float basePosition = 0;
        basePosition -= (cratesSpacing * (numberOfLootedEquipments - 1)) / 2;

        cratesToOpen = new List<EndArenaOpenableCrate>();
        openedCrates = new List<EndArenaOpenableCrate>();

        for (int i = 0; i < newOpenedCrates.Count; i++)
        {
            newOpenedCrates[i].transform.localPosition = new Vector3(basePosition + (cratesSpacing * i), 0, 0);
        }

        OpenReadyToContinuePanel();
    }

    public void OpenLootingPanelFirstTime() { OpenLootingPanel(false); }
    public void OpenLootingPanelAfterSort() { OpenLootingPanel(true); }
    public void OpenLootingPanel(bool alreadySorted)
    {
        lootingPanel.SetActive(true);

        List<ShipEquipment> modifiedLoot = GameManager.gameManager.EquipLootExchangeManager.GetPlayerModifiedLoot;

        if (modifiedLoot == null)
            modifiedLoot = new List<ShipEquipment>();

        int numberOfLootedEquipments = alreadySorted ? modifiedLoot.Count : playerLootManager.GetAllLootedEquipments.Count;
        if (alreadySorted)
        {
            ChangeLootingCratesAfterSort(modifiedLoot);
        }
        else
        {
            SetUpLootingCrates(playerLootManager.GetAllLootedEquipments);
            /*float basePosition = 0;
            basePosition -= (lootedEquipmentImagesSpacing * (numberOfLootedEquipments - 1)) / 2;

            SetUpLootingCrates(playerLootManager.GetAllLootedEquipments);

            int i = 0;
            for (i = 0; i < numberOfLootedEquipments; i++)
            {
                lootedEquipmentImages[i].gameObject.SetActive(true);
                lootedEquipmentImages[i].transform.localPosition = new Vector3(basePosition + (lootedEquipmentImagesSpacing * i), 0, 0);
            }

            while (i < lootedEquipmentImages.Length)
            {
                lootedEquipmentImages[i].gameObject.SetActive(false);
                i++;
            }*/
        }
    }

    public void CloseLootingPanel()
    {
        lootingPanel.SetActive(false);
    }
    #endregion
    #endregion
}

public enum ShowEndPanelType
{
    Win, Lose, AfterSort
}
