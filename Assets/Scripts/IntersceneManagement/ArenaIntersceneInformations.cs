using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ArenaIntersceneInformations
{
    #region Launching Informations
    ArenaParameters launchedArenaParameters;
    public ArenaParameters GetLaunchedArenaParameters { get { return launchedArenaParameters; } }

    public string GetLaunchedArenaBuildName
    {
        get
        {
            if (launchedArenaParameters != null)
                return launchedArenaParameters.GetSceneToLoadName;
            else
                return null;
        }
    }

    public void SetArenaLaunchInformations(ArenaParameters arenaParameters)
    {
        launchedArenaParameters = arenaParameters;
    }

    bool needToPassTutorial;
    public void SetNeedToPassTutorial(bool needToPassTuto)
    {
        needToPassTutorial = needToPassTuto;
    }

    public bool GetNeedToPassTutorial { get { return needToPassTutorial; } }

    bool justPassedTutorial;
    public bool GetJustPassedTutorial { get { return justPassedTutorial; } }
    public void SetJustPassedTutorial(bool passed)
    {
        justPassedTutorial = passed;
    }
    #endregion

    #region Quitting Informations
    [SerializeField] bool arenaPassed;
    public bool GetArenaPassed { get { return arenaPassed; } }
    [SerializeField] int numberOfStars;
    public int GetNumberOfStars { get { return numberOfStars; } }

    public void SetArenaPassed(bool passed, int stars)
    {
        arenaPassed = passed;
        numberOfStars = stars;
    }
    #endregion

    public void Reinitialize()
    {
        launchedArenaParameters = null;
        needToPassTutorial = true;
    }
}
