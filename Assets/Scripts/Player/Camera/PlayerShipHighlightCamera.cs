using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
public class PlayerShipHighlightCamera : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera virtualCamera;
    List<Collider> collidingObjects;
    Vector3 normalLocalPosition;
    public void InitializeCamera()
    {
        collidingObjects = new List<Collider>();
        normalLocalPosition = transform.localPosition;
    }

    public void StartCheck()
    {
        gameObject.SetActive(true);
        transform.parent.position = GameManager.gameManager.Player.transform.position;
        transform.parent.localRotation = Quaternion.Euler(GameManager.gameManager.Player.transform.localRotation.eulerAngles);
    }

    public void SetOn()
    {
        virtualCamera.gameObject.SetActive(true);
        CinemachineTransposer transposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        transposer.m_FollowOffset = transform.position - transform.parent.position;
        Debug.DrawRay(transform.parent.position, transform.parent.position - transform.position, Color.red, 2f);
    }

    public void SetOff()
    {
        gameObject.SetActive(false);
        virtualCamera.gameObject.SetActive(false);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (!collidingObjects.Contains(other))
            collidingObjects.Add(other);
    }

    private void OnTriggerExit(Collider other)
    {
        if (collidingObjects.Contains(other))
            collidingObjects.Remove(other);
    }

    public bool Usable
    {
        get
        {
            return collidingObjects.Count == 0;
        }
    }
}
