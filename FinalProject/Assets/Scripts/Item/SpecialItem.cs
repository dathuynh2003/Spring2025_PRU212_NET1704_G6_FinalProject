using UnityEngine;

public class SpecialItem : MonoBehaviour
{
    public string itemName; // Đặt trong Inspector ("Ngọn Lửa Hồi Sinh", "Bùa Hộ Mệnh Cổ Đại")
    private bool isCollected = false;

    public Portal portal;

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
        Debug.Log(itemName + " đã được nhặt!");
        //GameManager.Instance.SetItemCollected(itemName);
        Destroy(gameObject); // Xóa vật phẩm sau khi nhặt
    }
}
