using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MapIntersceneInformations
{
    public void SetMapIntersceneInfos(string mpSceneName, Vector3 playerPos)
    {
        mapSceneName = mpSceneName;
        playerPositionOnMap = playerPos;
    }

    string mapSceneName;
    public string GetMapSceneName
    {
        get
        {
            return mapSceneName;
        }
    }

    Vector3 playerPositionOnMap;
    public Vector3 GetPlayerPositionOnMap
    {
        get
        {
            return playerPositionOnMap;
        }
    }

    public void Reinitialize()
    {
        mapSceneName = "";
        playerPositionOnMap = Vector3.zero;
    }
}
