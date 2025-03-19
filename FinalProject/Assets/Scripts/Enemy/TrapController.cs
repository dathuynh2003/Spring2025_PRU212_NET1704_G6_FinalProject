using UnityEngine;

public class TrapController : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) // Khi nhân vật chạm vào bẫy
        {
            animator.SetTrigger("Activate"); // Kích hoạt animation
            Destroy(gameObject, 0.8f);
        }

    }

    // Hàm này sẽ được gọi trong Animation Event
    public void TriggerDamage()
    {
        Collider2D player = Physics2D.OverlapCircle(transform.position, 1f, LayerMask.GetMask("Player"));
        if (player)
        {
            player.GetComponent<DragronController>()?.TakeDame(1);
        }
    }
}
