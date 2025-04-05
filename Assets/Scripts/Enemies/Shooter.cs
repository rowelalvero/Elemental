using System;
using System.Collections;
using UnityEngine;

public class Shooter : MonoBehaviour, IEnemy
{
    [Header("Bullet Settings")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletMoveSpeed = 5f;
    [SerializeField] private int burstCount = 3;
    [SerializeField] private int projectilesPerBurst = 5;
    [SerializeField][Range(0, 359)] private float angleSpread = 90f;
    [SerializeField] private float startingDistance = 0.1f;

    [Header("Timing Settings")]
    [SerializeField] private float timeBetweenBursts = 0.3f;
    [SerializeField] private float restTime = 1f;

    [Header("Behavior")]
    [SerializeField] private bool stagger;
    [Tooltip("Stagger must be enabled for oscillate to function properly.")]
    [SerializeField] private bool oscillate;

    private bool isShooting = false;
    private Action onAttackFinished;

    public bool IsAttacking => isShooting;

    private void OnValidate()
    {
        if (oscillate) stagger = true;
        else stagger = false;

        projectilesPerBurst = Mathf.Max(1, projectilesPerBurst);
        burstCount = Mathf.Max(1, burstCount);
        timeBetweenBursts = Mathf.Max(0.1f, timeBetweenBursts);
        restTime = Mathf.Max(0.1f, restTime);
        startingDistance = Mathf.Max(0.1f, startingDistance);
        if (angleSpread == 0) projectilesPerBurst = 1;
        if (bulletMoveSpeed <= 0) bulletMoveSpeed = 0.1f;
    }

    public void Attack(Action onComplete)
    {
        if (!isShooting)
        {
            onAttackFinished = onComplete;
            StartCoroutine(ShootRoutine());
        }
    }

    private IEnumerator ShootRoutine()
    {
        isShooting = true;

        float startAngle, currentAngle, angleStep, endAngle;
        float timeBetweenProjectiles = stagger ? timeBetweenBursts / projectilesPerBurst : 0f;

        TargetConeOfInfluence(out startAngle, out currentAngle, out angleStep, out endAngle);

        for (int i = 0; i < burstCount; i++)
        {
            if (!oscillate)
            {
                TargetConeOfInfluence(out startAngle, out currentAngle, out angleStep, out endAngle);
            }

            if (oscillate && i % 2 != 1)
            {
                TargetConeOfInfluence(out startAngle, out currentAngle, out angleStep, out endAngle);
            }
            else if (oscillate)
            {
                float temp = startAngle;
                startAngle = endAngle;
                endAngle = temp;
                angleStep *= -1;
                currentAngle = startAngle;
            }

            for (int j = 0; j < projectilesPerBurst; j++)
            {
                Vector2 pos = FindBulletSpawnPos(currentAngle);
                GameObject newBullet = Instantiate(bulletPrefab, pos, Quaternion.identity);
                newBullet.transform.right = newBullet.transform.position - transform.position;

                if (newBullet.TryGetComponent(out Projectile projectile))
                {
                    projectile.UpdateMoveSpeed(bulletMoveSpeed);
                }

                currentAngle += angleStep;

                if (stagger)
                    yield return new WaitForSeconds(timeBetweenProjectiles);
            }

            currentAngle = startAngle;

            if (!stagger)
                yield return new WaitForSeconds(timeBetweenBursts);
        }

        yield return new WaitForSeconds(restTime);
        isShooting = false;

        onAttackFinished?.Invoke();
        onAttackFinished = null;
    }

    private void TargetConeOfInfluence(out float startAngle, out float currentAngle, out float angleStep, out float endAngle)
    {
        Vector2 targetDirection = PlayerController.Instance.transform.position - transform.position;
        float targetAngle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;

        float halfAngleSpread = angleSpread / 2f;

        if (angleSpread != 0)
        {
            startAngle = targetAngle - halfAngleSpread;
            endAngle = targetAngle + halfAngleSpread;
            angleStep = angleSpread / (projectilesPerBurst - 1);
        }
        else
        {
            startAngle = targetAngle;
            endAngle = targetAngle;
            angleStep = 0;
        }

        currentAngle = startAngle;
    }

    private Vector2 FindBulletSpawnPos(float angle)
    {
        float rad = angle * Mathf.Deg2Rad;
        float x = transform.position.x + startingDistance * Mathf.Cos(rad);
        float y = transform.position.y + startingDistance * Mathf.Sin(rad);
        return new Vector2(x, y);
    }
}
