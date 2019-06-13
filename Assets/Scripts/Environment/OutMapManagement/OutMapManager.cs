using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class OutMapManager
{
    LifeManager playerLifeManager;
    IDamageReceiver playerReceiver;

    [SerializeField] float timeBeforeFirstDamages;
    [SerializeField] float timeBetweenTwoDamages;
    float remainingTimeBeforeNextDamages;
    [SerializeField] DamagesParameters damagesParameters;
    [SerializeField] IntroControler[] warningCinematics;
    [SerializeField] Image[] dangerVignettes;
    [SerializeField] float dangerVignetteSpeed;
    AnimationCurve vignetteCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    float vignetteCounter;

    List<OutMapZone> activeOutMapZones;
    public void SetUp()
    {
        activeOutMapZones = new List<OutMapZone>();
        //playerLifeManager = GameManager.gameManager.Player.LfManager;
        playerReceiver = GameManager.gameManager.Player.GetShipDamageReceiver;
        vignetteCurve.preWrapMode = WrapMode.PingPong;
        vignetteCurve.postWrapMode = WrapMode.PingPong;
    }

    public void EnterOutMapZone(OutMapZone enteredZone)
    {
        if (!GameManager.gameManager.StartedFight && !GameManager.gameManager.Won)
            return;

        if (activeOutMapZones.Count == 0)
        {
            PlayWarningDialogue();
            GameManager.gameManager.Player.PlayDangerParticles();
        }

        activeOutMapZones.Add(enteredZone);

        if (activeOutMapZones.Count == 1)
            remainingTimeBeforeNextDamages = timeBeforeFirstDamages;
    }

    public void ExitOutMapZone(OutMapZone exitedZone)
    {
        if (activeOutMapZones.Contains(exitedZone))
            activeOutMapZones.Remove(exitedZone);

        if (activeOutMapZones.Count == 0)
        {
            remainingTimeBeforeNextDamages = 0;
            GameManager.gameManager.Player.StopDangerParticles();
        }
    }

    public void UpdateOutMapManagement()
    {
        if(activeOutMapZones.Count > 0)
        {
            if (remainingTimeBeforeNextDamages > 0)
                remainingTimeBeforeNextDamages -= Time.deltaTime;
            else if (remainingTimeBeforeNextDamages < 0)
            {
                if (GameManager.gameManager.Won)
                {
                    activeOutMapZones = new List<OutMapZone>();
                    remainingTimeBeforeNextDamages = 0;
                    return;
                }

                //playerLifeManager.Damage(damagesParameters.GetDamageAmount, damagesParameters.GetRecoveringTime, damagesParameters.GetRecoveringType);
                playerReceiver.ReceiveDamage(null, damagesParameters, null);
                remainingTimeBeforeNextDamages = timeBetweenTwoDamages;
            }
        }

        if(vignetteCounter != 0 || activeOutMapZones.Count > 0)
        {
            vignetteCounter += Time.deltaTime * dangerVignetteSpeed;

            foreach (Image dangerVignette in dangerVignettes)
            {
                Color vignetteColor = dangerVignette.color;
                vignetteColor.a = vignetteCurve.Evaluate(vignetteCounter);
                dangerVignette.color = vignetteColor;
            }

            if (vignetteCounter % 2 < 0.03f)
            {
                if (activeOutMapZones.Count > 0)
                    vignetteCounter -= 2;
                else
                    vignetteCounter = 0;
            }
        }
    }

    public void PlayWarningDialogue()
    {
        if (!GameManager.gameManager.CinematicMng.CinematicProcessing && warningCinematics.Length > 0)
            warningCinematics[Random.Range(0, warningCinematics.Length)].PlayCinematic();
    }
}
