using UnityEngine;

public class EnemyController : EntityController
{
    [Header("Follow")]
    [SerializeField] float attackDistance = 1.2f;
    [SerializeField] float turnDelay = 1f;

    [Header("Attack")]
    [SerializeField] float attackCooldown = 1.0f;

    Transform player;

    float lastTurnTime;
    float lastAttackTime;
    float currentDirection = 0;

    void Start()
    {
        player = GameObject.FindWithTag("Player")?.transform;
    }

    protected override void Update()
    {
        if (isDead)
            return;

        if (isHurt)
        {
            Debug.Log("Enemy is hurt, stopping movement.");
            desiredMove = Vector2.zero;
            base.Update();
            return;
        }

        if (player == null)
        {
            desiredMove = Vector2.zero;
            base.Update();
            return;
        }

        float distance = Vector2.Distance(transform.position, player.position);
        float targetDirection = Mathf.Sign(player.position.x - transform.position.x);

        if (targetDirection != currentDirection)
        {
            if (Time.time - lastTurnTime >= turnDelay)
            {
                currentDirection = targetDirection;
                lastTurnTime = Time.time;
            }
        }

        if (distance > attackDistance)
        {
            desiredMove = new Vector2(currentDirection, 0);
        }
        else
        {
            desiredMove = Vector2.zero;

            if (!isHurt && !isDead && Time.time - lastAttackTime >= attackCooldown)
            {
                mustPunch = true;
                lastAttackTime = Time.time;
            }
        }

        base.Update();
    }

    protected override void DieAnimation()
    {
        base.DieAnimation();
        EnemyInstantiator.Instance.EnemyDied();
    }

    public override void Die()
    {
        GameController.instance.IncreaseScore(5);
        enabled = false;
        Destroy(gameObject);
    }
}
