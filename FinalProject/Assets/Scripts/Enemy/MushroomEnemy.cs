﻿using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class MushroomEnemy : MonoBehaviour
{
    public bool facingLeft = true;
    public float moveSpeed = 1f;
    public float moveDistance = 600f; // Khoảng cách di chuyển trước khi quay đầu
    [SerializeField] private AudioSource audioSource;
    //public Transform checkPoint;
    //public float distance = 1f;
    //public LayerMask layerMask;

    private Vector2 startPosition;
    private float distanceMoved = 0f;

    private Animator animator;
    public float idleTime = 1f;
    private bool isDead = false;
    private bool isIdle = false;
    private float idleTimer = 0f;

    public GameObject explosionPrefab;

    public Transform attackPoint;
    public float attackRadius = 1f;

    public LayerMask playerLayer;
    public float explodeRadius = 2f;

    public int maxHealth = 5;
    private int currentHealth;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth;
        startPosition = transform.position;
        animator = GetComponent<Animator>();
        //animator.SetBool("IsRunning", true);
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead) return;

        if (isIdle)
        {
            // Đếm thời gian đứng yên
            idleTimer += Time.deltaTime;
            if (idleTimer >= idleTime)
            {
                // Kết thúc trạng thái đứng yên và quay đầu
                isIdle = false;
                animator.SetBool("isRunning", true); // Tiếp tục chạy
                Flip();
                distanceMoved = 0f;
                startPosition = transform.position;
            }
            return; // Không di chuyển trong trạng thái đứng yên
        }

        // Kiểm tra nếu player ở gần thì phát nổ
        Collider2D player = Physics2D.OverlapCircle(transform.position, explodeRadius, playerLayer);
        if (player != null)
        {
            Die(); // Kích hoạt nổ
            return;
        }

        // Di chuyển enemy
        transform.Translate(Vector2.left * Time.deltaTime * moveSpeed * (facingLeft ? 1 : -1));

        // Cập nhật khoảng cách đã di chuyển
        distanceMoved += Mathf.Abs(Vector2.Distance(transform.position, startPosition));

        // Kiểm tra nếu enemy đã di chuyển đủ khoảng cách
        if (distanceMoved >= moveDistance)
        {
            // Chuyển sang trạng thái đứng yên
            isIdle = true;
            idleTimer = 0f;
            animator.SetBool("isRunning", false); // Dừng animation Run
        }

        // Kiểm tra nếu enemy không còn trên mặt đất
        //RaycastHit2D hit = Physics2D.Raycast(checkPoint.position, Vector2.down, distance, layerMask);

        //if (hit.collider == null)
        //{
        //    Flip();
        //}
    }

    private void Flip()
    {
        facingLeft = !facingLeft;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    //private void OnDrawGizmosSelected()
    //{
    //    if (checkPoint == null)
    //    {
    //        return;
    //    }

    //    Gizmos.color = Color.yellow;
    //    Gizmos.DrawRay(checkPoint.position, Vector2.down * distance);
    //}

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        animator.SetTrigger("Hurt");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Hàm để kích hoạt animation Death
    public void Die()
    {
        if (isDead) return; // Nếu đã chết, không làm gì cả

        isDead = true;
        animator.SetTrigger("Death"); // Kích hoạt animation Death
        // Tạm dừng di chuyển
        moveSpeed = 0f;
        //enabled = false;

        // Bắt đầu coroutine để đợi animation Death hoàn tất
        StartCoroutine(WaitForDeathAnimation());
    }

    private IEnumerator WaitForDeathAnimation()
    {
        // Đợi animation Death chạy xong trước khi phát nổ
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        // Sau khi Death hoàn thành -> Gọi Explode()
        Explode();
    }

    private void Explode()
    {
        Debug.Log("Enemy đã nổ!");

        if (explosionPrefab != null)
        {
            GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);

            // Hủy hiệu ứng nổ sau khi animation kết thúc
            Destroy(explosion, 1f);
        }

        // Gây sát thương cho Player nếu ở trong bán kính nổ
        Collider2D player = Physics2D.OverlapCircle(transform.position, explodeRadius, playerLayer);
        if (player != null)
        {
            // Giả sử Player có script "PlayerHealth" với hàm "TakeDamage"
            KnightController knight = player.GetComponent<KnightController>();
            if (knight != null)
            {
                knight.TakeDame(3);
            }
            DragronController dragron = player.GetComponent<DragronController>();
            if (dragron != null)
            {
                dragron.TakeDame(3);
            }
        }

        // Ẩn enemy thay vì hủy ngay lập tức
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Collider2D>().isTrigger = true;

        // Hủy enemy sau 2 giây (đảm bảo hiệu ứng nổ hiển thị xong)
        Destroy(gameObject, 2f);
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
        }

        // Vẽ bán kính phát nổ
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, explodeRadius);
    }

    public void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
