using UnityEngine;
using System.Collections;

public class BossController : MonoBehaviour
{
    [SerializeField] private float speed = 3f;
    [SerializeField] private Transform player;
    private Animator anim;
    private Rigidbody2D body;
    private bool isMovingToPlayer = false;
    public float maxHealth = 30;
    private float currentHealth;
    private bool isGrounded = false;
    public int attackDamage = 1;
    public Transform attackPoint;
    public float attackRadius = 1f;
    public LayerMask playerLayer;
    public float attackDelay = 10f; // Để dễ test hơn, bạn có thể tăng lên 10f sau
    private bool isDead = false;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip bossStart;
    [SerializeField] private AudioClip attack1Sound;
    [SerializeField] private AudioClip attack2Sound;
    [SerializeField] private AudioClip runSound;
    [SerializeField] private AudioClip idleSound;
    [SerializeField] private AudioClip dieSound;
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip angrySound;
    [SerializeField] private AudioClip sumonSound;
    [SerializeField] private Transform[] spawnPoints; // Các điểm spawn enemy
    [SerializeField] private GameObject[] enemyPrefabs; // Các loại quái khác nhau
    [SerializeField] private GameObject summonEffect; // Hiệu ứng triệu hồi
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

        PlaySound(bossStart);

        // Triệu hồi quái vật khi trận đấu bắt đầu
        SummonEnemies();

        StartCoroutine(BossActionLoop());
    }

    void Update()
    {
        if (player == null || isDead) return;

        // Kiểm tra nếu Player cao hơn Boss => Boss nhảy lên
        //if (player.position.y > transform.position.y + 1f && isGrounded)
        //{
        //    Jump();
        //}

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
           

            // Nếu máu Boss dưới 50%, giảm thời gian delay tấn công
            float currentAttackDelay = (currentHealth < maxHealth / 2) ? attackDelay / 2 : attackDelay;
            yield return new WaitForSeconds(0.8f);
            // Move towards player
            isMovingToPlayer = true;
            anim.SetBool("boss_run", true);
            if (player == null)
            {
                break;
            }
            while (Vector2.Distance(attackPoint.position, player.position) > attackRadius)
            {

                MoveTowardsPlayer();
                yield return null; // Đợi tới frame tiếp theo
            }
           
            // Khi đã đến gần player thì ngừng di chuyển
            isMovingToPlayer = false;
            body.linearVelocity = Vector2.zero;
            anim.SetBool("boss_run", false);
            StopRunSound();
            // Attack player
           

            // Kiểm tra nếu Player cao hơn Boss 1 khoảng nhất định và trong tầm đánh
            if (player.position.y > transform.position.y && Vector2.Distance(attackPoint.position, player.position) <= attackRadius)
            {
                Attack1();
            }
            else Attack();

            // Đợi thời gian delay trước khi lặp lại
            yield return new WaitForSeconds(currentAttackDelay);
        }
    }

    void MoveTowardsPlayer()
    {
        if (!isMovingToPlayer) return;
        Vector2 direction = (player.position - transform.position).normalized;
        body.linearVelocity = new Vector2(direction.x * speed, body.linearVelocity.y);
        // Phát âm thanh chạy nếu nó chưa phát
        if (!audioSource.isPlaying || audioSource.clip != runSound)
        {
            PlayLoopSound(runSound);
        }
    }
    void Attack1()
    {

        //anim.SetBool("boss_run", false);
        anim.SetTrigger("boss_attack1");
        PlaySound(attack1Sound);
        body.linearVelocity = Vector2.zero;
        Debug.Log("Boss đang tấn công!");

        //DealDamageToPlayer();

        // Sau animation đánh thì cho boss nghỉ xíu rồi reset
        //Invoke(nameof(ResetAttack), 1f); // Thời gian chờ sau khi đánh (bạn có thể chỉnh)
    }
    void Attack()
    {

        //anim.SetBool("boss_run", false);
        anim.SetTrigger("boss_attack2");
        PlaySound(attack2Sound);
        body.linearVelocity = Vector2.zero;
        Debug.Log("Boss đang tấn công!");

       // DealDamageToPlayer();

        // Sau animation đánh thì cho boss nghỉ xíu rồi reset
        //Invoke(nameof(ResetAttack), 1f); // Thời gian chờ sau khi đánh (bạn có thể chỉnh)
    }

    void Jump()
    {
        if (isGrounded)
        {
            anim.SetTrigger("boss_jump");
            
            body.linearVelocity = new Vector2(body.linearVelocity.x, speed);
        }
    }

    void Die()
    {
        isDead = true;
        anim.SetTrigger("boss_die");
        PlaySound(dieSound);
        body.linearVelocity = Vector2.zero;
        Debug.Log("Boss die");
        // GetComponent<Collider2D>().enabled = false;

        StopAllCoroutines();
        Destroy(gameObject, 2f);

        player.gameObject.GetComponent<KnightController>().Victory();
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
                //Debug.Log("Player nhận sát thương!");
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

    public void TakeDamage(float damage)
    {
        if (currentHealth <= 0) return;

        currentHealth -= damage;
        Debug.Log("Current health boss: " + currentHealth);
        if (currentHealth <= maxHealth / 2 )
        {
           // hasEnraged = true; // Đánh dấu đã tức giận
            PlaySound(angrySound); // Phát âm thanh tức giận
            Debug.Log("Boss trở nên tức giận!");
        }
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

    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    private void PlayLoopSound(AudioClip clip)
    {
        if (clip == null || audioSource == null) return;

        // Nếu đang phát đúng clip rồi thì không phát lại
        if (audioSource.isPlaying && audioSource.clip == clip) return;

        audioSource.clip = clip;
        audioSource.loop = true;
        audioSource.Play();
    }
    private void StopRunSound()
    {
        if (audioSource.isPlaying && audioSource.clip == runSound)
        {
            audioSource.Stop();
        }
    }


    void SummonEnemies()
    {
        if (spawnPoints.Length == 0 || enemyPrefabs.Length == 0) return;

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            // Kiểm tra nếu mảng enemyPrefabs có đủ phần tử
            if (i < enemyPrefabs.Length && spawnPoints[i] != null && enemyPrefabs[i] != null)
            {

                // Tạo hiệu ứng triệu hồi
                GameObject effect = Instantiate(summonEffect, spawnPoints[i].position, Quaternion.identity);
                Destroy(effect, 1f); 
                PlaySound(sumonSound);
                Instantiate(enemyPrefabs[i], spawnPoints[i].position, Quaternion.identity);
            }
        }
    }
}
