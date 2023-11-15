using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour{
    // Start is called before the first frame update

    [Header("Refrences")]
    [SerializeField] private Rigidbody2D rb;

    [Header ("Attributes")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float health = 5; // Default health value


    private Transform target;
    private int pathIndex = 0;




    void Start()
    {
        target = LevelManager.main.path[pathIndex];
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector2.Distance(target.position, transform.position) <= 0.1f)
        {
            pathIndex++;

            if (pathIndex == LevelManager.main.path.Length)
            {
                LevelManager.main.EnemyReachedBase(); // Notify that an enemy reached the base
                EnemySpanwer.onEnemyDestroyed.Invoke(this.gameObject); // Notify that an enemy is "destroyed"
                Destroy(gameObject);
                return;
            }
            else
            {
                target = LevelManager.main.path[pathIndex];
            }
        }
    }

    private bool isDead = false;

    public void TakeDamage(int damage)
    {
        if (isDead) return; // Prevent multiple calls if already dead

        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return; // Prevent multiple calls

        isDead = true;
        LevelManager.main.EnemyDestroyed(); // Notify LevelManager that an enemy has been destroyed
        EnemySpanwer.onEnemyDestroyed.Invoke(this.gameObject);
        Destroy(gameObject); // Destroy the enemy
    }

    private void FixedUpdate()
    {
        Vector2 direction = (target.position - transform.position).normalized;

        rb.velocity = direction * moveSpeed;
    }
}
