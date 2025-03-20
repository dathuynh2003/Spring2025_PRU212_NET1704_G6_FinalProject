using UnityEngine;

public class Portal : MonoBehaviour
{
    public GameObject portalOn;  // Cổng mở
    public GameObject portalOff; // Cổng đóng
    public bool isActivated = false; // Trạng thái cổng
    private Collider2D portalCollider; // Collider của cổng

    void Start()
    {
        portalOn.SetActive(false);  // Ẩn cổng mở khi game bắt đầu
        portalCollider = GetComponent<Collider2D>();
        portalCollider.isTrigger = false;
    }

    public void ActivatePortal()
    {
        isActivated = true;
        portalOn.SetActive(true);   // Bật cổng mở
        portalOff.SetActive(false); // Ẩn cổng đóng
        portalCollider.isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isActivated && other.CompareTag("Player"))
        {
            Debug.Log("Player đi qua cổng!");
            // Chuyển sang màn tiếp theo
            UnityEngine.SceneManagement.SceneManager.LoadScene("NextScene");
        }
    }
}
