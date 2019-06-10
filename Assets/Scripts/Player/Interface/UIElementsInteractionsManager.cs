using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[System.Serializable]
/// <summary>
/// Permet d'intéragir avec des éléments de l'UI
/// </summary>
public class UIElementsInteractionsManager
{
    /// <summary>
    /// Graphic raycaster permettant de faire du raycast sur l'UI
    /// </summary>
    [Header("General References")]
    [SerializeField] GraphicRaycaster graphicRaycaster;
    /// <summary>
    /// Event System permettant de faire du raycast sur l'UI
    /// </summary>
    [SerializeField] EventSystem eventSystem;
    /// <summary>
    /// Ponter Event Data permettant de faire du raycast sur l'UI
    /// </summary>
    PointerEventData pointerEventData;

    /// <summary>
    /// Surface de contrôle de la barre du joueur
    /// </summary>
    [Header("Specific References")]
    [SerializeField] ShipsWheelControlSurface wheelControlSurface;

    CompetenceSlot currentCompetenceSlot;
    int currentlyAimingIndex = -1;

    [Header("Catapult Joystick")]
    [SerializeField] float minCatapultJoystickDistance;
    [SerializeField] float maxCatapultJoystickDistance;
    [SerializeField] AnimationCurve catapultJoystickSensibilityCurve;
    Vector2 catapultJoystickStartPosition;
    Vector2 catapultSlotStartPosition;
    [SerializeField] CatapultJoystick catapultJoystick;

    public void SetUp()
    {
        lastThreeCatapultJoystickVectors = new List<Vector2>();

        catapultJoystick.SetUp(minCatapultJoystickDistance, maxCatapultJoystickDistance);
    }

    public void UpdateInputManagement()
    {
        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                pointerEventData = new PointerEventData(eventSystem);
                pointerEventData.position = touch.position;

                List<RaycastResult> results = new List<RaycastResult>();
                graphicRaycaster.Raycast(pointerEventData, results);

                foreach (RaycastResult result in results)
                {
                    ShipsWheelControlSurface wheelControlSurface = result.gameObject.GetComponent<ShipsWheelControlSurface>();
                    if (wheelControlSurface != null)
                    {
                        wheelControlSurface.StartWheelControl(touch.fingerId);
                        break;
                    }

                    CompetenceSlot compSlot = result.gameObject.GetComponent<CompetenceSlot>();
                    if (compSlot != null)
                    {
                        if (compSlot.Usable && compSlot.IsOn && currentCompetenceSlot == null && currentlyAimingIndex < 0)
                        {
                            currentCompetenceSlot = compSlot;

                            if (currentCompetenceSlot.GetEquipmentType == EquipmentType.Hull)
                            {
                                compSlot.StartTouched(touch.fingerId, GetWorldPosition(touch.position));
                            }
                            else if (currentCompetenceSlot.GetEquipmentType == EquipmentType.Canon)
                            {
                                compSlot.StartTouched(touch.fingerId, GetWorldPosition(touch.position));
                                //StartCanonVirtualJoystick(touch.position);
                            }
                            else if (currentCompetenceSlot.GetEquipmentType == EquipmentType.Catapult)
                            {
                                compSlot.StartTouched(touch.fingerId, GetWorldPosition(touch.position));
                                StartCatapultVirtualJoystick(touch.position);
                            }
                        }
                        break;
                    }
                }
            }
            else if (touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Moved)
            {
                if (currentCompetenceSlot != null)
                {
                    if (currentCompetenceSlot.GetEquipmentType == EquipmentType.Hull)
                    {
                        /*bool currentSlotTouched = false;

                        pointerEventData = new PointerEventData(eventSystem);
                        pointerEventData.position = touch.position;

                        List<RaycastResult> results = new List<RaycastResult>();
                        graphicRaycaster.Raycast(pointerEventData, results);

                        foreach (RaycastResult result in results)
                        {
                            CompetenceSlot compSlot = result.gameObject.GetComponent<CompetenceSlot>();

                            if (compSlot == currentCompetenceSlot)
                            {
                                if (currentCompetenceSlot.GetCurrentFingerId == touch.fingerId)
                                {
                                    currentCompetenceSlot.UpdateTouch(true, GetWorldPosition(touch.position));
                                    currentSlotTouched = true;
                                    break;
                                }
                            }
                        }

                        if (currentCompetenceSlot.GetCurrentFingerId == touch.fingerId && !currentSlotTouched)
                            currentCompetenceSlot.UpdateTouch(false, GetWorldPosition(touch.position));*/
                        currentCompetenceSlot.UpdateTouch(true, GetWorldPosition(touch.position));
                    }
                    else if (currentCompetenceSlot.GetEquipmentType == EquipmentType.Canon)
                    {
                        if (currentCompetenceSlot.GetCurrentFingerId == touch.fingerId)
                        {
                            currentCompetenceSlot.UpdateTouch(true, new Vector3(0, 0, 0));
                        }
                    }
                    else if (currentCompetenceSlot.GetEquipmentType == EquipmentType.Catapult)
                    {
                        if (currentCompetenceSlot.GetCurrentFingerId == touch.fingerId)
                        {
                            Vector2 catapultDirectionVector = UpdateCatapultVirtualJoystick(touch.position);
                            currentCompetenceSlot.UpdateTouch(true, new Vector3(catapultDirectionVector.x, 0, catapultDirectionVector.y));
                        }
                    }
                }
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                if (touch.fingerId == wheelControlSurface.CurrentControlIndex)
                    wheelControlSurface.EndWheelControl();

                if (currentCompetenceSlot != null)
                {
                    if (currentCompetenceSlot.GetCurrentFingerId == touch.fingerId)
                    {
                        if (currentCompetenceSlot.GetEquipmentType == EquipmentType.Hull)
                        {
                            currentCompetenceSlot.EndTouch(true, GetWorldPosition(touch.position));
                            currentCompetenceSlot = null;
                        }
                        else if (currentCompetenceSlot.GetEquipmentType == EquipmentType.Canon)
                        {
                            currentCompetenceSlot.EndTouch(true, new Vector3(0, 0, 0));
                            currentCompetenceSlot = null;
                        }
                        else if (currentCompetenceSlot.GetEquipmentType == EquipmentType.Catapult)
                        {
                            Vector2 catapultDirectionVector = EndCatapultVirtualJoystick(touch.position);
                            currentCompetenceSlot.EndTouch(true, new Vector3(catapultDirectionVector.x, 0, catapultDirectionVector.y));
                            currentCompetenceSlot = null;
                        }
                    }
                }
            }
        }
    }

    #region CatapultVirtualJoystick
    List<Vector2> lastThreeCatapultJoystickVectors;
    public Vector2 CalculateAverageCatapultJoystickVector()
    {
        float lenght = 0;
        Vector2 averageVector = new Vector2();

        foreach(Vector2 vect in lastThreeCatapultJoystickVectors)
        {
            lenght++;
            averageVector += vect;
        }

        return averageVector / lenght;
    }

    public void StartCatapultVirtualJoystick(Vector2 startTouchPosition)
    {
        catapultJoystickStartPosition = startTouchPosition;
        catapultSlotStartPosition = currentCompetenceSlot.transform.localPosition;

        catapultJoystick.StartCatapultJoystickUse();
    }

    public Vector2 UpdateCatapultVirtualJoystick(Vector2 touchPosition)
    {
        Vector2 offsetVector = touchPosition - catapultJoystickStartPosition;

        if (offsetVector.magnitude < minCatapultJoystickDistance)
            offsetVector = offsetVector.normalized * minCatapultJoystickDistance;

        if (offsetVector.magnitude > maxCatapultJoystickDistance)
            offsetVector = offsetVector.normalized * maxCatapultJoystickDistance;

        currentCompetenceSlot.transform.localPosition = catapultSlotStartPosition + offsetVector;

        Vector2 percentageVector = offsetVector.normalized * Mathf.Clamp((offsetVector.magnitude -  minCatapultJoystickDistance) / (maxCatapultJoystickDistance - minCatapultJoystickDistance), 0.01f, 1f);
        percentageVector *= catapultJoystickSensibilityCurve.Evaluate(percentageVector.magnitude);

        if (percentageVector == Vector2.zero)
            percentageVector = new Vector2(0, 0.01f);

        lastThreeCatapultJoystickVectors.Add(percentageVector);
        if (lastThreeCatapultJoystickVectors.Count > 3)
            lastThreeCatapultJoystickVectors.RemoveAt(0);

        catapultJoystick.UpdateIndicatorPositionUse(percentageVector);

        return CalculateAverageCatapultJoystickVector();
    }

    public Vector2 EndCatapultVirtualJoystick(Vector2 touchPosition)
    {
        Vector2 offsetVector = touchPosition - catapultJoystickStartPosition;
        if (offsetVector.magnitude > maxCatapultJoystickDistance)
            offsetVector = offsetVector.normalized * maxCatapultJoystickDistance;

        currentCompetenceSlot.transform.localPosition = catapultSlotStartPosition;

        Vector2 percentageVector = offsetVector.normalized * Mathf.Clamp((offsetVector.magnitude - minCatapultJoystickDistance) / (maxCatapultJoystickDistance - minCatapultJoystickDistance), 0.01f, 1f);
        percentageVector *= catapultJoystickSensibilityCurve.Evaluate(percentageVector.magnitude);

        if (percentageVector == Vector2.zero)
            percentageVector = new Vector2(0, 0.01f);

        lastThreeCatapultJoystickVectors.Add(percentageVector);
        if (lastThreeCatapultJoystickVectors.Count > 3)
            lastThreeCatapultJoystickVectors.RemoveAt(0);

        percentageVector = CalculateAverageCatapultJoystickVector();
        lastThreeCatapultJoystickVectors.Clear();

        catapultJoystick.EndCatapultJoystickUse();

        return percentageVector;
    }
    #endregion

    

    #region World Position
    public Vector3 GetWorldPosition(Vector3 fingerPos)
    {
        Vector3 worldPosition = new Vector3();

        Ray targetRay = GameManager.gameManager.MainCamera.ScreenPointToRay(fingerPos + new Vector3(0, Screen.height / 10, 0));

        RaycastHit[] hits = Physics.RaycastAll(targetRay, 1000);

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.GetComponent<InputSurface>())
            {
                worldPosition = hit.point;
                break;
            }
        }

        return worldPosition;
    }
    #endregion
}
