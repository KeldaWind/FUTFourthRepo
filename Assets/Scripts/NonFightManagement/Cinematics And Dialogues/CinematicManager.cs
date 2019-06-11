using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

[System.Serializable]
public class CinematicManager
{
    #region Important References
    [Header("Important References")]
    [SerializeField] CinemachineBrain mainCameraBrain;
    [SerializeField] GameButton skipCinematicButton;
    [SerializeField] CinemachineVirtualCamera skipCamera;
    PlayerInterface playerInterface;
    CinemachineVirtualCamera playerCam;
    #endregion

    IntroControler currentIntroControler;
    CinematicParameters currentCinematicParameters;
    CinematicPart[] currentCinematicAllParts;
    CinemachineVirtualCamera firstCinematicCam;
    CinemachineVirtualCamera lastCinematicCam;
    int currentCinematicPartIndex;
    bool isTypingText;
    float remainingCinematicPartDuration;
    bool stoppedPlayer;
    bool cinematicProcessing;

    public void SetUp()
    {
        playerInterface = GameManager.gameManager.PlrInterface;
        playerCam = GameManager.gameManager.MainVirtualCamera;

        skipCinematicButton.Interaction = SkipCinematicPart;
        skipCinematicButton.gameObject.SetActive(false);

        skipCamera.gameObject.SetActive(false);
    }

    #region Start and End
    bool keepLastCameraAtTheEnd;
    public void StartCinematic(IntroControler controler)
    {
        currentIntroControler = controler;
        currentCinematicParameters = currentIntroControler.GetCinematicParameters;
        currentCinematicAllParts = currentCinematicParameters.GetAllCinematicsParts;
        currentCinematicPartIndex = 0;

        if (currentCinematicAllParts.Length == 0)
        {
            EndCinematic();
            return;
        }

        foreach (CinematicPart part in currentCinematicAllParts)
        {
            if(firstCinematicCam == null)
            {
                if (part.cam != null)
                {
                    firstCinematicCam = part.cam;
                    lastCinematicCam = firstCinematicCam;
                }
            }
            else
            {
                if (part.cam != null)
                    lastCinematicCam = part.cam;
            }
        }

        cinematicProcessing = true;

        StartCinematicPart();

        if (controler.Skippable)
            skipCinematicButton.Interaction = SkipCinematicPart;
        else
            skipCinematicButton.Interaction = ShowNotSkippable;
    }

    public void StartCinematic(IntroControler controler, List<EnemyShip> enemyShipsToStart)
    {
        shipsToStartOnSpecificPart = enemyShipsToStart;
        StartCinematic(controler);
    }

    public void StartCinematic(IntroControler controler, OnCinematicEnd actionAfterCinematic, bool keepCamera)
    {
        OnCinematicEnd = actionAfterCinematic;
        keepLastCameraAtTheEnd = keepCamera;
        StartCinematic(controler);
    }

    public void StartCinematic(IntroControler controler, List<EnemyShip> enemyShipsToStart, OnCinematicEnd actionAfterCinematic, bool keepCamera)
    {
        shipsToStartOnSpecificPart = enemyShipsToStart;
        OnCinematicEnd = actionAfterCinematic;
        keepLastCameraAtTheEnd = keepCamera;
        StartCinematic(controler);
    }

    public void SetShipsToStart(List<EnemyShip> enemyShipsToStart)
    {
        shipsToStartOnSpecificPart = enemyShipsToStart;
    }
    List<EnemyShip> shipsToStartOnSpecificPart;
    public void StartCinematicPart()
    {
        CinematicPart startedPart = currentCinematicAllParts[currentCinematicPartIndex];

        /*ShipMovements playerMovements = GameManager.gameManager.Player.ShipMvt;
        if (startedPart.GetMovePlayerBoatWhileCinematic && playerMovements.Stopped)*/

        if (startedPart.StartSpawnedEnemies && shipsToStartOnSpecificPart != null)
        {
            foreach (EnemyShip enemy in shipsToStartOnSpecificPart)
                enemy.ShipMvt.StartShip();

            shipsToStartOnSpecificPart = new List<EnemyShip>();
        }


        remainingCinematicPartDuration = startedPart.GetWaitTimeOnceEnded;
        if (remainingCinematicPartDuration == 0)
            remainingCinematicPartDuration = 1.5f;

        #region Cameras
        if (startedPart.cam != null)
        {
            if (startedPart.cam == firstCinematicCam)
                StartCameraMove(!startedPart.GetMovePlayerBoatWhileCinematic);

            startedPart.cam.gameObject.SetActive(true);

            cinematicPartHasCamMove = true;
            skipCinematicButton.gameObject.SetActive(true);
        }
        else
        {
            if (GameManager.gameManager.StartedFight)
            {
                cinematicPartHasCamMove = false;
                skipCinematicButton.gameObject.SetActive(false);
            }
            else
            {
                /*if (startedPart.cam == firstCinematicCam)
                    StartCameraMove(!startedPart.GetMovePlayerBoatWhileCinematic);*/

                //startedPart.cam.gameObject.SetActive(true);

                cinematicPartHasCamMove = true;
                skipCinematicButton.gameObject.SetActive(true);
            }
        }
        #endregion

        #region Dialogue
        CreateAText messageToType = currentCinematicAllParts[currentCinematicPartIndex].messageToType;
        if (messageToType != null && messageToType != currentMessageToType)
            StartDialogue(messageToType);
        else if (messageToType == null && messageToType != currentMessageToType)
            EndDialogue();
        #endregion

        #region Animators
        if (startedPart.GetAnimatorsToLaunchOnPartBeginning != null)
        {
            foreach (Animator animatorToLaunch in startedPart.GetAnimatorsToLaunchOnPartBeginning)
            {
                if (animatorToLaunch != null)
                    animatorToLaunch.SetTrigger("Cinematic");
            }
        }
        #endregion

        if (waitingToGetControlBackToPlayer && !startedPart.GetDontGivePlayerControlBack)
        {
            waitingToGetControlBackToPlayer = false;
            ShipMovements playerMovements = GameManager.gameManager.Player.ShipMvt;

            if (stoppedPlayer)
            {
                playerMovements.StartShip();
                stoppedPlayer = false;
            }

            playerInterface.ShowPlayerInterface();
        }

        startedPart.PlayStartEvent();
    }

    public void UpdateCinematicPart()
    {
        if (showingNotSkippable)
            UpdateShowNotSkippable();

        if (comingBackToPlayer)
        {
            if (mainCameraBrain.ActiveBlend != null)
            {
                if (mainCameraBrain.ActiveBlend.TimeInBlend / mainCameraBrain.ActiveBlend.Duration > 0.75f)
                {
                    EndCameraMoves();
                }
            }
        }

        if (currentIntroControler == null)
            return;

        if (!isTypingText)
        {
            if (!mainCameraBrain.IsBlending)
            {
                if (remainingCinematicPartDuration > 0)
                    remainingCinematicPartDuration -= Time.deltaTime;
                else if (remainingCinematicPartDuration < 0)
                    EndCinematicPart();
            }
        }
        else
            UpdateDialogue();
    }

    bool cinematicPartHasCamMove;
    bool skipped;
    public void SkipCinematicPart()
    {
        if (currentIntroControler == null)
            return;


        if (cinematicPartHasCamMove)
        {
            if (isTypingText || mainCameraBrain.IsBlending)
            {
                if (skipped)
                {
                    if (remainingCinematicPartDuration > 0)
                        EndCinematicPart();
                    return;
                }

                skipped = true;

                if (isTypingText)
                {
                    SetFullDialogue();
                }
                if (mainCameraBrain.IsBlending)
                {
                    /*mainCameraBrain.enabled = false;
                    mainCameraBrain.enabled = true;*/

                    mainCameraBrain.m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.Linear;

                    skipCamera.transform.position = mainCameraBrain.transform.position;
                    skipCamera.transform.rotation = mainCameraBrain.transform.rotation;


                    if(mainCameraBrain.ActiveBlend != null)
                        mainCameraBrain.ActiveBlend.CamA = skipCamera;
                    /*CameraState state = mainCameraBrain.ActiveBlend.CamA.State;
                    state.RawOrientation = mainCameraBrain.transform.rotation;*/


                    /*skipCamera.enabled = true;
                    skipCamera.enabled = false;*/
                }
            }
            else if(remainingCinematicPartDuration > 0)
                EndCinematicPart();
        }
    }

    public void EndCinematicPart()
    {
        skipped = false;

        mainCameraBrain.m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.EaseInOut;

        if (currentCinematicAllParts[currentCinematicPartIndex].cam == lastCinematicCam && !keepLastCameraAtTheEnd)
                StartComeBackToPlayer();

        if (currentCinematicAllParts[currentCinematicPartIndex].cam != null && !keepLastCameraAtTheEnd)
            currentCinematicAllParts[currentCinematicPartIndex].cam.gameObject.SetActive(false);

        remainingCinematicPartDuration = 0;

        currentCinematicAllParts[currentCinematicPartIndex].PlayEndEvent();

       /* if (textToShow != null && textToShow != "")
            EndDialogue();*/

        currentCinematicPartIndex++;

        if (currentCinematicPartIndex < currentCinematicParameters.GetAllCinematicsParts.Length)
        {
            StartCinematicPart();
        }
        else
            EndCinematic();
    }

    public OnCinematicEnd OnCinematicEnd;
    public void EndCinematic()
    {
        if (OnCinematicEnd != null)
        {
            OnCinematicEnd();
            OnCinematicEnd = null;
        }

        currentCinematicParameters = default;
        firstCinematicCam = null;
        lastCinematicCam = null;
        currentCinematicPartIndex = 0;

        currentIntroControler.SetCinematicEnded();
        currentIntroControler = null;

        skipCinematicButton.gameObject.SetActive(false);

        cinematicProcessing = false;

        if (currentMessageToType != null)
            EndDialogue();

        if (waitingToGetControlBackToPlayer)
        {
            waitingToGetControlBackToPlayer = false;
            ShipMovements playerMovements = GameManager.gameManager.Player.ShipMvt;

            if (stoppedPlayer)
            {
                playerMovements.StartShip();
                stoppedPlayer = false;
            }

            playerInterface.ShowPlayerInterface();
        }
    }
    #endregion

    #region Camera Moves
    public void StartCameraMove(bool stopPlayer)
    {
        ShipMovements playerMovements = GameManager.gameManager.Player.ShipMvt;
        if (!playerMovements.Stopped && stopPlayer)
        {
            playerMovements.StopShip();
            stoppedPlayer = true;
        }
        else if (playerMovements.Stopped && !stopPlayer)
        {
            playerMovements.StartShip();
            stoppedPlayer = false;
        }

        playerInterface.HidePlayerInterface();
    }

    bool comingBackToPlayer;
    public void StartComeBackToPlayer()
    {
        comingBackToPlayer = true;
    }

    bool waitingToGetControlBackToPlayer;
    public void EndCameraMoves()
    {
        comingBackToPlayer = false;

        if (currentCinematicAllParts[currentCinematicPartIndex].cam == null)
        {
            if (!currentCinematicAllParts[currentCinematicPartIndex].GetDontGivePlayerControlBack)
            {
                ShipMovements playerMovements = GameManager.gameManager.Player.ShipMvt;

                if (stoppedPlayer)
                {
                    playerMovements.StartShip();
                    stoppedPlayer = false;
                }

                playerInterface.ShowPlayerInterface();
            }
            else
            {
                waitingToGetControlBackToPlayer = true;
            }
        }
    }
    #endregion

    #region Dialogues
    [Header("Dialogues")]
    [SerializeField] GameObject dialoguePanel;
    [SerializeField] Text dialogueText;
    [SerializeField] int lettersPerSecond = 48;
    public float TimeBetweenTwoChars
    {
        get
        {
            return 1 / (float)lettersPerSecond;
        }
    }
    float currentTimeBeforeNextChar;
    int currentLetterIndex;
    string textToShow;
    string shownText;

    [SerializeField] Animator dialogueAnimationImage;
    CreateAText currentMessageToType;


    public void StartDialogue(CreateAText message)
    {
        currentMessageToType = message;
        dialoguePanel.SetActive(true);

        textToShow = message.textToWrite;

        if (currentMessageToType.GetSpeakerAnimator != null)
        {
            dialogueAnimationImage.runtimeAnimatorController = currentMessageToType.GetSpeakerAnimator;
            dialogueAnimationImage.SetTrigger(currentMessageToType.GetSpeakerEmotion.ToString());
        }

        currentTimeBeforeNextChar = TimeBetweenTwoChars;
        isTypingText = true;
        currentLetterIndex = 0;
        shownText = "";
        /*shownText = new string(' ', textToShow.Length);
        Debug.Log("\"" + shownText + "\"");*/
    }

    public void UpdateDialogue()
    {
        if (currentTimeBeforeNextChar > 0)
            currentTimeBeforeNextChar -= Time.deltaTime;

        for (int i = 0; currentTimeBeforeNextChar < 0 && currentLetterIndex < textToShow.Length && i < 10000; i++)
        {
            shownText += textToShow[currentLetterIndex];
            currentLetterIndex++;
            currentTimeBeforeNextChar += TimeBetweenTwoChars;
        }

        dialogueText.text = shownText /*+ new string('-', textToShow.Length - shownText.Length)*/;

        if (shownText == textToShow)
            isTypingText = false;
    }

    public void SetFullDialogue()
    {
        shownText = textToShow;
        dialogueText.text = shownText;
        isTypingText = false;
        currentTimeBeforeNextChar = 0;
    }

    public void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        currentTimeBeforeNextChar = 0;
        isTypingText = false;
        textToShow = "";
        currentLetterIndex = 0;
        shownText = "";
    }
    #endregion

    public bool CinematicProcessing
    {
        get
        {
            return cinematicProcessing;
        }
    }

    #region Not Skippable Management
    [Header("Not Skippable Management")]
    [SerializeField] MaskableGraphic[] notSkippableUIElements;
    [SerializeField] int numberOfNotSkippableSignal;
    [SerializeField] float speedOfNotSkippableSignal;
    [SerializeField] AnimationCurve notSkippableCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    float skipCounter;
    bool showingNotSkippable;

    public void ShowNotSkippable()
    {
        if (showingNotSkippable)
            return;

        showingNotSkippable = true;
        skipCounter = 0;

        foreach (MaskableGraphic element in notSkippableUIElements)
        {
            Color newColor = element.color;
            newColor.a = 0;
            element.color = newColor;
        }
    }

    public void UpdateShowNotSkippable()
    {
        skipCounter += Time.deltaTime * speedOfNotSkippableSignal;
        float coeff = notSkippableCurve.Evaluate(skipCounter);
        foreach (MaskableGraphic element in notSkippableUIElements)
        {
            Color newColor = element.color;
            newColor.a = coeff;
            element.color = newColor;
        }

        if(skipCounter > numberOfNotSkippableSignal * speedOfNotSkippableSignal)
        {
            StopShowNotSkippable();
        }
    }

    public void StopShowNotSkippable()
    {
        showingNotSkippable = false;
        skipCounter = 0;
        foreach (MaskableGraphic element in notSkippableUIElements)
        {
            Color newColor = element.color;
            newColor.a = 0;
            element.color = newColor;
        }
    }
    #endregion
}
public delegate void OnCinematicEnd();

[System.Serializable] 
public struct CinematicParameters
{
    public CinematicParameters(CinematicPart[] parts)
    {
        cinematicParts = parts;
        cinematicStartDelay = 0;
    }

    [SerializeField] float cinematicStartDelay;
    public float GetCinematicStartDelay { get { return cinematicStartDelay; } }

    [SerializeField] CinematicPart[] cinematicParts;
    public CinematicPart[] GetAllCinematicsParts { get { return cinematicParts; } }

    /*public CinematicType GetCinematicType
    {
        get
        {
            bool hasCam = false;
            bool hasText = false;

            foreach()

            /*if (cam != null)
            {
                if (messageToType != null)
                    return CinematicType.DialogueAndCameraMoves;
                else
                    return CinematicType.OnlyCameraMoves;
            }
            else if (messageToType != null)
                return CinematicType.OnlyDialogue;
            else
                return CinematicType.Null;
        }
    }*/
}
