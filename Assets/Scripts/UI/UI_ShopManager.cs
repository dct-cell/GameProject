using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.VersionControl;
using UnityEngine;

public class UI_ShopManager : MonoBehaviour {
    public static UI_ShopManager instance { get; private set; }
    private void Awake() {
        if (instance != null && instance != this) {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    [SerializeField] public GameObject shop;
    public UI_ShopSlot[] shopSlots;

    public void InitShopUI() {
        shopSlots = shop.GetComponentsInChildren<UI_ShopSlot>();
        infoText.gameObject.SetActive(false);
        UpdateSlotUI();
    }

    public void UpdateSlotUI() {
        for (int i = 0; i < shopSlots.Length; i++)
            shopSlots[i].CleanUpSlot();
        List<Character> members = ShopManager.instance.shopCharacter;
        for (int i = 0; i < members.Count; i++)
            shopSlots[i].UpdateSlot(members[i]);
    }

    public TextMeshProUGUI coinText;
    public TextMeshProUGUI infoText;
    private Coroutine displayCoroutine;

    public void UpdateCoinText() {
        coinText.text = "½ð±Ò£º" + BagManager.instance.coin.ToString();
    }

    public void ShowInfo(string message) {
        if (displayCoroutine != null) {
            StopCoroutine(displayCoroutine);
        }
        displayCoroutine = StartCoroutine(DisplayMessageForDuration(message));
    }

    private IEnumerator DisplayMessageForDuration(string message, float duration = 3f) {
        infoText.text = message;
        infoText.gameObject.SetActive(true);
        yield return new WaitForSeconds(duration);
        infoText.text = "";
        infoText.gameObject.SetActive(false);
        displayCoroutine = null;
    }
}
