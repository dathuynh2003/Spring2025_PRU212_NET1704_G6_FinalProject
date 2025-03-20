using UnityEngine;

public class HealPotion : MonoBehaviour
{
    public float healAmountPercent = 0.5f;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip sound;
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
                float healAmount = knightPlayer.maxHealth * healAmountPercent;
                PlaySound(sound);
                knightPlayer.Heal(healAmount);

                Debug.Log("Player healed for " + healAmount + " HP");

                // Sau khi heal xong thì biến mất
                Destroy(gameObject);
            }
            if (dragonPlayer != null)
            {
                PlaySound(sound);
                float healAmount = dragonPlayer.maxHealth * healAmountPercent;
                dragonPlayer.Heal(healAmount);
                Destroy(gameObject);
            }
        }
    }
    public void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
            Destroy(gameObject, clip.length); // Xóa sau khi âm thanh phát xong
        }
    }
}
