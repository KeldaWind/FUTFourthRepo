using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] PlayerShip relatedShip;
    Vector3 baseOffset;

    public void Start()
    {
        baseOffset = transform.position - relatedShip.transform.position;
    }

    public void Update()
    {
        transform.position = relatedShip.transform.position + baseOffset;
    }
}
