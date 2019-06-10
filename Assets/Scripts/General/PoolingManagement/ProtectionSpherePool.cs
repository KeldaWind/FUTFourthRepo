using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ProtectionSpherePool : Pool<ProtectionSphere>
{
    public Transform GetPoolParent { get { return poolParent; } }

    [SerializeField] ProtectionSpherePoolTag protectionSphereTag;
    public ProtectionSpherePoolTag GetProtectionSpherePoolTag { get { return protectionSphereTag; } }
}

public enum ProtectionSpherePoolTag
{
    Normal
}
