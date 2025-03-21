using UnityEngine;
using UnityEngine.Audio;

public class SkeletonController : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    public float maxHealth = 3;
    private float currentHealth;

    public float moveSpeed = 2f;
    public float attack2Speed = 5f;
    public float chaseSpeed = 4f;

    public float dameAttack1 = 1f;
    public float dameAttack2 = 3f;

    private Animator animator;
    private Rigidbody2D rb;

    public bool facingLeft = true;

    public float detectionRadius = 3f;
    public LayerMask playerLayer;
    private Transform playerTarget;

    private bool isAttacking = false;
    private bool hasUsedAttack2 = false;
    public float attack1Cooldown = 2f;
    private float attack1Timer;

    public Transform pointA;
    public Transform pointB;
    private Transform targetPoint;

    private Vector2 attackPosition;
    public Transform attackPoint;
    public float attackRadius = 0.5f;

    public float distance = 1f;

    void Start()
    {
        targetPoint = pointA;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        attack1Timer = attack1Cooldown;
    }

    void Update()
    {
        if (currentHealth <= 0)
        {
            Die();
            return;
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
                Attack2Charge();
            }
            else
            {
                HandleAttack1();
            }
        }

        SmoothRotate();
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
            attackPosition = playerTarget.position;
            isAttacking = true;
            animator.SetTrigger("Attack_1");
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
            Debug.Log("Tesst");
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
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

    public void Attack1DealDame()
    {
        Collider2D colliAttack = Physics2D.OverlapCircle(attackPoint.position, attackRadius, playerLayer);

        if (colliAttack)
        {
            Debug.Log("Attack1: " + colliAttack.gameObject.name + " takes dame");
            if (colliAttack.gameObject.name.Contains("Knight"))
            {
                var player = colliAttack.GetComponent<KnightController>();
                player.TakeDame(dameAttack1);
            }
            else if (colliAttack.gameObject.name.Contains("Dragron"))
            {
                var player = colliAttack.GetComponent<DragronController>();
                player.TakeDame(dameAttack1);
            }
        }
    }

    public void Attack2DealDame()
    {
        Collider2D colliAttack = Physics2D.OverlapCircle(attackPoint.position, attackRadius, playerLayer);

        if (colliAttack)
        {
            if (colliAttack.gameObject.name.Contains("Knight"))
            {
                var player = colliAttack.GetComponent<KnightController>();
                player.TakeDame(dameAttack2);
            }
            else if (colliAttack.gameObject.name.Contains("Dragron"))
            {
                var player = colliAttack.GetComponent<DragronController>();
                player.TakeDame(dameAttack1);
            }
        }
    }

    public void TakeDame(float dame)
    {
        if (currentHealth <= 0)
        {
            return;
        }

        currentHealth -= dame;
        animator.SetTrigger("Hurt");
        Debug.LogFormat("Skeleton remaining health {0}/{1} HP", currentHealth, maxHealth);
    }

    public void Die()
    {
        animator.SetTrigger("Die");
        rb.gravityScale = 1f;
        GetComponent<Collider2D>().isTrigger = false;
        // Allow Rigidbody to fall freely
        rb.constraints = RigidbodyConstraints2D.None;
        // Disable this script
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
        this.enabled = false;
        GetComponent<Collider2D>().isTrigger = true;
        Destroy(gameObject, 2f);  // Destroy enemy after 2 seconds
    }

    public void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}