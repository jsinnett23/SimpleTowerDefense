using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour{
    // Start is called before the first frame update

    [Header("Sprites")]
    [SerializeField] private Sprite[] waveSprites; // Array to hold sprites for different waves

    [Header("References")]
    [SerializeField] private Rigidbody2D rb;

    [Header("Base Attributes")]
    [SerializeField] private float baseMoveSpeed = 2f;
    [SerializeField] private float baseHealth = 5f;

    private float moveSpeed;
    private float health;

    private Transform target;
    private int pathIndex = 0;

    private SpriteRenderer spriteRenderer;




    void Start()
    {
        target = LevelManager.main.path[pathIndex];
        ScaleAttributes(EnemySpanwer.currentWave);
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            return;
        }

        // Call method to set the appropriate sprite for the current wave
        SetSpriteForWave(EnemySpanwer.currentWave);

        // Scale attributes based on the current wave
        ScaleAttributes(EnemySpanwer.currentWave);


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
    private void ScaleAttributes(int waveNumber)
    {
        // Increase these factors for more significant scaling per wave
        float healthScalingFactor = 1.1f; // Increased from 1.1f
        float speedScalingFactor = 1.05f; // Increased from 1.05f

        health = baseHealth * Mathf.Pow(healthScalingFactor, waveNumber - 1);
        moveSpeed = baseMoveSpeed * Mathf.Pow(speedScalingFactor, waveNumber - 1);
    }
    private void SetSpriteForWave(int waveNumber)
    {
        // Select sprite based on wave number
        if (waveNumber >= 20 && waveSprites.Length > 4)
            spriteRenderer.sprite = waveSprites[4];
        else if (waveNumber >= 15 && waveSprites.Length > 3)
            spriteRenderer.sprite = waveSprites[3];
        else if (waveNumber >= 10 && waveSprites.Length > 2)
            spriteRenderer.sprite = waveSprites[2];
        else if (waveNumber >= 5 && waveSprites.Length > 1)
            spriteRenderer.sprite = waveSprites[1];
        else
            spriteRenderer.sprite = waveSprites[0]; // Default sprite
    }

    private void FixedUpdate()
    {
        Vector2 direction = (target.position - transform.position).normalized;

        rb.velocity = direction * moveSpeed;
    }
}
