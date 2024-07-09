using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static UnityEngine.EventSystems.EventTrigger;

public class HealthEnemy : MonoBehaviour
{
    [Header("Multiplier Settings")]
    [SerializeField, Range(1, 5)]
    private int HealthMultiplier = 1;

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

    private int ActualHealth;
    [HideInInspector]
    public int ActualEnemyDamage;
    private Shooting playerShootingScript;
    [HideInInspector]
    public bool registerHit;
    [SerializeField]
    ParticleSystem EnemyParticles;

    private void Start()
    {
        ActualEnemyDamage = 1;
        InitializePlayerSettings();
        InitializeHealthSettings();
    }

    private void Update()
    {
        if (EnemyParticles.isStopped) { ActualEnemyDamage = 1; }
        else if(EnemyParticles.isPlaying) { ActualEnemyDamage = 10; }
        else { return;}
        if (ActualHealth <= 0)
        {
            Die();
        }

        UpdateHealthUI();
        RotateHealthCanvas();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.tag == "Projectile" && !registerHit)
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
    }

    /// <summary>
    /// Initializes health-related settings.
    /// </summary>
    private void InitializeHealthSettings()
    {
        ActualHealth = baseHealth * HealthMultiplier;
        healthTextProComp.color = healthColorOpaque;
    }

    /// <summary>
    /// Updates the health UI text.
    /// </summary>
    private void UpdateHealthUI()
    {
        healthTextProComp.text = ActualHealth.ToString();
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
        ActualHealth -= ActualEnemyDamage;
        registerHit = false;
    }

    /// <summary>
    /// Destroys the game object when health reaches zero.
    /// </summary>
    private void Die()
    {
        Destroy(gameObjectToDie);
    }
}
