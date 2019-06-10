using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LootPopUpObject : MonoBehaviour
{
    private void Update()
    {
        UpdateTimeAndPosition();
    }

    [Header("References")]
    [SerializeField] LootPopUpPoolTag popUpPoolTag;
    public LootPopUpPoolTag GetPopUpPoolTag { get { return popUpPoolTag; } }

    [SerializeField] SpriteRenderer equipmentSpriteRenderer;
    [SerializeField] Color equipmentSpriteNormalColor;
    [SerializeField] Text goldText;
    [SerializeField] Color goldTextNormalColor;

    [Header("Feedback Balancing")]
    [SerializeField] float floatingTime;
    [SerializeField] AnimationCurve floatingCurve;
    [SerializeField] float floatingHeight;
    Vector3 startPosition;
    float remainingTime;

    public void SetUp(Vector3 position, int goldAmount)
    {
        gameObject.SetActive(true);

        goldText.enabled = true;
        goldText.text = goldAmount + " G";
        goldText.color = goldTextNormalColor;

        equipmentSpriteRenderer.enabled = false;

        remainingTime = floatingTime;
        startPosition = position;
        transform.localScale = Vector3.zero;
    }

    public void SetUp(Vector3 position, Sprite equipmentSprite)
    {
        gameObject.SetActive(true);

        goldText.enabled = false;

        equipmentSpriteRenderer.enabled = true;
        /*equipmentSpriteRenderer.sprite = equipmentSprite;*/
        equipmentSpriteRenderer.color = equipmentSpriteNormalColor;

        remainingTime = floatingTime;
        startPosition = position;
        transform.localScale = Vector3.zero;
    }

    public float GetFloatingProgression
    {
        get
        {
            return 1 - (remainingTime / floatingTime);
        }
    }

    public void UpdateTimeAndPosition()
    {
        if (remainingTime > 0)
            remainingTime -= Time.deltaTime;
        else if (remainingTime < 0)
            remainingTime = 0;
        else if (remainingTime == 0)
        {
            if (equipmentSpriteRenderer.enabled)
            {
                Color imageColor = new Color(equipmentSpriteRenderer.color.r, equipmentSpriteRenderer.color.g, equipmentSpriteRenderer.color.b, Mathf.Lerp(equipmentSpriteRenderer.color.a, 0, 0.2f));
                equipmentSpriteRenderer.color = imageColor;
                if (imageColor.a < 0.01f)
                    Desactivate();
            }
            else if (goldText.enabled)
            {
                Color textColor = new Color(goldText.color.r, goldText.color.g, goldText.color.b, Mathf.Lerp(goldText.color.a, 0, 0.2f));
                goldText.color = textColor;
                if (textColor.a < 0.01f)
                    Desactivate();
            }
        }

        transform.localScale = Vector3.one * Mathf.Clamp(floatingCurve.Evaluate(GetFloatingProgression) * 2, 0, 1);
        transform.position = startPosition + new Vector3(0, floatingCurve.Evaluate(GetFloatingProgression) * floatingHeight, 0);
    }

    public void Desactivate()
    {
        gameObject.SetActive(false);

        //equipmentSpriteRenderer.enabled = false;
        //goldText.enabled = false;

        GameManager.gameManager.PoolManager.ReturnLootPopUp(this);
    }
}
