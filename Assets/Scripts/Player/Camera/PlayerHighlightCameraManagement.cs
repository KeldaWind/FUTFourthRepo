using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerHighlightCameraManagement
{
    [SerializeField] List<PlayerShipHighlightCamera> highlightCamera;
    bool needToCheck;
    float waitTime;

    public void Initialize()
    {
        foreach (PlayerShipHighlightCamera cam in highlightCamera)
        {
            cam.InitializeCamera();
        }
    }

    public void ActivateHighlightCamera()
    {
        needToCheck = true;
        waitTime = 0.01f;

        foreach (PlayerShipHighlightCamera cam in highlightCamera)
        {
            cam.StartCheck();
        }
    }

    public void UpdateManagement()
    {
        if (needToCheck)
        {
            if (waitTime > 0)
                waitTime -= Time.deltaTime;
            else if(waitTime < 0)
            {
                needToCheck = false;
                waitTime = 0;

                bool found = false;
                foreach (PlayerShipHighlightCamera cam in highlightCamera)
                {
                    if (!found)
                    {
                        if (cam.Usable)
                        {
                            found = true;
                            cam.SetOn();
                        }
                        else
                            cam.SetOff();
                    }
                    else
                        cam.SetOff();
                }
            }
        }
    }

    public void DesactivateHighlightCamera()
    {
        foreach (PlayerShipHighlightCamera cam in highlightCamera)
        {
            cam.SetOff();
        }
    }
}
