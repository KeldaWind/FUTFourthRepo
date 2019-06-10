using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowPreview : Preview
{
    public float testLenght = 10;

    [SerializeField] MeshFilter meshFilter;
    //[SerializeField] Material previewMaterial;
    [SerializeField] float arrowLineWidth = 5;
    [SerializeField] float arrowEndWidth = 10;
    [SerializeField] float arrowEndLenghth = 5;
    [SerializeField] float arrowThickness = 4;
    [SerializeField] float arrowMinLenght;
    [SerializeField] float arrowMinSpacing;
    [SerializeField] Transform[] arrows;

    List<Vector3> verts; 
    List<int> tris;

    /// <summary>
    /// Génère le mesh de la flèche. Place la preview à la position de départ, et le bout de la flèche à la position de fin
    /// </summary>
    /// <param name="distance"></param>
    public void GenerateArrowMesh(Vector3 startPosition, Vector3 endPosition)
    {
        #region V1
        /*verts = new List<Vector3>();
        tris = new List<int>();
        meshFilter.mesh = null;

        transform.position = startPosition;

        Vector3 directionVector = (endPosition - startPosition).normalized;
        float totalLenght = Vector3.Distance(startPosition, endPosition);
        float lineLenght = totalLenght - arrowEndLenghth;

        Vector3 t = new Vector3(-arrowLineWidth / 2, 0, 0) + new Vector3(0, arrowThickness / 2, 0);
        Vector3 u = new Vector3(arrowLineWidth / 2, 0, 0) + new Vector3(0, arrowThickness / 2, 0);

        Vector3 v = t + new Vector3(0, 0, lineLenght);
        Vector3 w = u + new Vector3(0, 0, lineLenght);

        Vector3 x = new Vector3(0, 0, totalLenght) + new Vector3(-arrowEndWidth/2, 0, -arrowEndLenghth) + new Vector3(0, arrowThickness / 2, 0);
        Vector3 y = new Vector3(0, 0, totalLenght) + new Vector3(arrowEndWidth/2, 0, -arrowEndLenghth) + new Vector3(0, arrowThickness / 2, 0);
        Vector3 z = new Vector3(0, 0, totalLenght) + new Vector3(0, arrowThickness / 2, 0);

        #region Thickness
        Vector3 t2 = new Vector3(-arrowLineWidth / 2, 0, 0) - new Vector3(0, arrowThickness / 2, 0);
        Vector3 u2 = new Vector3(arrowLineWidth / 2, 0, 0) - new Vector3(0, arrowThickness / 2, 0);

        Vector3 v2 = t2 + new Vector3(0, 0, lineLenght);
        Vector3 w2 = u2 + new Vector3(0, 0, lineLenght);

        Vector3 x2 = new Vector3(0, 0, totalLenght) + new Vector3(-arrowEndWidth / 2, 0, -arrowEndLenghth) - new Vector3(0, arrowThickness / 2, 0);
        Vector3 y2 = new Vector3(0, 0, totalLenght) + new Vector3(arrowEndWidth / 2, 0, -arrowEndLenghth) - new Vector3(0, arrowThickness / 2, 0);
        Vector3 z2 = new Vector3(0, 0, totalLenght) - new Vector3(0, arrowThickness / 2, 0);
        #endregion

        #region Top
        tris.Add(0 + verts.Count);
        tris.Add(2 + verts.Count);
        tris.Add(1 + verts.Count);

        tris.Add(1 + verts.Count);
        tris.Add(2 + verts.Count);
        tris.Add(3 + verts.Count);

        tris.Add(4 + verts.Count);
        tris.Add(6 + verts.Count);
        tris.Add(5 + verts.Count);
        #endregion

        #region Bottom
        tris.Add(7 + verts.Count);
        tris.Add(9 + verts.Count);
        tris.Add(8 + verts.Count);

        tris.Add(8 + verts.Count);
        tris.Add(9 + verts.Count);
        tris.Add(10 + verts.Count);

        tris.Add(11 + verts.Count);
        tris.Add(13 + verts.Count);
        tris.Add(12 + verts.Count);
        #endregion

        #region Sides
        tris.Add(0 + verts.Count);
        tris.Add(1 + verts.Count);
        tris.Add(7 + verts.Count);

        tris.Add(1 + verts.Count);
        tris.Add(8 + verts.Count);
        tris.Add(7 + verts.Count);

        tris.Add(1 + verts.Count);
        tris.Add(3 + verts.Count);
        tris.Add(8 + verts.Count);

        tris.Add(3 + verts.Count);
        tris.Add(10 + verts.Count);
        tris.Add(8 + verts.Count);

        tris.Add(0 + verts.Count);
        tris.Add(7 + verts.Count);
        tris.Add(2 + verts.Count);

        tris.Add(7 + verts.Count);
        tris.Add(9 + verts.Count);
        tris.Add(2 + verts.Count);

        tris.Add(4 + verts.Count);
        tris.Add(2 + verts.Count);
        tris.Add(11 + verts.Count);

        tris.Add(11 + verts.Count);
        tris.Add(2 + verts.Count);
        tris.Add(9 + verts.Count);

        tris.Add(3 + verts.Count);
        tris.Add(5 + verts.Count);
        tris.Add(10 + verts.Count);

        tris.Add(5 + verts.Count);
        tris.Add(12 + verts.Count);
        tris.Add(10 + verts.Count);

        tris.Add(6 + verts.Count);
        tris.Add(4 + verts.Count);
        tris.Add(11 + verts.Count);

        tris.Add(6 + verts.Count);
        tris.Add(11 + verts.Count);
        tris.Add(13 + verts.Count);

        tris.Add(5 + verts.Count);
        tris.Add(6 + verts.Count);
        tris.Add(13 + verts.Count);

        tris.Add(5 + verts.Count);
        tris.Add(13 + verts.Count);
        tris.Add(12 + verts.Count);
        #endregion

        #region Verts
        verts.Add(t);
        verts.Add(u);
        verts.Add(v);
        verts.Add(w);
        verts.Add(x);
        verts.Add(y);
        verts.Add(z);

        verts.Add(t2);
        verts.Add(u2);
        verts.Add(v2);
        verts.Add(w2);
        verts.Add(x2);
        verts.Add(y2);
        verts.Add(z2);
        #endregion

        Mesh arrowMesh = new Mesh();

        arrowMesh.SetVertices(verts);
        arrowMesh.triangles =  tris.ToArray();
        arrowMesh.name = "Arrow";

        meshFilter.mesh = arrowMesh;

        #region Rotation
        float rotY = Mathf.Atan2(directionVector.x, directionVector.z) * Mathf.Rad2Deg;

        transform.localRotation = Quaternion.Euler(new Vector3(0, rotY, 0));
        #endregion
        */
        #endregion

        #region V2
        transform.position = startPosition;

        Vector3 directionVector = (endPosition - startPosition).normalized;
        float totalLenght = Vector3.Distance(startPosition, endPosition);
        float lineLenght = totalLenght - arrowEndLenghth;

        float rotY = Mathf.Atan2(directionVector.x, directionVector.z) * Mathf.Rad2Deg;

        transform.localRotation = Quaternion.Euler(new Vector3(0, rotY, 0));

        int numberOfShownArrows = 1;
        for (int i = 1; i < arrows.Length; i++)
        {
            float spacing = totalLenght / ((float)(i + 1) / (float)arrows.Length);
            if (spacing < arrowMinSpacing)
                break;
            numberOfShownArrows++;
        }

        for (int i = 0; i < numberOfShownArrows; i++)
        {
            Transform tr = arrows[i];
            tr.gameObject.SetActive(true);
            float coeff = (float)(i + 1) / (float)numberOfShownArrows;
            tr.localPosition = Vector3.Lerp(Vector3.forward * arrowMinLenght, Vector3.forward * totalLenght, coeff);
        }

        for (int i = numberOfShownArrows; i < arrows.Length; i++)
        {
            Transform tr = arrows[i];
            tr.gameObject.SetActive(false);
        }
        #endregion
    }

    public void UpdateMesh(Vector3 startPosition, Vector3 endPosition)
    {
        /*verts = new List<Vector3>();

        transform.position = startPosition;

        Vector3 directionVector = (endPosition - startPosition).normalized;
        float totalLenght = Vector3.Distance(startPosition, endPosition);
        float lineLenght = totalLenght - arrowEndLenghth;

        Vector3 t = directionVector.GetLeftOrthogonalVectorXZ().normalized * arrowLineWidth / 2;
        Vector3 u = directionVector.GetRightOrthogonalVectorXZ().normalized * arrowLineWidth / 2;

        Vector3 v = t + directionVector * lineLenght;
        Vector3 w = u + directionVector * lineLenght;

        Vector3 x = (endPosition - startPosition) - directionVector * arrowEndLenghth + directionVector.GetLeftOrthogonalVectorXZ() * arrowEndWidth / 2;
        Vector3 y = (endPosition - startPosition) - directionVector * arrowEndLenghth + directionVector.GetRightOrthogonalVectorXZ() * arrowEndWidth / 2;
        Vector3 z = (endPosition - startPosition);

        verts.Add(t);
        verts.Add(u);
        verts.Add(v);
        verts.Add(w);
        verts.Add(x);
        verts.Add(y);
        verts.Add(z);

        meshFilter.mesh.SetVertices(verts);*/

        transform.position = startPosition;

        Vector3 directionVector = (endPosition - startPosition).normalized;

        float rotY = Mathf.Atan2(directionVector.x, directionVector.z) * Mathf.Rad2Deg;

        transform.localRotation = Quaternion.Euler(new Vector3(0, rotY, 0));
    }

    public void ClearMesh()
    {
        verts = new List<Vector3>();
        tris = new List<int>();
        meshFilter.mesh = null;
    }
}
