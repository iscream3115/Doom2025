using UnityEngine;

public class Imp : EnemyBase
{
    [Header("Imp Attack")]
    [SerializeField] private GameObject fireballPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float projectileSpeed = 18f;
    [SerializeField] private float attackCooldown = 1.5f;

    private float nextAttackTime;

    protected override void Attack()
    {
        if (Target == null) return;
        if (Time.time < nextAttackTime) return;

        Vector3 toTarget = Target.position - transform.position;
        toTarget.y = 0f;
        if (toTarget.sqrMagnitude <= 0.0001f) return;

        float distance = toTarget.magnitude;
        if (distance > RangedAttackRange) return;

        transform.rotation = Quaternion.LookRotation(toTarget.normalized, Vector3.up);

        ShootProjectile();
        nextAttackTime = Time.time + attackCooldown;
    }

    private void ShootProjectile()
    {
        if (fireballPrefab == null) return;

        Vector3 spawnPosition = firePoint != null ? firePoint.position : transform.position + transform.forward + Vector3.up;
        Vector3 toTarget = (Target.position - spawnPosition).normalized;
        Quaternion shootRotation = Quaternion.LookRotation(toTarget, Vector3.up);

        GameObject fireball = Instantiate(fireballPrefab, spawnPosition, shootRotation);
        Rigidbody fireballRb = fireball.GetComponent<Rigidbody>();
        if (fireballRb != null)
        {
            fireballRb.linearVelocity = toTarget * projectileSpeed;
        }
    }

    private void MeleeAttack()
    {
        
    }

}
