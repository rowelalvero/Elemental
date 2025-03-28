using System.Collections;
using UnityEngine;

public class EnemyChargeAttack : MonoBehaviour, IEnemy
{
    [Header("Detection & Attack Settings")]
    [SerializeField] private float detectionRadius = 5f;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float speed = 2f;
    [SerializeField] private float attackCooldown = 1.5f;

    [Header("Dash Attack Settings")]
    [SerializeField] private float windUpTime = 1f;
    [SerializeField] private float dashSpeed = 8f;
    [SerializeField] private float dashDuration = 0.5f;
    [SerializeField] private float dashDistance = 1.5f;

    [Header("References")]
    [SerializeField] private GameObject warningUIPrefab;
    [SerializeField] private GameObject damageCollider;
    [SerializeField] private Transform spawnAttack;

    [Header("Position Tracking")]
    [SerializeField] private bool returnToOriginalPositionAfterAttack = false;
    [SerializeField] private float returnSpeed = 3f;

    private bool isReturningFromAttack = false;
    private Transform player;
    private bool isAttacking = false;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Vector2 savedAttackDirection;
    private Vector2 savedOriginalPosition;
    private GameObject warningUIInstance;

    private const string PlayerTag = "Player";
    private readonly int AttackHash = Animator.StringToHash("isAttacking");

    private enum State
    {
        Roaming,
        Chasing,
        Attacking
    }

    private State currentState;
    private Coroutine roamingCoroutine;
    private Coroutine attackCoroutine;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        // Initialize with null checks
        player = GameObject.FindWithTag(PlayerTag)?.transform;

        if (warningUIPrefab != null)
        {
            warningUIInstance = Instantiate(warningUIPrefab, spawnAttack.position, Quaternion.identity);
            warningUIInstance.SetActive(false);
        }
        else
        {
            Debug.LogError("Warning UI Prefab is not assigned in the inspector!", this);
        }

        if (damageCollider != null)
        {
            damageCollider.SetActive(false);
        }
    }

    private void Start()
    {
        currentState = State.Roaming; // Set directly first
        TransitionToState(State.Roaming); // Then properly initialize
    }

    private void Update()
    {
        if (isReturningFromAttack) return;

        if (player == null)
        {
            TryFindPlayer();
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        switch (currentState)
        {
            case State.Roaming:
                if (distanceToPlayer <= detectionRadius)
                {
                    TransitionToState(State.Chasing);
                }
                break;

            case State.Chasing:
                if (distanceToPlayer > detectionRadius)
                {
                    TransitionToState(State.Roaming);
                }
                else if (!isAttacking)
                {
                    HandleAttackAI(distanceToPlayer);
                }
                break;

            case State.Attacking:
                // Attack behavior is handled in the coroutine
                break;
        }

        UpdateSpriteFlip();
    }

    private void TryFindPlayer()
    {
        player = GameObject.FindWithTag(PlayerTag)?.transform;
        if (player == null && currentState != State.Roaming)
        {
            TransitionToState(State.Roaming);
        }
    }

    private void TransitionToState(State newState)
    {

        Debug.Log($"Transitioning from {currentState} to {newState}");

        // Clean up previous state
        switch (currentState)
        {
            case State.Roaming:
                StopRoaming();
                break;
            case State.Chasing:
                rb.linearVelocity = Vector2.zero;
                break;
        }

        currentState = newState;

        // Initialize new state
        switch (newState)
        {
            case State.Roaming:
                StartRoaming();
                break;
            case State.Chasing:
                // Ensure we stop any roaming when chasing
                StopRoaming();
                break;
        }
    }

    private void UpdateSpriteFlip()
    {
        if (spriteRenderer == null || rb == null) return;

        // Only flip if we have significant horizontal movement
        if (Mathf.Abs(rb.linearVelocity.x) > 0.1f)
        {
            spriteRenderer.flipX = rb.linearVelocity.x > 0;
        }
    }

    public void Attack()
    {
        if (!isAttacking && attackCoroutine == null)
        {
            attackCoroutine = StartCoroutine(AttackRoutine());
        }
    }

    private void HandleAttackAI(float distance)
    {
        if (distance > attackRange)
        {
            MoveTowards(player.position);
        }
        else
        {
            Attack();
        }
    }

    private IEnumerator AttackRoutine()
    {

        yield return new WaitForSeconds(attackCooldown);

        isAttacking = true;
        savedOriginalPosition = rb.position;
        TransitionToState(State.Attacking);

        // Step 1: Prepare attack
        rb.linearVelocity = Vector2.zero;
        savedAttackDirection = (player.position - transform.position).normalized;

        // Update attack spawn position and rotation
        if (spawnAttack != null)
        {
            spawnAttack.position = transform.position + (Vector3)savedAttackDirection * dashDistance;
            float angle = Mathf.Atan2(savedAttackDirection.y, savedAttackDirection.x) * Mathf.Rad2Deg;
            spawnAttack.rotation = Quaternion.Euler(0, 0, angle);
        }

        // Show warning UI
        if (warningUIInstance != null)
        {
            warningUIInstance.transform.SetPositionAndRotation(
                spawnAttack.position,
                spawnAttack.rotation);
            warningUIInstance.SetActive(true);
        }

        if (animator != null)
        {
            animator.SetBool(AttackHash, true);
        }

        // Step 2: Wind-up delay
        yield return new WaitForSeconds(windUpTime);

        // Step 3: Execute dash attack
        if (warningUIInstance != null)
        {
            warningUIInstance.SetActive(false);
        }

        if (damageCollider != null)
        {
            damageCollider.SetActive(true);
        }

        float dashTime = 0f;
        Vector2 dashTarget = rb.position + savedAttackDirection * dashDistance;

        while (dashTime < dashDuration)
        {
            rb.linearVelocity = savedAttackDirection * dashSpeed;
            dashTime += Time.deltaTime;

            // Early exit if we reach the target
            if (Vector2.Distance(rb.position, dashTarget) <= 0.1f)
            {
                break;
            }

            yield return null;
        }

        // Clean up attack
        rb.linearVelocity = Vector2.zero;
        rb.position = dashTarget;

        if (damageCollider != null)
        {
            damageCollider.SetActive(false);
        }

        // After attack completes
        if (returnToOriginalPositionAfterAttack)
        {
            yield return StartCoroutine(ReturnToOriginalPosition());
        }

        // Reset attack state
        isAttacking = false;
        if (animator != null)
        {
            animator.SetBool(AttackHash, false);
        }

        attackCoroutine = null;
        TransitionToState(State.Chasing);
    }

    private IEnumerator ReturnToOriginalPosition()
    {
        isReturningFromAttack = true;

        while (Vector2.Distance(rb.position, savedOriginalPosition) > 0.1f)
        {
            Vector2 direction = (savedOriginalPosition - rb.position).normalized;
            rb.linearVelocity = direction * returnSpeed;
            yield return null;
        }

        rb.linearVelocity = Vector2.zero;
        rb.position = savedOriginalPosition;
        isReturningFromAttack = false;
    }

    private void MoveTowards(Vector2 target)
    {
        if (rb == null) return;

        Vector2 direction = (target - rb.position).normalized;
        rb.linearVelocity = direction * speed;
    }

    #region Roaming Behavior
    private IEnumerator RoamingRoutine()
    {
        Debug.Log("Roaming routine started");
        while (currentState == State.Roaming)
        {
            if (rb == null) yield break;

            Vector2 roamPosition = GetRoamingPosition();
            Debug.Log($"Moving to roam position: {roamPosition}");
            MoveTowards(roamPosition);
            yield return new WaitForSeconds(2f);
        }
        Debug.Log("Roaming routine ended");
    }

    private void StartRoaming()
    {
        StopRoaming();
        roamingCoroutine = StartCoroutine(RoamingRoutine());
    }

    private void StopRoaming()
    {
        if (roamingCoroutine != null)
        {
            StopCoroutine(roamingCoroutine);
            roamingCoroutine = null;
        }

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    private Vector2 GetRoamingPosition()
    {
        Vector2 randomDirection = new Vector2(
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f)).normalized;
        return rb.position + randomDirection * 2f;
    }
    #endregion

    public Transform GetAttackCollider()
    {
        return damageCollider != null ? damageCollider.transform : null;
    }

    private void OnDestroy()
    {
        if (warningUIInstance != null)
        {
            Destroy(warningUIInstance);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}