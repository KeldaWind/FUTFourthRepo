using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Preview : MonoBehaviour
{
    [SerializeField] PreviewPoolTag previewType;
    public PreviewPoolTag GetPreviewType
    {
        get
        {
            return previewType;
        }
    }

    #region Preparation
    /// <summary>
    /// Commence à prévisualiser la préparation
    /// </summary>
    public virtual void ShowPreparePreview()
    {

    }

    /// <summary>
    /// Actualise la prévisualisation de la préparation
    /// </summary>
    /// <param name="originPos">Position d'ou provient la compétence</param>
    /// <param name="aimedPos">Position visée par la compétence</param>
    public virtual void UpdatePreparePreview(Vector3 originPos, Vector3 aimedPos)
    {

    }

    /// <summary>
    /// Finit de prévisualiser la préparation
    /// </summary>
    public virtual void HidePreparePreview()
    {

    }
    #endregion

    #region Launched
    /// <summary>
    /// Commence à prévisualiser la compétence lancée
    /// </summary>
    /// <param name="startPos">Position d'ou provient la compétence</param>
    /// <param name="aimPos">Les Positions visées par la compétence</param>
    /// <param name="parameter">Paramètre particulier</param>
    public virtual void StartLaunchedPreview(Vector3 startPos, List<Vector3> aimPos, float parameter)
    {

    }

    /// <summary>
    /// Actualise la prévisualisation de la compétence lancée
    /// </summary>
    public virtual void UpdateLaunchedPreview()
    {

    }

    /// <summary>
    /// Finit de prévisualiser la compétence lancée
    /// </summary>
    public virtual void EndLaunchedPreview()
    {

    }
    #endregion 
}
