using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfirmationPanel : MonoBehaviour
{
    public void Awake()
    {
        SetUp();
    }

    public void SetUp()
    {
        cancelButton.Interaction = CloseConfirmationPanel;
    }

    [Header("Confirmation Panel")]
    [SerializeField] GameObject confirmationPanel;
    [SerializeField] GameButton confirmButton;
    [SerializeField] GameButton cancelButton;

    public void OpenConfirmationPanel()
    {
        confirmationPanel.SetActive(true);
    }

    public void CloseConfirmationPanel()
    {
        confirmationPanel.SetActive(false);
    }

    public void Ask(GameButton.InteractionDeleguate confirmationAction)
    {
        confirmButton.Interaction = confirmationAction;
        confirmButton.Interaction += CloseConfirmationPanel;

        cancelButton.Interaction = CloseConfirmationPanel;
    }

    public void Ask(GameButton.InteractionDeleguate confirmationAction, GameButton.InteractionDeleguate cancelAction)
    {
        confirmButton.Interaction = confirmationAction;
        confirmButton.Interaction += CloseConfirmationPanel;

        cancelButton.Interaction = cancelAction;
        cancelButton.Interaction += CloseConfirmationPanel;
    }

    public bool ConfirmingSomething { get { return confirmationPanel.activeInHierarchy; } }
}
