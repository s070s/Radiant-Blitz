using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HealthPlayer : MonoBehaviour
{
    [Header("Multiplier Settings")]
    [SerializeField, Range(1, 5)]
    private int DamageTakenMultiplier = 1;
    [SerializeField, Range(1, 5)]
    private int startHealthMultiplier = 1;

    [Header("Health Settings")]
    [SerializeField]
    private int baseHealth = 3;
    [SerializeField]
    private int baseDamageTaken = 2;

    [Header("UI Settings")]
    [SerializeField]
    private GameObject gameObjectToDie;
    [SerializeField]
    private TMP_Text healthTextProComp;
    [SerializeField]
    private Transform healthCanvas;

    private int health;
    [HideInInspector]
    public int ActualPlayerDamage;
    private Color healthColorOpaque = Color.white;

    private void Start()
    {
        InitializeHealthSettings();
        UpdateHealthUI();
    }

    private void Update()
    {
        ActualPlayerDamage = baseDamageTaken * DamageTakenMultiplier;
        CheckHealth();
        UpdateHealthUI();
    }

    private void OnTriggerEnter(Collider hit)
    {
        if (hit.gameObject.name=="Enemy")
        {
            Debug.Log("Collision");
            SubtractHealth();
        }
    }

    /// <summary>
    /// Initializes the health settings based on multipliers.
    /// </summary>
    private void InitializeHealthSettings()
    {
        health = baseHealth * startHealthMultiplier;

        if (this.name == "Player")
        {
            healthTextProComp.color = healthColorOpaque;
        }
    }

    /// <summary>
    /// Updates the health UI text.
    /// </summary>
    private void UpdateHealthUI()
    {
        healthTextProComp.text = "Health: " + health.ToString();
    }

    /// <summary>
    /// Checks the player's health and triggers death if health is zero or less.
    /// </summary>
    private void CheckHealth()
    {
        if (health <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// Subtracts health based on damage taken and spotlight damage multiplier.
    /// </summary>
    private void SubtractHealth()
    {
        health -= ActualPlayerDamage;
    }

    /// <summary>
    /// Destroys the game object when health reaches zero and quits the application.
    /// </summary>
    private void Die()
    {
        Destroy(gameObjectToDie);
        Application.Quit();
    }
}
