using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    //[SerializeField] private KnightController knightPlayer;
    //[SerializeField] private DragronController dragonPlayer;
    [SerializeField] private Image healthBar;
    private IPlayerStats playerStats;
    private void Start()
    {
        Invoke(nameof(FindPlayer), 0.1f);
    }

    private void FindPlayer()
    {
        GameObject currentPlayer = GameObject.FindWithTag("Player");

        if (currentPlayer != null)
        {
            playerStats = currentPlayer.GetComponent<IPlayerStats>();
        }

        if (playerStats == null)
        {
            Debug.LogError("Không tìm thấy Player Stats!");
        }
    }

    private void Update()
    {
        if (playerStats != null)
        {
            float ratio = playerStats.GetCurHealth() / playerStats.GetMaxHealth();
            healthBar.fillAmount = ratio;
        }
    }
}
