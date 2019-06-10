using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Objet qui va transiter entre les scènes en conservant les informations qui doivent être conservées (inventaire du joueu,...)
/// </summary>
public class IntersceneManager : MonoBehaviour
{
    /// <summary>
    /// Instance unique du manager à travers tout le jeu
    /// </summary>
    public static IntersceneManager intersceneManager;

    /// <summary>
    /// Les données actuelles liées au joueur (équipements, inventaire,...)
    /// </summary>
    [Header("Player")]
    [SerializeField] PlayerIntersceneDatas playerDatas;
    /// <summary>
    /// Les données actuelles liées au joueur (équipements, inventaire,...)
    /// </summary>
    public PlayerIntersceneDatas GetPlayerDatas
    {
        get
        {
            return playerDatas;
        }
    }

    /// <summary>
    /// Utilisé pour composer les données à sauvegarder entre les différentes scènes
    /// </summary>
    /// <param name="playerEquipments">La nouvelle liste d'équipements du joueur</param>
    public void SetPlayerDatas(EquipmentsSet playerEquipedEquipments)
    {
        playerDatas.SetEquipedEquipements(playerEquipedEquipments);
    }

    /// <summary>
    /// Initialisation de cette object dans le menu du jeu
    /// </summary>
    public void SetUp()
    {
        intersceneManager = this;
        DontDestroyOnLoad(gameObject);
    }

    [Header("Map Informations")]
    [SerializeField] MapIntersceneInformations mapIntersceneInformations;
    public MapIntersceneInformations MapInterscInformations
    {
        get
        {
            return mapIntersceneInformations;
        }
    }

    [Header("Arena Informations")]
    [SerializeField] ArenaIntersceneInformations arenaIntersceneInformations;
    public ArenaIntersceneInformations ArenaInterscInformations { get { return arenaIntersceneInformations; } }
}
