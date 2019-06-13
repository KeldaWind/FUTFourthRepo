using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapArenaSpot : MapSpecialPlaceSpot
{
    float waitTimeBeforePlayUnlockCinematic = 0.5f;
    bool waitingForCinematic;

    private void Update()
    {
        if(waitingForCinematic)
        {
            if (waitTimeBeforePlayUnlockCinematic > 0)
                waitTimeBeforePlayUnlockCinematic -= Time.deltaTime;
            else if (waitTimeBeforePlayUnlockCinematic < 0)
            {
                PlayUnlockCinematic();
                waitTimeBeforePlayUnlockCinematic = 0;
            }
        }
    }

    [Header("Arena Parameters")]
    [SerializeField] ArenaParameters arenaParameters;
    public ArenaParameters GetArenaParameters
    {
        get
        {
            return arenaParameters;
        }
    }

    public override void StartSpotInteraction(PlayerShip player)
    {
        base.StartSpotInteraction(player);

        MapManager.mapManager.MpArenaManager.OpenArenaPanel(cameraWhenOnSpot, this);
    }

    #region Progression
    [Header("Progression")]
    [SerializeField] List<MapSpecialPlaceSpot> nextSpecialPlaceUnlockOnPass;
    [SerializeField] IntroControler cinematicToPlayOnArenaPassed;
    int starsNumber;
    public int GetStarsNumber { get { return starsNumber; } }

    public void UnlockNextProgressionContent(bool justPassed, int stars)
    {
        /*foreach(MapSpecialPlaceSpot spot in nextSpecialPlaceUnlockOnPass)
        {
            spot.gameObject.SetActive(true);
        }*/
        if (justPassed)
        {
            if (cinematicToPlayOnArenaPassed != null)
                waitingForCinematic = true;
            else
            {
                foreach (MapSpecialPlaceSpot spot in nextSpecialPlaceUnlockOnPass)
                    spot.UnlockSpot(true);
            }
        }
        else
        {
            foreach (MapSpecialPlaceSpot spot in nextSpecialPlaceUnlockOnPass)
                spot.UnlockSpot(false);
        }


        starsNumber = stars;
    }

    public void PlayUnlockCinematic()
    {
        waitingForCinematic = false;
        cinematicToPlayOnArenaPassed.PlayCinematic();
    }
    #endregion
}
