using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Gouvernail du joueur
/// </summary>
public class ShipsWheel : MonoBehaviour
{
    /// <summary>
    /// Multiplicateur contrôlant la vitesse de rotation de la barre par rapport à la vitesse de déplacement du doigt sur la barre
    /// </summary>
    [Header("Balancing")]
    [SerializeField] float wheelInputRotationMultiplicator;
    /// <summary>
    /// Vitesse de rotation maximale de la barre (par seconde)
    /// </summary>
    [SerializeField] float maxWheelRotation;
    /// <summary>
    /// Descelération de la vitesse de rotation de la barre (par seconde)
    /// </summary>
    [SerializeField] float wheelRotationDesceleration;
    /// <summary>
    /// Vitesse de rotation actuelle de la barre
    /// </summary>
    float currentWheelRotationSpeed;
    /// <summary>
    /// La barre n'est actuellement pas contrôllée par le doigt du joueur
    /// </summary>
    bool free = true;
    /// <summary>
    /// Renvoit le poucentage de rotation actuel (0% : la barre ne tourne pas ; 100% : la barre tourne à sa vitesse maximale)
    /// </summary>
    public float GetRotationSpeedPercent
    {
        get
        {
            return -currentWheelRotationSpeed / maxWheelRotation;
        }
    }

    /// <summary>
    /// Référence au système de déplacement du joueur
    /// </summary>
    [Header("References")]
    ShipMovements playerShipMovements;

    /// <summary>
    /// Initialisation de la barre du bateau
    /// </summary>
    /// <param name="movements"></param>
    public void SetUp(ShipMovements movements)
    {
        playerShipMovements = movements;
    }

    #region Controlled Wheel Rotation
    /// <summary>
    /// Commence le contrôle de la barre par le joueur
    /// </summary>
    public void StartControlledWheelRotation()
    {
        free = false;
        currentWheelRotationSpeed = 0;
    }

    /// <summary>
    /// Actualise le contrôle de la barre par le joueur
    /// </summary>
    /// <param name="delta">Différence de positionnement du doigt du joueur en X</param>
    public void UpdateControlledWheelRotation(float delta)
    {
        delta = Mathf.Clamp(delta, -maxWheelRotation, maxWheelRotation);

        currentWheelRotationSpeed = delta;

        transform.Rotate(new Vector3(0, 0, delta * wheelInputRotationMultiplicator * Time.deltaTime));
    }
    #endregion

    #region Free Wheel Rotation
    /// <summary>
    /// Commence la rotation libre de la barre
    /// </summary>
    /// <param name="delta">Déplacement du doigt en X au moment où le joueur lâche la barre</param>
    public void StartFreeWheelRotation(float delta)
    {
        free = true;
        currentWheelRotationSpeed = Mathf.Clamp(delta, -maxWheelRotation, maxWheelRotation);
    }

    /// <summary>
    /// Actualise la rotation libre de la barre
    /// </summary>
    public void UpdateFreeWheelRotation()
    {
        transform.Rotate(new Vector3(0, 0, currentWheelRotationSpeed * wheelInputRotationMultiplicator * Time.deltaTime));

        if(currentWheelRotationSpeed != 0)
            currentWheelRotationSpeed -= Mathf.Sign(currentWheelRotationSpeed) * wheelRotationDesceleration * Time.deltaTime;

        if (Mathf.Abs(currentWheelRotationSpeed) < wheelRotationDesceleration * Time.deltaTime)
            currentWheelRotationSpeed = 0;
    }
    #endregion

    private void Update()
    {
        if (free)
            UpdateFreeWheelRotation();

        playerShipMovements.UpdateMovementValues(GetRotationSpeedPercent);
    }
}
