using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipHullRenderer : MonoBehaviour
{
    [SerializeField] GameObject frontCanon;
    [SerializeField] GameObject[] sideCanons;
    public void CheckObjectsToShow(WeaponInformationType weaponType)
    {
        if(weaponType == WeaponInformationType.FrontCanons)
        {
            if(frontCanon != null)
                frontCanon.SetActive(true);

            foreach(GameObject canon in sideCanons)
                if (canon != null)
                    canon.SetActive(false);
        }
        else if (weaponType == WeaponInformationType.SideCanons)
        {
            if (frontCanon != null)
                frontCanon.SetActive(false);

            foreach (GameObject canon in sideCanons)
                if (canon != null)
                    canon.SetActive(true);
        }
        else if (weaponType == WeaponInformationType.MultiCanons)
        {
            if (frontCanon != null)
                frontCanon.SetActive(true);

            foreach (GameObject canon in sideCanons)
                if (canon != null)
                    canon.SetActive(true);
        }
    }
}
