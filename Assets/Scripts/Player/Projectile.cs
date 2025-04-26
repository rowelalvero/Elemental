using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 22f;
    [SerializeField] private GameObject particleOnHitPrefabVFX;
    [SerializeField] private bool isEnemyProjectile = false;
    [SerializeField] private float projectileRange = 10f;

    [Header("AOE Settings")]
    [SerializeField] private bool spawnAOEOnHit = false;
    [SerializeField] private GameObject aoePrefab;

    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        MoveProjectile();
        DetectFireDistance();
    }

    public void UpdateProjectileRange(float projectileRange)
    {
        this.projectileRange = projectileRange;
    }

    public void UpdateMoveSpeed(float moveSpeed)
    {
        this.moveSpeed = moveSpeed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        EnemyHealth enemyHealth = other.gameObject.GetComponent<EnemyHealth>();
        Indestructible indestructible = other.gameObject.GetComponent<Indestructible>();
        PlayerHealth player = other.gameObject.GetComponent<PlayerHealth>();

        if (!other.isTrigger && (enemyHealth || indestructible || player))
        {
            bool validTarget = (player && isEnemyProjectile) || (enemyHealth && !isEnemyProjectile);

            if (validTarget || indestructible)
            {
                player?.TakeDamage(1, transform);

                if (particleOnHitPrefabVFX)
                    Instantiate(particleOnHitPrefabVFX, transform.position, transform.rotation);

                if (spawnAOEOnHit && aoePrefab != null)
                    Instantiate(aoePrefab, transform.position, Quaternion.identity);

                Destroy(gameObject);
            }
        }
    }
private void DetectFireDistance()
    {
        if (Vector3.Distance(transform.position, startPosition) > projectileRange)
        {
            if (spawnAOEOnHit && aoePrefab != null)
            {
                Instantiate(aoePrefab, transform.position, Quaternion.identity);
            }
            Destroy(gameObject);
        }
    }


    private void MoveProjectile()
    {
        transform.Translate(Vector3.right * Time.deltaTime * moveSpeed);
    }
}
