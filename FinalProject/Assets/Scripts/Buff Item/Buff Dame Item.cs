using UnityEngine;

public class BuffDameItem : MonoBehaviour
{
    public float dameIncrease = 1f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Giả sử player có tag là "Player"
        if (other.CompareTag("Player"))
        {
            var knightPlayer = other.GetComponent<KnightController>();
            var dragonPlayer = other.GetComponent<DragronController>();

            if (knightPlayer != null)
            {
                knightPlayer.buffDame(dameIncrease);

                Debug.Log("Player increase " + dameIncrease + " dame(s)");
                Destroy(gameObject);
            }
            if (dragonPlayer != null)
            {
                Debug.Log("Dragon increase dames");
            }
        }
    }
}
