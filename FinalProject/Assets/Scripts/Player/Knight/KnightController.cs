using UnityEngine;
using UnityEngine.InputSystem.XR;

public class KnightController : MonoBehaviour
{

    public int maxHealth = 5;

    public float jumpHeight = 10f;
    public float moveSpeed = 5f;
    private float movement;

    private Animator animator;
    private Rigidbody2D rb;
    private bool isGrounded = true;

    public Transform attackPoint;
    public float attackRadius = 1f;
    public LayerMask attackLayer;
    //[SerializeField] private Transform enemy;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        //GameObject enemyOject = GameObject.FindWithTag("Enemy");
        //if (enemyOject != null)
        //{
        //    enemy = enemyOject.transform;
     
        //}
    }

    // Update is called once per frame
    void Update()
    {
        if (maxHealth < 0)
        {
            Die();
        }

        movement = Input.GetAxis("Horizontal"); // Lấy input di chuyển

        // Chạy
        if (movement != 0)
        {
            animator.SetBool("isRunning", true);
        }
        else
        {
            animator.SetBool("isRunning", false);
        }

        // Nhảy
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
            isGrounded = false;
            animator.SetBool("isJumping", true);
        }

        // Đánh
        if (Input.GetMouseButtonDown(0))
        {
            animator.SetTrigger("Attack");
        }
    }

    private void FixedUpdate()
    {
        transform.Translate(Vector2.right * moveSpeed * movement * Time.deltaTime);
        transform.localScale = new Vector3(Mathf.Sign(movement), 1, 1);
    }

    //public void Attack()
    //{
    //    Collider2D colliAttack = Physics2D.OverlapCircle(attackPoint.position, attackRadius, attackLayer);
    //    Debug.Log(colliAttack);
    //    if (colliAttack)
    //    {
    //        Debug.Log(colliAttack.gameObject.name + " takes dame");
    //    }
    //    BossController bossController = enemy.GetComponent<BossController>();
    //    bossController.TakeDamage(1);
    //}
    public void Attack()
    {
        Debug.Log("Player thực hiện đòn tấn công!");
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRadius, attackLayer);
        Debug.Log("Số lượng enemy bị đánh trúng: " + hitEnemies.Length);
        foreach (Collider2D enemy in hitEnemies)
        {
            // Kiểm tra từng loại enemy và gọi TakeDamage()
            if (enemy.TryGetComponent<BossController>(out BossController boss))
            {

                boss.TakeDamage(1);
                Debug.Log("Player đã trừ máu Boss! Máu còn lại: " + boss.GetCurrentHealth());
            }
            //else if (enemy.TryGetComponent<GoblinController>(out GoblinController goblin))
            //{
            //    goblin.TakeDamage(1);
            //}
            //else if (enemy.TryGetComponent<OrcController>(out OrcController orc))
            //{
            //    orc.TakeDamage(1);
            //}
        }
    }
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
        {
            return;
        }
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }

    private void Jump() 
    {
        //rb.linearVelocity = new Vector2(rb.linearVelocityX, 10f);
        rb.AddForce(new Vector2(0f, jumpHeight), ForceMode2D.Impulse);
        isGrounded = false;
    }

    // Xử lý va chạm
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            animator.SetBool("isJumping", false);
        }
    }

    public void TakeDame(int dame)
    {
        if (maxHealth < 0)
        {
            return;
        }
        maxHealth -= dame;
        Debug.Log("Player current health: " + maxHealth);  
    }

    void Die ()
    {
        Debug.Log("Player Die");
    }
}
