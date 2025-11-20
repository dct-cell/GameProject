using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class UI_HealthBar : MonoBehaviour
{
    public Character character;
    public GameObject healthBar;
    public GameObject fillHealth;
    public GameObject fillShield;

    private void Awake() {
        character = GetComponentInParent<Character>();
        healthBar = transform.Find("HealthBar").gameObject;
        fillHealth = transform.Find("HealthBar/Fill_Health").gameObject;
        fillShield = transform.Find("HealthBar/Fill_Shield").gameObject;
    }

    private void OnEnable() {
        UpdateHealthUI();
    }

    public void UpdateHealthUI() {
        if (character.teamId == 0)
            fillHealth.GetComponent<Image>().color = Color.green;
        else
            fillHealth.GetComponent<Image>().color = Color.red;
        fillShield.GetComponent<Image>().color = Color.gray;
        float totalLength = healthBar.GetComponent<RectTransform>().rect.width;
        float height = healthBar.GetComponent<RectTransform>().rect.height;
        float healthPercent = 1.0f * character.currentHealth / character.maxHealth;
        float shieldPercent = 1.0f * character.shield / character.maxHealth;
        fillHealth.GetComponent<RectTransform>().sizeDelta = new Vector2(healthPercent * totalLength, height);
        fillShield.GetComponent<RectTransform>().sizeDelta = new Vector2((healthPercent + shieldPercent) * totalLength, height);
    }
}
