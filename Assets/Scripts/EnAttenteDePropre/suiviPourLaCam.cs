using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class suiviPourLaCam : MonoBehaviour
{
    [SerializeField] Transform playerPos;
    Vector3 camOffset;

    void FixedUpdate()
    {
        transform.position = playerPos.position + camOffset;
    }

    public void SetOffset(Vector3 offset)
    {
        camOffset = offset;
    }
}
    