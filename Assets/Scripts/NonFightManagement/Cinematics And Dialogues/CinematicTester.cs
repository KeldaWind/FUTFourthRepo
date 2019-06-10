using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinematicTester : MonoBehaviour
{
    [SerializeField] FeedbackObjectPoolTag testTag;

    bool started;
    private void Update()
    {
        if (Input.touchCount > 3)
        {
            if (!started)
            {
                started = true;
                FeedbackObject obj = GameManager.gameManager.PoolManager.GetFeedbackObject(testTag, PoolInteractionType.GetFromPool);
                obj.gameObject.SetActive(true);
                obj.transform.position = transform.position;
                //obj.StartFeedback();
            }
        }
        else
            started = false;
    }
}
