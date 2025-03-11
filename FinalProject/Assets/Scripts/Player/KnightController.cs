using UnityEngine;

public class KnightController : MonoBehaviour
{

    private Animator animator;
    private Rigidbody2D rb;
    private float moveSpeed = 5f;
    private bool isGrounded = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

    }

    // Update is called once per frame
    void Update()
    {
        float move = Input.GetAxis("Horizontal"); // Lấy input di chuyển

        // Chạy
        if (move != 0)
        {
            animator.SetBool("isRunning", true);
            transform.Translate(Vector2.right * moveSpeed * move * Time.deltaTime);
            transform.localScale = new Vector3(Mathf.Sign(move), 1, 1); // Đổi hướng nhân vật
        }
        else
        {
            animator.SetBool("isRunning", false);
        }

        // Nhảy
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocityX, 10f);
            isGrounded = false;
        }

        // Đánh
        if (Input.GetMouseButtonDown(0))
        {
            animator.SetTrigger("Attack");
            animator.SetBool("isAttack", true);
        }
        if (Input.GetMouseButtonUp(0))
        {
            animator.ResetTrigger("Attack");
            animator.SetBool("isAttack", false);

        }
    }

    // Xử lý va chạm
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
}
