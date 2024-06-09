using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    [Tooltip("Damage dealt by the projectile")]
    public int damage;
    [Tooltip("Speed of the projectile")]
    public float speed;
    [Tooltip("Lifetime of the projectile in seconds")]
    public float lifeTime = 2f;
    [Tooltip("Number of projectiles to be fired")]
    [Range(1, 3)]
    public int projectileCount;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        Initialize();
    }

    /// <summary>
    /// Initializes the projectile by setting its velocity and scheduling its destruction.
    /// </summary>
    public void Initialize()
    {
        SetVelocity();
        ScheduleDestruction();
    }

    /// <summary>
    /// Sets the velocity of the projectile based on its speed and direction.
    /// </summary>
    private void SetVelocity()
    {
        rb.velocity = transform.forward * speed;
    }

    /// <summary>
    /// Schedules the destruction of the projectile after its lifetime has elapsed.
    /// </summary>
    private void ScheduleDestruction()
    {
        Destroy(gameObject, lifeTime);
    }

    /// <summary>
    /// Sets the damage value for the projectile.
    /// </summary>
    /// <param name="newDamage">New damage value to be set.</param>
    public void SetDamage(int newDamage)
    {
        damage = newDamage;
    }

    /// <summary>
    /// Sets the speed value for the projectile.
    /// </summary>
    /// <param name="newSpeed">New speed value to be set.</param>
    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    /// <summary>
    /// Sets the lifetime value for the projectile.
    /// </summary>
    /// <param name="newLifetime">New lifetime value to be set.</param>
    public void SetLifetime(float newLifetime)
    {
        lifeTime = newLifetime;
    }

    /// <summary>
    /// Sets the projectile count value.
    /// </summary>
    /// <param name="newCount">New projectile count to be set.</param>
    public void SetProjectileCount(int newCount)
    {
        projectileCount = Mathf.Clamp(newCount, 1, 3);
    }
}
