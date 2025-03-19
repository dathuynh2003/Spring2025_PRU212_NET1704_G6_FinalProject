using UnityEngine;

public class FlyingEye : MonoBehaviour
{
    public float maxHealth = 5;
    private float currentHealth;

    public float moveSpeed = 2f;
    public float attack2Speed = 5f;

    public float dameAttack1 = 1f;
    public float dameAttack2 = 3f;

    private Animator animator;
    private Rigidbody2D rb;

    public Transform pointA;
    public Transform pointB;
    private Transform targetPoint;
    private bool facingLeft = true;

    public float detectionRadius = 3f;
    public LayerMask playerLayer;
    private Transform playerTarget;

    private bool isAttacking = false;
    private bool hasUsedAttack2 = false;

    public float attack1Cooldown = 2f;
    private float attack1Timer;

    private Vector2 attackPosition;
    public Transform attackPoint;
    public float attackRadius = 1f;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;

        targetPoint = pointA;
        attack1Timer = attack1Cooldown;
    }

    void Update()
    {
        if (currentHealth <= 0)
        {
            Die();
        }

        if (!isAttacking)
        {
            MoveBetweenPoints();
            DetectPlayer();
        }
        else
        {
            if (!hasUsedAttack2)
            {
                // Attack_2 → bay nhanh tới Player
                Attack2Charge();
            }
            else
            {
                // Attack_1 → bay gần và cắn mỗi 2 giây
                HandleAttack1();
            }
        }

        SmoothRotate();
    }

    void MoveBetweenPoints()
    {
        transform.position = Vector2.MoveTowards(transform.position, targetPoint.position, moveSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, targetPoint.position) < 0.1f)
        {
            if (targetPoint == pointB)
            {
                targetPoint = pointA;
                facingLeft = true;
            }
            else
            {
                targetPoint = pointB;
                facingLeft = false;
            }
        }
    }

    void SmoothRotate()
    {
        float targetYRotation = facingLeft ? 180f : 0f;

        if (playerTarget != null)
        {
            facingLeft = (playerTarget.position.x < transform.position.x);
            targetYRotation = facingLeft ? 180f : 0f;
        }

        transform.eulerAngles = new Vector3(0, targetYRotation, 0);
    }

    void DetectPlayer()
    {
        Collider2D hitPlayer = Physics2D.OverlapCircle(transform.position, detectionRadius, playerLayer);
        if (hitPlayer != null)
        {
            playerTarget = hitPlayer.transform;
            attackPosition = playerTarget.position; // Lưu vị trí ngay lúc phát hiện
            isAttacking = true;
            animator.SetTrigger("Attack_2");
        }
    }

    void Attack2Charge()
    {
        transform.position = Vector2.MoveTowards(transform.position, attackPosition, attack2Speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, attackPosition) < 0.1f)
        {
            hasUsedAttack2 = true;
            attack1Timer = attack1Cooldown;
        }
    }

    void HandleAttack1()
    {
        attack1Timer -= Time.deltaTime;
        if (attack1Timer <= 0f)
        {
            // Lưu vị trí player để lần sau tấn công tiếp
            if (playerTarget != null)
            {
                attackPosition = playerTarget.position;
            }
            // Bay tới vị trí đã lưu
            transform.position = Vector2.MoveTowards(transform.position, attackPosition, moveSpeed * Time.deltaTime);

            // Khi tới nơi → thực hiện Attack_1
            if (Vector2.Distance(transform.position, attackPosition) < 1f)
            {
                animator.SetTrigger("Attack_1");
                attack1Timer = attack1Cooldown;
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }

    public void Attack1DealDame()
    {
        Collider2D colliAttack = Physics2D.OverlapCircle(attackPoint.position, attackRadius, playerLayer);

        if (colliAttack)
        {
            Debug.Log("Attack1: " + colliAttack.gameObject.name + " takes dame");
            if (colliAttack.gameObject.name == "Knight_Player")
            {
                var player = colliAttack.GetComponent<KnightController>();
                player.TakeDame(dameAttack1);
            }
        }
    }

    public void Attack2DealDame()
    {
        Collider2D colliAttack = Physics2D.OverlapCircle(attackPoint.position, attackRadius, playerLayer);

        if (colliAttack)
        {
            if (colliAttack.gameObject.name == "Knight_Player")
            {
                var player = colliAttack.GetComponent<KnightController>();
                player.TakeDame(dameAttack2);
            }
        }
    }

    public void TakeDame(float dame)
    {
        if (currentHealth < 0)
        {
            return;
        }
        currentHealth -= dame;
        animator.SetTrigger("Hurt");
        Debug.LogFormat("FlyingEye remaining health {0}/{1} HP", currentHealth, maxHealth);
    }

    public void Die()
    {
        animator.SetTrigger("Die");
        rb.gravityScale = 1f;
        GetComponent<Collider2D>().isTrigger = false;
        // Cho phép Rigidbody rơi tự do
        rb.constraints = RigidbodyConstraints2D.None;
        // (Tuỳ chọn) Vô hiệu hóa script di chuyển, tấn công...
        this.enabled = false;
        Destroy(gameObject, 2f);  // Xoá enemy sau 3 giây
    }
}
