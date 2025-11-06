using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_HealthBar : MonoBehaviour
{
    public Character character;
    public Slider slider;
    public Image fillImage;

    private void Awake() {
        character = GetComponentInParent<Character>();
        slider = GetComponentInChildren<Slider>();
        fillImage = slider.GetComponentInChildren<Image>();
    }

    private void OnEnable() {
        UpdateHealthUI();
    }

    public void UpdateHealthUI() {
        if (character.teamId == 0)
            fillImage.color = Color.green;
        else
            fillImage.color = Color.red;
        slider.maxValue = character.maxHealth;
        slider.value = character.health;
    }
}
