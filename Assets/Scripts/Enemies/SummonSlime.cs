using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonSlime : MonoBehaviour, IEnemy
{
    [SerializeField] private GameObject grapeProjectilePrefab;

    private Animator myAnimator;
    private SpriteRenderer spriteRenderer;

    private Action onAttackFinished;
    private bool isAttacking;

    readonly int ATTACK_TRIGGER_HASH = Animator.StringToHash("isSummoning");

    public bool IsAttacking => isAttacking;

    private void Awake()
    {
        myAnimator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Attack(Action onComplete)
    {
        if (isAttacking) return;

        onAttackFinished = onComplete;
        isAttacking = true;

        myAnimator.SetTrigger(ATTACK_TRIGGER_HASH);

        // Flip sprite to face the player
        spriteRenderer.flipX = transform.position.x > PlayerController.Instance.transform.position.x;

        StartCoroutine(WaitForAttackAnimation());
    }

    private IEnumerator WaitForAttackAnimation()
    {
        // Wait until the summoning animation state ends
        while (myAnimator.GetCurrentAnimatorStateInfo(0).IsTag("Summoning"))
        {
            yield return null;
        }

        // Once animation ends, notify the system
        isAttacking = false;
        onAttackFinished?.Invoke();
        onAttackFinished = null;
    }

    // Called via animation event
    public void SpawnProjectileAnimEvent()
    {
        Instantiate(grapeProjectilePrefab, transform.position, Quaternion.identity);
    }
}
