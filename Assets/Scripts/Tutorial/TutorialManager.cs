using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    float waitTimeBeforeStart = 1f;

    [SerializeField] TutorialStep[] tutorialAllSteps;
    [SerializeField] EnemySpawningManager enemySpawningManager;
    int currentTutorialStepIndex;
    

    private void Update()
    {
        if(currentTutorialStepIndex < tutorialAllSteps.Length)
            UpdateTutorial();

        if (waitTimeBeforeStart > 0)
            waitTimeBeforeStart -= Time.deltaTime;
        else if (waitTimeBeforeStart < 0)
        {
            waitTimeBeforeStart = 0;
            StartTutorial();
        }
    }

    #region GlobalManagement
    public void StartTutorial()
    {
        if(tutorialAllSteps.Length > 0)
            tutorialAllSteps[currentTutorialStepIndex].SetUpStep(enemySpawningManager);
    }

    public void UpdateTutorial()
    {
        TutorialStep currentStep = tutorialAllSteps[currentTutorialStepIndex];
        switch (currentStep.GetCurrentPhase)
        {
            case (TutorialStepPhase.Intro):
                currentStep.UpdateTutorialStep();
                break;

            case (TutorialStepPhase.Resolution):
                if(currentStep.CheckIfDone())
                    currentStep.EndStep();
                break;

            case (TutorialStepPhase.Ended):
                GoToNextStep();
                break;
        }
    }

    public void GoToNextStep()
    {
        CloseInstructionsPanel();

        currentTutorialStepIndex++;
        if (currentTutorialStepIndex < tutorialAllSteps.Length)
        {
            tutorialAllSteps[currentTutorialStepIndex].SetUpStep(enemySpawningManager);
        }
        else
            EndTutorial();
    }

    public void EndTutorial()
    {
        Debug.Log("Tuto fini");
    }
    #endregion

    #region Instructions Interface
    [Header("Instructions Interface")]
    [SerializeField] GameObject instructionPanel;
    [SerializeField] Text instructionText;

    public void OpenInstructionsAndSetText(string instruction)
    {
        instructionPanel.SetActive(true);
        instructionText.text = instruction;
    }

    public void CloseInstructionsPanel()
    {
        if(instructionPanel != null)
            instructionPanel.SetActive(false);
    }
    #endregion
}

[System.Serializable]
public class TutorialStep
{
    TutorialStepPhase currentPhase;
    public TutorialStepPhase GetCurrentPhase
    {
        get
        {
            return currentPhase;
        }
    }

    #region SetUp
    [Header("Set Up")]
    [SerializeField] UnityEvent StartEvent;
    [SerializeField] CompetencesUsabilityType availableCompetences;

    public void SetUpStep(EnemySpawningManager enemySpawningManager)
    {
        targetsToDestroyReceivers = new List<IDamageReceiver>();

        foreach (GameObject targetObject in targetsToDestroy)
        {
            if (targetObject != null)
            {
                IDamageReceiver damageReceiver = targetObject.GetComponent<IDamageReceiver>();
                if (damageReceiver != null)
                {
                    targetsToDestroyReceivers.Add(damageReceiver);
                    damageReceiver.OnDeath += new OnLifeEvent(RemoveTargetToDestroy);
                }
            }
        }

        remainingZones = new List<TargetZone>();

        foreach (TargetZone zoneToGoTo in zonesToGoTo)
        {
            remainingZones.Add(zoneToGoTo);
            zoneToGoTo.OnPlayerEnter += RemoveZoneToGoTo;
            zoneToGoTo.StartZone();
        }

        PlayStepIntro();

        StartEvent.Invoke();

        GameManager.gameManager.Player.PlrInterface.ChangeCompetencesAvailability(availableCompetences);

        if (spawnSpecificWaveOnStepStart)
        {
            List<IDamageReceiver> waveTargetsToDestroy = enemySpawningManager.SpawnEnemiesExternaly(waveToSpawnIndex);
            if (needToClearSpawnedWaveToValidate)
            {
                foreach (IDamageReceiver receiver in waveTargetsToDestroy)
                {
                    targetsToDestroyReceivers.Add(receiver);
                    receiver.OnDeath += new OnLifeEvent(RemoveTargetToDestroy);
                }
            }
        }
    }
    #endregion

    #region Update
    public void UpdateTutorialStep()
    {
        switch (currentPhase)
        {
            case (TutorialStepPhase.Intro):
                if (introCinematic.Ended)
                    StartStepResolution();
                break;
        }
    }
    #endregion

    #region Intro
    [Header("Intro")]
    [SerializeField] bool stopThePlayerOnIntro;
    [SerializeField] IntroControler introCinematic;
    [SerializeField] Animator[] animatorsToPlayInIntro;

    public void PlayStepIntro()
    {
        currentPhase = TutorialStepPhase.Intro;

        foreach(Animator animator in animatorsToPlayInIntro)
        {
            animator.SetInteger("Tuto", animator.GetInteger("Tuto") + 1);
        }

        if (stopThePlayerOnIntro)
            GameManager.gameManager.Player.ShipMvt.StopShip();

        if (introCinematic != null)
            introCinematic.PlayCinematic();
        else
            StartStepResolution();
    }
    #endregion

    #region Resolution
    [Header("Resolution")]
    [SerializeField] UnityEvent StartResolutionEvent;
    [SerializeField] Animator[] animatorsToPlayWhileResolving;
    [SerializeField] GameObject[] targetsToDestroy;
    List<IDamageReceiver> targetsToDestroyReceivers;

    public void RemoveTargetToDestroy(IDamageReceiver deadTarget)
    {
        targetsToDestroyReceivers.Remove(deadTarget);
    }

    [SerializeField] TargetZone[] zonesToGoTo;
    List<TargetZone> remainingZones;
    public void RemoveZoneToGoTo(TargetZone removedZone)
    {
        remainingZones.Remove(removedZone);
    }

    [Header("Resolution : Waves")]
    [SerializeField] bool spawnSpecificWaveOnStepStart;
    [SerializeField] bool needToClearSpawnedWaveToValidate;
    [SerializeField] int waveToSpawnIndex;

    public void StartStepResolution()
    {
        currentPhase = TutorialStepPhase.Resolution;

        if (stopThePlayerOnIntro)
            GameManager.gameManager.Player.ShipMvt.StartShip();

        foreach (Animator animator in animatorsToPlayInIntro)
        {
            animator.SetInteger("Tuto", animator.GetInteger("Tuto") + 1);
        }

        foreach (Animator animator in animatorsToPlayWhileResolving)
        {
            animator.SetInteger("Tuto", animator.GetInteger("Tuto") + 1);
        }

        StartResolutionEvent.Invoke();
    }

    public bool CheckIfDone()
    {
        return targetsToDestroyReceivers.Count == 0 && remainingZones.Count == 0;
    }
    #endregion

    public void EndStep()
    {
        foreach (Animator animator in animatorsToPlayWhileResolving)
        {
            animator.SetInteger("Tuto", animator.GetInteger("Tuto") + 1);
        }

        currentPhase = TutorialStepPhase.Ended;

        GameManager.gameManager.Player.PlrInterface.ChangeCompetencesAvailability(CompetencesUsabilityType.All);
    }
}

public enum TutorialStepPhase
{
    NotStarted,
    Intro,
    Resolution,
    Ended
}
