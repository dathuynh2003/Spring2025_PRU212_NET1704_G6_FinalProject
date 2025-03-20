using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterSelectUI : MonoBehaviour
{
    [Header("Button References")]
    [SerializeField] private Button knightButton;
    [SerializeField] private Button dragonButton;
    [SerializeField] private Button playButton;

/*    [Header("Image Overlays (Highlight)")]
    [SerializeField] private Image knightHighlight;
    [SerializeField] private Image dragonHighlight;*/

    [Header("Images in Buttons")]
    [SerializeField] private Image knightImage;
    [SerializeField] private Image dragonImage;

    [Header("Sprites")]
    [SerializeField] private Sprite knightNormalSprite;
    [SerializeField] private Sprite knightSelectedSprite;
    [SerializeField] private Sprite dragonNormalSprite;
    [SerializeField] private Sprite dragonSelectedSprite;

    private void Start()
    {
        // Khởi đầu: disable highlight và nút Play
        playButton.interactable = false;

        // Sự kiện chọn nhân vật
        knightButton.onClick.AddListener(OnKnightSelected);
        dragonButton.onClick.AddListener(OnDragonSelected);

        // Play button chỉ hoạt động khi chọn nhân vật
        playButton.onClick.AddListener(OnPlayClicked);

        knightImage.sprite = knightNormalSprite;
        dragonImage.sprite = dragonNormalSprite;
    }

    void OnKnightSelected()
    {
        GameManager.Instance.SelectKnight();

        // Đổi sprite khi chọn
        knightImage.sprite = knightSelectedSprite;
        dragonImage.sprite = dragonNormalSprite;

        //// Đổi màu cho nhân vật được chọn (tuỳ chọn)
        //knightImage.color = Color.green;  // Màu highlight
        //dragonImage.color = Color.white;  // Reset màu cũ

        playButton.interactable = true;
    }

    void OnDragonSelected()
    {
        GameManager.Instance.SelectDragon();

        dragonImage.sprite = dragonSelectedSprite;
        knightImage.sprite = knightNormalSprite;

        //// Bật highlight Dragon, tắt highlight Knight
        //dragonHighlight.enabled = true;
        //knightHighlight.enabled = false;

        // Bật nút Play
        playButton.interactable = true;
    }

    void OnPlayClicked()
    {
        Debug.Log("Play Clicked! Loading MainGameScene...");
        SceneManager.LoadScene("Story1", LoadSceneMode.Single);
    }
}
