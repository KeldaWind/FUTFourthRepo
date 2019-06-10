using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CompetenceSlot : MonoBehaviour
{
    [SerializeField] EquipmentType equipmentType;
    public EquipmentType GetEquipmentType
    {
        get
        {
            return equipmentType;
        }
    }

    [Header("References")]
    [SerializeField] Image competenceIconImage;
    Color iconImageNormalColor;
    [SerializeField] Image cooldownFillImage;
    Color cooldownImageNormalColor;

    Competence relatedCompetence;
    Ship relatedShip;

    #region Setting up
    public void SetUp(Competence competence, Ship ship)
    {
        relatedShip = ship;

        if (iconImageNormalColor == null || iconImageNormalColor == Color.clear)
            iconImageNormalColor = competenceIconImage.color;
        if (cooldownImageNormalColor == null || cooldownImageNormalColor == Color.clear)
            cooldownImageNormalColor = cooldownFillImage.color;

        if (competence != null)
        {
            relatedCompetence = competence;
            totalCompetenceCooldown = relatedCompetence.GetCompetenceCooldown;
            currentCompetenceCooldown = 0;
            cooldownFillImage.fillAmount = 0;

            competenceIconImage.sprite = relatedCompetence.GetLinkEquipment.GetEquipmentInformations.GetEquipmentIcon;

            SetOn(true);
        }
        else
            SetOn(false);
    }
    #endregion

    private void Update()
    {
        UpdateCooldown();
    }

    #region Usable/Unusable
    bool isOn = true;
    public bool IsOn
    {
        get
        {
            return isOn;
        }
    }

    public void SetOn(bool on)
    {
        isOn = on;

        if (isOn)
        {
            competenceIconImage.color = iconImageNormalColor;
            cooldownFillImage.color = cooldownImageNormalColor;
        }
        else
        {
            Color iconColor = iconImageNormalColor / 2;
            iconColor.a = iconImageNormalColor.a;
            competenceIconImage.color = iconColor;

            Color cooldownColor = cooldownImageNormalColor / 2;
            cooldownColor.a = cooldownImageNormalColor.a;
            cooldownFillImage.color = cooldownColor;
        }
    }
    #endregion

    [Header("Variables")]
    #region Cooldown and Usability
    float currentCompetenceCooldown;
    float totalCompetenceCooldown;

    public bool Usable
    {
        get
        {
            if (relatedCompetence != null)
                return currentCompetenceCooldown == 0 && relatedCompetence.GetLinkEquipment.Usable;
            else
                return false;
        }
    }

    public void SetCooldown()
    {
        currentCompetenceCooldown = totalCompetenceCooldown;
        cooldownFillImage.fillAmount = 1;
    }

    public void UpdateCooldown()
    {
        if (Usable)
            return;

        if(currentCompetenceCooldown > 0)
        {
            currentCompetenceCooldown -= Time.deltaTime;
            cooldownFillImage.fillAmount = currentCompetenceCooldown/totalCompetenceCooldown;
        }
        else if(currentCompetenceCooldown < 0)
        {
            currentCompetenceCooldown = 0;
            cooldownFillImage.fillAmount = 0;
        }
    }
    #endregion

    #region Inputs
    bool startedTouch;
    bool fingerOnSlot;
    int currentFingerTouchId = -1;
    public int GetCurrentFingerId
    {
        get
        {
            return currentFingerTouchId;
        }
    }

    public void StartTouched(int fingerId, Vector3 worldPositon)
    {
        startedTouch = true;
        fingerOnSlot = true;

        if (relatedCompetence as CompetenceRamming != null)
            (relatedCompetence as CompetenceRamming).StartShowPreview(relatedShip.transform.position, relatedShip.GetShipVelocity.normalized);
        else if(relatedCompetence as CompetenceProtection != null)
            (relatedCompetence as CompetenceProtection).StartShowPreview(relatedShip);
        else if (relatedCompetence as CompetenceSpecialShipMove != null)
            (relatedCompetence as CompetenceSpecialShipMove).StartShowPreview(relatedShip);
        else 
            relatedCompetence.StartShowPreview();

        currentFingerTouchId = fingerId;
    }

    public void UpdateTouch(bool fingerOn, Vector3 worldPositon)
    {
        if (fingerOnSlot && !fingerOn)
        {
            fingerOnSlot = false;

            relatedCompetence.EndShowPreview();
        }

        if (!fingerOnSlot && fingerOn)
        {
            fingerOnSlot = true;

            relatedCompetence.StartShowPreview();
        }

        if (fingerOnSlot)
        {
            if (equipmentType == EquipmentType.Hull)
            {
                if (relatedCompetence as CompetenceRamming != null)
                    (relatedCompetence as CompetenceRamming).UpdateCompetence(relatedShip.transform.position, relatedShip.GetShipVelocity.normalized);
                else
                    relatedCompetence.UpdateCompetence(relatedShip.GetShipVelocity);
            }

            #region Canon
            else if (equipmentType == EquipmentType.Canon)
            {
                foreach (EquipmentObject equipObj in relatedCompetence.GetLinkEquipment.GetAllSpawnedEquipments)
                {
                    ShootParameters shootParameters = (relatedCompetence as CompetenceShoot).shootParameters;

                    if (equipObj as EquipmentObjectShoot != null)
                        (equipObj as EquipmentObjectShoot).UpdateShootOriginsModifiedDirection(worldPositon.x, 0);
                }
                relatedCompetence.UpdateCompetence(worldPositon);
            }
            #endregion

            #region Catapult
            else if (equipmentType == EquipmentType.Catapult)
            {
                ShootParameters shootParameters = (relatedCompetence as CompetenceShoot).shootParameters;

                Vector3 catapultVector = worldPositon;
                float distance = Mathf.Lerp(shootParameters.GetCatapultMinDistance, shootParameters.GetCatapultMaxDistance, worldPositon.magnitude);

                relatedCompetence.UpdateCompetence(relatedShip.transform.position + (worldPositon.normalized * distance));
            }
            #endregion
        }
    }

    public void EndTouch(bool endedOnButton, Vector3 worldPositon)
    {
        if (endedOnButton)
        {
            if (startedTouch && fingerOnSlot)
            {
                if (relatedCompetence as CompetenceShoot != null)
                {
                    if (!relatedCompetence.IsAimCompetence())
                    {
                        relatedCompetence.UseCompetence(worldPositon);
                        SetCooldown();
                    }
                    else
                    {

                        ShootParameters shootParameters = (relatedCompetence as CompetenceShoot).shootParameters;

                        Vector3 catapultVector = worldPositon;
                        float distance = Mathf.Lerp(shootParameters.GetCatapultMinDistance, shootParameters.GetCatapultMaxDistance, worldPositon.magnitude);

                        relatedCompetence.UseCompetence(relatedShip.transform.position + (worldPositon.normalized * distance));

                        SetCooldown();
                    }
                }
                else if (relatedCompetence as CompetenceRamming != null)
                {
                    relatedCompetence.EndShowPreview();
                    relatedCompetence.UseCompetence(relatedShip.ShipMvt);
                    SetCooldown();
                }
                else if (relatedCompetence as CompetenceProtection != null)
                {
                    relatedCompetence.EndShowPreview();
                    (relatedCompetence as CompetenceProtection).UseCompetence(relatedShip);
                    SetCooldown();
                }
                else if (relatedCompetence as CompetenceSpecialShipMove != null)
                {
                    relatedCompetence.EndShowPreview();
                    (relatedCompetence as CompetenceSpecialShipMove).UseCompetence(relatedShip);
                    SetCooldown();
                }
            }
        }

        startedTouch = false;
        fingerOnSlot = false;
        currentFingerTouchId = -1;
    }
    #endregion
}

public enum CompetencesUsabilityType
{
    All,
    Hull,
    Canons,
    Catapult,
    HullAndCanons,
    HullAndCatapult,
    CanonsAndCatapult,
    None
}
