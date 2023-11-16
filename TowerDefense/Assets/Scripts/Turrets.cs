using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

public class Turrets : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private Transform turretRotationPoint;
    [SerializeField] private LayerMask enemyMask;
    private Transform exitPoint;
    [SerializeField] private AudioSource shootingAudioSource;

    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;

    [Header("Attribute")]
    [SerializeField] private float targetingRange = 5f;
    [SerializeField] List<GameObject> enemies = new List<GameObject>();
    [SerializeField] private float fireRate = 1f; // Shots per second
    private float fireTimer = 0f;



    private Transform target;


   

    // Start is called before the first frame update
    void Start()
    {
        EnemySpanwer.enemySpawn.AddListener(addToList);
        EnemySpanwer.enemyDestroyed.AddListener(removeFromList);

        // Get the exitPoint from GameManager
        if (GameManager.Instance != null)
        {
            exitPoint = GameManager.Instance.exitPoint;
        }
        else
        {
        }
    }


    // Update is called once per frame
    private void Update()
    {
        if (target == null || !IsTargetInRange(target))
        {
            FindTarget();
        }

        if (target != null)
        {
            RotateTowardsTarget();

            fireTimer += Time.deltaTime;

            if (fireTimer >= 1f / fireRate)
            {
                Fire(target);
                fireTimer = 0f;
            }
        }
        else
        {
        }
    }
    private void RotateTowardsTarget()
    {
        // If there's no target or it's out of range, don't rotate
        if (target == null || !IsTargetInRange(target))
            return;

        Vector3 targetDirection = target.position - turretRotationPoint.position;
        float angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg - 90;
        turretRotationPoint.rotation = Quaternion.Euler(0, 0, angle);
    }


    private void addToList(GameObject enemy)
    {
        enemies.Add(enemy);
    }
    private void FindTarget()
    {
        // Filter out null entries and enemies that are out of range
        var inRangeEnemies = enemies.Where(enemy => enemy != null && IsTargetInRange(enemy.transform)).ToList();

        if (!inRangeEnemies.Any())
        {
            target = null;
            return;
        }

        inRangeEnemies.Sort((a, b) =>
            Vector3.Distance(a.transform.position, exitPoint.position)
            .CompareTo(Vector3.Distance(b.transform.position, exitPoint.position))
        );

        target = inRangeEnemies[0].transform;
    }
    private bool IsTargetInRange(Transform target)
    {
        return Vector3.Distance(turretRotationPoint.position, target.position) <= targetingRange;
    }
    private void removeFromList(GameObject enemy)
    {
        enemies.Remove(enemy);
    }

    private void Fire(Transform enemyTarget)
    {
        if (enemyTarget == null)
        {
            Debug.Log("Fire called but no target.");
            return;
        }

        // Instantiate the projectile at the firePoint's position
        GameObject projectileInstance = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Debug.Log("Projectile instantiated.");

        Projectile projectileScript = projectileInstance.GetComponent<Projectile>();

        if (projectileScript != null)
        {
            // Set the projectile's rotation to match the Z-axis rotation of the firePoint
            float zRotation = firePoint.eulerAngles.z;
            projectileInstance.transform.rotation = Quaternion.Euler(0, 0, zRotation);
            Debug.Log("Projectile rotation set to: " + zRotation);

            // Initialize the projectile
            Vector2 fireDirection = (enemyTarget.position - firePoint.position).normalized;
            projectileScript.Initialize(fireDirection);  // Assuming your projectile script has an Initialize method
            Debug.Log("Projectile initialized with direction: " + fireDirection);
        }
        if (shootingAudioSource != null)
        {
            shootingAudioSource.Play();
        }
        else
        {
            Debug.Log("Projectile script not found.");
        }
    }
}
