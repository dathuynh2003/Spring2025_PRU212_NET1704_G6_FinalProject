using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private KnightController knightPlayer;
    [SerializeField] private DragronController dragonPlayer;
    [SerializeField] private Image healthBar;
    private void Start()
    {
        if (knightPlayer.isActiveAndEnabled)
        {
            healthBar.fillAmount = knightPlayer.maxHealth / 10f;
        }
        else if (dragonPlayer.isActiveAndEnabled)
        {
            healthBar.fillAmount = dragonPlayer.maxHealth / 10f;
        }
    }
    private void Update()
    {
        if (knightPlayer.isActiveAndEnabled)
        {
            healthBar.fillAmount = knightPlayer.currentHealth / 10f;
        }
        else if (dragonPlayer.isActiveAndEnabled)
        {
            healthBar.fillAmount = dragonPlayer.currentHealth / 10f;
        }
    }
}
