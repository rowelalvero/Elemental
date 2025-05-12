using System.Collections;
using UnityEngine;

public class PlayerController : Singleton<PlayerController>
{
    public bool FacingLeft { get { return facingLeft; } }

    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float dashSpeed = 4f;
    [SerializeField] private float runSpeed = 2f;
    [SerializeField] private TrailRenderer myTrailRenderer;
    [SerializeField] private Transform weaponCollider;
    [SerializeField] private Transform parryCollider;
    [SerializeField] private float parryDuration = 0.2f;
    [SerializeField] private float parryCooldown = 1f;
    [SerializeField] private GameObject slashAnimPrefab;
    [SerializeField] private Transform slashAnimSpawnPoint;

    private PlayerControls playerControls;
    private Vector2 movement;
    private Rigidbody2D rb;
    private Animator myAnimator;
    private SpriteRenderer mySpriteRender;
    private Knockback knockback;
    private float startingMoveSpeed;

    private bool facingLeft = false;
    private bool isDashing = false;
    private bool isRunning = false;
    private bool canParry = true;
 
    private GameObject slashAnim;
    readonly int SLIDE_HASH = Animator.StringToHash("isSliding");

    protected override void Awake()
    {
        base.Awake();

        playerControls = new PlayerControls();
        rb = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        mySpriteRender = GetComponent<SpriteRenderer>();
        knockback = GetComponent<Knockback>();
    }

    private void Start()
    {
        playerControls.Combat.Dash.performed += _ => Dash();
        playerControls.Combat.Run.performed += _ => StartRunning();
        playerControls.Combat.Run.canceled += _ => StopRunning();
        playerControls.Combat.Parry.performed += _ => Parry();

        startingMoveSpeed = moveSpeed;
        ActiveInventory.Instance.EquipStartingWeapon();
        slashAnimSpawnPoint = GameObject.Find("SlashSpawnPoint").transform;
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    private void Update()
    {
        PlayerInput();
    }

    private void FixedUpdate()
    {
        AdjustPlayerFacingDirection();
        Move();
    }

    public Transform GetWeaponCollider()
    {
        return weaponCollider;
    }

    private void PlayerInput()
    {
        movement = playerControls.Movement.Move.ReadValue<Vector2>();

        myAnimator.SetFloat("moveX", movement.x);
        myAnimator.SetFloat("moveY", movement.y);
    }

    private void Move()
    {
        if (knockback.GettingKnockedBack || PlayerHealth.Instance.isDead) { return; }

        float currentMoveSpeed = isRunning ? moveSpeed + runSpeed : moveSpeed;

        rb.MovePosition(rb.position + movement * (currentMoveSpeed * Time.fixedDeltaTime));
    }

    private void AdjustPlayerFacingDirection()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector3 playerScreenPoint = Camera.main.WorldToScreenPoint(transform.position);

        if (mousePos.x < playerScreenPoint.x)
        {
            mySpriteRender.flipX = true;
            facingLeft = true;
        }
        else
        {
            mySpriteRender.flipX = false;
            facingLeft = false;
        }
    }

    private void Dash()
    {
        if (!isDashing && Stamina.Instance.CurrentStamina > 0)
        {
            Stamina.Instance.UseStamina();
            isDashing = true;
            GetComponent<Animator>().SetBool(SLIDE_HASH, true);
            moveSpeed *= dashSpeed;
            StartCoroutine(EndDashRoutine());
        }
    }

    private IEnumerator EndDashRoutine()
    {
        float dashTime = .2f;
        float dashCD = .25f;
        yield return new WaitForSeconds(dashTime);
        moveSpeed = startingMoveSpeed;
        yield return new WaitForSeconds(dashCD);
        isDashing = false;
        GetComponent<Animator>().SetBool(SLIDE_HASH, false);
    }

    private void StartRunning()
    {
        isRunning = true;
    }

    private void StopRunning()
    {
        isRunning = false;
    }

    private void Parry()
    {
        if (!canParry || parryCollider == null) return;

        canParry = false;
        parryCollider.gameObject.SetActive(true); 

        Vector3 mousePos = Input.mousePosition;
        Vector3 playerScreenPoint = Camera.main.WorldToScreenPoint(transform.position);

        if (mousePos.x < playerScreenPoint.x)
        {
            parryCollider.rotation = Quaternion.Euler(0, -180, 0);
        }
        else
        {
            parryCollider.rotation = Quaternion.Euler(0, 0, 0);
        }

        ResetSlashAnim();
        slashAnim = Instantiate(slashAnimPrefab, slashAnimSpawnPoint.position, Quaternion.identity);
        slashAnim.transform.parent = this.transform.parent;

        if (slashAnim != null)
        {
            var slashSprite = slashAnim.GetComponent<SpriteRenderer>();
            if (slashSprite != null)
            {
                slashSprite.flipX = facingLeft;
            }
        }

        StartCoroutine(DisableParryColliderAfterDelay());
        StartCoroutine(ParryCooldown());
    }

    private IEnumerator DisableParryColliderAfterDelay()
    {
        yield return new WaitForSeconds(parryDuration);
        if (parryCollider != null)
            parryCollider.gameObject.SetActive(false);

        ResetSlashAnim();
    }

    private IEnumerator ParryCooldown()
    {
        yield return new WaitForSeconds(parryCooldown);
        canParry = true; 
    }

    private void ResetSlashAnim()
    {
        if (slashAnim != null)
        {
            Destroy(slashAnim);
            slashAnim = null;
        }
    }
}
