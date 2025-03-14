using UnityEngine;
using UnityEngine.Audio;

public class Fireball : MonoBehaviour
{
    public float fireballSpeed;
    private Rigidbody2D myBody;

    private float maxX;
    public float damage;

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

    // Update is called once per frame
    void Update()
    {
        // Move the fireball
        myBody.linearVelocity = new Vector2(fireballSpeed, 0);
        OnBecameInvisible();
    }

    private void OnBecameInvisible()
    {
        if (transform.position.x > maxX)
        {
            Destroy(gameObject);
        }
    }
}
