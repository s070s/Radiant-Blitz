using System;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    [Header("Shooting Settings")]
    [Tooltip("The anchor point from where projectiles are fired")]
    [SerializeField]
    private GameObject anchor;
    [Tooltip("Index to choose the projectile prefab")]
    [SerializeField, Range(0, 1)]
    public int projectilePrefabChosenIndex;
    [Tooltip("Array of projectile prefabs")]
    [SerializeField]
    public GameObject[] projectilePrefabs;

    [Tooltip("Reference to the SmoothOrbitCamera script")]
    [SerializeField]
    private SmoothOrbitCamera orbitCamera; // Add this reference

    private Projectile[] projectiles;
    private GameObject selectedProjectileGameObject;
    private Projectile selectedProjectile;
    private int projectileCount;
    private Vector3[] positions;

    private void Awake()
    {
        InitializeProjectiles();
    }

    private void Start()
    {
        UpdateSelectedProjectile();
    }

    private void Update()
    {
        HandleInput();
        UpdateSelectedProjectile();
        UpdateAnchorRotation(); // Update the anchor rotation
    }

    private void UpdateAnchorRotation()
    {
        if (orbitCamera != null)
        {
            if (Input.GetButton("Fire2"))
            {
                Vector3 rotation = anchor.transform.eulerAngles;
                rotation.x = orbitCamera.pitch; // Apply the pitch rotation
                anchor.transform.eulerAngles = rotation;
            }

        }
    }

    private void InitializeProjectiles()
    {
        projectiles = new Projectile[projectilePrefabs.Length];

        for (int i = 0; i < projectilePrefabs.Length; i++)
        {
            GameObject projectileGameObject = projectilePrefabs[i];
            projectiles[i] = projectileGameObject.GetComponent<Projectile>();
        }
    }

    private void UpdateSelectedProjectile()
    {
        selectedProjectileGameObject = projectilePrefabs[projectilePrefabChosenIndex];
        selectedProjectile = projectiles[projectilePrefabChosenIndex];
        projectileCount = selectedProjectile.projectileCount;
        positions = new Vector3[projectileCount];
    }

    private void HandleInput()
    {
        if (Input.GetButtonDown("Fire1")) // Assuming "Fire1" is set up in the Input Manager
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        if (projectileCount == 1)
        {
            InstantiateProjectile(anchor.transform.position);
        }
        else if (projectileCount > 1)
        {
            for (int i = 0; i < projectileCount; i++)
            {
                Vector3 startPos = anchor.transform.position - anchor.transform.right;
                positions[i] = startPos + anchor.transform.right * i;
                InstantiateProjectile(positions[i]);
            }
        }
    }

    private void InstantiateProjectile(Vector3 position)
    {
        GameObject instance = Instantiate(selectedProjectileGameObject, position, Quaternion.identity);
        instance.transform.forward = anchor.transform.forward;
    }
}
