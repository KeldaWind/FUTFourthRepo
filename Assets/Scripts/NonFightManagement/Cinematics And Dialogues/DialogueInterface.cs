using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueInterface : MonoBehaviour
{
    [Header("Texte")]
    [SerializeField] float textTypingSpeed;
    [SerializeField] Text dialogueText;

    [Header("Animation")]
    [SerializeField] Animator speakerAnimationImage;
}
