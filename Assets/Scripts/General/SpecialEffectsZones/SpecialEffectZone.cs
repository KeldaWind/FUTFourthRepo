using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialEffectZone : MonoBehaviour
{
    [SerializeField] SpecialEffectZonePoolTag specialEffectZoneType;
    public SpecialEffectZonePoolTag GetSpecialEffectZoneType { get { return specialEffectZoneType; } }

    #region References
    [Header("Functionnality References")]
    [SerializeField] Collider zoneCollider;

    [Header("Feedback References")]
    [SerializeField] Renderer zoneRenderer;
    [SerializeField] Color zoneColor;
    Color clearZoneColor;
    float appearingCoeff = 0.3f;
    #endregion

    #region Management
    public virtual void SetUpZone(float duration, float size, object specialParameter)
    {
        gameObject.SetActive(true);
        zoneCollider.enabled = true;
        transform.localScale = Vector3.one * size;
        clearZoneColor = zoneColor;
        clearZoneColor.a = 0;
        zoneRenderer.material.color = clearZoneColor;
        zoneRemainingDuration = duration;
    }

    public virtual void UpdateZone()
    {
        if(zoneRemainingDuration > 0)
        {
            zoneRemainingDuration -= Time.deltaTime;
            zoneRenderer.material.color = Color.Lerp(zoneRenderer.material.color, zoneColor, appearingCoeff);
        }
        else if (zoneRemainingDuration < 0)
        {
            zoneRemainingDuration = 0;
            SetIneffective();
        }
        else
        {
            zoneRenderer.material.color = Color.Lerp(zoneRenderer.material.color, clearZoneColor, appearingCoeff);
            if (ReadyToBeTurnedOff)
                SetOff();
        }
    }

    /// <summary>
    /// A appeler quand on veut rendre la zone ineffective, sans la renvoyer de suite dans la pool (pour laisser le temps de finit les particles systems,...)
    /// </summary>
    public virtual void SetIneffective()
    {
        zoneCollider.enabled = false;
    }

    public virtual bool ReadyToBeTurnedOff
    {
        get
        {
            return zoneRenderer.material.color.a < 0.01f && zoneRemainingDuration == 0;
        }
    }

    /// <summary>
    /// A appeler quand tout ce qui devait se jouer sur cette zone est fini et qu'il faut la remettre dans la pool
    /// </summary>
    public virtual void SetOff()
    {
        gameObject.SetActive(false);
        GameManager.gameManager.PoolManager.ReturnSpecialEffectZone(this);
    }
    #endregion

    #region Variables
    protected float zoneRemainingDuration;
    #endregion

    #region Trigger
    public virtual void OnTriggerEnter(Collider other)
    {
        
    }

    public virtual void OnTriggerStay(Collider other)
    {
        
    }

    public virtual void OnTriggerExit(Collider other)
    {
        
    }
    #endregion

    private void Update()
    {
        UpdateZone();
    }
}
