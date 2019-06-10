using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SlowMoManager
{
    public void UpdateSlowMoManagement()
    {
        if (paused)
            return;

        UpdateAimSlowMo();
        UpdateFeedbackSlowMo();
    }

    [Header("Aminig Slow Mo")]
    [SerializeField] float aimToNormalDuration;
    float currentAimToNormalDuration;
    [SerializeField] AnimationCurve aimToNormalCurve;

    float currentAimDuration;
    float pickedAimSlowMo;
    [Header("Aminig Slow Mo : Canon")]
    [SerializeField] float aimSlowMoCoeffCanon;
    [SerializeField] float aimDurationCanon;

    [Header("Aminig Slow Mo : Catapult")]
    [SerializeField] float aimSlowMoCoeffCatapult;
    [SerializeField] float aimDurationCatapult;

    [Header("Aminig Slow Mo : Hull")]
    [SerializeField] float aimSlowMoCoeffHull;
    [SerializeField] float aimDurationHull;


    bool aimSlowMoOn;

    SlowMoParameters currentSlowMoParameters;
    float remainingSlowMoTime;

    #region AimSlowMo
    public void StartAimSlowMo(EquipmentType equipmentType)
    {
        switch (equipmentType)
        {
            case (EquipmentType.Canon):
                pickedAimSlowMo = aimSlowMoCoeffCanon;
                currentAimDuration = aimDurationCanon;
                break;

            case (EquipmentType.Catapult):
                pickedAimSlowMo = aimSlowMoCoeffCatapult;
                currentAimDuration = aimDurationCatapult;
                break;

            case (EquipmentType.Hull):
                pickedAimSlowMo = aimSlowMoCoeffHull;
                currentAimDuration = aimDurationHull;
                break;
        }

        Time.timeScale = pickedAimSlowMo;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
        aimSlowMoOn = true;

        //currentAimDuration = aimDuration;
        currentAimToNormalDuration = aimToNormalDuration;
    }

    public void UpdateAimSlowMo()
    {
        if (currentAimDuration > 0)
            currentAimDuration -= Time.unscaledDeltaTime;
        else if (currentAimDuration < 0)
            currentAimDuration = 0;
        else
        {
            if (currentAimToNormalDuration > 0)
            {
                currentAimToNormalDuration -= Time.unscaledDeltaTime;
                Time.timeScale = Mathf.Lerp(pickedAimSlowMo, GameManager.gameManager.normalTimeScale, aimToNormalCurve.Evaluate(1 - (currentAimToNormalDuration / aimToNormalDuration)));
            }
            else if (currentAimToNormalDuration < 0)
                StopAimSlowMo();
        }
    }

    public void StopAimSlowMo()
    {
        Time.timeScale = GameManager.gameManager.normalTimeScale;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
        aimSlowMoOn = false;

        currentAimDuration = 0;
        currentAimToNormalDuration = 0;
    }
    #endregion

    #region Feedback SlowMo
    public void SetSlowMo(SlowMoParameters slowMoParameters)
    {
        if (aimSlowMoOn)
            return;

        currentSlowMoParameters = slowMoParameters;

        Time.timeScale = currentSlowMoParameters.slowMoCoeff;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        remainingSlowMoTime = currentSlowMoParameters.slowMoUnscaledDuration;
    }

    public void UpdateFeedbackSlowMo()
    {
        if (Time.timeScale == 0 || Time.timeScale == GameManager.gameManager.normalTimeScale || aimSlowMoOn)
            return;

        if (remainingSlowMoTime > 0)
            remainingSlowMoTime -= Time.unscaledDeltaTime;
        else if (remainingSlowMoTime < 0)
            remainingSlowMoTime = 0;
        else
        {
            Time.timeScale += (1f / currentSlowMoParameters.slowMoAttenuationDuration) * Time.unscaledDeltaTime;

            if (Time.timeScale > GameManager.gameManager.normalTimeScale)
                Time.timeScale = GameManager.gameManager.normalTimeScale;

            Time.fixedDeltaTime = 0.02f * Time.timeScale;
        }
    }
    #endregion

    #region Pause Game
    bool paused;
    float lastTimeScale = 1f;

    bool hidInterface;

    public void PauseGame()
    {
        paused = true;
        lastTimeScale = Time.timeScale;
        Time.timeScale = 0;

        if (!GameManager.gameManager.PlrInterface.Hidden)
        {
            hidInterface = true;
            GameManager.gameManager.PlrInterface.HidePlayerInterface();
        }
    }

    public void UnpauseGame()
    {
        paused = false;
        Time.timeScale = lastTimeScale;

        if (hidInterface)
        {
            GameManager.gameManager.PlrInterface.ShowPlayerInterface();
            hidInterface = false;
        }
    }

    public bool Paused
    {
        get
        {
            return paused;
        }
    }
    #endregion
}
