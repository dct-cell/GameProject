using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_StatsPanel : MonoBehaviour {
    public static UI_StatsPanel instance;

    private void Awake() {
        if (instance != null && instance != this) {
            Destroy(gameObject);
        }
        instance = this;
    }

    [SerializeField] public TextMeshProUGUI characterName;
    [SerializeField] public Image characterImage;
    [SerializeField] public TextMeshProUGUI health;
    [SerializeField] public TextMeshProUGUI attack;
    [SerializeField] public TextMeshProUGUI attackDistance;
    [SerializeField] public TextMeshProUGUI attackRange;
    [SerializeField] public TextMeshProUGUI speed;
    [SerializeField] public TextMeshProUGUI skillDescription;

    private void Start() {
        Clear();
    }

    public void ShowStats(Character character) {
        characterName.text = character.characterName;
        characterImage.sprite = character.GetComponent<SpriteRenderer>().sprite;
        characterImage.color = character.GetComponent<SpriteRenderer>().color;
        health.text = "ÉúÃü£º" + character.currentHealth.ToString() + " / " + character.maxHealth.ToString();
        attack.text = "¹¥»÷Á¦£º" + character.attack.ToString();
        attackDistance.text = "¹¥»÷¾àÀë£º" + character.attackDistance.ToString();
        attackRange.text = "¹¥»÷·¶Î§£º" + character.attackRange.ToString();
        speed.text = "ËÙ¶È£º" + character.speed.ToString();
        skillDescription.text = "¼¼ÄÜ£º" + character.skillDescription;
    }

    public void Clear() {
        characterName.text = "µã»÷½ÇÉ«²é¿´ÐÅÏ¢";
        characterImage.sprite = null;
        characterImage.color = Color.clear;
        health.text = "ÉúÃü£º";
        attack.text = "¹¥»÷Á¦£º";
        attackDistance.text = "¹¥»÷¾àÀë£º";
        attackRange.text = "¹¥»÷·¶Î§£º";
        speed.text = "ËÙ¶È£º";
        skillDescription.text = "¼¼ÄÜ£º";
    }
}
