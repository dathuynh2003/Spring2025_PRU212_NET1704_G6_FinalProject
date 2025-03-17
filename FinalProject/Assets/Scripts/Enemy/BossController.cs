using UnityEngine;

public class BossController : MonoBehaviour
{
    [SerializeField] private float speed = 3f;
    [SerializeField] private Transform player;
    [SerializeField] private float attackRange = 1.5f;
    private Animator anim;
    private Rigidbody2D body;
    private bool isAttacking = false;
    public int maxHealth = 30;
    private int currentHealth;
    private bool isGrounded = false;
    public int attackDamage = 1;
    public Transform attackPoint; //điểm trung tâm của vùng tấn công
    public float attackRadius = 1f;
    public LayerMask playerLayer; //Lớp của Player
    public float attackDelay = 10f;

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
    }

    void Update()
    {
        //if (maxHealth <= 0)
        //{
        //    anim.SetTrigger("boss_die");
        //    return;
        //}
        if (player == null) return;

        
        //float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        float stopDistance = attackRadius * 0.7f; // Dừng lại khi Player gần trúng tâm
                                                  //if (distanceToPlayer > stopDistance)
                                                  //{      
                                                  //    MoveTowardsPlayer();           
                                                  //}
                                                  //else
                                                  //{

        //    if (!isAttacking) // Chỉ tấn công nếu chưa đánh
        //    {
        //        Attack();
        //    }

        //}
        // Kiểm tra nếu Player cao hơn Boss
        if (player.position.y > transform.position.y && isGrounded) // 1f là khoảng cách tối thiểu để nhảy
        {
            Jump();
        }
        float distanceToAttackPoint = Vector2.Distance(attackPoint.position, player.position);
        if (distanceToAttackPoint > stopDistance)
        {
            MoveTowardsPlayer();
        }
        else
        {
            if (!isAttacking)
            {
                Attack();
            }
        }
    }

    void MoveTowardsPlayer()
    {
        if (isAttacking) return;

        anim.SetBool("boss_run", true);
        ResetAttack();

        Vector2 direction = (player.position - transform.position).normalized;
        float distanceToAttackPoint = Vector2.Distance(attackPoint.position, player.position);
        float stopDistance = attackRadius * 0.3f;

        if (distanceToAttackPoint > stopDistance)
        {
            body.linearVelocity = new Vector2(direction.x * speed, body.linearVelocity.y);
        }
        else
        {
            body.linearVelocity = Vector2.zero;
        }

        if (direction.x > 0)
            transform.localScale = new Vector3(5, 5, 5);
        else
            transform.localScale = new Vector3(-5, 5, 5);
    }

    void Attack()
    {
        if (!isAttacking)
        {
            isAttacking = true;
            anim.SetBool("boss_run", false); // Ngừng chạy khi tấn công
            anim.SetTrigger("boss_attack2");
            Debug.Log("Boss đang tấn công!");
            DealDamageToPlayer();                           
            body.linearVelocity = Vector2.zero; // Dừng lại khi đánh
            Invoke(nameof(ResetAttack), attackDelay); // Thời gian delay giữa các đòn đánh
        }
 
    }

    void Jump()
    {
        if (isGrounded)
        {
            anim.SetTrigger("boss_jump");
            anim.SetTrigger("boss_fall");
            body.linearVelocity = new Vector2(body.linearVelocity.x, speed);
            isAttacking = false;
        }

    }
    void Die()
    {
        anim.SetTrigger("boss_die");
        body.linearVelocity = Vector2.zero;
        Debug.Log("Boss die");
        //GetComponent<Collider2D>().enabled = false;
        //Destroy(gameObject, 2f);
    }
    void ResetAttack()
    {
        isAttacking = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
       
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    //trừ máu player khi bị boss tấn công
    //private void DealDamageToPlayer()
    //{
    //    Collider2D hitPlayer = Physics2D.OverlapCircle(attackPoint.position, attackRadius, playerLayer);

    //    if (player != null)
    //    {
    //        KnightController knightController = player.GetComponent<KnightController>();
    //        knightController.TakeDame(attackDamage);
    //    }
    //}
    private void DealDamageToPlayer()
    {
        Collider2D hitPlayer = Physics2D.OverlapCircle(attackPoint.position, attackRadius, playerLayer);
        Debug.Log("LayerMask của Player là: " + hitPlayer.gameObject.layer);

        if (hitPlayer != null)
        {
           // float distanceToCenter = Vector2.Distance(hitPlayer.transform.position, attackPoint.position);

     //       if (distanceToCenter < attackRadius * 0.5f) // Chỉ trúng nếu gần tâm
       //     {
                KnightController knightController = hitPlayer.GetComponent<KnightController>();
                DragronController dragon = hitPlayer.GetComponent<DragronController>();
            if (knightController != null)
                {
                    Debug.Log("Player nhận sát thương!");
                    knightController.TakeDame(attackDamage);
                }
            //    }
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

    //trừ máu của boss khi bị tấn công
    public void TakeDamage(int damage)
    {
        if (currentHealth <= 0) return;

        currentHealth -= damage;
       // anim.SetTrigger("boss_take_hit");
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
}
