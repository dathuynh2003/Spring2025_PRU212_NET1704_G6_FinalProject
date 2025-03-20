using UnityEngine;
using System.Collections;

public class BossController : MonoBehaviour
{
    [SerializeField] private float speed = 3f;
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
    [SerializeField] private AudioClip bossHurtSound;
    [SerializeField] private Transform[] spawnPoints; // Các điểm spawn enemy
    [SerializeField] private GameObject[] enemyPrefabs; // Các loại quái khác nhau
    [SerializeField] private GameObject summonEffect; // Hiệu ứng triệu hồi

    public float detectionRadius = 2f;
    private Transform player;
    private bool hasDetectedPlayer = false;

    [SerializeField] private float enrageSpeedMultiplier = 1.5f; // Tốc độ nhân lên khi enrage
    [SerializeField] private float enrageAttackDelayMultiplier = 0.7f; // Delay giảm còn 70% khi enrage
    [SerializeField] private float postAttackRestTime = 2f;
    private bool isEnraged = false; // Trạng thái giận dữ

    private Vector3 initialPosition;    // Vị trí ban đầu
    private bool isReturning = false;   // Boss đang quay về
    private Coroutine bossRoutine;      // Lưu coroutine boss hành động

    public PlayerManager playerManager;
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        initialPosition = transform.position;
        currentHealth = maxHealth;

        if (attackPoint == null)
        {
            Debug.LogError("LỖI: attackPoint chưa được gán trong Inspector!");
        }

        // Bắt đầu chu trình hành động

        PlaySound(bossStart);

        // Triệu hồi quái vật khi trận đấu bắt đầu
        SummonEnemies();
    }

    void Update()
    {
        if (isDead) return;

        DetectPlayer();

        if (hasDetectedPlayer == false && isReturning)
        {
            isMovingToPlayer = false;
            StartCoroutine(ReturnToInitialPosition());
        }

        if (player == null) return;

        // Xử lý hướng quay của boss theo hướng player
        Vector2 direction = (player.position - transform.position).normalized;
        if (direction.x > 0)
            transform.localScale = new Vector3(3, 3, 3);
        else
            transform.localScale = new Vector3(-3, 3, 3);
    }

    private IEnumerator BossActionLoop()
    {
        if (playerManager != null)
            playerManager.setCanSwap(false);

        while (!isDead)
        {
            if (player == null)
            {
                hasDetectedPlayer = false;
                yield break;
            }

            yield return new WaitForSeconds(0.8f);

            // Move towards player
            isMovingToPlayer = true;
            anim.SetBool("boss_run", true);
            Debug.Log("Player: " + player);
            Debug.Log("Attack Point: " + attackPoint);

            if (player == null) yield break;
            while (Vector2.Distance(attackPoint.position, player.position) > attackRadius)
            {
                // Nếu player rời khỏi detectionRadius -> dừng boss
                //float distanceToPlayer = Vector2.Distance(transform.position, player.position);
                //if (distanceToPlayer > detectionRadius)
                //{
                //    StartReturnToPosition();
                //    yield break; // Dừng BossActionLoop
                //}

                MoveTowardsPlayer();
                yield return null;
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
            yield return new WaitForSeconds(postAttackRestTime);
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
        anim.SetTrigger("boss_take_hit");
        PlaySound(bossHurtSound);
        currentHealth -= damage;
        Debug.Log("Current health boss: " + currentHealth);
        if (!isEnraged && currentHealth <= maxHealth / 2)
        {
            isEnraged = true;
            PlaySound(angrySound);
            speed *= enrageSpeedMultiplier; // Tăng tốc độ chạy
            postAttackRestTime *= enrageAttackDelayMultiplier; // Giảm delay tấn công
            Debug.Log("Boss trở nên tức giận! Tốc độ và tốc độ đánh tăng!");
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
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
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

    void DetectPlayer()
    {
        Collider2D hitPlayer = Physics2D.OverlapCircle(transform.position, detectionRadius, playerLayer);
        if (hitPlayer == null)
        {
            if (bossRoutine != null)
            {
                StopCoroutine(bossRoutine);
                bossRoutine = null;
                hasDetectedPlayer = false;
                isReturning = true;
                Debug.Log("Player đã đi xa boss. Boss dừng tấn công!");
            }
            player = null;
        }
        else
        {
            player = hitPlayer.transform;
            if (bossRoutine == null)
            {
                bossRoutine = StartCoroutine(BossActionLoop());
                Debug.Log("Boss phát hiện Player. Bắt đầu tấn công");
            }
        }
    }

    private IEnumerator ReturnToInitialPosition()
    {
        if (playerManager != null)
            playerManager.setCanSwap(true);
        while (true)
        {
            float distance = Vector2.Distance(transform.position, initialPosition);
            if (distance < 1f)
            {
                body.linearVelocity = Vector2.zero;

                // Reset các thông số
                anim.SetBool("boss_run", false);
                StopRunSound();

                currentHealth = maxHealth;  // Reset máu
                isEnraged = false;          // Thoát trạng thái giận dữ
                postAttackRestTime = 2f;    // Reset delay đánh
                speed = 3f;                 // Reset speed

                Debug.Log("Boss đã trở về vị trí cũ và hồi máu!");

                // Tắt trạng thái quay về
                isReturning = false;
                hasDetectedPlayer = false;

                bossRoutine = null;
                yield break;
            }
            else
            {
                Vector3 direction = (initialPosition - (Vector3)transform.position).normalized;

                // Di chuyển boss
                body.linearVelocity = new Vector2(direction.x * speed, body.linearVelocity.y);

                // Quay mặt
                if (direction.x > 0)
                    transform.localScale = new Vector3(3, 3, 3);
                else
                    transform.localScale = new Vector3(-3, 3, 3);
            }
            yield return null;
        }
    }
}
