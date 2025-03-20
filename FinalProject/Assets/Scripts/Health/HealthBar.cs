using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private Image totalhealthBar;
    [SerializeField] private Image currenthealthBar;

    private void Awake()
    {
        
    }

    private void Start()
    {
        //totalhealthBar.fillAmount = playerHealth.currentHealth / 10;
    }
    private void Update()
    {
        //currenthealthBar.fillAmount = playerHealth.currentHealth / 10;
        if (playerManager != null)
        {
            float playerHealth = playerManager.CurrentPlayerHealth();
                currenthealthBar.fillAmount = playerHealth / 10;
        }
    }
}
