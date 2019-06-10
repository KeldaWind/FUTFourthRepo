using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class GameButton : MonoBehaviour
{
    /// <summary>
    /// The delegate used to create the interaction of a button.
    /// </summary>
    public delegate void InteractionDeleguate();
    /// <summary>
    /// The interaction of this button.
    /// </summary>
    public InteractionDeleguate Interaction;

    /// <summary>
    /// Call this function to call the Interact delegate and make the interaction actions.
    /// </summary>
    public virtual void Interact()
    {
        Interaction();
    }

    [SerializeField] Text label;
    public void SetButtonLabel(string text)
    {
        if (label != null)
            label.text = text;
    }

    [SerializeField] Button linkedButton;
    public void SetButtonInteractable(bool interactable)
    {
        linkedButton.interactable = interactable;
    }
}
