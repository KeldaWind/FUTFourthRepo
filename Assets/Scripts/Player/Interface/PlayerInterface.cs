using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInterface : MonoBehaviour
{
    [SerializeField] Canvas playerInterfaceCanvas;

    [Header("Interactions")]
    [SerializeField] UIElementsInteractionsManager uiElementsInteractionsManager;

    #region Setting Up
    public void SetUp(Ship relatedShip, EquipmentsSet equipments)
    {
        CompetenceSet competenceSet = new CompetenceSet();
        competenceSet.ComposeCompetenceSet(equipments);

        hullMainCompSlot.SetUp(competenceSet.HullCompetence, relatedShip);
        mainWeaponPrimaryCompSlot.SetUp(competenceSet.MainWeaponPrimaryCompetence, relatedShip);
        secondaryWeaponCompSlot.SetUp(competenceSet.SecondaryWeaponCompetence, relatedShip);

        uiElementsInteractionsManager.SetUp();

        blindingMaskClearColor = blindingMaskMaxColor;
        blindingMaskClearColor.a = 0;
        blindingMask.color = blindingMaskClearColor;
    }

    public void SetUpSideHanded(HandedType handedType)
    {
        switch (handedType)
        {
            case (HandedType.RightHanded):
                completeShipWheel.SetParent(shipWheelRightHandedParent);
                completeShipWheel.localPosition = Vector3.zero;

                hullMainCompSlot.transform.SetParent(hullCompetenceRightHandedParent);
                hullMainCompSlot.transform.localPosition = Vector3.zero;

                mainWeaponPrimaryCompSlot.transform.SetParent(canonCompetenceRightHandedParent);
                mainWeaponPrimaryCompSlot.transform.localPosition = Vector3.zero;

                catapultJoystick.transform.SetParent(catapultCompetenceRightHandedParent);
                catapultJoystick.transform.localPosition = Vector3.zero;
                secondaryWeaponCompSlot.transform.SetParent(catapultCompetenceRightHandedParent);
                secondaryWeaponCompSlot.transform.localPosition = Vector3.zero;
                
                break;

            case (HandedType.LeftHanded):
                completeShipWheel.SetParent(shipWheelLeftHandedParent);
                completeShipWheel.localPosition = Vector3.zero;

                hullMainCompSlot.transform.SetParent(hullCompetenceLeftHandedParent);
                hullMainCompSlot.transform.localPosition = Vector3.zero;

                mainWeaponPrimaryCompSlot.transform.SetParent(canonCompetenceLeftHandedParent);
                mainWeaponPrimaryCompSlot.transform.localPosition = Vector3.zero;

                catapultJoystick.transform.SetParent(catapultCompetenceLeftHandedParent);
                catapultJoystick.transform.localPosition = Vector3.zero;
                secondaryWeaponCompSlot.transform.SetParent(catapultCompetenceLeftHandedParent);
                secondaryWeaponCompSlot.transform.localPosition = Vector3.zero;
                break;
        }
    }
    #endregion

    public void Update()
    {
        uiElementsInteractionsManager.UpdateInputManagement();

        UpdateLifeDisparition();

        UpdateBlinding();
    }

    bool hidden;
    public void ShowPlayerInterface()
    {
        GameManager gm = GameManager.gameManager;

        if (ArenaManager.arenaManager != null)
        {
            if (!gm.StartedFight || gm.Won || gm.Lost)
                return;
        }

        playerInterfaceCanvas.enabled = true;
        hidden = false;
    }

    public void HidePlayerInterface()
    {
        playerInterfaceCanvas.enabled = false;
        hidden = true;
    }

    public bool Hidden
    {
        get { return hidden; }
    }

    #region Life
    [Header("Life")]
    [SerializeField] PlayerLifePoint[] lifePoints;
    int currentLifeAmount;
    int lastLifeAmount;

    float timeBetweenTwoLifeDisparition = 0.08f;
    float remainingTimeBeforeNextDisparition;

    public void SetLifeBar(int normalLifePointCount, int currentArmorLifePointCount, int maximumArmorLifePointCount)
    {
        int counter = 0;
        while(counter < normalLifePointCount && counter < lifePoints.Length)
        {
            lifePoints[counter].SetUp(LifePointType.NormalLifePoint);
            counter++;
        }

        while (counter < normalLifePointCount + maximumArmorLifePointCount && counter < lifePoints.Length)
        {
            lifePoints[counter].SetUp(LifePointType.ArmorLifePoint);
            if (counter >= normalLifePointCount + currentArmorLifePointCount)
            {
                lifePoints[counter].SetPointBroken();
            }
            counter++;
        }

        while (counter < lifePoints.Length)
        {
            lifePoints[counter].HideLifePoint();
            counter++;
        }

        currentLifeAmount = normalLifePointCount + currentArmorLifePointCount;
    }

    public void UpdateLifeBar(int lifeAmount)
    {
        for (int i = (int)Mathf.Clamp(lifeAmount, 0, Mathf.Infinity);  i < currentLifeAmount; i++)
        {
            lifePoints[i].SetPointBreaking();
        }

        lastLifeAmount = currentLifeAmount;
        currentLifeAmount = lifeAmount;
        if (currentLifeAmount < 0)
            currentLifeAmount = 0;

        remainingTimeBeforeNextDisparition = timeBetweenTwoLifeDisparition;
    }

    public void UpdateLifeDisparition()
    {
        if (remainingTimeBeforeNextDisparition > 0)
            remainingTimeBeforeNextDisparition -= Time.deltaTime;
        else if(remainingTimeBeforeNextDisparition < 0)
        {
            remainingTimeBeforeNextDisparition = 0;
            if (lastLifeAmount > 0)
                lifePoints[lastLifeAmount - 1].SetPointBroken();
            lastLifeAmount--;

            if (lastLifeAmount > currentLifeAmount)
            {
                remainingTimeBeforeNextDisparition = timeBetweenTwoLifeDisparition;
            }
        }
    }
    #endregion

    #region
    [Header("Ship Wheel")]
    [SerializeField] Transform completeShipWheel;
    #endregion

    #region Competences
    [Header("Competences")]
    [SerializeField] CompetenceSlot hullMainCompSlot;
    [SerializeField] CompetenceSlot mainWeaponPrimaryCompSlot;
    [SerializeField] CompetenceSlot secondaryWeaponCompSlot;
    [SerializeField] CatapultJoystick catapultJoystick;

    public void UpdateEquipments(Ship relatedShip, EquipmentsSet equipments)
    {
        CompetenceSet competenceSet = new CompetenceSet();
        competenceSet.ComposeCompetenceSet(equipments);

        hullMainCompSlot.SetUp(competenceSet.HullCompetence, relatedShip);
        mainWeaponPrimaryCompSlot.SetUp(competenceSet.MainWeaponPrimaryCompetence, relatedShip);
        secondaryWeaponCompSlot.SetUp(competenceSet.SecondaryWeaponCompetence, relatedShip);

        //ChangeCompetencesAvailability(CompetencesUsabilityType.All);
    }

    #region Availability
    public void ChangeCompetencesAvailability(CompetencesUsabilityType type)
    {
        switch (type)
        {
            case (CompetencesUsabilityType.None):
                hullMainCompSlot.SetOn(false);
                mainWeaponPrimaryCompSlot.SetOn(false);
                secondaryWeaponCompSlot.SetOn(false);
                break;

            case (CompetencesUsabilityType.Hull):
                hullMainCompSlot.SetOn(true);
                mainWeaponPrimaryCompSlot.SetOn(false);
                secondaryWeaponCompSlot.SetOn(false);
                break;

            case (CompetencesUsabilityType.Canons):
                hullMainCompSlot.SetOn(false);
                mainWeaponPrimaryCompSlot.SetOn(true);
                secondaryWeaponCompSlot.SetOn(false);
                break;

            case (CompetencesUsabilityType.Catapult):
                hullMainCompSlot.SetOn(false);
                mainWeaponPrimaryCompSlot.SetOn(false);
                secondaryWeaponCompSlot.SetOn(true);
                break;

            case (CompetencesUsabilityType.HullAndCanons):
                hullMainCompSlot.SetOn(true);
                mainWeaponPrimaryCompSlot.SetOn(true);
                secondaryWeaponCompSlot.SetOn(false);
                break;

            case (CompetencesUsabilityType.HullAndCatapult):
                hullMainCompSlot.SetOn(true);
                mainWeaponPrimaryCompSlot.SetOn(false);
                secondaryWeaponCompSlot.SetOn(true);
                break;

            case (CompetencesUsabilityType.CanonsAndCatapult):
                hullMainCompSlot.SetOn(false);
                mainWeaponPrimaryCompSlot.SetOn(true);
                secondaryWeaponCompSlot.SetOn(true);
                break;

            case (CompetencesUsabilityType.All):
                hullMainCompSlot.SetOn(true);
                mainWeaponPrimaryCompSlot.SetOn(true);
                secondaryWeaponCompSlot.SetOn(true);
                break;
        }
    }
    #endregion

    #endregion

    #region Blinding
    [Header("Blinding")]
    [SerializeField] Image blindingMask;
    [SerializeField] Color blindingMaskMaxColor;
    Color blindingMaskClearColor;
    bool blind;
    float remainingBlindDuration;
    float blindingApparitionCoeff = 0.1f;

    public void Blind()
    {
        blind = true;
    }

    public void Blind(float duration)
    {
        blind = true;
        remainingBlindDuration = duration;
    }

    public void UpdateBlinding()
    {
        if (remainingBlindDuration > 0)
            remainingBlindDuration -= Time.deltaTime;
        else if (remainingBlindDuration < 0)
            Unblind();

        if (blind)
            blindingMask.color = Color.Lerp(blindingMask.color, blindingMaskMaxColor, blindingApparitionCoeff);
        else
            blindingMask.color = Color.Lerp(blindingMask.color, blindingMaskClearColor, blindingApparitionCoeff);
    }

    public void Unblind()
    {
        blind = false;
        remainingBlindDuration = 0;
    }
    #endregion

    [Header("Interface Elements Parents : Right-Handed")]
    [SerializeField] Transform shipWheelRightHandedParent;
    [SerializeField] Transform canonCompetenceRightHandedParent;
    [SerializeField] Transform catapultCompetenceRightHandedParent;
    [SerializeField] Transform hullCompetenceRightHandedParent;

    [Header("Interface Elements Parents : Left-Handed")]
    [SerializeField] Transform shipWheelLeftHandedParent;
    [SerializeField] Transform canonCompetenceLeftHandedParent;
    [SerializeField] Transform catapultCompetenceLeftHandedParent;
    [SerializeField] Transform hullCompetenceLeftHandedParent;

    #region Enemies Highlighting
    [Header("Enemies Highlighting")]
    [SerializeField] Transform[] highlightingArrows;
    [SerializeField] Transform[] importantHighlightingArrows;
    [SerializeField] Transform highlightingLimitUpRight;
    [SerializeField] Transform highlightingLimitDownLeft;

    /*public void StartEnemyHighlight(Vector3 enemyDirection)
    {
        highlightingArrow.gameObject.SetActive(true);
        UpdateEnemyHighlight(enemyDirection);
    }*/

    public void UpdateEnemyHighlight(List<Vector3> enemyDirections, List<Vector3> importantEnemyDirections)
    {
        #region Normal
        for (int i = 0; i < enemyDirections.Count && i < highlightingArrows.Length; i++)
        {
            Vector3 enemyDirection = enemyDirections[i];
            if (!highlightingArrows[i].gameObject.activeInHierarchy)
                highlightingArrows[i].gameObject.SetActive(true);

            float rotZ = Mathf.Atan2(enemyDirection.x, enemyDirection.z) * Mathf.Rad2Deg;
            highlightingArrows[i].localRotation = Quaternion.Euler(new Vector3(0, 0, -rotZ));

            float xCoeff = enemyDirection.x > 0 ? Mathf.Lerp(0.5f, 1, enemyDirection.x) : Mathf.Lerp(0.5f, 0, -enemyDirection.x);
            float yCoeff = enemyDirection.z > 0 ? Mathf.Lerp(0.5f, 1, enemyDirection.z / 0.5f) : Mathf.Lerp(0.5f, 0, -enemyDirection.z / 0.5f);

            highlightingArrows[i].transform.localPosition = new Vector2(
                Mathf.Lerp(highlightingLimitDownLeft.localPosition.x, highlightingLimitUpRight.localPosition.x, xCoeff),
                Mathf.Lerp(highlightingLimitDownLeft.localPosition.y, highlightingLimitUpRight.localPosition.y, yCoeff));
        }

        for (int i = enemyDirections.Count; i < highlightingArrows.Length; i++)
        {
            if (highlightingArrows[i].gameObject.activeInHierarchy)
                highlightingArrows[i].gameObject.SetActive(false);
        }
        #endregion

        #region Important
        for (int i = 0; i < importantEnemyDirections.Count && i < importantHighlightingArrows.Length; i++)
        {
            Vector3 enemyDirection = importantEnemyDirections[i];
            if (!importantHighlightingArrows[i].gameObject.activeInHierarchy)
                importantHighlightingArrows[i].gameObject.SetActive(true);

            float rotZ = Mathf.Atan2(enemyDirection.x, enemyDirection.z) * Mathf.Rad2Deg;
            importantHighlightingArrows[i].localRotation = Quaternion.Euler(new Vector3(0, 0, -rotZ));

            float xCoeff = enemyDirection.x > 0 ? Mathf.Lerp(0.5f, 1, enemyDirection.x) : Mathf.Lerp(0.5f, 0, -enemyDirection.x);
            float yCoeff = enemyDirection.z > 0 ? Mathf.Lerp(0.5f, 1, enemyDirection.z / 0.5f) : Mathf.Lerp(0.5f, 0, -enemyDirection.z / 0.5f);

            importantHighlightingArrows[i].transform.localPosition = new Vector2(
                Mathf.Lerp(highlightingLimitDownLeft.localPosition.x, highlightingLimitUpRight.localPosition.x, xCoeff),
                Mathf.Lerp(highlightingLimitDownLeft.localPosition.y, highlightingLimitUpRight.localPosition.y, yCoeff));
        }

        for (int i = importantEnemyDirections.Count; i < importantHighlightingArrows.Length; i++)
        {
            if (importantHighlightingArrows[i].gameObject.activeInHierarchy)
                importantHighlightingArrows[i].gameObject.SetActive(false);
        }
        #endregion
    }

    /*public void EndEnemyHighlight()
    {
        highlightingArrow.gameObject.SetActive(false);
    }*/
    #endregion
}

public enum HandedType
{
    RightHanded = 1, LeftHanded = 2
}

public enum HighlightingType
{
    Normal, None, Important
}
