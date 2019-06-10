using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Cinemachine;
using UnityEngine.UI;
using NaughtyAttributes;


public class IntroControler : MonoBehaviour
{
    [SerializeField] bool isNotSkippable;
    public bool Skippable { get { return !isNotSkippable; } }
    [Header("Mouvement De Cam")]
    [SerializeField] CinematicPart[] allParts;
    public CinematicParameters GetCinematicParameters
    {
        get
        {
            return new CinematicParameters(allParts);
        }
    }
    [Header("Ennemies")]
    [SerializeField] bool stopSpawnedEnemies;
    public bool StopSpawnedEnemies { get { return stopSpawnedEnemies; } }

    bool ended;
    public bool Ended
    {
        get
        {
            return ended;
        }
    }

    public void SetCinematicEnded()
    {
        ended = true;
    }

    public void PlayCinematic()
    {
        GameManager.gameManager.CinematicMng.StartCinematic(this);
    }

    public void PlayCinematic(List<EnemyShip> enemiesToStartOnSpecificPart)
    {
        GameManager.gameManager.CinematicMng.StartCinematic(this, enemiesToStartOnSpecificPart);
    }

    public void PlayCinematic(OnCinematicEnd actionAfterCinematic, bool keepCamera)
    {
        GameManager.gameManager.CinematicMng.StartCinematic(this, actionAfterCinematic, keepCamera);
    }

    public void PlayCinematic(List<EnemyShip> enemiesToStartOnSpecificPart, OnCinematicEnd actionAfterCinematic, bool keepCamera)
    {
        GameManager.gameManager.CinematicMng.StartCinematic(this, enemiesToStartOnSpecificPart, actionAfterCinematic, keepCamera);
    }
}

[System.Serializable]
public struct CinematicPart
{
    public CinematicType GetCinematicPartType
    {
        get
        {
            if(cam != null)
            {
                if (messageToType != null)
                    return CinematicType.DialogueAndCameraMoves;
                else
                    return CinematicType.OnlyCameraMoves;
            }
            else if(messageToType != null)
                return CinematicType.OnlyDialogue;
            else
                return CinematicType.Null;
        }
    }

    [SerializeField] float waitTimeOnceEnded;
    public float GetWaitTimeOnceEnded { get { return waitTimeOnceEnded; } }
    [SerializeField] bool movePlayerBoatWhileCinematic;
    public bool GetMovePlayerBoatWhileCinematic { get { return movePlayerBoatWhileCinematic; } }
    [SerializeField] bool dontGivePlayerControlBack;
    public bool GetDontGivePlayerControlBack { get { return dontGivePlayerControlBack; } }

    [Header("Camera")]
    public CinemachineVirtualCamera cam;

    [Header("Texte")]
    public CreateAText messageToType;

    [Header("Enemies")]
    [SerializeField] bool startSpawnedEnemies;
    public bool StartSpawnedEnemies { get { return startSpawnedEnemies; } }

    [Header("Animations and Actions")]
    [SerializeField] Animator[] animatorsToLaunchOnPartBeginning;
    public Animator[] GetAnimatorsToLaunchOnPartBeginning { get { return animatorsToLaunchOnPartBeginning; } }
    [SerializeField] UnityEvent eventToPlayOnCinematicPartStart;
    public void PlayStartEvent()
    {
        if (eventToPlayOnCinematicPartStart != null)
            eventToPlayOnCinematicPartStart.Invoke();
    }
    [SerializeField] UnityEvent eventToPlayOnCinematicPartEnd;
    public void PlayEndEvent()
    {
        if (eventToPlayOnCinematicPartEnd != null)
            eventToPlayOnCinematicPartEnd.Invoke();
    }

    public bool Empty
    {
        get
        {
            return cam == null;
        }
    }
}

public enum CinematicType
{
    Null, OnlyDialogue, OnlyCameraMoves, DialogueAndCameraMoves
}