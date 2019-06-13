using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProtectionSphere : MonoBehaviour, IDamageReceiver
{
    AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] float animationOffset = 3;
    [SerializeField] float animationSpeed = 3;
    float animationCounter;

    private void Update()
    {
        UpdateSphere();
    }

    #region Management
    public void SetUpSphere(float duration, float radius, Transform parent, AttackTag attackTag)
    {
        animationCounter = 0.5f;
        curve.preWrapMode = WrapMode.PingPong;
        curve.postWrapMode = WrapMode.PingPong;

        gameObject.SetActive(true);
        protectionTag = attackTag;
        remainingLifeTime = duration;
        targetRadius = radius;
        transform.localScale = Vector3.zero;
        transform.parent = parent;
        transform.localPosition = Vector3.zero;

        if (protectionAudioSource != null)
            protectionAudioSource.PlaySound(protectionSphereOpen);
    }

    public void UpdateSphere()
    {
        if (remainingLifeTime > 0)
        {
            animationCounter += animationSpeed * Time.deltaTime;

            remainingLifeTime -= Time.deltaTime;
            transform.localScale = Mathf.Lerp(transform.localScale.x, targetRadius + curve.Evaluate(animationCounter) * animationOffset, expansionCoeff) * Vector3.one;
        }
        else if (remainingLifeTime < 0)
        {
            remainingLifeTime = 0;
            if (protectionAudioSource != null)
                protectionAudioSource.PlaySound(protectionSphereClose);
        }
        else
        {
            transform.localScale = Mathf.Lerp(transform.localScale.x, 0, expansionCoeff) * Vector3.one;
            if (transform.localScale.x < 0.1f)
                SetOff();
        }

    }

    public void SetOff()
    {
        gameObject.SetActive(false);
        GameManager.gameManager.PoolManager.ReturnProtectionSphere(this);
    }
    #endregion

    #region Pooling
    [Header("Pooling")]
    [SerializeField] ProtectionSpherePoolTag poolTag;
    public ProtectionSpherePoolTag GetProtectionSpherePoolTag { get { return poolTag; } }
    #endregion

    #region Sphere Parameters
    AttackTag protectionTag;
    float remainingLifeTime;
    float targetRadius;
    float expansionCoeff = 0.3f;
    #endregion

    #region Rendering
    [Header("Rendering")]
    [SerializeField] Renderer sphereRenderer;
    #endregion

    public AttackTag GetDamageTag { get { return protectionTag; } }

    public bool IsDamageable { get { return true; } }

    [Header("Feedbacks")]
    [SerializeField] AudioSource protectionAudioSource;
    [SerializeField] Sound protectionSphereOpen;
    [SerializeField] Sound protectionSphereClose;

    #region Useless
    public event OnLifeEvent OnDeath;

    public void Die()
    {
        
    }

    public void ReceiveDamage(IDamageSource damageSource, DamagesParameters damagesParameters, ProjectileSpecialParameters projSpecialParameters)
    {
        
    }

    public void UpdateLifeBar(int lifeAmount)
    {
    }
    #endregion
}
