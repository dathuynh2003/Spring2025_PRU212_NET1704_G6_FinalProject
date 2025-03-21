using UnityEngine;

public class BuffDameItem : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip sound;
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
            PlaySound(sound);
            if (knightPlayer != null)
            {
    
                knightPlayer.buffDame(dameIncrease);

                Debug.Log("Player increase " + dameIncrease + " dame(s)");
                gameObject.GetComponent<SpriteRenderer>().enabled = false;
                Destroy(gameObject, sound.length);
            }
            if (dragonPlayer != null)
            {
                dragonPlayer.buffDame(dameIncrease);
                gameObject.GetComponent<SpriteRenderer>().enabled = false;
                Destroy(gameObject, sound.length);
            }
        }
    }
    public void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
