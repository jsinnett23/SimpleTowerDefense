using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float damage = 10f; // Damage the projectile deals
    public Transform target;  // Ensure this line is present
    private Vector2 direction;


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
        if (target != null)
        {
            Vector2 direction = (target.position - transform.position).normalized;
            transform.Translate(direction * speed * Time.deltaTime);
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            collision.gameObject.GetComponent<EnemyMovement>()?.TakeDamage(1);
            Destroy(gameObject);
        }
    }
}
