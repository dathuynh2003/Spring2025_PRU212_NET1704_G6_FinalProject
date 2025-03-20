using UnityEngine;

public class DragronController : MonoBehaviour
{
    private Animator animator;
    private Rigidbody2D rb;
    public float maxHealth = 5;
    public float jumpHeight = 15f;
    public float currentHealth;
    private float moveSpeed = 5f;
    private float move;
    private bool isGrounded = true;

    public float baseDame = 2;
    private float currentDame;

    public Transform attackPoint;
    public float attackRadius = 1f;
    public LayerMask attackLayer;
    public GameObject fireBall;

    public float fireRate = 0.2f;
    private float nextFireTime = 0f;

    private bool isDead = false;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip runSound;
    private bool hasPlayedDizzySound = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        animator.ResetTrigger("Dizzy");
        animator.SetBool("isDizzy", false);
        currentHealth = maxHealth;
        currentDame = baseDame;
    }

    // Update is called once per frame
    void Update()
    {
        if (maxHealth < 0)
        {
            Die();
        }

        move = Input.GetAxis("Horizontal"); // Lấy input di chuyển

        // Chạy
        if (move != 0)
        {
            animator.SetBool("isRunning", true);
            transform.Translate(Vector2.right * moveSpeed * move * Time.deltaTime);
            transform.localScale = new Vector3(Mathf.Sign(move), 1, 1); // Đổi hướng nhân vật
            // Kiểm tra nếu runSound chưa phát thì mới phát
            if (!audioSource.isPlaying || audioSource.clip != runSound)
            {
                audioSource.clip = runSound;
                audioSource.loop = true; // Để nó chạy lặp lại mượt mà
                audioSource.Play();
            }
        }
        else
        {
            animator.SetBool("isRunning", false);
            StopRunSound();
        }


        // Nhảy
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
            animator.SetBool("isJumping", true);
        }

        // Đánh
        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            animator.SetTrigger("Attack");
            nextFireTime = Time.time + fireRate;
        }

        // Đánh chuột phải
        if (Input.GetMouseButtonDown(1))
        {
            animator.SetTrigger("Strike");
        }

        // Hàm test = nhấn T
        //if (Input.GetKeyDown(KeyCode.T))
        //{
        //    Die();
        //}

        // Hàm test = nhấn Y
        if (Input.GetKeyDown(KeyCode.Y))
        {
            animator.SetTrigger("Hurt");
        }

        // Hàm test = nhấn U
        if (Input.GetKeyDown(KeyCode.U))
        {
            TriggerDizzyEffect();
        }
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

    public void Attack()
    {
        GameObject fireballInstance = Instantiate(fireBall, attackPoint.position, attackPoint.rotation);
        Fireball fireballScript = fireballInstance.GetComponent<Fireball>();

        // Xác định hướng của fireball dựa trên hướng của nhân vật
        if (transform.localScale.x < 0)
        {
            fireballScript.SetDirection(-1); // Bắn sang trái
        }
        else
        {
            fireballScript.SetDirection(1); // Bắn sang phải
        }

        fireballScript.SetDamage((int)currentDame);

        //Collider2D colliAttack = Physics2D.OverlapCircle(attackPoint.position, attackRadius, attackLayer);
        //Debug.Log(colliAttack);
        //if (colliAttack)
        //{
        //    Debug.Log(colliAttack.gameObject.name + " takes dame");
        //}
    }

    public void Attack2()
    {
        Collider2D colliAttack = Physics2D.OverlapCircle(attackPoint.position, attackRadius, attackLayer);
        if (colliAttack)
        {
            Debug.Log(colliAttack.gameObject.name + " takes dame");
            if (colliAttack.gameObject.name == "Boss_Enemy")
            {
                var boss = colliAttack.GetComponent<BossController>();
                boss.TakeDamage(currentDame);
            }
            else if (colliAttack.gameObject.name == "FlyingEye")
            {
                var enemy = colliAttack.GetComponent<FlyingEye>();
                enemy.TakeDame(currentDame);
            } else if (colliAttack.gameObject.name == "Mushroom_Enemy")
            {
                var enemy = colliAttack.GetComponent<MushroomEnemy>();
                enemy.TakeDamage((int)currentDame);
            }
            else if (colliAttack.gameObject.name == "Goblin_Enemy")
            {
                var enemy = colliAttack.GetComponent<GoblinController>();
                enemy.TakeDame(currentDame);
            }
            else if (colliAttack.gameObject.name == "Skeleton_Enemy")
            {
                var enemy = colliAttack.GetComponent<SkeletonController>();
                enemy.TakeDame(currentDame);
            }
            
        }
    }

    private void Jump()
    {
        PlaySound(jumpSound);
        //rb.linearVelocity = new Vector2(rb.linearVelocityX, 10f);
        rb.AddForce(new Vector2(0f, jumpHeight), ForceMode2D.Impulse);
        isGrounded = false;
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
        {
            return;
        }
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }

    public void TakeDame(float dame)
    {

        if (maxHealth <= 0)
        {
            Die();
            return;
        }

        currentHealth -= dame;

        animator.SetTrigger("Hurt");
    }

    void Die()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Die")) return; // Nếu đã chết thì không làm gì nữa

        isDead = true;
        animator.SetTrigger("Die");

        // Ngừng di chuyển và tấn công
        moveSpeed = 0;
        this.enabled = false; // Tắt script để không nhận input nữa

        Debug.Log("Player Die");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Trap")) // Nếu va chạm với bẫy (chưa làm bẫy)
        {
            TriggerDizzyEffect();
        }
    }

    void TriggerDizzyEffect()
    {
        if (animator.GetBool("isDizzy")) return;

        animator.SetTrigger("Dizzy");
        animator.SetBool("isDizzy", true);
        // Tạm thời vô hiệu hóa di chuyển
        moveSpeed = 0;

        // Sau 3 giây, rồng sẽ hết choáng và có thể di chuyển lại
        Invoke("RecoverFromDizzy", 3f);
    }

    void RecoverFromDizzy()
    {
        animator.SetBool("isDizzy", false);
        moveSpeed = 5f; // Trả lại tốc độ di chuyển
        hasPlayedDizzySound = false;
    }

    public void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    public void PlayDizzySound(AudioClip clip)
    {
        if (clip != null && audioSource != null && !hasPlayedDizzySound)
        {
            audioSource.PlayOneShot(clip);
            hasPlayedDizzySound = true; // Đánh dấu đã phát âm thanh
        }
    }

    private void StopRunSound()
    {
        if (audioSource.isPlaying && audioSource.clip == runSound)
        {
            audioSource.Stop();
            audioSource.clip = null; // Xóa clip để tránh xung đột
        }
    }
}
