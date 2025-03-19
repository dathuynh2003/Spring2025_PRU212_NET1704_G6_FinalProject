using UnityEngine;
using UnityEngine.Audio;

public class Fireball : MonoBehaviour
{
    public float fireballSpeed;
    private Rigidbody2D myBody;

    private float maxX;
    public int damage = 1;
    private int direction = 1;

    private void Awake()
    {
        myBody = GetComponent<Rigidbody2D>();

        Vector3 screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
        maxX = screenBounds.x;

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void SetDirection(int dir)
    {
        direction = dir;
        // Đảo ngược hướng của fireball nếu cần
        if (direction == -1)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Move the fireball
        myBody.linearVelocity = new Vector2(fireballSpeed * direction, 0);
        OnBecameInvisible();
    }

    private void OnBecameInvisible()
    {
        if (Mathf.Abs(transform.position.x) > maxX)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            MushroomEnemy enemy = collision.GetComponent<MushroomEnemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage); 
            }

            Destroy(gameObject); 
        }
    }
}
