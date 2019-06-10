using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Ancienne classe d'input du joueur
/// </summary>
[System.Serializable]
public class InputManager
{
    public void SetUp()
    {
        currentSwipes = new List<Swipe>();
    }

    public static Vector3 ScreenToWorldPoint(Vector3 screenPosition)
    {
        Vector3 worldPosition = new Vector3();

        Ray targetRay = GameManager.gameManager.MainCamera.ScreenPointToRay(screenPosition);

        Debug.DrawRay(targetRay.origin, targetRay.direction * 1000, Color.red);

        RaycastHit[] hits = Physics.RaycastAll(targetRay, 1000);

        foreach(RaycastHit hit in hits)
        {
            if (hit.collider.GetComponent<InputSurface>())
            {
                worldPosition = hit.point;
                break;
            }
        }

        return worldPosition;
    }

    public static Vector3 RelativeScreenToWorldPoint(Vector3 screenPosition, Vector3 refPosition)
    {
        Vector3 worldPosition = new Vector3();

        Ray targetRay = GameManager.gameManager.MainCamera.ScreenPointToRay(screenPosition);

        Debug.DrawRay(targetRay.origin, targetRay.direction * 1000, Color.red);

        RaycastHit[] hits = Physics.RaycastAll(targetRay, 1000);

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.GetComponent<InputSurface>())
            {
                worldPosition = hit.point;
                break;
            }
        }

        return worldPosition - refPosition;
    }

    #region Swipes
    List<Swipe> currentSwipes;


    public void StartNewSwipe(Vector3 screenPosition, int touchIndex)
    {
        Swipe newSwipe = new Swipe();

        newSwipe.StartSwipe(screenPosition, touchIndex);

        currentSwipes.Add(newSwipe);
    }

    public void StartNewSwipe(Vector3 screenPosition, int touchIndex, Vector3 refPosition)
    {
        Swipe newSwipe = new Swipe();

        newSwipe.StartSwipe(screenPosition, touchIndex, refPosition);

        currentSwipes.Add(newSwipe);
    }


    public Swipe UpdateSwipe(Vector3 screenPosition, int touchIndex)
    {
        foreach(Swipe swipe in currentSwipes)
        {
            if(swipe.GetTouchIndex == touchIndex)
            {
                swipe.UpdateSwipe(screenPosition);
                swipe.ShowSwipe();
                return swipe;
            }
        }
        return null;
    }

    public Swipe UpdateSwipe(Vector3 screenPosition, int touchIndex, Vector3 refPosition)
    {
        foreach (Swipe swipe in currentSwipes)
        {
            if (swipe.GetTouchIndex == touchIndex)
            {
                swipe.UpdateSwipe(screenPosition, refPosition);
                swipe.ShowSwipe();
                return swipe;
            }
        }
        return null;
    }


    public Swipe EndSwipe(Vector3 screenPosition, int touchIndex)
    {
        foreach (Swipe swipe in currentSwipes)
        {
            if (swipe.GetTouchIndex == touchIndex)
            {
                swipe.EndSwipe(screenPosition);
                currentSwipes.Remove(swipe);
                return swipe;
            }
        }
        return null;
    }

    public Swipe EndSwipe(Vector3 screenPosition, int touchIndex, Vector3 refPosition)
    {
        foreach (Swipe swipe in currentSwipes)
        {
            if (swipe.GetTouchIndex == touchIndex)
            {
                swipe.EndSwipe(screenPosition, refPosition);
                currentSwipes.Remove(swipe);
                return swipe;
            }
        }
        return null;
    }
    #endregion
}

public class Swipe
{
    Vector3 startPosition;
    Vector3 endPosition;
    int touchIndex;

    public void StartSwipe(Vector3 startScreenPosition, int index)
    {
        Vector3 worldPosition = InputManager.ScreenToWorldPoint(startScreenPosition);
        startPosition = worldPosition;
        touchIndex = index;
    }

    public void StartSwipe(Vector3 startScreenPosition, int index, Vector3 refPosition)
    {
        Vector3 worldPosition = InputManager.RelativeScreenToWorldPoint(startScreenPosition, refPosition);
        startPosition = worldPosition;
        touchIndex = index;
    }


    public void UpdateSwipe(Vector3 currentScreenPosition)
    {
        Vector3 worldPosition = InputManager.ScreenToWorldPoint(currentScreenPosition);
        endPosition = worldPosition;
    }

    public void UpdateSwipe(Vector3 currentScreenPosition, Vector3 refPosition)
    {
        Vector3 worldPosition = InputManager.RelativeScreenToWorldPoint(currentScreenPosition, refPosition);
        endPosition = worldPosition;
    }


    public void EndSwipe(Vector3 currentScreenPosition)
    {
        Vector3 worldPosition = InputManager.ScreenToWorldPoint(currentScreenPosition);
        endPosition = worldPosition;
    }

    public void EndSwipe(Vector3 currentScreenPosition, Vector3 refPosition)
    {
        Vector3 worldPosition = InputManager.RelativeScreenToWorldPoint(currentScreenPosition, refPosition);
        endPosition = worldPosition;
    }


    public Vector3 GetSwipeDirection
    {
        get
        {
            return (endPosition - startPosition).normalized;
        }
    }

    public float GetSwipeDistance
    {
        get
        {
            return (endPosition - startPosition).magnitude;
        }
    }

    public int GetTouchIndex
    {
        get
        {
            return touchIndex;
        }
    }

    #region Debug
    public void ShowSwipe()
    {
        Debug.DrawLine(startPosition, endPosition, Color.red);
    }
    #endregion
}