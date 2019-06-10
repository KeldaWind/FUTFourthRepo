using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
[CreateAssetMenu(fileName ="NewDialogue", menuName = "Interface/Dialogue")]
public class CreateAText : ScriptableObject
{
    [ResizableTextArea] public string textToWrite;
    [SerializeField] RuntimeAnimatorController speakerAnimator;
    public RuntimeAnimatorController GetSpeakerAnimator { get { return speakerAnimator; } }
    [SerializeField] SpeakerEmotion speakerEmotion;
    public SpeakerEmotion GetSpeakerEmotion { get { return speakerEmotion; } }
}

public enum SpeakerType
{
    Ally, Enemy
}

public enum SpeakerEmotion
{
    Happy, Angry, Sad
}