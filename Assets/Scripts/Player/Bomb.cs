using System.Collections;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField] private float delayBeforeCollider = 2f;
    [SerializeField] private GameObject explosionColliderObject;
    [SerializeField] private float destroyDelayAfterActivation = 0.5f;

    private void Start()
    {
        // Disable the whole GameObject that holds the collider
        if (explosionColliderObject != null)
            explosionColliderObject.SetActive(false);

        StartCoroutine(ExplosionRoutine());
    }

    private IEnumerator ExplosionRoutine()
    {
        yield return new WaitForSeconds(delayBeforeCollider);

        if (explosionColliderObject != null)
            explosionColliderObject.SetActive(true);

        yield return new WaitForSeconds(destroyDelayAfterActivation);

        Destroy(gameObject);
    }
}
