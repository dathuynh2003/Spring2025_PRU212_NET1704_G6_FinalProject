﻿using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.SceneManagement;

public class KnightController : MonoBehaviour, IPlayerStats
{
    public bool facingRight = true;

    public float maxHealth = 5;
    public float currentHealth;

    public float baseDame = 2;
    private float currentDame;

    public float jumpHeight = 10f;
    public float moveSpeed = 5f;
    private float movement;

    private Animator animator;
    private Rigidbody2D rb;
    private bool isGrounded = true;

    public Transform attackPoint;
    public float attackRadius = 1f;
    public LayerMask attackLayer;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip idleSound;
    [SerializeField] private AudioClip dieSound;
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip attackSound;
    [SerializeField] private AudioClip victorySound;
    [SerializeField] private AudioClip runSound;
    [SerializeField] private AudioClip hurtSound;
   // [SerializeField] private AudioClip boostHealth;
  //  [SerializeField] private AudioClip boostDame;
    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        currentDame = baseDame;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentHealth <= 0)
        {
            PlaySound(dieSound);
            Die();
        }

        movement = Input.GetAxis("Horizontal"); // Lấy input di chuyển

        // Chạy
        if (movement != 0)
        {
            if (movement < 0 && facingRight)
            {

                transform.eulerAngles = new Vector3(0, -180, 0);
                facingRight = false;
            } 
            else if (movement > 0 && facingRight == false)
            {
                transform.eulerAngles = new Vector3(0, 0, 0);
                facingRight = true;
            }
            animator.SetBool("isRunning", true);
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
        if (Input.GetMouseButtonDown(0))
        {
                animator.SetTrigger("Attack");
        }
    }
    private void FixedUpdate()
    {
        //transform.Translate(Vector2.right * moveSpeed * movement * Time.deltaTime);
        transform.position += new Vector3(movement, 0, 0) *  Time.fixedDeltaTime * moveSpeed;
        //transform.localScale = new Vector3(Mathf.Sign(movement), 1, 1);
    }

    public void Attack()
    {
        PlaySound(attackSound);
        Collider2D colliAttack = Physics2D.OverlapCircle(attackPoint.position, attackRadius, attackLayer);
        if (colliAttack)
        {
            Debug.Log(colliAttack.gameObject.name + " takes dame");
            if (colliAttack.gameObject.name.Contains("Boss"))
            {
                var boss = colliAttack.GetComponent<BossController>();
                boss.TakeDamage(currentDame);
            } else if (colliAttack.gameObject.name.Contains("FlyingEye"))
            {
                var enemy = colliAttack.GetComponent<FlyingEye>();
                enemy.TakeDame(currentDame);
            } else if (colliAttack.gameObject.name.Contains("Skeleton"))
            {
                var enemy = colliAttack.GetComponent<SkeletonController>();
                enemy.TakeDame(currentDame);
            }
            else if (colliAttack.gameObject.name.Contains("Goblin"))
            {
                var enemy = colliAttack.GetComponent<GoblinController>();
                enemy.TakeDame(currentDame);
            }
            else if (colliAttack.gameObject.name.Contains("Mushroom"))
            {
                var enemy = colliAttack.GetComponent<MushroomEnemy>();
                enemy.TakeDamage((int)currentDame);
            }
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
        PlaySound(jumpSound);
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

    public void TakeDame(float dame)
    {
        if (currentHealth <= 0)
        {
            return;
        }
        currentHealth -= dame;
        animator.SetTrigger("Hurt");
        Debug.LogFormat("Players remaining health {0}/{1} HP", currentHealth, maxHealth);
    }

    void Die()
    {
        animator.SetTrigger("Die");
    }

    public void DestroyPlayer()
    {
        Destroy(gameObject);
        loadLoseScene();
    }

    public void Heal(float healAmount)
    {
       // PlaySound(boostHealth);
        currentHealth = (currentHealth + healAmount > maxHealth) ? maxHealth : currentHealth + healAmount;
        Debug.Log("Player HP: " + currentHealth);
    }

    public void Victory()
    {
        PlaySound(victorySound);
        animator.SetTrigger("Win");
    }

    public void buffDame(float dameIncrease)
    {
      //  PlaySound(boostDame);
        currentDame += dameIncrease;
    }

    public float GetDame()
    {
        return currentDame;
    }

    public void SetDame(float value)
    {
        currentDame = value;
    }
    public void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
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

    public float GetCurHealth()
    {
        return currentHealth;
    }

    public float GetMaxHealth()
    {
        return maxHealth;
    }

    public void SetCurHealth(float value)
    {
        currentHealth = value;
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

        // Sau 3 giây, sẽ hết choáng và có thể di chuyển lại
        Invoke("RecoverFromDizzy", 3f);
    }

    void RecoverFromDizzy()
    {
        animator.SetBool("isDizzy", false);
        moveSpeed = 5f; // Trả lại tốc độ di chuyển
    }

    public void loadLoseScene()
    {
        SceneManager.LoadScene("LoseScene");
    }
}

