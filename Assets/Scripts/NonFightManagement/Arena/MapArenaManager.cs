using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Cinemachine;

[System.Serializable]
public class MapArenaManager
{
    MapManager mapManager;
    public void SetUp(MapManager mpManager)
    {
        mapManager = mpManager;
        arenaInterface.SetUp(StartLaunchArena, CloseArenaPanel);
    }

    bool loadingArena;

    [SerializeField] MapArenaInterface arenaInterface;

    public void OpenArenaPanel(CinemachineVirtualCamera virtualCamera, MapArenaSpot arenaSpot)
    {
        virtualCamera.gameObject.SetActive(true);

        mapManager.SetLastActivatedVirtualCamera(virtualCamera);
        mapManager.SetOpenedSpecialPlace(arenaSpot);

        mapManager.SetPlayerNotPlayable();

        arenaInterface.OpenArenaPanel(arenaSpot.GetArenaParameters.GetDisplayName, arenaSpot.GetArenaParameters.GetDisplayDescription, arenaSpot.GetStarsNumber);

        mapManager.ShipCompoManager.OpenEquipedEquipmentPanel();
    }

    public void CloseArenaPanel()
    {
        mapManager.GetLastActivatedVirtualCamera?.gameObject.SetActive(false);

        mapManager.SetLastActivatedVirtualCamera(null);
        mapManager.SetOpenedSpecialPlace(null);

        mapManager.SetPlayerPlayable();

        mapManager.ShipCompoManager.CloseEquipedEquipmentPanel();

        arenaInterface.CloseArenaPanel();
    }

    string sceneToLoad;
    public void StartLaunchArena()
    {
        if (loadingArena)
            return;

        mapManager.SavePlayerDatas();

        MapArenaSpot arenaSpot = mapManager.GetOpenedSpecialPlace as MapArenaSpot;
        if (arenaSpot == null)
            return;

        EquipmentsSet equipmentsSet = mapManager.ShipCompoManager.ComposeEquipmentSet();
        if (equipmentsSet.GetHullEquipment == null)
        {
            Debug.LogWarning("Attention : il n'y a aucune coque sur ce bateau. Impossible de lancer le jeu.");
            return;
        }
        else
        {
            IntersceneManager.intersceneManager.SetPlayerDatas(equipmentsSet);
        }

        ArenaParameters launchedArenaParameters = arenaSpot.GetArenaParameters;
        string arenaBuildName = launchedArenaParameters.GetSceneToLoadName;

        try
        {
            IntersceneManager.intersceneManager.MapInterscInformations.SetMapIntersceneInfos(SceneManager.GetActiveScene().name, GameManager.gameManager.Player.transform.position);
            IntersceneManager.intersceneManager.ArenaInterscInformations.SetArenaLaunchInformations(launchedArenaParameters);

            //SceneManager.LoadSceneAsync(arenaBuildName);
            sceneToLoad = arenaBuildName;
            loadingArena = true;
            GameManager.gameManager.LoadScreenManager.StartEndLoad(LaunchArena);
        }
        catch
        {
            Debug.LogWarning("Cette scene n'existe pas ou n'as pas été ajoutée à la liste des scenes buildées");
        }
    }

    public void LaunchArena()
    {
        SceneManager.LoadSceneAsync(sceneToLoad);
    }
}
