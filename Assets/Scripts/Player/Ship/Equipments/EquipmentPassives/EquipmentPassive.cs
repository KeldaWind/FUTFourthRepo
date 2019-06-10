using UnityEngine;
using UnityEditor;

/// <summary>
/// Passif d'un équipement
/// </summary>
[CreateAssetMenu(fileName = "NewPassive", menuName = "Equipments and Co/Passive", order = 2)]
public class EquipmentPassive : ScriptableObject
{
    /// <summary>
    /// Type de passif
    /// </summary>
    [Header("Common Parameters")]
    [SerializeField] PassiveType passiveType;
    /// <summary>
    /// Type de passif
    /// </summary>
    public PassiveType GetPassiveType
    {
        get
        {
            return passiveType;
        }
    }

    #region temp test
    [HideInInspector] public int healthBonus;
    [HideInInspector] public int damagesBonus;
    [HideInInspector] public float speedBonus;
    [HideInInspector] public float reloadBonus;
    #endregion
}

public enum PassiveType
{
    Health, Damages, Speed, Reload
}

/*
[CustomEditor(typeof(EquipmentPassive))]
public class EquipmentPassiveEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EquipmentPassive relatedPasssive = (EquipmentPassive)target;

        #region temp test
        switch (relatedPasssive.GetPassiveType)
        {
            case (PassiveType.Health):
                relatedPasssive.healthBonus = EditorGUILayout.IntField("Health Bonus", relatedPasssive.healthBonus);
                break;

            case (PassiveType.Damages):
                relatedPasssive.damagesBonus = EditorGUILayout.IntField("Damages Bonus", relatedPasssive.damagesBonus);
                break;

            case (PassiveType.Speed):
                relatedPasssive.speedBonus = EditorGUILayout.FloatField("Speed Bonus", relatedPasssive.speedBonus);
                break;

            case (PassiveType.Reload):
                relatedPasssive.reloadBonus = EditorGUILayout.FloatField("Reload Bonus", relatedPasssive.reloadBonus);
                break;
        }
        #endregion 
    }
}*/