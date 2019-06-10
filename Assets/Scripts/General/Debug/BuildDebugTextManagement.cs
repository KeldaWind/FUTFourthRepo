using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildDebugTextManagement : MonoBehaviour
{
    public static BuildDebugTextManagement manager;

    private void Awake()
    {
        manager = this;
    }

    public void SetText(string text)
    {
        debugText.text = text;
    }

    [SerializeField] Text debugText;
}
