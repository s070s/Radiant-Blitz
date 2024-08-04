using UnityEngine;
using System.Collections;
using UnityEditor.PackageManager;

[RequireComponent(typeof(Light))]
public class SpotlightDetector : MonoBehaviour
{
    [Header("Spotlight Configuration")]
    [Tooltip("The spotlight used for detection")]
    public Light detectionSpotlight;
    [Tooltip("Layer mask to specify detectable entities")]
    public LayerMask detectableLayers;
    [Tooltip("Frequency of detection checks in seconds")]
    public float detectionFrequency = 0.10f;

    private void Start()
    {
        ValidateLightType();
        StartCoroutine(DetectionCycle());

    }

    /// <summary>
    /// Ensures the attached light is a spotlight.
    /// </summary>
    private void ValidateLightType()
    {
        if (detectionSpotlight.type != LightType.Spot)
        {
            Debug.LogError("The attached light is not a spotlight. Please attach the script to a spotlight.");
            enabled = false;
        }
    }

    /// <summary>
    /// Repeatedly checks for entities within the spotlight's effective range.
    /// </summary>
    private IEnumerator DetectionCycle()
    {
        while (true)
        {
            yield return new WaitForSeconds(detectionFrequency);
            DetectEntitiesWithinRange();
        }
    }

    /// <summary>
    /// Detects entities within the range of the spotlight and performs actions.
    /// </summary>
    private void DetectEntitiesWithinRange()
    {
        Collider[] detectedEntities = Physics.OverlapSphere(detectionSpotlight.transform.position, detectionSpotlight.range, detectableLayers);
        foreach (Collider entity in detectedEntities)
        {
            Vector3 directionToEntity = (entity.transform.position - detectionSpotlight.transform.position).normalized;
            float angleToEntity = Vector3.Angle(detectionSpotlight.transform.forward, directionToEntity);

            if (angleToEntity < detectionSpotlight.spotAngle / 2f)
            {
                if (Physics.Raycast(detectionSpotlight.transform.position, directionToEntity, out RaycastHit hitInfo, detectionSpotlight.range))
                {
                    if (hitInfo.collider == entity)
                    {
                        ActivateEntityResponse(hitInfo.collider.gameObject);
                    }
                }
            }
        }
    }
    private void ActivateEntityResponse(GameObject entity)
    {
        ParticleSystem EnemyEffects = entity.GetComponent<ParticleSystem>();
        EnemyEffects.Play();
    }
}
