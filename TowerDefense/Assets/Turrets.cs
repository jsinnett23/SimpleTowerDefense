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

    [Header("Attribute")]
    [SerializeField] private float targetingRange = 5f;
    [SerializeField] List<GameObject> enemies = new List<GameObject>();

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
        // Filter out all enemies that are out of range
        var inRangeEnemies = enemies.Where(enemy => IsTargetInRange(enemy.transform)).ToList();

        // If there are no enemies in range, clear the target and return
        if (!inRangeEnemies.Any())
        {
            target = null;
            return;
        }

        // Sort in-range enemies by distance to the exit point
        inRangeEnemies.Sort((a, b) =>
            Vector3.Distance(a.transform.position, exitPoint.position)
            .CompareTo(Vector3.Distance(b.transform.position, exitPoint.position))
        );

        // The closest enemy to the exit that is within range becomes the new target
        target = inRangeEnemies[0].transform;
    }
    private bool IsTargetInRange(Transform target)
    {
        return Vector3.Distance(turretRotationPoint.position, target.position) <= targetingRange;
    }
}
