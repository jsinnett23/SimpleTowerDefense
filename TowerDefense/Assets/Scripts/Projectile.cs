using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float lifespan = 2f; // Duration before the projectile is destroyed


    public Transform target;  // Ensure this line is present
    private Vector2 direction;
    private float timer;

    public void Initialize(Vector2 initialDirection)
    {
        direction = initialDirection;
        timer = lifespan;
    }

    void Start()
    {

        if (target != null)
        {
            // Calculate direction towards the target
            direction = (target.position - transform.position).normalized;
        }
    }
    void Update()
    {
        // Move the projectile in the set direction
        transform.Translate(direction * speed * Time.deltaTime);

        // Destroy the projectile after its lifespan is over
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            Destroy(gameObject);
        }

    }

    private bool hasCollided = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (hasCollided) return; // Prevent multiple triggers

        if (collision.gameObject.CompareTag("Enemy"))
        {
            hasCollided = true;
            collision.gameObject.GetComponent<EnemyMovement>()?.TakeDamage(1);
            Destroy(gameObject); // Destroy the projectile immediately to prevent multiple collisions
        }
    }
    public void SetInitialRotation(Transform turretGunTransform)
    {
        // Match the rotation of the projectile to the rotation of the turret's gun
        this.transform.rotation = turretGunTransform.rotation;

        
    }
}
