using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossHealthBar : MonoBehaviour
{
    private Image image;
    private BossController bossController;
    private TMPro.TextMeshProUGUI bossHealthText;
    private float bossInitialHealth;
    private float bossCurrentHealth;

    void Start()
    {
        image = gameObject.GetComponent<Image>();
        bossController = GameObject.Find("Boss").GetComponent<BossController>();
        bossInitialHealth = bossController.health;
        bossHealthText = gameObject.GetComponentInChildren<TMPro.TextMeshProUGUI>();
    }
    void FixedUpdate()
    {
        bossCurrentHealth = bossController.health;
        image.fillAmount = bossCurrentHealth/bossInitialHealth;
        bossHealthText.text = string.Format("{0} / {1}", bossCurrentHealth, bossInitialHealth);
    }
}
