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
    [SerializeField] private Transform exitPoint;

    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;

    [Header("Attribute")]
    [SerializeField] private float targetingRange = 5f;
    [SerializeField] List<GameObject> enemies = new List<GameObject>();
    [SerializeField] private float fireRate = 1f; // Shots per second
    private float fireTimer = 0f;



    private Transform target;

    private void OnDrawGizmosSelected()
    {
        Handles.color = Color.cyan;
        Handles.DrawWireDisc(turretRotationPoint.position, transform.forward, targetingRange);
    }

    // Start is called before the first frame update
    void Start()
    {
        EnemySpanwer.enemySpawn.AddListener(addToList);
        EnemySpanwer.enemyDestroyed.AddListener(removeFromList);  // New line
    }

    // Update is called once per frame
    private void Update()
    {
        // Check if the current target is out of range or null, then find a new target
        if (target == null || !IsTargetInRange(target))
        {
            FindTarget();
        }

        RotateTowardsTarget();

        fireTimer += Time.deltaTime;

        if (target != null && fireTimer >= 1f / fireRate)
        {
            Fire(target);
            fireTimer = 0f;
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
        GameObject projectileInstance = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Projectile projectileScript = projectileInstance.GetComponent<Projectile>();

        if (projectileScript != null && enemyTarget != null)
        {
            Vector2 fireDirection = (enemyTarget.position - firePoint.position).normalized;
            projectileScript.Initialize(fireDirection);
        }
    }
}
