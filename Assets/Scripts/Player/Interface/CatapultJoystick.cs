using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CatapultJoystick : MonoBehaviour
{
    public void SetUp(float minDistance, float maxDistance)
    {
        //catapultJoystickInnerMask.rectTransform.sizeDelta = new Vector2(minDistance * 2, minDistance * 2);
        minJoystickIndicatorDistance = minDistance;

        //catapultJoystickOuterMask.rectTransform.sizeDelta = new Vector2(maxDistance * 2, maxDistance * 2);
        maxJoystickIndicatorDistance = maxDistance;

        catapultJoystickIndicatorImageBasePositon = catapultJoystickIndicatorImage.transform.localPosition;

        //catapultJoystickOuterMask.enabled = false;
        catapultJoystickIndicatorImage.enabled = false;
        catapultJoystickOutline.enabled = false;
        catapultJoystickInnerMask.enabled = false;
    }

    [SerializeField] Image catapultJoystickInnerMask;
    float minJoystickIndicatorDistance;
    //[SerializeField] Image catapultJoystickOuterMask;
    float maxJoystickIndicatorDistance;
    [SerializeField] Image catapultJoystickIndicatorImage;
    Vector2 catapultJoystickIndicatorImageBasePositon;
    [SerializeField] Image catapultJoystickOutline;

    [SerializeField] Color minColor;
    [SerializeField] Color maxColor;

    public void StartCatapultJoystickUse()
    {
        //catapultJoystickOuterMask.enabled = true;
        catapultJoystickInnerMask.enabled = true;
        catapultJoystickIndicatorImage.enabled = true;
        catapultJoystickIndicatorImage.transform.localPosition = catapultJoystickIndicatorImageBasePositon;
        catapultJoystickOutline.enabled = true;
    }

    public void UpdateIndicatorPositionUse(Vector2 percentageVector)
    {
        float percentage = percentageVector.magnitude;
        Vector2 offsetVector = percentageVector.normalized * Mathf.Lerp(minJoystickIndicatorDistance, maxJoystickIndicatorDistance, percentage);
        catapultJoystickIndicatorImage.transform.localPosition = catapultJoystickIndicatorImageBasePositon + offsetVector;
        catapultJoystickIndicatorImage.color = Color.Lerp(minColor, maxColor, percentage);
    }

    public void EndCatapultJoystickUse()
    {
        //catapultJoystickOuterMask.enabled = false;
        catapultJoystickInnerMask.enabled = false;
        catapultJoystickIndicatorImage.enabled = false;
        catapultJoystickIndicatorImage.transform.localPosition = catapultJoystickIndicatorImageBasePositon;
        catapultJoystickOutline.enabled = false;
    }
}
