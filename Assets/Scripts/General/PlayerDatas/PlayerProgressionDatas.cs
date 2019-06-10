using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerProgressionDatas
{
    #region V2
    [SerializeField] string serializedArenaProgressionData;
    [SerializeField] bool passedTutorial;
    public bool GetPassedTuto { get { return passedTutorial; } }

    public void SetPassedTutorial()
    {
        passedTutorial = true;
    }

    public void SetProgressionDatas(List<PassedArenaData> passedArenaDatas)
    {
        serializedArenaProgressionData = JsonUtility.ToJson(new ArenaProgressionData(passedArenaDatas));
    }

    public List<PassedArenaData> GetAllPassedArenaDatas
    {
        get
        {
            try
            {
                ArenaProgressionData arenaProgressionData = JsonUtility.FromJson<ArenaProgressionData>(serializedArenaProgressionData);
                return arenaProgressionData.GetAllArenasDatas;
            }
            catch
            {
                Debug.LogWarning("erreur de désérialisation des données de progression des arènes");
                return null;
            }
        }
    }
    #endregion

    public void PrintDatas()
    {
        ArenaProgressionData data = new ArenaProgressionData(GetAllPassedArenaDatas);
        Debug.Log(JsonUtility.ToJson(data));
    }
}

[System.Serializable]
public class ArenaProgressionData
{
    [SerializeField] List<PassedArenaData> allArenasDatas;
    public List<PassedArenaData> GetAllArenasDatas { get { return allArenasDatas; } }
    public ArenaProgressionData(List<PassedArenaData> allArenas)
    {
        allArenasDatas = allArenas;
    }
}

[System.Serializable]
public class PassedArenaData
{
    public PassedArenaData(ArenaParameters arenaParameters, bool passed, int arenaStarsNumber)
    {
        arenaName = arenaParameters.GetSceneToLoadName;
        arenaPassed = passed;
        numberOfStars = arenaStarsNumber;
    }

    public void ChangeNumberOfStars(int newNumber)
    {
        if (numberOfStars < newNumber)
            numberOfStars = newNumber;
    }

    [SerializeField] string arenaName;
    public string GetArenaName { get { return arenaName; } }
    [SerializeField] bool arenaPassed;
    public bool GetArenaPassed { get { return arenaPassed; } }
    [SerializeField] int numberOfStars;
    public int GetNumberOfStars { get { return numberOfStars; } }
}
