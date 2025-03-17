using UnityEngine;
using System.Collections;

public class BossController : MonoBehaviour
{
    [SerializeField] private float speed = 3f;
    [SerializeField] private Transform player;
    [SerializeField] private float attackRange = 1.5f;
    private Animator anim;
    private Rigidbody2D body;
    private bool isAttacking = false;
    private bool isMovingToPlayer = false;
    public int maxHealth = 30;
    private int currentHealth;
    private bool isGrounded = false;
    public int attackDamage = 1;
    public Transform attackPoint;
    public float attackRadius = 1f;
    public LayerMask playerLayer;
    public float attackDelay = 3f; // Để dễ test hơn, bạn có thể tăng lên 10f sau
    private bool isDead = false;

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }

        currentHealth = maxHealth;

        if (attackPoint == null)
        {
            Debug.LogError("LỖI: attackPoint chưa được gán trong Inspector!");
        }

        // Bắt đầu chu trình hành động
        StartCoroutine(BossActionLoop());
    }

    void Update()
    {
        if (player == null || isDead) return;

        // Kiểm tra nếu Player cao hơn Boss => Boss nhảy lên
        if (player.position.y > transform.position.y + 1f && isGrounded)
        {
            Jump();
        }

        // Xử lý hướng quay của boss theo hướng player
        Vector2 direction = (player.position - transform.position).normalized;
        if (direction.x > 0)
            transform.localScale = new Vector3(3, 3, 3);
        else
            transform.localScale = new Vector3(-3, 3, 3);
    }

    private IEnumerator BossActionLoop()
    {
        while (!isDead)
        {
            yield return new WaitForSeconds(attackDelay);

            // Move towards player
            isMovingToPlayer = true;
            anim.SetBool("boss_run", true);

            while (Vector2.Distance(attackPoint.position, player.position) > attackRadius * 0.7f)
            {
                MoveTowardsPlayer();
                yield return null; // Đợi tới frame tiếp theo
            }

            // Khi đã đến gần player thì ngừng di chuyển
            isMovingToPlayer = false;
            body.linearVelocity = Vector2.zero;
            anim.SetBool("boss_run", false);

            // Attack player
            Attack();

            // Đợi thời gian delay trước khi lặp lại
            yield return new WaitForSeconds(attackDelay);
        }
    }

    void MoveTowardsPlayer()
    {
        if (!isMovingToPlayer) return;

        Vector2 direction = (player.position - transform.position).normalized;
        body.linearVelocity = new Vector2(direction.x * speed, body.linearVelocity.y);
    }

    void Attack()
    {

        //anim.SetBool("boss_run", false);
        anim.SetTrigger("boss_attack2");

        body.linearVelocity = Vector2.zero;
        Debug.Log("Boss đang tấn công!");

        DealDamageToPlayer();

        // Sau animation đánh thì cho boss nghỉ xíu rồi reset
        //Invoke(nameof(ResetAttack), 1f); // Thời gian chờ sau khi đánh (bạn có thể chỉnh)
    }

    void Jump()
    {
        if (isGrounded)
        {
            anim.SetTrigger("boss_jump");
            body.linearVelocity = new Vector2(body.linearVelocity.x, speed);
            isAttacking = false;
        }
    }

    void ResetAttack()
    {
        isAttacking = false;
    }

    void Die()
    {
        isDead = true;
        anim.SetTrigger("boss_die");
        body.linearVelocity = Vector2.zero;
        Debug.Log("Boss die");
        GetComponent<Collider2D>().enabled = false;
        StopAllCoroutines();
        Destroy(gameObject, 2f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    private void DealDamageToPlayer()
    {
        Collider2D hitPlayer = Physics2D.OverlapCircle(attackPoint.position, attackRadius, playerLayer);

        if (hitPlayer != null)
        {
            anim.SetBool("boss_run", false);

            KnightController knightController = hitPlayer.GetComponent<KnightController>();
            DragronController dragon = hitPlayer.GetComponent<DragronController>();

            if (knightController != null)
            {
                Debug.Log("Player nhận sát thương!");
                knightController.TakeDame(attackDamage);
            }
            else if (dragon != null)
            {
                Debug.Log("Dragon nhận sát thương!");
                // dragon.TakeDame(attackDamage);
            }
        }
        else
        {
            Debug.Log("Không tìm thấy Player trong vùng tấn công!");
        }
    }

    public void TakeDamage(int damage)
    {
        if (currentHealth <= 0) return;

        currentHealth -= damage;
        Debug.Log("Current health boss: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }
}
