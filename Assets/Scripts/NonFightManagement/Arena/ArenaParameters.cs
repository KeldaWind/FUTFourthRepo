using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "NewArenaParameters", menuName = "Arena/ArenaParameters")]
public class ArenaParameters : ScriptableObject
{
    [Header("Important Parameters")]
    [SerializeField] string sceneToLoadName;
    public string GetSceneToLoadName
    {
        get
        {
            return sceneToLoadName;
        }
    }

    [Header("Display Parameters")]
    [SerializeField] string displayName;
    public string GetDisplayName
    {
        get
        {
            return displayName;
        }
    }

    [SerializeField] string displayDescription;
    public string GetDisplayDescription
    {
        get
        {
            return displayDescription;
        }
    }

    [Header("Scores Parameters")]
    [SerializeField] int maximumNumberOfDamagesToHaveStar;
    public int GetMaximumNumberOfDamagesToHaveStar { get { return maximumNumberOfDamagesToHaveStar; } }
    [SerializeField] float maximumArenaTimeToHaveStar;
    public float GetMaximumArenaTimeToHaveStar { get { return maximumArenaTimeToHaveStar; } }
}
