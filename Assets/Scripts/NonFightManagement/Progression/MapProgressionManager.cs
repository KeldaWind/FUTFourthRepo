using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MapProgressionManager
{
    MapManager mapManager;
    public void SetUp(MapManager mpManager)
    {
        mapManager = mpManager;
    }

    [Header("Tutorial")]
    [SerializeField] IntroControler cinematicToPlayAfterTutorial;

    [Header("Arenas")]
    [SerializeField] List<MapArenaSpot> allMapArenaSpots;
    public List<MapArenaSpot> GetAllMapArenaSpots { get { return allMapArenaSpots; } }

    public void CheckMapProgression()
    {
        IntersceneManager intersceneManager = IntersceneManager.intersceneManager;
        if (intersceneManager != null)
        {
            bool passedTutorial = false;

            PlayerProgressionDatas progressionDatas = PlayerDataSaver.LoadProgressionDatas();

            ArenaIntersceneInformations arenaIntersceneInformations = intersceneManager.ArenaInterscInformations;
            
            #region Tutorial
            if (arenaIntersceneInformations.GetNeedToPassTutorial)
            {
                if (arenaIntersceneInformations.GetArenaPassed)
                {
                    progressionDatas.SetPassedTutorial();
                    arenaIntersceneInformations.SetNeedToPassTutorial(false);
                    PlayerDataSaver.SavePlayerProgressionDatas(progressionDatas.GetAllPassedArenaDatas, true);
                }
            }

            passedTutorial = progressionDatas.GetPassedTuto;
            #endregion

            #region Already Unlocked

            #region V2
            List<PassedArenaData> allPassedArenaDatas = new List<PassedArenaData>();
            if (progressionDatas != null)
            {
                allPassedArenaDatas = progressionDatas.GetAllPassedArenaDatas;
                if (allPassedArenaDatas == null)
                    allPassedArenaDatas = new List<PassedArenaData>();

                foreach (PassedArenaData datas in allPassedArenaDatas)
                {
                    foreach (MapArenaSpot arenaSpot in allMapArenaSpots)
                    {
                        if (arenaSpot.GetArenaParameters.GetSceneToLoadName == datas.GetArenaName && datas.GetArenaPassed)
                        {
                            arenaSpot.UnlockNextProgressionContent(false, datas.GetNumberOfStars);
                            break;
                        }
                    }
                }
            }
            else
                Debug.Log("pas encore d'arènes débloquées");
            #endregion
            #endregion

            #region Just Unlocked
            ArenaParameters lastArenaParameters = arenaIntersceneInformations.GetLaunchedArenaParameters;
            if (lastArenaParameters == null)
            {
                arenaIntersceneInformations.SetArenaPassed(false, 0);

                if (intersceneManager.ArenaInterscInformations.GetJustPassedTutorial)
                {
                    intersceneManager.ArenaInterscInformations.SetJustPassedTutorial(false);
                    if (cinematicToPlayAfterTutorial != null)
                        cinematicToPlayAfterTutorial.PlayCinematic();
                }

                return;
            }

            #region V2
            List<string> passedArenaNames = new List<string>();

            if (allPassedArenaDatas == null)
                allPassedArenaDatas = new List<PassedArenaData>();

            foreach (PassedArenaData datas in allPassedArenaDatas)
            {
                if (datas.GetArenaPassed)
                    passedArenaNames.Add(datas.GetArenaName);
            }

            if (arenaIntersceneInformations.GetArenaPassed)
            {
                int stars = arenaIntersceneInformations.GetNumberOfStars;

                if (passedArenaNames.Contains(lastArenaParameters.GetSceneToLoadName))
                {
                    foreach (MapArenaSpot arenaSpot in allMapArenaSpots)
                    {
                        if (arenaSpot.GetArenaParameters.GetSceneToLoadName == lastArenaParameters.GetSceneToLoadName)
                        {
                            if(stars > arenaSpot.GetStarsNumber)
                                arenaSpot.UnlockNextProgressionContent(false, stars);

                            foreach (PassedArenaData datas in allPassedArenaDatas)
                            {
                                if(datas.GetArenaName == lastArenaParameters.GetSceneToLoadName)
                                {
                                    datas.ChangeNumberOfStars(stars);
                                    break;
                                }
                            }
                            PlayerDataSaver.SavePlayerProgressionDatas(allPassedArenaDatas, passedTutorial);
                            break;
                        }
                    }
                    return;
                }

                foreach (MapArenaSpot arenaSpot in allMapArenaSpots)
                {
                    if (arenaSpot.GetArenaParameters.GetSceneToLoadName == lastArenaParameters.GetSceneToLoadName)
                    {
                        arenaSpot.UnlockNextProgressionContent(true, stars);
                        break;
                    }
                }

                allPassedArenaDatas.Add(new PassedArenaData(lastArenaParameters, true, stars));
                PlayerDataSaver.SavePlayerProgressionDatas(allPassedArenaDatas, passedTutorial);
            }
            #endregion
            #endregion

            bool arenaPassed = arenaIntersceneInformations.GetArenaPassed;
            int numberOfStars = arenaIntersceneInformations.GetNumberOfStars;

            //arenaIntersceneInformations.SetArenaLaunchInformations(null);
            arenaIntersceneInformations.SetArenaPassed(false, 0);
        }
    }
}
