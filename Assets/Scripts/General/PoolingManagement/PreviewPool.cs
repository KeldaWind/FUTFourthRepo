using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PreviewPool : Pool<Preview>
{
    public Transform GetPoolParent { get { return poolParent; } }

    [SerializeField] PreviewPoolTag previewTag;
    public PreviewPoolTag GetPreviewPoolTag
    {
        get
        {
            return previewTag;
        }
    }
}

public enum PreviewPoolTag
{
    Line, Circle, Arrow, Sphere, Anchor, Boost
}
