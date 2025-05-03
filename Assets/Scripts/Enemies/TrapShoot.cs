using UnityEngine;

public class TrapShooter : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Collider2D triggerCollider;
    [SerializeField] private GameObject arrowPrefab;

    [Header("Arrow Settings")]
    [SerializeField] private float arrowSpeed = 5f;
    [SerializeField] private float spawnOffset = 0.5f;

    private void OnValidate()
    {
        if (triggerCollider == null)
        {
            triggerCollider = GetComponentInChildren<Collider2D>();
            Debug.Log($"[TrapShooter] Auto-assigned triggerCollider on {name}");
        }

        if (triggerCollider != null && !triggerCollider.isTrigger)
        {
            Debug.LogWarning($"[TrapShooter] {name}'s assigned collider is not set as a trigger.");
        }

        if (arrowPrefab == null)
        {
            Debug.LogWarning($"[TrapShooter] {name} is missing an arrow prefab.");
        }
    }

    private void OnEnable()
    {
        if (triggerCollider != null)
            triggerCollider.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"[TrapShooter] Trigger entered by: {other.name}");

        if (other.CompareTag("Player"))
        {
            Debug.Log("[TrapShooter] Player detected � firing arrow!");
            FireArrowLeft();
        }
    }

    private void FireArrowLeft()
    {
        if (arrowPrefab == null)
        {
            Debug.LogError("[TrapShooter] No arrow prefab assigned!");
            return;
        }

        Vector2 spawnPosition = (Vector2)transform.position + Vector2.left * spawnOffset;
        GameObject arrow = Instantiate(arrowPrefab, spawnPosition, Quaternion.identity);
        Debug.Log($"[TrapShooter] Arrow spawned at {spawnPosition}");

        arrow.transform.right = Vector2.left;

        if (arrow.TryGetComponent(out Rigidbody2D rb))
        {
            rb.linearVelocity = Vector2.left * arrowSpeed;
            Debug.Log($"[TrapShooter] Arrow given velocity {rb.linearVelocity}");
        }
        else
        {
            Debug.LogWarning("[TrapShooter] Spawned arrow has no Rigidbody2D.");
        }
    }
}
