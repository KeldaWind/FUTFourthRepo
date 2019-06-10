using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the screenshake of all the game
/// </summary>
[System.Serializable]
public class ScreenshakeManager {
    /// <summary>
    /// The main Camera of the game
    /// </summary>
    [SerializeField] suiviPourLaCam screenshakeObject;
    /// <summary>
    /// The normal position of the camera, when the screen isn't shaked
    /// </summary>
    Vector3 normalScreenshakeObjectLocalPosition;

    /// <summary>
    /// The currently occuring shake parameters
    /// </summary>
    [SerializeField] List<ScreenshakeParameters> currentScreenshakeParameters;

    /// <summary>
    /// Call this on start to set up the screenshake by attributing references
    /// </summary>
    /// <param name="mC">The main camera of the game</param>
    public void SetUpScreenshake()
    {
        currentScreenshakeParameters = new List<ScreenshakeParameters>();
    }

    public void AddScreenshakeParameters(ScreenshakeParameters parameters)
    {
        currentScreenshakeParameters.Add(parameters);
    }

    public void RemoveScreenshakeParameters(ScreenshakeParameters parameters)
    {
        if (currentScreenshakeParameters.Contains(parameters))
            currentScreenshakeParameters.Remove(parameters);
    }

    public float GetTotalForce()
    {
        float totalForce = 0;

        List<ScreenshakeParameters> parametersToDelete = new List<ScreenshakeParameters>();

        foreach (ScreenshakeParameters parameters in currentScreenshakeParameters)
        {
            float newForce = parameters.UpdateShakeDurationAndGetCurrentForce(this);
            if (newForce > 0.005f)
                totalForce += newForce;
            else
                parametersToDelete.Add(parameters);
        }

        foreach(ScreenshakeParameters parameters in parametersToDelete)
        {
            RemoveScreenshakeParameters(parameters);
        }

        return totalForce;
    }

    public List<ScreenshakeDirection> GetParametersTypesList()
    {
        List<ScreenshakeDirection> list = new List<ScreenshakeDirection>();
        foreach (ScreenshakeParameters parameters in currentScreenshakeParameters)
            list.Add(parameters.Type);
        return list;
    }

    /// <summary>
    /// Call this method to start a screenshake with the inputed parameters
    /// </summary>
    /// <param name="parameters">The parameters of the screenshake that is going to start</param>
    public void StartScreenshake(ScreenshakeParameters parameters)
    {
        ScreenshakeParameters newParam = new ScreenshakeParameters(parameters.Force, parameters.Duration, parameters.Attenuation, parameters.Type);

        AddScreenshakeParameters(newParam);
    }

    /// <summary>
    /// Updates the state of the screenshake
    /// </summary>
    public void UpdateScreenshake()
    {
        float force = GetTotalForce();
        List<ScreenshakeDirection> list = GetParametersTypesList();

        float xPos = Random.Range(-force, force) * (list.Contains(ScreenshakeDirection.X) || list.Contains(ScreenshakeDirection.XY) || list.Contains(ScreenshakeDirection.XZ) || list.Contains(ScreenshakeDirection.XYZ) ? 1 : 0);
        float yPos = Random.Range(-force, force) * (list.Contains(ScreenshakeDirection.Y) || list.Contains(ScreenshakeDirection.XY) || list.Contains(ScreenshakeDirection.YZ) || list.Contains(ScreenshakeDirection.XYZ)? 1 : 0);
        float zPos = Random.Range(-force, force) * (list.Contains(ScreenshakeDirection.Z) || list.Contains(ScreenshakeDirection.XZ) || list.Contains(ScreenshakeDirection.YZ) || list.Contains(ScreenshakeDirection.XYZ) ? 1 : 0);

        screenshakeObject.SetOffset(new Vector3(xPos, yPos, zPos));
    }

    public void SetUpNormalCamPos(Vector3 normalPos)
    {
        normalScreenshakeObjectLocalPosition = normalPos;
    }
}

/// <summary>
/// The parameters of a screenshake
/// </summary>
[System.Serializable]
public class ScreenshakeParameters
{
    public ScreenshakeParameters(float forc, float dur, float attenuation, ScreenshakeDirection typ)
    {
        force = forc;
        duration = dur;
        attenuationCoeff = attenuation;
        type = typ;
    }

    /// <summary>
    /// The force of the screenshake.
    /// </summary>
    [SerializeField] float force;
    /// <summary>
    /// The force of the screenshake.
    /// </summary>
    public float Force
    {
        get
        {
            return force;
        }
    }

    /// <summary>
    /// The duration of the screenshake
    /// </summary>
    [SerializeField] float duration;
    public float Duration
    {
        get
        {
            return duration;
        }
    }

    /// <summary>
    /// The coefficient of reduction of the screenshake. Never let it at 0 or less. 1 for instant end.
    /// </summary>
    [SerializeField] float attenuationCoeff;
    public float Attenuation
    {
        get
        {
            return attenuationCoeff;
        }
    }

    /// <summary>
    /// The type of the screenshake (coordinates that will change)
    /// </summary>
    [SerializeField] ScreenshakeDirection type;
    /// <summary>
    /// The type of the screenshake (coordinates that will change)
    /// </summary>
    public ScreenshakeDirection Type
    {
        get
        {
            return type;
        }
    }

    /// <summary>
    /// Updates the duration and force of the screenshake, and also returns the current force
    /// </summary>
    /// <returns></returns>
    public float UpdateShakeDurationAndGetCurrentForce(ScreenshakeManager screenshakeManager)
    {
        if (duration > 0)
            duration -= Time.deltaTime;
        else if (duration < 0)
            duration = 0;
        else if (duration == 0)
        {
            force = Mathf.Lerp(force, 0, attenuationCoeff);
        }

        return force;
    }

    public static ScreenshakeParameters Lerp(ScreenshakeParameters min, ScreenshakeParameters max, float coeff)
    {
        float force = Mathf.Lerp(min.force, max.force, coeff);
        float duration = Mathf.Lerp(min.duration, max.duration, coeff);
        float attenuation = Mathf.Lerp(min.attenuationCoeff, max.attenuationCoeff, coeff);

        return new ScreenshakeParameters(force, duration, attenuation, max.type);
    }
}

public enum ScreenshakeDirection
{
    X, Y, Z, XY, XZ, YZ, XYZ
}
