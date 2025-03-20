using UnityEngine;

public class GoblinController : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    public float maxHealth = 3;
    private float currentHealth;

    public float moveSpeed = 2f;
    public float chaseSpeed = 4f;

    public float attackDamage = 1f;

    private Animator animator;
    private Rigidbody2D rb;

    public float detectionRadius = 10f;
    public float attackRadius = 2f;
    public LayerMask playerLayer;
    private Transform playerTarget;

    public Transform attackPoint;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
    }

    void Update()
    {
        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        DetectPlayer();
    }

    void DetectPlayer()
    {
        Collider2D hitPlayer = Physics2D.OverlapCircle(transform.position, detectionRadius, playerLayer);

        if (hitPlayer != null)
        {
            playerTarget = hitPlayer.transform;
            FlipTowardsPlayer();

            float distanceToPlayer = Vector2.Distance(transform.position, playerTarget.position);
            //Debug.Log("Khoảng cách đến player: " + distanceToPlayer);

            if (distanceToPlayer > attackRadius)
            {
                // Nếu xa hơn attackRadius -> CHẠY tới player
                animator.SetBool("Run", true);
                animator.SetBool("Attack", false); 
                MoveTowardsPlayer();
            }
            else
            {
                // Nếu TRONG attackRadius -> Attack
                animator.SetBool("Run", false);
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("GoblinAttack"))
                {
                    animator.SetBool("Attack", true);
                    Debug.Log("Goblin bắt đầu Attack!");
                }
            }
        }
        else
        {
            // Không phát hiện player, trở về trạng thái Idle
            animator.SetBool("Run", false);
            animator.SetBool("Attack", false);
        }
    }

    void MoveTowardsPlayer()
    {
        float step = moveSpeed * Time.deltaTime;
        transform.position = Vector2.MoveTowards(transform.position, playerTarget.position, step);
    }



    void FlipTowardsPlayer()
    {
        if (playerTarget != null)
        {
            float direction = playerTarget.position.x - transform.position.x;

            if ((direction < 0 && transform.localScale.x > 0) || (direction > 0 && transform.localScale.x < 0))
            {
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            }
        }
    }

    public void AttackDealDame()
    {
        Collider2D colliAttack = Physics2D.OverlapCircle(attackPoint.position, attackRadius, playerLayer);

        if (colliAttack)
        {
            Debug.Log("Goblin attacks: " + colliAttack.gameObject.name);
            if (colliAttack.gameObject.name == "Knight_Player")
            {
                var player = colliAttack.GetComponent<KnightController>();
                player.TakeDame(attackDamage);
            }
            if (colliAttack.gameObject.name == "Dragon")
            {
                var player = colliAttack.GetComponent <DragronController>();
                player.TakeDame(attackDamage);
            }
        }
    }

    public void TakeDame(float damage)
    {
        if (currentHealth <= 0)
        {
            return;
        }

        currentHealth -= damage;
        animator.SetTrigger("Hurt");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        animator.SetTrigger("Die");
        rb.gravityScale = 1f;
        GetComponent<Collider2D>().isTrigger = false;
        rb.constraints = RigidbodyConstraints2D.None;
        this.enabled = false;
        Destroy(gameObject, 2f);  // Destroy after 2 seconds
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }

    public void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
