using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private KnightController knightPlayer;
    [SerializeField] private DragronController dragonPlayer;
    [SerializeField] private Image healthBar;
    private void Start()
    {
        if (knightPlayer != null)
        {
            healthBar.fillAmount = knightPlayer.maxHealth / 10f;
        }
        else if (dragonPlayer != null)
        {
            healthBar.fillAmount = dragonPlayer.maxHealth / 10f;
        }
    }
    private void Update()
    {
        if (knightPlayer != null)
        {
            healthBar.fillAmount = knightPlayer.currentHealth / 10f;
        }
        else if (dragonPlayer != null)
        {
            healthBar.fillAmount = dragonPlayer.currentHealth / 10f;
        }
    }
}
