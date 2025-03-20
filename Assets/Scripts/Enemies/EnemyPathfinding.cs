using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPathfinding : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;

    private Rigidbody2D rb;
    private Vector2 moveDir;
    private Knockback knockback;

    // New Additions for Roaming
    private enum State { Roaming, Chasing, Idle }
    private State state;
    private Vector2 lastRoamDirection;

    private void Awake()
    {
        knockback = GetComponent<Knockback>();
        rb = GetComponent<Rigidbody2D>();

        // Start roaming behavior
        state = State.Roaming;
        StartCoroutine(RoamingRoutine());
    }

    private void FixedUpdate()
    {
        if (knockback.GettingKnockedBack) { return; }

        rb.MovePosition(rb.position + moveDir * (moveSpeed * Time.fixedDeltaTime));
    }

    public void MoveTo(Vector2 targetPosition)
    {
        moveDir = (targetPosition - rb.position).normalized;
    }

    // Roaming Routine Integration
    private IEnumerator RoamingRoutine()
    {
        while (state == State.Roaming)
        {
            Vector2 roamPosition = GetRoamingPosition();
            MoveTo(roamPosition);
            yield return new WaitForSeconds(2f);
        }
    }

    private Vector2 GetRoamingPosition()
    {
        lastRoamDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        return (Vector2)transform.position + lastRoamDirection * 2f;
    }
}
