using UnityEngine;

public class Portal : MonoBehaviour
{
    public GameObject portalOn;  // Cổng mở
    public GameObject portalOff; // Cổng đóng
    public bool isActivated = false; // Trạng thái cổng
    private Collider2D portalCollider; // Collider của cổng
    public string currentMap;

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
            if (currentMap.Contains("Map1"))
            {
                Debug.Log("Player đi qua map 2!");
                // Chuyển sang màn tiếp theo
                UnityEngine.SceneManagement.SceneManager.LoadScene("Story2");
            }
            if (currentMap.Contains("Map2"))
            {
                Debug.Log("Player đi qua map 3!");
                // Chuyển sang màn tiếp theo
                UnityEngine.SceneManagement.SceneManager.LoadScene("Story3");
            }
        }
    }
}
