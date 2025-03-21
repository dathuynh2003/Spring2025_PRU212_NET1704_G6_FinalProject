using UnityEngine;

public class SpecialItem : MonoBehaviour
{
    public string itemName; // Đặt trong Inspector ("Ngọn Lửa Hồi Sinh", "Bùa Hộ Mệnh Cổ Đại")
    private bool isCollected = false;

    public Portal portal;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip sound;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isCollected)
        {
            isCollected = true;
            portal.ActivatePortal(); // Mở cổng
            portal.currentMap = itemName; // Lưu tên map hiện tại
            CollectItem();
        }
    }

    void CollectItem()
    {
        PlaySound(sound);
        Debug.Log(itemName + " đã được nhặt!");
        //GameManager.Instance.SetItemCollected(itemName);
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
        Destroy(gameObject, sound.length); // Xóa vật phẩm sau khi nhặt
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
