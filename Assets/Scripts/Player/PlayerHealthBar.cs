using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHealthBar : MonoBehaviour
{
    private PlayerController playerController;
    private Image healthBarImage;
    private TextMeshProUGUI healthBarText;
    private Image shieldBarImage;
    private TextMeshProUGUI shieldBarText;
    private float playerHealth;
    private float playerMaxHealth;
    private float playerShield;
    private float playerMaxShield;

    void Start()
    {
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        healthBarImage = GameObject.Find("Canvas/PlayerHealthBar/HealthBar").GetComponent<Image>();
        healthBarText = GameObject.Find("Canvas/PlayerHealthBar/HealthText").GetComponent<TextMeshProUGUI>();
        shieldBarImage = GameObject.Find("Canvas/PlayerHealthBar/ShieldBar").GetComponent<Image>();
        shieldBarText = GameObject.Find("Canvas/PlayerHealthBar/ShieldText").GetComponent<TextMeshProUGUI>();

        playerHealth = playerController.health;
        playerMaxHealth = playerController.maxHealth;
        playerShield = playerController.shield;
        playerMaxShield = playerController.maxShield;
        
    }
    void FixedUpdate()
    {
        playerHealth = playerController.health;
        playerMaxHealth = playerController.maxHealth;
        playerShield = playerController.shield;
        playerMaxShield = playerController.maxShield;

        healthBarImage.fillAmount = playerHealth/playerMaxHealth;
        healthBarText.text = string.Format("{0} / {1}", playerHealth, playerMaxHealth);

        shieldBarImage.fillAmount = playerShield/playerMaxShield;
        shieldBarText.text = string.Format("{0} / {1}", playerShield, playerMaxShield);

    }
}
