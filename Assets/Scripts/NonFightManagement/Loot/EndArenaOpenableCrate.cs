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
    [SerializeField] Text lootedEquipmentText;
    [SerializeField] AudioSource crateAudioSource;
    [SerializeField] Sound appearingSound;
    [SerializeField] Sound highlightSound;
    [SerializeField] Sound level1OpeningSound;
    [SerializeField] Sound level2OpeningSound;
    [SerializeField] Sound level3OpeningSound;
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

        lootedEquipmentText.text = containedEquipment.GetEquipmentInformations.GetEquipmentName;
        lootedEquipmentText.enabled = false;
    }

    public void Appear()
    {
        lootedEquipmentText.enabled = false;
        crateAnimator.SetTrigger("appear");
    }

    bool highlighted;
    public void StartCrateHighlight()
    {
        highlighted = true;
        crateAnimator.SetBool("highlight", highlighted);
        crateAudioSource.PlaySound(highlightSound);
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
    }

    public void PlayAppearingSound()
    {
        crateAudioSource.PlaySound(appearingSound);
    }

    public void PlayOpeningSound()
    {
        if (containedEquipment.GetEquipmentQuality == EquipmentQuality.Common)
            crateAudioSource.PlaySound(level1OpeningSound);
        else if (containedEquipment.GetEquipmentQuality == EquipmentQuality.Rare)
            crateAudioSource.PlaySound(level2OpeningSound);
        else if (containedEquipment.GetEquipmentQuality == EquipmentQuality.Legendary)
            crateAudioSource.PlaySound(level3OpeningSound);
    }
}
