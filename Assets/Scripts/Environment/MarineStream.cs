using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using PathCreation.Examples;

public class MarineStream : MonoBehaviour
{
    [Header("References")]
    [SerializeField] PathCreator pathCreator;
    [SerializeField] RoadMeshCreator streamCreator;
    VertexPath vertexPath;
    float streamWidhth;

    List<Ship> shipsInStrem;
    List<EnemyLootCrate> cratesInStream;
    List<Projectile> projectilesInStream;

    [Header("Balancing")]
    [SerializeField] float streamForce;
    [SerializeField] float streamPrecision;
    [SerializeField] float streamWidthOffset;

    public void SetUp()
    {
        shipsInStrem = new List<Ship>();
        cratesInStream = new List<EnemyLootCrate>();
        projectilesInStream = new List<Projectile>();
        vertexPath = pathCreator.path;
        streamWidhth = streamCreator.roadWidth * streamWidthOffset;

        SetUpStreamRenderer();

        if (helpingMeshRenderer != null)
            helpingMeshRenderer.enabled = false;
    }

    public void Start()
    {
        SetUp();
    }

    private void FixedUpdate()
    {
        UpdateShipManagement();
        UpdateCratesManagement();
        UpdateProjectilesManagement();
    }

    #region Triggering
    private void OnTriggerEnter(Collider other)
    {
        Ship ship = other.GetComponent<Ship>();
        if (ship != null)
        {
            shipsInStrem.Add(ship);
            ship.ShipMvt.StartStreamForce(GetStreamForce(ship.transform.position));
        }
        else
        {
            EnemyLootCrate crate = other.GetComponent<EnemyLootCrate>();
            if (crate != null)
            {
                cratesInStream.Add(crate);
                crate.StartStreamForce(GetStreamForce(crate.transform.position));
            }
            else
            {
                Projectile placedProjectile = other.GetComponent<Projectile>();
                if (placedProjectile != null)
                {
                    projectilesInStream.Add(placedProjectile);
                    placedProjectile.StartStreamForce(GetStreamForce(placedProjectile.transform.position));
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Ship ship = other.GetComponent<Ship>();
        if (ship != null)
        {
            if (shipsInStrem.Contains(ship))
            {
                shipsInStrem.Remove(ship);
                ship.ShipMvt.StopStreamForce(GetStreamForce(ship.transform.position));
            }
        }
        else { 
        EnemyLootCrate crate = other.GetComponent<EnemyLootCrate>();
            if (crate != null)
            {
                if (cratesInStream.Contains(crate))
                {
                    cratesInStream.Remove(crate);
                    crate.StopStreamForce(GetStreamForce(crate.transform.position));
                }
            }
            else
            {
                Projectile placedProjectile = other.GetComponent<Projectile>();
                if (placedProjectile != null)
                {
                    projectilesInStream.Remove(placedProjectile);
                    placedProjectile.StopStreamForce(GetStreamForce(placedProjectile.transform.position));
                }
            }
        }
    }
    #endregion

    #region Stream Force
    public Vector3 GetStreamForce(Vector3 position)
    {
        if (Time.timeScale != 0)
            return vertexPath.GetNearestPointDirectionOnTheCurve(position, streamPrecision, streamWidthOffset) * streamForce * 50 * Time.deltaTime / Time.timeScale;
        else
            return Vector3.zero;
    }
    #endregion

    #region Ships Management
    public void UpdateShipManagement()
    {
        List<Ship> shipsToRemove = new List<Ship>();
        foreach (Ship ship in shipsInStrem)
        {
            if (ship == null)
                shipsToRemove.Add(ship);
            else
                ship.ShipMvt.UpdateStreamForce(GetStreamForce(ship.transform.position));
        }

        foreach (Ship ship in shipsToRemove)
        {
            shipsInStrem.Remove(ship);
        }
    }
    #endregion

    #region Crates Management
    public void UpdateCratesManagement()
    {
        List<EnemyLootCrate> cratesToRemove = new List<EnemyLootCrate>();
        foreach (EnemyLootCrate crate in cratesInStream)
        {
            if (crate == null)
                cratesToRemove.Add(crate);
            else
                crate.UpdateStreamForce(GetStreamForce(crate.transform.position));
        }

        foreach (EnemyLootCrate crate in cratesToRemove)
        {
            cratesInStream.Remove(crate);
        }
    }
    #endregion

    #region Projectiles Management
    public void UpdateProjectilesManagement()
    {
        List<Projectile> projectilesToRemove = new List<Projectile>();
        foreach (Projectile proj in projectilesInStream)
        {
            if (proj == null)
                projectilesToRemove.Add(proj);
            else
                proj.UpdateStreamForce(GetStreamForce(proj.transform.position));
        }

        foreach (Projectile proj in projectilesToRemove)
        {
            projectilesInStream.Remove(proj);
        }
    }
    #endregion

    #region Rendering 
    [Header("Rendering")]
    [SerializeField] MeshRenderer helpingMeshRenderer;
    [SerializeField] ParticleSystem[] streamParticles;
    [SerializeField] float streamRenderingPrecision = 1;
    [SerializeField] float streamSpeedPerUnit = 1;
    public float GetStreamSpeedPerUnit { get { return streamSpeedPerUnit; } }
    [SerializeField] float streamLifetimePerUnit = 1;
    public float GetStreamLifetimePerUnit { get { return streamLifetimePerUnit; } }
    [SerializeField] GameObject arrowPrefab;
    [SerializeField] float maximumSpaceBetweenTwoArrows;
    [SerializeField] float firstArrowDistance;
    [SerializeField] float arrowVerticalOffset;

    public void SetUpStreamRenderer()
    {
        if (vertexPath == null)
            vertexPath = pathCreator.path;

        #region Calculate Movement Curve
        #region  V1
        /*float lifeTime = streamLifetimePerUnit * vertexPath.length;
        float speedCoeff = streamSpeedPerUnit * lifeTime;
        float width = streamCreator.roadWidth;*/
        #endregion

        #region  V2
        float speedCoeff = streamSpeedPerUnit;
        float lifeTime = streamLifetimePerUnit * vertexPath.length;
        float width = streamCreator.roadWidth;
        #endregion

        ParticleSystem.MinMaxCurve xFollowCurve = new ParticleSystem.MinMaxCurve(speedCoeff, new AnimationCurve());
        ParticleSystem.MinMaxCurve zFollowCurve = new ParticleSystem.MinMaxCurve(speedCoeff, new AnimationCurve());

        for (float i = 0; i < vertexPath.length; i+= streamRenderingPrecision)
        {
            float time = i / vertexPath.length;
            Vector3 direction = vertexPath.GetDirectionAtDistance(i);

            Keyframe xKeyFr = new Keyframe(time, direction.x);
            xFollowCurve.curve.AddKey(xKeyFr);
            Keyframe zKeyFr = new Keyframe(time, direction.z);
            zFollowCurve.curve.AddKey(zKeyFr);
        }        
        #endregion

        foreach (ParticleSystem streamPart in streamParticles)
        {
            streamPart.transform.position = vertexPath.GetPointAtDistance(0);

            ParticleSystem.MainModule mainModule = streamPart.main;
            mainModule.startLifetime = lifeTime;

            ParticleSystem.ShapeModule shapeModule = streamPart.shape;
            shapeModule.radius = width;

            ParticleSystem.VelocityOverLifetimeModule velocityOverLifetime = streamPart.velocityOverLifetime;
            velocityOverLifetime.x = xFollowCurve;
            velocityOverLifetime.z = zFollowCurve;
        }
#if UNITY_EDITOR
        if (!UnityEditor.EditorApplication.isPlaying)
            return;
#endif
            int numberOfArrows = Mathf.Clamp((int)(vertexPath.length / maximumSpaceBetweenTwoArrows), 1, 100);

        for(int i = 0; i < numberOfArrows + 1; i++)
        {
            GameObject newArrow = Instantiate(arrowPrefab);
            float coeff = (float)(i) / (float)numberOfArrows;
            float distance = Mathf.Lerp(firstArrowDistance, vertexPath.length, coeff);
            newArrow.transform.position = vertexPath.GetPointAtDistance(distance - 0.01f) + new Vector3(0, arrowVerticalOffset, 0);

            Vector3 direction = vertexPath.GetDirectionAtDistance(distance - 0.01f);
            float rotY = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            newArrow.transform.localRotation = Quaternion.Euler(new Vector3(0, rotY, 0));
            newArrow.transform.localScale = Vector3.one * streamCreator.roadWidth * 2;
        }
    }
    #endregion
}
