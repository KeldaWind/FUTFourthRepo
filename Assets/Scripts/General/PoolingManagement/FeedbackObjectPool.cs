using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FeedbackObjectPool : Pool<FeedbackObject>
{
    [SerializeField] FeedbackObjectPoolTag feedbackObjectTag;
    public FeedbackObjectPoolTag GetFeedbackObjectTag { get { return feedbackObjectTag; } }
}

public enum FeedbackObjectPoolTag
{
    Test, WoodDestruction
}
