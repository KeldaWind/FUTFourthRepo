using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ProtectionParameters
{
    public static ProtectionParameters UnlimitedLifeProtection
    {
        get
        {
            return new ProtectionParameters { unlimitedProtection = true, protectionDuration = 0.01f, lifeProtection = true }; 
        }
    }

    bool unlimitedProtection;

    [Header("Base Parameters")]
    [SerializeField] float protectionDuration;
    public float GetProtectionDuration { get { return protectionDuration; } }

    [SerializeField] bool lifeProtection;
    public bool IsLifeProtection { get { return lifeProtection; } }

    [SerializeField] bool knockbackProtection;
    public bool IsKnockbackProtection { get { return knockbackProtection; } }

    [Header("Protection Sphere")]
    [SerializeField] bool generatesProtectionSphere;
    public bool GeneratesProtectionSphere { get { return generatesProtectionSphere; } }

    [SerializeField] float protectionSphereRadius;
    public float GetProtectionSphereRadius { get { return protectionSphereRadius; } }

    [SerializeField] ProtectionSpherePoolTag protectionSphereType;
    public ProtectionSpherePoolTag GetProtectionSphereType { get { return protectionSphereType; } }

    #region Variables and Management
    float remainingProtectionTime;
    public bool Protecting { get { return remainingProtectionTime != 0; } }

    public void StartProtection()
    {
        remainingProtectionTime = protectionDuration;
    }

    public void UpdateProtection()
    {
        if (unlimitedProtection)
            return;

        if (remainingProtectionTime > 0)
            remainingProtectionTime -= Time.deltaTime;
        else if (remainingProtectionTime < 0)
            remainingProtectionTime = 0;
    }
    #endregion
}
