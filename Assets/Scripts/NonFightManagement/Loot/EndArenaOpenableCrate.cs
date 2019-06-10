using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public delegate void OnCrateOpened(EndArenaOpenableCrate crate);

public class EndArenaOpenableCrate : MonoBehaviour
{
    ShipEquipment containedEquipment;
    public ShipEquipment GetContainedEquipment { get { return containedEquipment; } }

    /*[SerializeField] Image crateImage;
    [SerializeField] Image lootedEquipmentImage;*/
    [SerializeField] MeshRenderer crateRenderer;
    [SerializeField] Animator crateAnimator;
    [SerializeField] SpriteRenderer lootedEquipmentImage;
    bool openable;

    float remainingTimeBeforeAppear;
    Vector3 basePos;

    public OnCrateOpened OnCrateOpened;

    private void Start()
    {
        openable = true;
    }

    public void SetUpCrate(ShipEquipment equipment, Transform cameraTransform, float waitTimeBeforeAppear)
    {
        basePos = transform.localPosition;

        Vector3 lookDirection = cameraTransform.position - transform.position;

        if (Mathf.Abs(lookDirection.z) > 1)
        {
            float rotY = Mathf.Atan2(lookDirection.x, lookDirection.z) * Mathf.Rad2Deg;
            transform.localRotation = Quaternion.Euler(new Vector3(0, 180 + rotY, 0));
        }

        containedEquipment = equipment;
        gameObject.SetActive(true);
        crateRenderer.gameObject.SetActive(true);

        if (lootedEquipmentImage != null)
        {
            lootedEquipmentImage.gameObject.SetActive(false);
            if(containedEquipment.GetEquipmentInformations.GetEquipmentIcon != null)
                lootedEquipmentImage.sprite = containedEquipment.GetEquipmentInformations.GetEquipmentIcon;
        }

        openable = true;
        //Interaction = OpenCrate;
        if (waitTimeBeforeAppear != 0)
            remainingTimeBeforeAppear = waitTimeBeforeAppear;
        else
            Appear();
    }

    public void Appear()
    {
        crateAnimator.SetTrigger("appear");
    }

    bool highlighted;
    public void StartCrateHighlight()
    {
        highlighted = true;
        crateAnimator.SetBool("highlight", highlighted);
    }

    public void EndCrateHighlight()
    {
        highlighted = false;
        crateAnimator.SetBool("highlight", highlighted);
    }

    public void OpenCrate()
    {
        if (!openable)
            return;

        crateAnimator.SetTrigger("open");

        openable = false;
        highlighted = false;

        //crateRenderer.gameObject.SetActive(false);
        //lootedEquipmentImage.gameObject.SetActive(true);

        OnCrateOpened(this);
    }

    private void Update()
    {
        if (remainingTimeBeforeAppear > 0)
            remainingTimeBeforeAppear -= Time.deltaTime;
        else if (remainingTimeBeforeAppear < 0)
        {
            remainingTimeBeforeAppear = 0;
            Appear();
        }
        /*if (highlighted)
            transform.localPosition = basePos + new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f));*/
    }
}
