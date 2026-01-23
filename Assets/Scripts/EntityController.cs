using UnityEngine;

public class EntityController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Header("Movement")]
    [SerializeField] protected float walkSpeed = 3f;
    protected float speedMultiplier = 1;
    [SerializeField] float jumpSpeed = 10f;

    [Header("Combat")]
    [SerializeField] int maxHealth = 5;
    [SerializeField] int attackDamage = 1;
    [SerializeField] float attackRange = 0.8f;
    [SerializeField] LayerMask damageLayers;
    [SerializeField] Transform attackPoint;
    Vector3 attackPointLocalPos;
    protected int currentHealth;

    [Header("Knockback")]
    [SerializeField] float knockbackForce = 6f;
    [SerializeField] float knockbackDuration = 0.15f;

    bool isKnockback = false;
    float knockbackEndTime;

    [Header("References")]
    protected Rigidbody2D rb2d;
    protected Animator ani;
    SpriteRenderer spriteRenderer;
    AudioSource audioSource;

    [Header("Audio")]
    [SerializeField] AudioClip punchSound;
    [SerializeField] AudioClip hurtSound;
    [SerializeField] AudioClip deathSound;

    [Header("State")]
    protected Vector2 desiredMove = Vector2.zero;
    protected bool mustJump = false;
    protected bool mustPunch = false;
    protected bool isAttacking = false;
    protected bool isHurt = false;
    protected bool isDead = false;

    protected virtual void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        ani = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentHealth = maxHealth;

        if (attackPoint != null)
            attackPointLocalPos = attackPoint.localPosition;
    }

    protected virtual void Update()
    {
        if (isDead) return;

        if (isKnockback)
        {
            if (Time.time >= knockbackEndTime)
            {
                isKnockback = false;
            }
            else
            {
                return;
            }
        }

        if (isHurt)
        {
            rb2d.linearVelocityX = 0;
            ani.SetBool("IsWalking", false);
            return;
        }

        rb2d.linearVelocityX = desiredMove.x * walkSpeed * speedMultiplier;
        ani.SetBool("IsWalking", desiredMove.x != 0);

        if (desiredMove.x < 0)
        {
            spriteRenderer.flipX = true;
            UpdateAttackPoint(true);
        }
        else if (desiredMove.x > 0)
        {
            spriteRenderer.flipX = false;
            UpdateAttackPoint(false);
        }


        if (mustPunch)
        {
            mustPunch = false;
            isAttacking = true;
            ani.SetTrigger("PerformPunch");
        }

        if (mustJump)
        {
            mustJump = false;

            if (Mathf.Abs(rb2d.linearVelocityY) < 0.01f)
            {
                rb2d.linearVelocityY = jumpSpeed;
                ani.SetTrigger("Jump");
            }
        }

        HandleJumpAnimation();
    }

    void UpdateAttackPoint(bool flipped)
    {
        if (attackPoint == null) return;

        attackPoint.localPosition = new Vector3(
            flipped ? -attackPointLocalPos.x : attackPointLocalPos.x,
            attackPointLocalPos.y,
            attackPointLocalPos.z
        );
    }

    void HandleJumpAnimation()
    {
        if (ani.GetCurrentAnimatorStateInfo(0).IsName("Jump"))
        {
            if (Mathf.Abs(rb2d.linearVelocityY) < 0.01f)
            {
                ani.Play(desiredMove.x != 0 ? "Walk" : "Idle");
            }
        }
    }

    // ===================== COMBAT =====================
    public void DealDamage()
    {
        audioSource.PlayOneShot(punchSound);

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            attackPoint.position,
            attackRange,
            damageLayers
        );

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<EntityController>(out var entity)
                && entity != this)
            {
                entity.TakeDamage(attackDamage);
            }
        }
    }

    public virtual void TakeDamage(int damage)
    {
        if (isDead || isHurt) return;

        isHurt = true;

        desiredMove = Vector2.zero;
        mustPunch = false;
        mustJump = false;

        ApplyKnockback();
        ani.SetTrigger("Hurt");
        audioSource.PlayOneShot(hurtSound);

        CurrentHealth -= damage;

        HealthUI ui = GetComponentInChildren<HealthUI>();
        if (ui != null)
        {
            ui.Show();
        }
    }

    void ApplyKnockback()
    {
        float direction = spriteRenderer.flipX ? 1f : -1f;

        rb2d.linearVelocity = Vector2.zero;
        rb2d.AddForce(new Vector2(direction * knockbackForce, knockbackForce * 0.4f),
                      ForceMode2D.Impulse);

        isKnockback = true;
        knockbackEndTime = Time.time + knockbackDuration;
    }

    public void EndHurt()
    {
        isHurt = false;
    }

    public void EndAttack()
    {
        isAttacking = false;
    }

    protected virtual void DieAnimation()
    {
        if (isDead) return;

        isDead = true;
        mustPunch = false;
        ani.SetTrigger("Die");
        rb2d.linearVelocity = Vector2.zero;
        audioSource.PlayOneShot(deathSound);
    }

    public virtual void Die()
    {
        isDead = true;
        mustPunch = false;
    }

    public int CurrentHealth
    {
        get => currentHealth;
        set
        {
            currentHealth = Mathf.Clamp(value, 0, maxHealth);

            if (currentHealth <= 0)
            {
                DieAnimation();
            }
        }
    }

    public int MaxHealth => maxHealth;

    private void OnDrawGizmos()
    {
        if (attackPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
