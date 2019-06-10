using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLifePoint : MonoBehaviour
{
    [SerializeField] Image lifePointImage;
    [SerializeField] Animator lifePointAnimator;
    [SerializeField] RuntimeAnimatorController normalLifePointAnimatorController;
    [SerializeField] RuntimeAnimatorController armorLifePointAnimatorController;

    public void SetUp(LifePointType lifePointType)
    {
        switch (lifePointType)
        {
            case (LifePointType.NormalLifePoint):
                lifePointAnimator.runtimeAnimatorController = normalLifePointAnimatorController;
                break;

            case (LifePointType.ArmorLifePoint):
                lifePointAnimator.runtimeAnimatorController = armorLifePointAnimatorController;
                break;
        }

        ShowLifePoint();

        lifePointAnimator.SetBool("broken", false);
    }

    public void ShowLifePoint()
    {
        lifePointImage.enabled = true;
    }

    public void HideLifePoint()
    {
        lifePointImage.enabled = false;
    }

    public void SetPointBreaking()
    {
        lifePointAnimator.SetTrigger("breaking");
    }

    public void SetPointBroken()
    {
        lifePointAnimator.SetBool("broken", true);
    }
}

public enum LifePointType
{
    NormalLifePoint, ArmorLifePoint
}
