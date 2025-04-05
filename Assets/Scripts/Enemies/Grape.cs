using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grape : MonoBehaviour, IEnemy
{
    [SerializeField] private GameObject grapeProjectilePrefab;

    private Animator myAnimator;
    private SpriteRenderer spriteRenderer;

    private Action onAttackFinished;
    private bool isAttacking;

    private static readonly int ATTACK_HASH = Animator.StringToHash("Attack");

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

        myAnimator.SetTrigger(ATTACK_HASH);

        // Flip to face the player
        spriteRenderer.flipX = transform.position.x > PlayerController.Instance.transform.position.x;

        StartCoroutine(WaitForAttackAnimation());
    }

    private IEnumerator WaitForAttackAnimation()
    {
        // Wait while animation tagged "Attack" is playing
        while (myAnimator.GetCurrentAnimatorStateInfo(0).IsTag("Attack"))
        {
            yield return null;
        }

        isAttacking = false;
        onAttackFinished?.Invoke();
        onAttackFinished = null;
    }

    // Called through animation event
    public void SpawnProjectileAnimEvent()
    {
        Instantiate(grapeProjectilePrefab, transform.position, Quaternion.identity);
    }
}
