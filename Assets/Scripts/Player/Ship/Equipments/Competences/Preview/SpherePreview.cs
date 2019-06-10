using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpherePreview : Preview
{
    public void ShowPreparePreview(Ship relatedShip, float radius)
    {
        gameObject.SetActive(true);
        transform.localScale = Vector3.one * radius;
        transform.parent = relatedShip.transform;
        transform.localPosition = Vector3.zero;
    }

    public override void HidePreparePreview()
    {
        gameObject.SetActive(false);
        GameManager.gameManager.PoolManager.ReturnPreview(this);
    }
}
