using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CompetenceSet
{
    Competence hullCompetence;
    public Competence HullCompetence
    {
        get
        {
            return hullCompetence;
        }
    }

    Competence mainWeaponPrimaryCompetence;
    public Competence MainWeaponPrimaryCompetence
    {
        get
        {
            return mainWeaponPrimaryCompetence;
        }
    }

    Competence mainWeaponSecondaryCompetence;
    public Competence MainWeaponSecondaryCompetence
    {
        get
        {
            return mainWeaponSecondaryCompetence;
        }
    }

    Competence secondaryWeaponCompetence;
    public Competence SecondaryWeaponCompetence
    {
        get
        {
            return secondaryWeaponCompetence;
        }
    }


    /// <summary>
    /// Affects all competences to the slots
    /// </summary>
    /// <param name="equipmentSet"></param>
    public void ComposeCompetenceSet(EquipmentsSet equipmentSet)
    {
        if (equipmentSet.GetHullEquipment != null)
        {
            if (equipmentSet.GetHullEquipment.GetPrimaryComp != null)
            {
                hullCompetence = ScriptableObject.Instantiate(equipmentSet.GetHullEquipment.GetPrimaryComp);
                hullCompetence.LinkEquipment(equipmentSet.GetHullEquipment);
            }
        }

        if (equipmentSet.GetMainWeaponEquipment != null)
        {
            if (equipmentSet.GetMainWeaponEquipment.GetPrimaryComp != null)
            {
                mainWeaponPrimaryCompetence = ScriptableObject.Instantiate(equipmentSet.GetMainWeaponEquipment.GetPrimaryComp);
                mainWeaponPrimaryCompetence.LinkEquipment(equipmentSet.GetMainWeaponEquipment);
            }
        }

        if (equipmentSet.GetSecondaryWeaponEquipment != null)
        {
            if (equipmentSet.GetSecondaryWeaponEquipment.GetPrimaryComp != null)
            {
                secondaryWeaponCompetence = ScriptableObject.Instantiate(equipmentSet.GetSecondaryWeaponEquipment.GetPrimaryComp);
                secondaryWeaponCompetence.LinkEquipment(equipmentSet.GetSecondaryWeaponEquipment);
            }
        }
    }
}
