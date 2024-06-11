using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HealthEnemy : MonoBehaviour
{
    [Header("Multiplier Settings")]
    [SerializeField, Range(1, 5)]
    private int startDamageMultiplier = 1;
    [SerializeField, Range(1, 5)]
    private int startHealthMultiplier = 1;
    [Range(1, 10)]
    public int SpotlightDamageMultiplier = 1;

    [Header("Health Settings")]
    [SerializeField]
    private int baseHealth = 3;

    [Header("UI Settings")]
    [SerializeField]
    private GameObject gameObjectToDie;
    [SerializeField]
    private TMP_Text healthTextProComp;
    [SerializeField]
    private Color healthColorOpaque;
    [SerializeField]
    private Transform healthCanvas;

    [Header("Player Settings")]
    [SerializeField]
    private Transform player;

    private int health;
    private int damageTaken;
    private Shooting playerShootingScript;
    private GameObject playerCurrentProjectile;
    private Projectile projectileComponent;

    public bool registerHit;

    private void Start()
    {
        InitializePlayerSettings();
        InitializeHealthSettings();
        UpdateHealthUI();
    }

    private void Update()
    {
        if (health <= 0)
        {
            Die();
        }

        UpdateHealthUI();
        RotateHealthCanvas();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.tag=="Projectile")
        {
            registerHit = true;
            SubtractHealth();
        }
    }

    /// <summary>
    /// Initializes player-related settings.
    /// </summary>
    private void InitializePlayerSettings()
    {
        playerShootingScript = player.GetComponent<Shooting>();
        int projectileIndex = playerShootingScript.projectilePrefabChosenIndex;
        playerCurrentProjectile = playerShootingScript.projectilePrefabs[projectileIndex];
        projectileComponent = playerCurrentProjectile.GetComponent<Projectile>();
        damageTaken = projectileComponent.damage * startDamageMultiplier;
    }

    /// <summary>
    /// Initializes health-related settings.
    /// </summary>
    private void InitializeHealthSettings()
    {
        health = baseHealth * startHealthMultiplier;
        SpotlightDamageMultiplier = 1;
        healthTextProComp.color = healthColorOpaque;
    }

    /// <summary>
    /// Updates the health UI text.
    /// </summary>
    private void UpdateHealthUI()
    {
        healthTextProComp.text = health.ToString();
    }

    /// <summary>
    /// Rotates the health canvas to face the player.
    /// </summary>
    private void RotateHealthCanvas()
    {
        healthCanvas.LookAt(player);
    }

    /// <summary>
    /// Subtracts health based on damage taken and spotlight damage multiplier.
    /// </summary>
    private void SubtractHealth()
    {
        health -= damageTaken * SpotlightDamageMultiplier;
    }

    /// <summary>
    /// Destroys the game object when health reaches zero.
    /// </summary>
    private void Die()
    {
        Destroy(gameObjectToDie);
    }
}
