using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Light))]
public class SpotlightTrigger : MonoBehaviour
{
    [Header("Spotlight Settings")]
    [Tooltip("The spotlight that triggers the detection")]
    public Light spotlight;
    [Tooltip("Layer mask to define which entities should be detected")]
    public LayerMask entityLayer;
    [Tooltip("Interval between entity checks in seconds")]
    public float checkInterval = 0.25f;

    private void Start()
    {
        ValidateSpotlight();
        StartCoroutine(CheckEntitiesInLightRoutine());
    }

    /// <summary>
    /// Validates if the attached light is a spotlight.
    /// </summary>
    private void ValidateSpotlight()
    {
        if (spotlight.type != LightType.Spot)
        {
            Debug.LogError("This script should be attached to a spotlight.");
            enabled = false;
        }
    }

    /// <summary>
    /// Coroutine to periodically check for entities within the spotlight's range.
    /// </summary>
    private IEnumerator CheckEntitiesInLightRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(checkInterval);
            CheckEntities();
        }
    }

    /// <summary>
    /// Checks for entities within the spotlight's range and triggers the appropriate actions.
    /// </summary>
    private void CheckEntities()
    {
        Collider[] colliders = Physics.OverlapSphere(spotlight.transform.position, spotlight.range, entityLayer);
        foreach (Collider collider in colliders)
        {

            Vector3 directionToCollider = (collider.transform.position - spotlight.transform.position).normalized;
            float angleToCollider = Vector3.Angle(spotlight.transform.forward, directionToCollider);

            if (angleToCollider < spotlight.spotAngle / 2f)
            {
                if (Physics.Raycast(spotlight.transform.position, directionToCollider, out RaycastHit hit, spotlight.range))
                {
                    if (hit.collider == collider)
                    {
                        TriggerScript(hit.collider.gameObject);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Triggers the necessary actions on the detected entity.
    /// </summary>
    /// <param name="entity">The detected entity game object</param>
    private void TriggerScript(GameObject entity)
    {
        HealthPlayer playerHealth = entity.GetComponent<HealthPlayer>();
        HealthEnemy enemyHealth = entity.GetComponent<HealthEnemy>();
        ParticleSystem particleSystem = entity.GetComponent<ParticleSystem>();

        if (particleSystem != null)
        {
            particleSystem.Play();
        }

        if (playerHealth != null)
        {
            playerHealth.spotlightDamageMultiplier = 3;
        }
        else if (enemyHealth != null)
        {
            enemyHealth.SpotlightDamageMultiplier = 3;
        }
        else
        {
            Debug.LogWarning("Entity does not have a HealthPlayer or HealthEnemy component");
        }
    }

    /// <summary>
    /// Sets the spotlight for the script.
    /// </summary>
    /// <param name="newSpotlight">New spotlight to be set</param>
    public void SetSpotlight(Light newSpotlight)
    {
        spotlight = newSpotlight;
        ValidateSpotlight();
    }

    /// <summary>
    /// Sets the layer mask for detecting entities.
    /// </summary>
    /// <param name="newEntityLayer">New entity layer mask</param>
    public void SetEntityLayer(LayerMask newEntityLayer)
    {
        entityLayer = newEntityLayer;
    }

    /// <summary>
    /// Sets the interval between entity checks.
    /// </summary>
    /// <param name="newCheckInterval">New check interval in seconds</param>
    public void SetCheckInterval(float newCheckInterval)
    {
        checkInterval = newCheckInterval;
    }
}
