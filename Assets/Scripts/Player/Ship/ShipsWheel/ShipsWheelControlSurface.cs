using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Surface permettant au joueur d'interagir avec la barre du bateau
/// </summary>
public class ShipsWheelControlSurface : MonoBehaviour
{
    /// <summary>
    /// Barre du bateau reliée à ce système
    /// </summary>
    [SerializeField] ShipsWheel relatedWheel;

    /// <summary>
    /// Index de contrôle actuel, correspond au doigt en train d'utiliser la barre. Si la barre n'est pas contrôlée, la valeur est de -1
    /// </summary>
    int currentControlIndex = -1;
    /// <summary>
    /// Index de contrôle actuel, correspond au doigt en train d'utiliser la barre. Si la barre n'est pas contrôlée, la valeur est de -1
    /// </summary>
    public int CurrentControlIndex
    {
        get
        {
            return currentControlIndex;
        }
    }

    /// <summary>
    /// Commence le contrôle de la barre par le joueur
    /// </summary>
    /// <param name="index">Index du doigt utilisé par le joueur pour contrôler la barre</param>
    public void StartWheelControl(int index)
    {
        if (currentControlIndex < 0)
        {
            threeLastsDeltaSpeeds = new List<float>();
            currentControlIndex = index;
            relatedWheel.StartControlledWheelRotation();
        }
    }

    /// <summary>
    /// Actualise le contrôle de la barre par le joueur
    /// </summary>
    public void UpdateWheelControl()
    {
        Touch controlTouch = GetTouchWithIndex(currentControlIndex);
        relatedWheel.UpdateControlledWheelRotation(-controlTouch.deltaPosition.x);
    }

    /// <summary>
    /// Termine le contrôle de la barre par le joueur
    /// </summary>
    public void EndWheelControl()
    {
        Touch controlTouch = GetTouchWithIndex(currentControlIndex);
        currentControlIndex = -1;
        relatedWheel.StartFreeWheelRotation(GetAverageDeltaSpeed);
    }

    /// <summary>
    /// Permet d'obtenir le touch relié à l'index de doigt indiqué
    /// </summary>
    /// <param name="index">Index du doigt utilisé</param>
    /// <returns>Touch relié à l'index du doigt utilisé</returns>
    public Touch GetTouchWithIndex(int index)
    {
        foreach (Touch touch in Input.touches)
        {
            if (touch.fingerId == index)
                return touch;
        }

        return new Touch();
    }

    /// <summary>
    /// Les trois dernier mouvement du doigt en X
    /// </summary>
    List<float> threeLastsDeltaSpeeds;
    /// <summary>
    /// Actualisation du delta moyen
    /// </summary>
    public void UpdateAverageDeltaSpeed()
    {
        Touch controlTouch = GetTouchWithIndex(currentControlIndex);
        threeLastsDeltaSpeeds.Add(-controlTouch.deltaPosition.x);

        if (threeLastsDeltaSpeeds.Count > 3)
            threeLastsDeltaSpeeds.RemoveAt(0);
    }

    /// <summary>
    /// Renvoie la moyenne de déplacement du doigt en X sur les dernières frames
    /// </summary>
    public float GetAverageDeltaSpeed
    {
        get
        {
            float totalSpeed = 0;
            foreach (float speed in threeLastsDeltaSpeeds)
                totalSpeed += speed;

            return totalSpeed / threeLastsDeltaSpeeds.Count;
        }
    }

    private void Update()
    {
        if (currentControlIndex >= 0)
        {
            UpdateWheelControl();
            UpdateAverageDeltaSpeed();
        }
    }
}
