using System.Collections;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField] private float delayBeforeCollider = 2f;
    [SerializeField] private Collider2D explosionCollider;
    [SerializeField] private float destroyDelayAfterActivation = 0.5f;

    private void Start()
    {
        // Optional: Make sure collider is off at start
        if (explosionCollider != null)
            explosionCollider.enabled = false;

        // Start the delay timer
        StartCoroutine(ExplosionRoutine());
    }

    private IEnumerator ExplosionRoutine()
    {
        // Wait before enabling the collider
        yield return new WaitForSeconds(delayBeforeCollider);

        if (explosionCollider != null)
            explosionCollider.enabled = true;

        // Wait a little longer before cleanup
        yield return new WaitForSeconds(destroyDelayAfterActivation);

        Destroy(gameObject);
    }
}
